#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AllEffectMaster))]
public class AllEffectMasterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AllEffectMaster master = (AllEffectMaster)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("CSV‚©‚çŒø‰Êˆê——‚ğ“Ç‚İ‚Ş"))
        {
            master.ImportFromCsv();
        }
    }
}
#endif