using System;
using System.Collections;
using System.Collections.Generic;
using GameData.Item;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName="GameData/CreateItemData") ]
public class ItemData : ScriptableObject
{
    [Tooltip("アイテムの名前")]
    public string Name = "";
    public int ID ;
    public List<EffectRef> Effect;      // 効果の種類
	public string Explanation = ""; // 説明
    public int Price;               // 売値
    [SpriteSearch("Assets/Sprites/GameData/Icon"), Preview]
    public Sprite sprite;           // アイコン

}
