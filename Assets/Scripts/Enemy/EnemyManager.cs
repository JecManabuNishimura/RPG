using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    private List<string[]> csvDatas = new();

    private void Awake()
    {
        Instance ??= this;
        csvDatas = DataReader.ReadData("EnemyData");
        
    }
    
    public Parameter GetParam(string name)
    {
        var result = csvDatas.Where(_ => _.Length > 0 && _[0] == name);
        Parameter param = new Parameter();
        foreach (var val in result)
        {
            param.Hp = int.Parse(val[1]);
            param.Mp = int.Parse(val[2]);
            param.Atk = int.Parse(val[3]);
            param.Def = int.Parse(val[4]);
            param.Qui = int.Parse(val[5]);
            param.Exp = int.Parse(val[6]);
        }

        return param;
    }

    public int GetGold(string name)
    {
        var result = csvDatas.Where(_ => _.Length > 0 && _[0] == name);
        int gold = 0;
        foreach (var val in result)
        {
            gold = int.Parse(val[7]);
        }

        return gold;
    }
    
}
