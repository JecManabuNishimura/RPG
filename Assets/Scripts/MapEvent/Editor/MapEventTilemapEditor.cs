
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(MapEventTilemap))]
public class MapEventTilemapEditor : Editor
{
    private bool editMode;
    private int selectedIndex = -1;

    private SerializedProperty tilemapProp;
    private SerializedProperty eventsProp;

    private void OnEnable()
    {
        tilemapProp =
            serializedObject.FindProperty("eventTilemap");

        eventsProp =
            serializedObject.FindProperty("events");
    }

    private void OnDisable()
    {
        editMode = false;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(tilemapProp);

        EditorGUILayout.Space(10);

        bool nextMode = GUILayout.Toggle(
            editMode,
            editMode ? "イベント編集を終了" : "イベント編集を開始",
            "Button",
            GUILayout.Height(30)
        );

        if (nextMode != editMode)
        {
            editMode = nextMode;
            SceneView.RepaintAll();
        }

        EditorGUILayout.HelpBox(
            editMode
                ? "Sceneビューでイベントタイルを左クリックしてください"
                : "編集ボタンでイベントマスを選択できます",
            MessageType.Info
        );

        EditorGUILayout.LabelField(
            "登録イベント数: " + eventsProp.arraySize
        );

        EditorGUILayout.Space(10);

        DrawEventNavigation();
        DrawSelectedEvent();
        

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSelectedEvent()
    {
        if (selectedIndex < 0 ||
            selectedIndex >= eventsProp.arraySize)
        {
            EditorGUILayout.LabelField(
                "イベントが選択されていません");
            return;
        }

        SerializedProperty data =
            eventsProp.GetArrayElementAtIndex(selectedIndex);

        EditorGUILayout.LabelField(
            "選択イベント",
            EditorStyles.boldLabel
        );

        EditorGUILayout.PropertyField(
            data.FindPropertyRelative("cellPosition"),
            new GUIContent("タイル座標")
        );

        EditorGUILayout.PropertyField(
            data.FindPropertyRelative("eventName"),
            new GUIContent("イベント名")
        );

        EditorGUILayout.PropertyField(
            data.FindPropertyRelative("eventType"),
            new GUIContent("イベント種類")
        );

        SerializedProperty typeProp =
            data.FindPropertyRelative("eventType");

        if (typeProp.enumValueIndex ==
            (int)MapEventType.SceneMove)
        {
            DrawSceneField(data);

            EditorGUILayout.PropertyField(
                data.FindPropertyRelative("spawnPointId"),
                new GUIContent("出現地点ID")
            );
        }

        EditorGUILayout.Space(10);

        if (GUILayout.Button("このイベントを削除"))
        {
            eventsProp.DeleteArrayElementAtIndex(
                selectedIndex);

            selectedIndex = -1;
            SceneView.RepaintAll();
        }
    }

    private void DrawEventNavigation()
    {
        int count = eventsProp.arraySize;

        if (count == 0)
            return;

        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("◀ 前へ"))
        {
            SelectEvent(selectedIndex <= 0
                ? count - 1
                : selectedIndex - 1);
        }

        EditorGUILayout.LabelField(
            $"{(selectedIndex >= 0 ? selectedIndex + 1 : 0)} / {count}",
            GUILayout.Width(60));

        if (GUILayout.Button("次へ ▶"))
        {
            SelectEvent((selectedIndex + 1) % count);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void SelectEvent(int index)
    {
        if (index < 0 || index >= eventsProp.arraySize)
            return;

        selectedIndex = index;

        MapEventTilemap mapEvent =
            (MapEventTilemap)target;

        Tilemap tilemap = mapEvent.EventTilemap;

        if (tilemap == null)
            return;

        SerializedProperty data =
            eventsProp.GetArrayElementAtIndex(index);

        Vector3Int cell =
            data.FindPropertyRelative("cellPosition")
                .vector3IntValue;

        Vector3 worldPosition =
            tilemap.GetCellCenterWorld(cell);

        // Sceneビューのカメラを対象地点へ移動
        SceneView sceneView = SceneView.lastActiveSceneView;

        if (sceneView != null)
        {
            sceneView.LookAt(
                worldPosition,
                sceneView.rotation,
                2f,
                false
            );
            sceneView.Repaint();
        }

        Repaint();
    }

    private void DrawSceneField(SerializedProperty data)
    {
        SerializedProperty pathProp =
            data.FindPropertyRelative("scenePath");

        SceneAsset currentScene =
            AssetDatabase.LoadAssetAtPath<SceneAsset>(
                pathProp.stringValue
            );

        EditorGUI.BeginChangeCheck();

        SceneAsset newScene =
            (SceneAsset)EditorGUILayout.ObjectField(
                "移動先シーン",
                currentScene,
                typeof(SceneAsset),
                false
            );

        if (EditorGUI.EndChangeCheck())
        {
            pathProp.stringValue = newScene != null
                ? AssetDatabase.GetAssetPath(newScene)
                : "";
        }

        if (newScene != null &&
            !IsSceneInBuild(pathProp.stringValue))
        {
            EditorGUILayout.HelpBox(
                "移動先シーンをBuild ProfilesのScene Listに追加してください",
                MessageType.Warning
            );
        }
    }

    private static bool IsSceneInBuild(string path)
    {
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled && scene.path == path)
                return true;
        }

        return false;
    }

