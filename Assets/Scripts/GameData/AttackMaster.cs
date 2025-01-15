using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackMaster", menuName="Master/CreateAttackMaster") ]
public class AttackMaster : ScriptableObject
{
    private static AttackMaster _entity;

    public static AttackMaster Entity
    {
        get
        {
            //初アクセス時にロードする
            if (_entity == null)
            {
                _entity = Resources.Load<AttackMaster>("Master/AttackMaster");

                //ロード出来なかった場合はエラーログを表示
                if (_entity == null)
                {
                    Debug.LogError(nameof(EffectMaster) + " not found");
                }
            }

            return _entity;
        }
    }

    public List<AttackData> AttackDatas = new();
}


[Serializable]
public class AttackData
{
    public string name;
    [Effect]
    public int effectId;

    public AttackScope scope;
}

public enum AttackScope
{
    One,
    All,
    
}