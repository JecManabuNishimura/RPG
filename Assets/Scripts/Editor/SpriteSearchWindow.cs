#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SpriteSearchWindow : EditorWindow
{
    private string folderPath;
    private Object targetObject;
    private string propertyPath;

    private string searchText = "";
    private Vector2 scroll;

    private List<Sprite> sprites = new List<Sprite>();

    private const float IconSize = 64f;
    private const float Padding = 8f;

    public static void Open(string folderPath, Object targetObject, string propertyPath)
    {
        SpriteSearchWindow window = CreateInstance<SpriteSearchWindow>();

        window.folderPath = folderPath;
        window.targetObject = targetObject;
        window.propertyPath = propertyPath;

        window.titleContent = new GUIContent("Sprite検索");
        window.minSize = new Vector2(420, 500);

        window.LoadSprites();
        window.ShowUtility();
    }

    private void LoadSprites()
    {
        sprites.Clear();

        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath });

        HashSet<Sprite> spriteSet = new HashSet<Sprite>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            Sprite mainSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (mainSprite != null)
            {
                spriteSet.Add(mainSprite);
            }

            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);

            foreach (Object subAsset in subAssets)
            {
                if (subAsset is Sprite sprite)
                {
                    spriteSet.Add(sprite);
                }
            }
        }

        sprites = spriteSet
            .OrderBy(s => s.name)
            .ToList();
    }

    private void OnGUI()
    {
        DrawHeader();
        DrawSpriteList();
    }

    private void DrawHeader()
    {
        EditorGUILayout.Space(6);

        EditorGUILayout.LabelField("検索対象フォルダ", folderPath);

        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("検索", GUILayout.Width(40));
            searchText = EditorGUILayout.TextField(searchText);
        }

        EditorGUILayout.Space(6);

        if (GUILayout.Button("再読み込み"))
        {
            LoadSprites();
        }

        EditorGUILayout.Space(6);
    }

    private void DrawSpriteList()
    {
        IEnumerable<Sprite> filteredSprites = sprites;

        if (!string.IsNullOrEmpty(searchText))
        {
            filteredSprites = filteredSprites.Where(sprite =>
                sprite.name.ToLower().Contains(searchText.ToLower())
            );
        }

        List<Sprite> list = filteredSprites.ToList();

        if (list.Count == 0)
        {
            EditorGUILayout.HelpBox("該当するSpriteがありません。", MessageType.Info);
            return;
        }

        scroll = EditorGUILayout.BeginScrollView(scroll);

        int columnCount = Mathf.Max(1, Mathf.FloorToInt(position.width / (IconSize + Padding + 60f)));
        int index = 0;

        while (index < list.Count)
        {
            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < columnCount && index < list.Count; i++)
            {
                DrawSpriteButton(list[index]);
                index++;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawSpriteButton(Sprite sprite)
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(IconSize + Padding));

        Rect buttonRect = GUILayoutUtility.GetRect(
            IconSize,
            IconSize,
            GUILayout.Width(IconSize),
            GUILayout.Height(IconSize)
        );

        if (GUI.Button(buttonRect, GUIContent.none))
        {
            ApplySprite(sprite);
            Close();
        }

        DrawSpritePreview(buttonRect, sprite);

        GUILayout.Label(
            sprite.name,
            EditorStyles.miniLabel,
            GUILayout.Width(IconSize + Padding),
            GUILayout.Height(32)
        );

        EditorGUILayout.EndVertical();
    }
    private void DrawSpritePreview(Rect rect, Sprite sprite)
    {
        if (sprite == null || sprite.texture == null)
        {
            return;
        }

        Texture2D texture = sprite.texture;
        Rect textureRect = sprite.textureRect;

        Rect texCoords = new Rect(
            textureRect.x / texture.width,
            textureRect.y / texture.height,
            textureRect.width / texture.width,
            textureRect.height / texture.height
        );

        GUI.DrawTextureWithTexCoords(
            rect,
            texture,
            texCoords,
            true
        );
    }

    private void ApplySprite(Sprite sprite)
    {
        if (targetObject == null)
        {
            return;
        }

        SerializedObject serializedObject = new SerializedObject(targetObject);
        SerializedProperty property = serializedObject.FindProperty(propertyPath);

        if (property == null)
        {
            Debug.LogWarning($"プロパティが見つかりません: {propertyPath}");
            return;
        }

        property.objectReferenceValue = sprite;
        serializedObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(targetObject);
    }
}
#endif