
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public enum MapEventType
{
    SceneMove,
    // 今後、会話や宝箱などを追加
}

[Serializable]
public class MapEventData
{
    public string eventName;
    public Vector3Int cellPosition;
    public MapEventType eventType;
    public string scenePath;
    public string spawnPointId;
}

public class MapEventTilemap : MonoBehaviour
{
    [SerializeField] private Tilemap eventTilemap;

    [SerializeField, HideInInspector]
    private List<MapEventData> events = new();

    public Tilemap EventTilemap => eventTilemap;
    public List<MapEventData> Events => events;

    private Dictionary<Vector3Int, MapEventData> lookup;
    private Vector3Int previousCell;
    private bool hasPreviousCell;
    private bool transitioning;

    private void Awake()
    {
        if (eventTilemap == null)
            eventTilemap = GetComponent<Tilemap>();

        lookup = new Dictionary<Vector3Int, MapEventData>();

        foreach (var data in events)
        {
            lookup[data.cellPosition] = data;
        }
    }



    private void Update()
    {
        if (transitioning || eventTilemap == null)
            return;

        // PlayerDataRepositoryから現在位置を取得
        Vector3 playerPos =
            PlayerDataRepository.Instance.playerPos;

        // ワールド座標 → Tilemapのセル座標
        Vector3Int cell = eventTilemap.WorldToCell(playerPos);

        // 同じマスなら処理しない
        if (hasPreviousCell && cell == previousCell)
            return;

        previousCell = cell;
        hasPreviousCell = true;

        // イベントタイルが存在するか
        if (!eventTilemap.HasTile(cell))
            return;

        // 登録されたイベントを取得
        if (!lookup.TryGetValue(cell, out var data))
            return;

        ExecuteEvent(data);
    }



    private void ExecuteEvent(MapEventData data)
    {
        switch (data.eventType)
        {
            case MapEventType.SceneMove:
                MoveScene(data);
                break;
        }
    }

    private void MoveScene(MapEventData data)
    {
        if (string.IsNullOrEmpty(data.scenePath))
        {
            Debug.LogWarning("移動先シーンが未設定です");
            return;
        }

        // Assets/Scenes/town.unity → town
        string sceneName =
            System.IO.Path.GetFileNameWithoutExtension(
                data.scenePath
            );

        // シーンが読み込み可能か確認
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"シーンが見つかりません: {sceneName}"
            );
            return;
        }

        transitioning = true;

        SceneTransition.NextSpawnPointId =
            data.spawnPointId;

        SceneManager.LoadScene(sceneName);
    }
}