    private void OnSceneGUI()
    {
        MapEventTilemap mapEvent =
            (MapEventTilemap)target;

        Tilemap tilemap = mapEvent.EventTilemap;

        if (tilemap == null)
            return;

        DrawEventMarkers(mapEvent, tilemap);

        if (!editMode)
            return;

        Event e = Event.current;

        if (e.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(
                GUIUtility.GetControlID(FocusType.Passive));
        }

        if (e.type != EventType.MouseDown ||
            e.button != 0 ||
            e.alt)
            return;

        Ray ray =
            HandleUtility.GUIPointToWorldRay(e.mousePosition);

        Plane plane = new Plane(
            tilemap.transform.forward,
            tilemap.transform.position
        );

        if (!plane.Raycast(ray, out float distance))
            return;

        Vector3 worldPosition =
            ray.GetPoint(distance);

        Vector3Int cell =
            tilemap.WorldToCell(worldPosition);

        if (!tilemap.HasTile(cell))
            return;

        SelectOrCreateEvent(mapEvent, cell);

        e.Use();
    }

    private void SelectOrCreateEvent(
        MapEventTilemap mapEvent,
        Vector3Int cell)
    {
        serializedObject.Update();

        for (int i = 0; i < eventsProp.arraySize; i++)
        {
            SerializedProperty data =
                eventsProp.GetArrayElementAtIndex(i);

            Vector3Int position =
                data.FindPropertyRelative("cellPosition")
                    .vector3IntValue;

            if (position == cell)
            {
                selectedIndex = i;
                Repaint();
                SceneView.RepaintAll();
                return;
            }
        }

        // イベント未登録なら新規作成
        int index = eventsProp.arraySize;
        eventsProp.InsertArrayElementAtIndex(index);

        SerializedProperty newData =
            eventsProp.GetArrayElementAtIndex(index);

        newData.FindPropertyRelative("cellPosition")
            .vector3IntValue = cell;

        newData.FindPropertyRelative("eventName")
            .stringValue = "New Event";

        newData.FindPropertyRelative("eventType")
            .enumValueIndex = (int)MapEventType.SceneMove;

        newData.FindPropertyRelative("scenePath")
            .stringValue = "";

        newData.FindPropertyRelative("spawnPointId")
            .stringValue = "";

        serializedObject.ApplyModifiedProperties();

        selectedIndex = index;

        Repaint();
        SceneView.RepaintAll();
    }

    private void DrawEventMarkers(
        MapEventTilemap mapEvent,
        Tilemap tilemap)
    {
        for (int i = 0; i < mapEvent.Events.Count; i++)
        {
            MapEventData data = mapEvent.Events[i];

            Vector3 center =
                tilemap.GetCellCenterWorld(data.cellPosition);

            bool selected = i == selectedIndex;

            Handles.color = selected
                ? Color.yellow
                : Color.cyan;

            Handles.DrawWireCube(
                center,
                Vector3.Scale(
                    tilemap.cellSize,
                    tilemap.transform.lossyScale
                )
            );

            Handles.Label(
                center + Vector3.up * 0.6f,
                data.eventName
            );
        }
    }
}
