
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class SpawnTileData
{
    public string spawnPointId;
    public Vector3Int cellPosition;
}

[RequireComponent(typeof(Tilemap))]
public class SpawnTilemap : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;

    [SerializeField, HideInInspector]       
    private List<SpawnTileData> spawnPoints = new();

    public Tilemap Tilemap => tilemap;
    public List<SpawnTileData> SpawnPoints => spawnPoints;

    private void Reset()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public bool TryGetSpawnPosition(
        string spawnPointId,
        out Vector3 position)
    {
        position = default;

        if (tilemap == null)
            return false;

        foreach (var data in spawnPoints)
        {
            if (data.spawnPointId != spawnPointId)
                continue;

            if (!tilemap.HasTile(data.cellPosition))
                return false;

            position = tilemap.GetCellCenterWorld(
                data.cellPosition);

            return true;
        }

        return false;
    }
}
