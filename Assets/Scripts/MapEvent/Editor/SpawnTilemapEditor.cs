
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(SpawnTilemap))]
public class SpawnTilemapEditor : Editor
{
    private bool editMode;
    private int selectedIndex = -1;

    private SerializedProperty tilemapProp;
    private SerializedProperty spawnPointsProp;

    private void OnEnable()
    {
        tilemapProp =
            serializedObject.FindProperty("tilemap");

        spawnPointsProp =
            serializedObject.FindProperty("spawnPoints");
    }

    private void OnDisable()
    {
        editMode = false;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(
            tilemapProp, new GUIContent("Spawn Tilemap"));

        EditorGUILayout.Space();

        bool newMode = GUILayout.Toggle(
            editMode,
            editMode ? "編集終了" : "出現地点を編集",
            "Button",
            GUILayout.Height(30));

        if (newMode != editMode)
        {
            editMode = newMode;
            SceneView.RepaintAll();
        }

        EditorGUILayout.HelpBox(
            editMode
                ? "SceneビューでSpawnタイルをクリックしてください"
                : "編集モードで出現地点を登録できます",
            MessageType.Info);

        EditorGUILayout.LabelField(
            $"登録地点数: {spawnPointsProp.arraySize}");

        DrawSpawnNavigation();
        DrawSelectedSpawn();


        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSpawnNavigation()
    {
        int count = spawnPointsProp.arraySize;

        if (count == 0)
            return;

        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("◀ 前へ"))
        {
            SelectSpawn(selectedIndex <= 0
                ? count - 1
                : selectedIndex - 1);
        }

        EditorGUILayout.LabelField(
            $"{(selectedIndex >= 0 ? selectedIndex + 1 : 0)} / {count}",
            GUILayout.Width(60));

        if (GUILayout.Button("次へ ▶"))
        {
            SelectSpawn((selectedIndex + 1) % count);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void SelectSpawn(int index)
    {
        if (index < 0 || index >= spawnPointsProp.arraySize)
            return;

        selectedIndex = index;

        SpawnTilemap spawn = (SpawnTilemap)target;
        Tilemap tilemap = spawn.Tilemap;

        if (tilemap == null)
            return;

        SerializedProperty data =
            spawnPointsProp.GetArrayElementAtIndex(index);

        Vector3Int cell =
            data.FindPropertyRelative("cellPosition")
                .vector3IntValue;

        Vector3 worldPosition =
            tilemap.GetCellCenterWorld(cell);

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

    private void DrawSelectedSpawn()
    {
        if (selectedIndex < 0 ||
            selectedIndex >= spawnPointsProp.arraySize)
            return;

        SerializedProperty data =
            spawnPointsProp.GetArrayElementAtIndex(
                selectedIndex);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(
            "選択中の出現地点",
            EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(
            data.FindPropertyRelative("cellPosition"),
            new GUIContent("セル座標"));

        EditorGUILayout.PropertyField(
            data.FindPropertyRelative("spawnPointId"),
            new GUIContent("出現地点ID"));

        if (GUILayout.Button("出現地点を削除"))
        {
            spawnPointsProp.DeleteArrayElementAtIndex(
                selectedIndex);

            selectedIndex = -1;
            SceneView.RepaintAll();
        }
    }

    private void OnSceneGUI()
    {
        SpawnTilemap spawn = (SpawnTilemap)target;
        Tilemap tilemap = spawn.Tilemap;

        if (tilemap == null)
            return;

        DrawMarkers(spawn, tilemap);

        if (!editMode)
            return;

        Event e = Event.current;

        if (e.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(
                GUIUtility.GetControlID(
                    FocusType.Passive));
        }

        if (e.type != EventType.MouseDown ||
            e.button != 0 || e.alt)
            return;

        Ray ray = HandleUtility.GUIPointToWorldRay(
            e.mousePosition);

        Plane plane = new Plane(
            tilemap.transform.forward,
            tilemap.transform.position);

        if (!plane.Raycast(ray, out float distance))
            return;

        Vector3 worldPos = ray.GetPoint(distance);
        Vector3Int cell = tilemap.WorldToCell(worldPos);

        if (!tilemap.HasTile(cell))
            return;

        SelectOrCreate(cell);

        e.Use();
    }

    private void SelectOrCreate(Vector3Int cell)
    {
        serializedObject.Update();

        for (int i = 0; i < spawnPointsProp.arraySize; i++)
        {
            var data =
                spawnPointsProp.GetArrayElementAtIndex(i);

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

        int index = spawnPointsProp.arraySize;

        spawnPointsProp.InsertArrayElementAtIndex(index);

        var newData =
            spawnPointsProp.GetArrayElementAtIndex(index);

        newData.FindPropertyRelative("cellPosition")
            .vector3IntValue = cell;

        newData.FindPropertyRelative("spawnPointId")
            .stringValue = $"Spawn_{cell.x}_{cell.y}";

        serializedObject.ApplyModifiedProperties();

        selectedIndex = index;

        Repaint();
        SceneView.RepaintAll();
    }

    private void DrawMarkers(
        SpawnTilemap spawn, Tilemap tilemap)
    {
        for (int i = 0; i < spawn.SpawnPoints.Count; i++)
        {
            SpawnTileData data = spawn.SpawnPoints[i];

            Vector3 center = tilemap.GetCellCenterWorld(
                data.cellPosition);

            Handles.color = i == selectedIndex
                ? Color.yellow
                : Color.cyan;

            Handles.DrawWireCube(
                center,
                Vector3.Scale(
                    tilemap.cellSize,
                    tilemap.transform.lossyScale));

            Handles.Label(
                center + Vector3.up * 0.6f,
                data.spawnPointId);
        }
    }
}
