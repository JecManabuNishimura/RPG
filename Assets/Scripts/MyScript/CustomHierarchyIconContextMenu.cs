using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

/// <summary>
/// Hierarchy右クリックメニュー。
/// </summary>
public static class CustomHierarchyIconContextMenu
{
    private const string MenuPath = "GameObject/アイコン変更...";

    [MenuItem(MenuPath, false, 49)]
    private static void OpenIconPicker(MenuCommand command)
    {
        GameObject clickedObject = command.context as GameObject;

        GameObject[] targets;

        // 複数選択したオブジェクトのうち一つを右クリックした場合
        if (clickedObject != null &&
            Selection.gameObjects.Contains(clickedObject))
        {
            targets = Selection.gameObjects;
        }
        // 選択外のオブジェクトを右クリックした場合
        else if (clickedObject != null)
        {
            targets = new[]
            {
                clickedObject
            };
        }
        else
        {
            targets = Selection.gameObjects;
        }

        targets = targets
            .Where(target => target != null)
            .Distinct()
            .ToArray();

        if (targets.Length == 0)
        {
            return;
        }

        CustomHierarchyIconPickerWindow.Open(targets);
    }

    [MenuItem(MenuPath, true)]
    private static bool ValidateOpenIconPicker()
    {
        return Selection.activeGameObject != null;
    }
}

/// <summary>
/// アイコン選択ウィンドウ。
/// </summary>
public class CustomHierarchyIconPickerWindow : EditorWindow
{
    private const float IconSize = 56f;
    private const float CellWidth = 72f;

    private readonly List<Sprite> icons = new();

    private GameObject[] targets = Array.Empty<GameObject>();

    private Vector2 scrollPosition;
    private string searchText = string.Empty;
    private Sprite directIcon;
    private const string GeneratedIconFolder =
    "Assets/Editor/GeneratedHierarchyIcons";

    public static void Open(GameObject[] selectedObjects)
    {
        CustomHierarchyIconPickerWindow window =
            CreateInstance<CustomHierarchyIconPickerWindow>();

        window.targets = selectedObjects
            .Where(target => target != null)
            .Distinct()
            .ToArray();

        window.titleContent =
            new GUIContent("アイコン変更");

        window.minSize =
            new Vector2(400f, 350f);

        window.ReloadIcons();
        window.ShowUtility();
    }

    private void OnGUI()
    {
        if (targets == null || targets.Length == 0)
        {
            EditorGUILayout.HelpBox(
                "変更対象のGameObjectがありません。",
                MessageType.Warning);

            return;
        }

        DrawTargetArea();

        EditorGUILayout.Space(5f);

        DrawDirectSelection();

        EditorGUILayout.Space(5f);

        DrawSearchArea();

        EditorGUILayout.Space(5f);

        DrawIconGrid();
    }

    private void DrawTargetArea()
    {
        using (new EditorGUILayout.VerticalScope(
                   EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField(
                "変更対象",
                EditorStyles.boldLabel);

            if (targets.Length == 1)
            {
                EditorGUILayout.ObjectField(
                    targets[0],
                    typeof(GameObject),
                    true);
            }
            else
            {
                EditorGUILayout.LabelField(
                    $"{targets.Length}個のGameObject");
            }

            Texture2D currentIcon =
                EditorGUIUtility.GetIconForObject(targets[0]);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(
                    "現在のアイコン",
                    GUILayout.Width(100f));

                if (currentIcon != null)
                {
                    GUILayout.Label(
                        currentIcon,
                        GUILayout.Width(24f),
                        GUILayout.Height(24f));

                    GUILayout.Label(currentIcon.name);
                }
                else
                {
                    GUILayout.Label("標準アイコン");
                }
            }

            if (GUILayout.Button("標準アイコンに戻す"))
            {
                ApplyIcon(null);
            }
        }
    }

