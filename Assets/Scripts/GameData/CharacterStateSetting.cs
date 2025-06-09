using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStateSetting", menuName="CharacterInitParam") ]
public class CharacterStateSetting : ScriptableObject
{
    private static CharacterStateSetting _entity;
    public static CharacterStateSetting Entity
    {
        get
        {
            //初アクセス時にロードする
            if (_entity == null)
            {
               
                _entity = Resources.Load<CharacterStateSetting>("CharacterStateSetting");

                //ロード出来なかった場合はエラーログを表示
                if (_entity == null)
                {
                    Debug.LogError(nameof(CharacterStateSetting) + " not found");
                }
            }

            return _entity;
        }
    }
    public List<CharaInitDataParam> CharacterParam = new();
    
}

[Serializable]
public class CharaInitDataParam
{
    public string name;
    public Parameter intParam;
    public MagicData[] magicLearning;
    public ParamItemData[] itemList; // アイテム名と個数
    public int startLevel;
    public string loadTateName;
}

[Serializable]
public class ParamItemData
{
    public string name;
    public int count;
}