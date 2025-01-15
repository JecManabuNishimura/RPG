using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;

public class EnemyCharactor : EnemyData 
{
    private void Start()
    {
        parameter = EnemyManager.Instance.GetParam(CharaName);
        gold = EnemyManager.Instance.GetGold(CharaName);
        BattleManager.Instance.EnemyList.Add(this);
        Initialize();
    }
}


