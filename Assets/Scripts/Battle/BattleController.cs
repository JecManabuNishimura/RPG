using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class BattleController : MonoBehaviour
{
    [SerializeField] private Tilemap RegionMap;
    [SerializeField] private List<MapEnemyData> EnemyList = new();
    [SerializeField] private int encout;

    public int nextEnemyEncount;

    void Start()
    {
        BattleManager.Instance._BattleController = this;
        BattleManager.Instance.EnemyDataList = EnemyList;
        
        SetNextEnemyEncounter();
    }
    
    public GameObject SelectEnemy()
    {
        
        RegionMap = GameObject.Find("RegionMap").GetComponent<Tilemap>();
        List<MapEnemyData> chooseEnemy = GetRegionEnemy(PlayerDataRepository.Instance.playerPos);
        int totalWeight = chooseEnemy.Sum(data => data.weight);
        return ChooseRandomEnemy(chooseEnemy, totalWeight);
    }
    
    private GameObject ChooseRandomEnemy(List<MapEnemyData> enemies, int totalWeight)
    {
        int randomNumber = UnityEngine.Random.Range(1, totalWeight + 1);

        foreach (var enemy in enemies)
        {
            if (randomNumber <= enemy.weight)
            {
                return enemy.enemy;
            }
            randomNumber -= enemy.weight;
        }

        return null;
    }

    public bool FindRegionEnemy()
    {
        if (RegionMap == null)
        {
            RegionMap = GameObject.Find("RegionMap")?.GetComponent<Tilemap>();
        }

        if (RegionMap != null)
        {
            List<MapEnemyData> chooseEnemy = GetRegionEnemy(PlayerDataRepository.Instance.playerPos);
            if (chooseEnemy != null)
            {
                return chooseEnemy.Count != 0;    
            }
        }

        return false;


    }

    public void SetNextEnemyEncounter()
    {
        nextEnemyEncount = (Random.Range(0, encout - 1)) + (Random.Range(0, encout - 1)) + 1;
    }

    private List<MapEnemyData> GetRegionEnemy(Vector2 playerPos)
    {
        if (playerPos.x < 0)
        {
            playerPos.x -= 1;
        }
        if (playerPos.y < 0)
        {
            playerPos.y -= 1;
        }
        Vector3Int cellPos = new Vector3Int((int)playerPos.x,(int)playerPos.y);
        if (RegionMap.HasTile(cellPos))
        {
            List<MapEnemyData> enemy = new List<MapEnemyData>();
            int cellId = int.Parse(RegionMap.GetTile(cellPos).name);
            foreach (var e in EnemyList)
            {
                if (e.RegionId.x == cellId || e.RegionId.y == cellId || e.RegionId.z == cellId)
                {
                    enemy.Add(e);
                }
            }
            return enemy;
        }
        else
        {
            return null;
        }

    }
}

[Serializable]
public class MapEnemyData
{
    public string groupName;
    public GameObject enemy;
    public int weight;
    public Vector3Int RegionId;
}