    private void DrawDirectSelection()
    {
        using (new EditorGUILayout.VerticalScope(
                   EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField(
                "画像を直接指定",
                EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                directIcon =
                 (Sprite)EditorGUILayout.ObjectField(
                     directIcon,
                     typeof(Sprite),
                     false);

                using (new EditorGUI.DisabledScope(
                           directIcon == null))
                {
                    if (GUILayout.Button(
                            "適用",
                            GUILayout.Width(60f)))
                    {
                        ApplyIcon(directIcon);
                    }
                }
            }
        }
    }

    private void DrawSearchArea()
    {
        using (new EditorGUILayout.HorizontalScope(
                   EditorStyles.toolbar))
        {
            searchText = EditorGUILayout.TextField(
                searchText,
                GUILayout.ExpandWidth(true));

            if (GUILayout.Button(
                    "消去",
                    EditorStyles.toolbarButton,
                    GUILayout.Width(45f)))
            {
                searchText = string.Empty;
                GUI.FocusControl(null);
            }

            if (GUILayout.Button(
                    "再読込",
                    EditorStyles.toolbarButton,
                    GUILayout.Width(55f)))
            {
                ReloadIcons();
            }
        }
    }

    private void DrawIconGrid()
    {
        List<Sprite> filteredIcons = icons
            .Where(MatchesSearch)
            .ToList();

        EditorGUILayout.LabelField(
            $"画像一覧：{filteredIcons.Count}件",
            EditorStyles.miniLabel);

        scrollPosition =
            EditorGUILayout.BeginScrollView(scrollPosition);

        float availableWidth =
            Mathf.Max(position.width - 24f, CellWidth);

        int columnCount =
            Mathf.Max(
                1,
                Mathf.FloorToInt(
                    availableWidth / CellWidth));

        int index = 0;

        while (index < filteredIcons.Count)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                for (int column = 0;
                     column < columnCount;
                     column++)
                {
                    if (index >= filteredIcons.Count)
                    {
                        GUILayout.Space(CellWidth);
                        continue;
                    }

                    DrawIconCell(filteredIcons[index]);
                    index++;
                }

                GUILayout.FlexibleSpace();
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawIconCell(Sprite sprite)
    {
        using (new EditorGUILayout.VerticalScope(
                   GUILayout.Width(CellWidth)))
        {
            string path =
                AssetDatabase.GetAssetPath(sprite);

            string tooltip =
                $"{sprite.name}\n{path}";

            // ボタン領域を先に確保
            Rect buttonRect =
                GUILayoutUtility.GetRect(
                    IconSize,
                    IconSize,
                    GUILayout.Width(IconSize),
                    GUILayout.Height(IconSize));

            // 背景とクリック判定
            if (GUI.Button(
                    buttonRect,
                    GUIContent.none))
            {
                ApplyIcon(sprite);
            }

            // AssetPreviewを使用せず、
            // Spriteの画像範囲を直接描画
            DrawSpritePreview(
                buttonRect,
                sprite);

            // ツールチップ
            GUI.Label(
                buttonRect,
                new GUIContent(
                    string.Empty,
                    tooltip));

            GUIStyle labelStyle =
                new GUIStyle(
                    EditorStyles.centeredGreyMiniLabel)
                {
                    clipping = TextClipping.Clip
                };

            GUILayout.Label(
                sprite.name,
                labelStyle,
                GUILayout.Width(IconSize));
        }
    }

    private static void DrawSpritePreview(
    Rect destinationRect,
    Sprite sprite)
    {
        if (sprite == null ||
            sprite.texture == null)
        {
            return;
        }

        if (Event.current.type != UnityEngine.EventType.Repaint)
        {
            return;
        }

        Texture2D texture = sprite.texture;

        Rect spriteRect;

        try
        {
            // Texture内でSpriteが使用している範囲
            spriteRect = sprite.textureRect;
        }
        catch
        {
            // Tight PackingされたSpriteなどへの保険
            spriteRect = sprite.rect;
        }

        if (spriteRect.width <= 0f ||
            spriteRect.height <= 0f)
        {
            return;
        }

        // Texture座標を0～1へ変換
        Rect textureCoordinates =
            new Rect(
                spriteRect.x / texture.width,
                spriteRect.y / texture.height,
                spriteRect.width / texture.width,
                spriteRect.height / texture.height);

        // ボタンの縁に重ならないよう少し内側にする
        Rect previewRect =
            new Rect(
                destinationRect.x + 4f,
                destinationRect.y + 4f,
                destinationRect.width - 8f,
                destinationRect.height - 8f);

        // Spriteの縦横比を維持する
        float spriteAspect =
            spriteRect.width / spriteRect.height;

        float previewAspect =
            previewRect.width / previewRect.height;

        if (spriteAspect > previewAspect)
        {
            float height =
                previewRect.width / spriteAspect;

            previewRect.y +=
                (previewRect.height - height) * 0.5f;

            previewRect.height = height;
        }
        else
        {
            float width =
                previewRect.height * spriteAspect;

            previewRect.x +=
                (previewRect.width - width) * 0.5f;

            previewRect.width = width;
        }

        Color previousColor = GUI.color;

        GUI.color = Color.white;

        GUI.DrawTextureWithTexCoords(
            previewRect,
            texture,
            textureCoordinates,
            true);

        GUI.color = previousColor;
    }
    private bool MatchesSearch(Sprite icon)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return true;
        }

        string path =
            AssetDatabase.GetAssetPath(icon);

        return icon.name.IndexOf(
                   searchText,
                   StringComparison.OrdinalIgnoreCase) >= 0
               ||
               path.IndexOf(
                   searchText,
                   StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void ReloadIcons()
    {
        icons.Clear();

        string[] guids =
            AssetDatabase.FindAssets(
                "t:Texture2D",
                new[]
                {
                "Assets"
                });

        foreach (string guid in guids)
        {
            string path =
                AssetDatabase.GUIDToAssetPath(guid);

            // 元テクスチャと、その中にある分割Spriteをすべて取得
            UnityEngine.Object[] subAssets =
                AssetDatabase.LoadAllAssetsAtPath(path);

            foreach (UnityEngine.Object subAsset in subAssets)
            {
                if (subAsset is not Sprite sprite)
                {
                    continue;
                }

                icons.Add(sprite);
            }
        }

        icons.Sort((left, right) =>
        {
            string leftPath =
                AssetDatabase.GetAssetPath(left);

            string rightPath =
                AssetDatabase.GetAssetPath(right);

            int pathResult = string.Compare(
                leftPath,
                rightPath,
                StringComparison.OrdinalIgnoreCase);

            if (pathResult != 0)
            {
                return pathResult;
            }

            return string.Compare(
                left.name,
                right.name,
                StringComparison.OrdinalIgnoreCase);
        });

        Repaint();
    }

    private void ApplyIcon(Sprite sprite)
    {
        GameObject[] validTargets = targets
            .Where(target => target != null)
            .ToArray();

        if (validTargets.Length == 0)
        {
            return;
        }

        Texture2D iconTexture =
            sprite == null
                ? null
                : GetOrCreateIconTexture(sprite);

        if (sprite != null &&
            iconTexture == null)
        {
            Debug.LogError(
                $"Spriteのアイコン生成に失敗しました: {sprite.name}");

            return;
        }

        UnityEngine.Object[] undoTargets =
            validTargets
                .Cast<UnityEngine.Object>()
                .ToArray();

        Undo.RecordObjects(
            undoTargets,
            sprite == null
                ? "Reset Hierarchy Icon"
                : "Change Hierarchy Icon");

        HashSet<Scene> changedScenes =
            new HashSet<Scene>();

        foreach (GameObject target in validTargets)
        {
            EditorGUIUtility.SetIconForObject(
                target,
                iconTexture);

            EditorUtility.SetDirty(target);

            if (target.scene.IsValid() &&
                target.scene.isLoaded)
            {
                changedScenes.Add(target.scene);
            }
        }

        foreach (Scene scene in changedScenes)
        {
            EditorSceneManager.MarkSceneDirty(scene);
        }

        EditorApplication.RepaintHierarchyWindow();
        SceneView.RepaintAll();

        Close();
    }
    

    /// <summary>
    /// 分割Sprite部分だけをTexture2Dとして生成する。
    /// </summary>
    private static Texture2D GetOrCreateIconTexture(
        Sprite sprite)
    {
        if (sprite == null)
        {
            return null;
        }

        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(
            sprite,
            out string guid,
            out long localId);

        string iconPath =
            $"{GeneratedIconFolder}/{guid}_{localId}.png";

        // すでに生成済みなら再利用
        Texture2D existingIcon =
            AssetDatabase.LoadAssetAtPath<Texture2D>(
                iconPath);

        if (existingIcon != null)
        {
            return existingIcon;
        }

        Directory.CreateDirectory(
            Path.GetFullPath(GeneratedIconFolder));

        Rect spriteRect = sprite.textureRect;

        int width =
            Mathf.Max(
                1,
                Mathf.RoundToInt(spriteRect.width));

        int height =
            Mathf.Max(
                1,
                Mathf.RoundToInt(spriteRect.height));

        RenderTexture renderTexture =
            RenderTexture.GetTemporary(
                width,
                height,
                0,
                RenderTextureFormat.ARGB32,
                RenderTextureReadWrite.sRGB);

        RenderTexture previousRenderTexture =
            RenderTexture.active;

        try
        {
            RenderTexture.active = renderTexture;

            GL.Clear(
                true,
                true,
                Color.clear);

            Texture2D sourceTexture =
                sprite.texture;

            Vector2 scale =
                new Vector2(
                    spriteRect.width /
                    sourceTexture.width,

                    spriteRect.height /
                    sourceTexture.height);

            Vector2 offset =
                new Vector2(
                    spriteRect.x /
                    sourceTexture.width,

                    spriteRect.y /
                    sourceTexture.height);

            // Sprite部分だけRenderTextureへ描画
            Graphics.Blit(
                sourceTexture,
                renderTexture,
                scale,
                offset);

            Texture2D croppedTexture =
                new Texture2D(
                    width,
                    height,
                    TextureFormat.RGBA32,
                    false);

            croppedTexture.ReadPixels(
                new Rect(
                    0f,
                    0f,
                    width,
                    height),
                0,
                0);

            croppedTexture.Apply();

            byte[] pngData =
                croppedTexture.EncodeToPNG();

            File.WriteAllBytes(
                Path.GetFullPath(iconPath),
                pngData);

            DestroyImmediate(croppedTexture);
        }
        finally
        {
            RenderTexture.active =
                previousRenderTexture;

            RenderTexture.ReleaseTemporary(
                renderTexture);
        }

        AssetDatabase.ImportAsset(
            iconPath,
            ImportAssetOptions.ForceUpdate);

        TextureImporter importer =
            AssetImporter.GetAtPath(iconPath)
            as TextureImporter;

        if (importer != null)
        {
            importer.textureType =
                TextureImporterType.Default;

            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.wrapMode =
                TextureWrapMode.Clamp;

            importer.filterMode =
                sprite.texture.filterMode;

            importer.textureCompression =
                TextureImporterCompression.Uncompressed;

            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Texture2D>(
            iconPath);
    }
}