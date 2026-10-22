
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

    private Vector3Int spawnCell;
    private bool ignoreSpawnCell;

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

        Vector3 playerPos =
            PlayerDataRepository.Instance.playerPos;

        Vector3Int cell = eventTilemap.WorldToCell(playerPos);

        // スポーンしたマスにいる間はイベント無効
        if (ignoreSpawnCell)
        {
            if (cell == spawnCell)
                return;

            // スポーンしたマスを離れた
            ignoreSpawnCell = false;
        }

        // 同じマスなら何もしない
        if (hasPreviousCell && cell == previousCell)
            return;

        previousCell = cell;
        hasPreviousCell = true;

        if (!eventTilemap.HasTile(cell))
            return;

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
            return;

        string sceneName =
            System.IO.Path.GetFileNameWithoutExtension(
                data.scenePath);

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"シーンが見つかりません: {sceneName}");
            return;
        }

        transitioning = true;

        // Player生成を予約
        GameManager.Instance.ReservePlayerSpawn(
            data.spawnPointId);

        // シーン移動
        SceneManager.LoadScene(sceneName);
    }

    public void IgnoreSpawnCell()
    {
        Vector3 playerPos =
            PlayerDataRepository.Instance.playerPos;

        spawnCell = eventTilemap.WorldToCell(playerPos);

        ignoreSpawnCell = true;

        // 現在位置を記録
        previousCell = spawnCell;
        hasPreviousCell = true;
    }
}
