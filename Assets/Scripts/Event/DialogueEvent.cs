
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueEvent", menuName = "Events/DialogEvent")]
public class DialogueEvent : ScriptableObject
{
    public DialogData[] data;
}

[System.Serializable]
public struct DialogData
{
    public string name;       // ダイアログの発言者
    [TextArea(3, 10)]
    public string messages;
}
[CustomEditor(typeof(DialogueEvent))]
public class DialogueEventEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // DialogData配列を表示
        SerializedProperty dataProp = serializedObject.FindProperty("data");

        EditorGUILayout.PropertyField(dataProp, new GUIContent("Dialog Data"), true);

        serializedObject.ApplyModifiedProperties();
    }
}