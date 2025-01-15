using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public enum EventType
{
    Dialogue,
    Move,
    Animation,
}




[System.Serializable]
public class EventEntry
{
    public EventType eventType;
    public DialogueEvent dialogueEvent;
    public MoveEvent moveEvent;
}

[CreateAssetMenu(fileName = "EventData", menuName = "Event/EventData")]
public class EventData : ScriptableObject
{
    public EventEntry[] eventList; // ダイアログのリスト
}

[CustomEditor(typeof(EventData))]
public class EventDataEditor : Editor
{
    SerializedProperty eventList;
    ReorderableList reorderableList;

    void OnEnable()
    {
        eventList = serializedObject.FindProperty("eventList");

        reorderableList = new ReorderableList(serializedObject, eventList, true, true, true, true)
        {
            drawElementCallback = DrawEventEntry,
            onAddCallback = AddEventEntry,
            onRemoveCallback = RemoveEventEntry,
            onReorderCallback = ReorderEventEntry,
            elementHeightCallback = CalculateElementHeight
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        reorderableList.DoLayoutList(); // ReorderableListの描画
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawEventEntry(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);

        // EventTypeフィールドを描画
        SerializedProperty eventType = element.FindPropertyRelative("eventType");
        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), eventType,
            GUIContent.none);

        EventType type = (EventType)eventType.enumValueIndex;
        float yOffset = EditorGUIUtility.singleLineHeight + 2;
        float xOffset = 10;

        switch (type)
        {
            case EventType.Dialogue:
                SerializedProperty dialogueEvent = element.FindPropertyRelative("dialogueEvent");
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + yOffset, rect.width, EditorGUIUtility.singleLineHeight),
                    dialogueEvent.FindPropertyRelative("name"), new GUIContent("Name"));
                yOffset += EditorGUIUtility.singleLineHeight + 2;

                SerializedProperty messages = dialogueEvent.FindPropertyRelative("messages");
                // 固定高さに設定
                float messageFieldHeight = 100; // 高さを固定する
                Rect messageRect = new Rect(rect.x, rect.y + yOffset, rect.width, messageFieldHeight);
                EditorGUI.PropertyField(messageRect, messages, new GUIContent("Messages"), true);
                break;

            case EventType.Move:
                var moveEvent = element.FindPropertyRelative("moveEvent");
            
                // `MoveEvent` のプロパティを描画する
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + yOffset, rect.width, EditorGUI.GetPropertyHeight(moveEvent)),
                    moveEvent, new GUIContent("Move Event")
                );
                /*
                // チェックボックスの描画
                SerializedProperty selectPlayerProp = moveEvent.FindPropertyRelative("selectPlayer");
                selectPlayerProp.boolValue = EditorGUI.Toggle(new Rect(rect.x, rect.y + yOffset, rect.width, EditorGUIUtility.singleLineHeight),
                    "Select Player", selectPlayerProp.boolValue);
                yOffset += EditorGUIUtility.singleLineHeight + 2;
                
                // NPCオブジェクトのフィールドを描画
                EditorGUI.BeginDisabledGroup(selectPlayerProp.boolValue);
                SerializedProperty npcProp = moveEvent.FindPropertyRelative("npc");
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + yOffset, rect.width, EditorGUIUtility.singleLineHeight),
                    npcProp, new GUIContent("NPC")
                );
                EditorGUI.EndDisabledGroup();
                yOffset += EditorGUIUtility.singleLineHeight + 2;
                // createNpc チェックボックスの描画
                SerializedProperty createNpcProp = moveEvent.FindPropertyRelative("createNpc");
                createNpcProp.boolValue = EditorGUI.Toggle(
                    new Rect(rect.x, rect.y + yOffset, rect.width, EditorGUIUtility.singleLineHeight),
                    "Create NPC", createNpcProp.boolValue
                );
                yOffset += EditorGUIUtility.singleLineHeight + 2;

                // createNpc がオンのときだけ createPos を表示
                if (createNpcProp.boolValue)
                {
                    SerializedProperty createPosProp = moveEvent.FindPropertyRelative("createPos");
                    EditorGUI.PropertyField(
                        new Rect(rect.x + xOffset, rect.y + yOffset, rect.width - xOffset, EditorGUIUtility.singleLineHeight),
                        createPosProp, new GUIContent("Create Position")
                    );
                    yOffset += EditorGUIUtility.singleLineHeight + 2;
                }
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + yOffset, rect.width, EditorGUIUtility.singleLineHeight),
                    moveEvent.FindPropertyRelative("targetPosition"), new GUIContent("Target Position"));
                */
                break;

            case EventType.Animation:
                EditorGUI.LabelField(new Rect(rect.x, rect.y + yOffset, rect.width, EditorGUIUtility.singleLineHeight),
                    "Animation Event settings will go here.");
                break;
        }
    }

    private void AddEventEntry(ReorderableList list)
    {
        list.serializedProperty.InsertArrayElementAtIndex(list.serializedProperty.arraySize);
        SerializedProperty newEventEntry =
            list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1);
        newEventEntry.FindPropertyRelative("eventType").enumValueIndex = (int)EventType.Dialogue; // デフォルトのイベントタイプ
    }

    private void RemoveEventEntry(ReorderableList list)
    {
        if (list.index >= 0 && list.index < list.serializedProperty.arraySize)
        {
            list.serializedProperty.DeleteArrayElementAtIndex(list.index);
        }
    }

    private void ReorderEventEntry(ReorderableList list)
    {
        // 順序が変更されたときの処理が必要であればここに追加
    }

    private float CalculateElementHeight(int index)
    {
        SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
        SerializedProperty eventType = element.FindPropertyRelative("eventType");

        EventType type = (EventType)eventType.enumValueIndex;
        float height = EditorGUIUtility.singleLineHeight + 2;

        switch (type)
        {
            case EventType.Dialogue:
                // 高さの固定設定を追加
                height += EditorGUIUtility.singleLineHeight + 2; // Nameフィールドの高さ
                height += 100; // 固定メッセージフィールドの高さ
                height += EditorGUIUtility.singleLineHeight + 2;
                break;

            case EventType.Move:
                
                SerializedProperty moveEvent = element.FindPropertyRelative("moveEvent");
                height += EditorGUIUtility.singleLineHeight + 2; 
                height += EditorGUIUtility.singleLineHeight + 2; 
                height += EditorGUIUtility.singleLineHeight + 2; 
                height += EditorGUIUtility.singleLineHeight + 2; 
                /*
                // createNpc がオンの場合、createPos フィールドの高さを追加
                if (moveEvent.FindPropertyRelative("createNpc").boolValue)
                {
                    height += EditorGUIUtility.singleLineHeight + 2; // createPos フィールドの高さ
                }
                height += 40;
                for (int i = 0; i < moveEvent.FindPropertyRelative("targetPosition").arraySize+1; i++)
                {
                    height += EditorGUIUtility.singleLineHeight + 2; 
                }
                */

                break;

            case EventType.Animation:
                height += EditorGUIUtility.singleLineHeight; // Animation Eventの高さ
                height += EditorGUIUtility.singleLineHeight + 2;
                break;
        }

        return height;

    }
}