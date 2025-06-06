using System;
using System.Collections;
using System.Collections.Generic;
using GameData.Item;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName="GameData/CreateItemData") ]
public class ItemData : ScriptableObject
{
    [Tooltip("アイテムの名前")]
    public string Name = "";
    public int ID ;
    public EffectType Effect = EffectType.none;      // 効果の種類
    public SubjectType Subject = SubjectType.None;     // 効果の対象
    public int Power;               // 効果の値
    public string Explanation = ""; // 説明
    public int Price;               // 売値
    public Sprite sprite;           // アイコン
}

[Serializable]
public enum EffectType
{
    Recovery,
    Weapon,
    none,
}
[Serializable]
public enum SubjectType
{
    HP,
    MP,
    None,
}
