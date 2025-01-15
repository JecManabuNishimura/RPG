using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectMaster", menuName="Master/CreateEffectMaster") ]
public class EffectMaster : ScriptableObject
{
    private static EffectMaster _entity;

    public static EffectMaster Entity
    {
        get
        {
            //初アクセス時にロードする
            if (_entity == null)
            {
                _entity = Resources.Load<EffectMaster>("Master/EffectMaster");

                //ロード出来なかった場合はエラーログを表示
                if (_entity == null)
                {
                    Debug.LogError(nameof(EffectMaster) + " not found");
                }
            }

            return _entity;
        }
    }

    public List<EffectData> effectData = new();
}


[Serializable]
public class EffectData
{
    public string name;
    public GameObject effect;
}