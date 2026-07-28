using UnityEditor;
using UnityEngine;

/// <summary>
/// GameObjectに設定されたカスタムアイコンを
/// Hierarchyの標準アイコン位置へ描画する。
/// </summary>
[InitializeOnLoad]
public static class HierarchyCustomIconDrawer
{
    private const float IconSize = 16f;

    static HierarchyCustomIconDrawer()
    {
#if UNITY_6000_5_OR_NEWER
        // Unity 6.5以降
        EditorApplication.hierarchyWindowItemByEntityIdOnGUI
            -= OnHierarchyGUI;

        EditorApplication.hierarchyWindowItemByEntityIdOnGUI
            += OnHierarchyGUI;
#else
        // Unity 6.4以前
        EditorApplication.hierarchyWindowItemOnGUI
            -= OnHierarchyGUI;

        EditorApplication.hierarchyWindowItemOnGUI
            += OnHierarchyGUI;
#endif

        EditorApplication.delayCall += () =>
        {
            EditorApplication.RepaintHierarchyWindow();
        };
    }

#if UNITY_6000_5_OR_NEWER

    /// <summary>
    /// Unity 6.5以降用。
    /// </summary>
    private static void OnHierarchyGUI(
        EntityId entityId,
        Rect selectionRect)
    {
        GameObject gameObject =
            EditorUtility.EntityIdToObject(entityId)
            as GameObject;

        DrawIcon(gameObject, selectionRect);
    }

#else

    /// <summary>
    /// Unity 6.4以前用。
    /// </summary>
    private static void OnHierarchyGUI(
        int instanceId,
        Rect selectionRect)
    {
        GameObject gameObject =
            EditorUtility.InstanceIDToObject(instanceId)
            as GameObject;

        DrawIcon(gameObject, selectionRect);
    }

#endif

    private static void DrawIcon(
    GameObject gameObject,
    Rect selectionRect)
    {
        if (gameObject == null)
        {
            return;
        }

        if (Event.current.type != UnityEngine.EventType.Repaint)
        {
            return;
        }

        Texture2D icon =
            EditorGUIUtility.GetIconForObject(gameObject);

        if (icon == null)
        {
            // カスタムアイコン未設定なら
            // Unity標準アイコンをそのまま表示する
            return;
        }

        Rect iconRect = new Rect(
            selectionRect.x,
            selectionRect.y +
            (selectionRect.height - IconSize) * 0.5f,
            IconSize,
            IconSize);

        // 標準アイコンを完全に隠すため、
        // アイコンより少し広い領域を塗りつぶす
        Rect clearRect = new Rect(
            iconRect.x - 1f,
            selectionRect.y,
            IconSize + 2f,
            selectionRect.height);

        bool isSelected =
            IsHierarchyObjectSelected(gameObject);

        bool isHovered =
            selectionRect.Contains(Event.current.mousePosition);

        bool isHierarchyFocused =
            IsHierarchyWindowFocused();

        Color backgroundColor =
            GetHierarchyBackgroundColor(
                isSelected,
                isHovered,
                isHierarchyFocused);

        int previousDepth = GUI.depth;
        Color previousColor = GUI.color;

        // Unity標準描画より手前に表示
        GUI.depth = -10000;

        // 標準のキューブアイコンを背景色で消す
        EditorGUI.DrawRect(
            clearRect,
            backgroundColor);

        // カスタムアイコンを描画
        GUI.color = gameObject.activeInHierarchy
            ? Color.white
            : new Color(1f, 1f, 1f, 0.5f);

        GUI.DrawTexture(
            iconRect,
            icon,
            ScaleMode.ScaleToFit,
            true);

        GUI.color = previousColor;
        GUI.depth = previousDepth;
    }
    /// <summary>
    /// GameObjectがHierarchyで選択されているか確認する。
    /// 複数選択にも対応。
    /// </summary>
    private static bool IsHierarchyObjectSelected(
        GameObject gameObject)
    {
        int targetId = gameObject.GetInstanceID();
        int[] selectedIds = Selection.instanceIDs;

        for (int i = 0; i < selectedIds.Length; i++)
        {
            if (selectedIds[i] == targetId)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 現在Hierarchyウィンドウにフォーカスがあるか確認する。
    /// </summary>
    private static bool IsHierarchyWindowFocused()
    {
        EditorWindow focusedWindow =
            EditorWindow.focusedWindow;

        if (focusedWindow == null)
        {
            return false;
        }

        string typeName =
            focusedWindow.GetType().Name;

        return typeName.Contains("Hierarchy");
    }

    /// <summary>
    /// Hierarchyの状態に合わせた背景色を返す。
    /// </summary>
    private static Color GetHierarchyBackgroundColor(
        bool isSelected,
        bool isHovered,
        bool isHierarchyFocused)
    {
        if (EditorGUIUtility.isProSkin)
        {
            // ダークテーマ
            if (isSelected)
            {
                return isHierarchyFocused
                    ? new Color32(44, 93, 135, 255)
                    : new Color32(77, 77, 77, 255);
            }

            if (isHovered)
            {
                return new Color32(69, 69, 69, 255);
            }

            return new Color32(56, 56, 56, 255);
        }

        // ライトテーマ
        if (isSelected)
        {
            return isHierarchyFocused
                ? new Color32(58, 114, 176, 255)
                : new Color32(174, 174, 174, 255);
        }

        if (isHovered)
        {
            return new Color32(211, 211, 211, 255);
        }

        return new Color32(194, 194, 194, 255);
    }
}