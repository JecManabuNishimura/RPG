using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using System.IO;

public class TMPFontReplacer : EditorWindow
{
    private TMP_FontAsset newFontAsset;

    [MenuItem("Tools/Replace TMP Font (Scenes + Prefabs)")]
    public static void ShowWindow()
    {
        GetWindow<TMPFontReplacer>("TMP Font Replacer");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("一括置換する TextMeshPro フォントを選択", EditorStyles.boldLabel);
        newFontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField("New TMP Font", newFontAsset, typeof(TMP_FontAsset), false);

        if (newFontAsset == null)
        {
            EditorGUILayout.HelpBox("新しい TMP_FontAsset を設定してください。", MessageType.Warning);
        }

        if (GUILayout.Button("全シーン + 全プレハブでフォントを置換") && newFontAsset != null)
        {
            ReplaceFontsInAllScenes();
            ReplaceFontsInAllPrefabs();
            Debug.Log("TMPフォントの一括置換が完了しました！");
        }
    }

    private void ReplaceFontsInAllScenes()
    {
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");

        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            int count = 0;

            foreach (var tmp in GameObject.FindObjectsOfType<TMP_Text>(true))
            {
                Undo.RecordObject(tmp, "Replace TMP Font");
                tmp.font = newFontAsset;
                EditorUtility.SetDirty(tmp);
                count++;
            }

            EditorSceneManager.SaveScene(scene);
            Debug.Log($"シーン '{scene.name}' で {count} 個の TMP_Text を置換しました。");
        }
    }

    private void ReplaceFontsInAllPrefabs()
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

        foreach (string guid in prefabGuids)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            bool changed = false;

            foreach (var tmp in prefab.GetComponentsInChildren<TMP_Text>(true))
            {
                if (tmp.font != newFontAsset)
                {
                    Undo.RecordObject(tmp, "Replace TMP Font in Prefab");
                    tmp.font = newFontAsset;
                    EditorUtility.SetDirty(tmp);
                    changed = true;
                }
            }

            if (changed)
            {
                PrefabUtility.SavePrefabAsset(prefab);
                Debug.Log($"プレハブ '{prefab.name}' のフォントを更新しました。");
            }
        }
    }
}
