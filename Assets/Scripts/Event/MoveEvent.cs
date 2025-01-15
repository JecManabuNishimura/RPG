using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMoveEvent", menuName = "Events/MoveEvent")]
public class MoveEvent : ScriptableObject
{
    public bool selectPlayer;
    public GameObject npc;
    public bool createNpc;
    public Transform createPos;
    public Transform[] targetPosition;
}
[CustomEditor(typeof(MoveEvent))]
public class MoveEventEditor : Editor
{
    private SerializedProperty selectPlayerProp;
    private SerializedProperty npcProp;
    private SerializedProperty createNpcProp;
    private SerializedProperty createPosProp;
    private SerializedProperty targetPositionProp;

    private void OnEnable()
    {
        // プロパティを初期化
        selectPlayerProp = serializedObject.FindProperty("selectPlayer");
        npcProp = serializedObject.FindProperty("npc");
        createNpcProp = serializedObject.FindProperty("createNpc");
        createPosProp = serializedObject.FindProperty("createPos");
        targetPositionProp = serializedObject.FindProperty("targetPosition");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // チェックボックスの描画
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(selectPlayerProp, new GUIContent("Select Player"));
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        // NPCオブジェクトのフィールドを描画
        EditorGUI.BeginDisabledGroup(selectPlayerProp.boolValue);
        EditorGUILayout.PropertyField(npcProp, new GUIContent("NPC"));
        EditorGUI.EndDisabledGroup();

        // createNpc チェックボックスの描画
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(createNpcProp, new GUIContent("Create NPC"));
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        // createNpc がオンのときだけ createPos を表示
        if (createNpcProp.boolValue)
        {
            EditorGUILayout.PropertyField(createPosProp, new GUIContent("Create Position"));
        }

        // targetPosition 配列の描画
        EditorGUILayout.PropertyField(targetPositionProp, new GUIContent("Target Position"), true);

        serializedObject.ApplyModifiedProperties();
    }

}