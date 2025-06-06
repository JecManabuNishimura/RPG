using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameData.Item;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "ItemMaster", menuName="Master/CreateItemMaster") ]
public class ItemMaster : ScriptableObject
{
    private static ItemMaster _entity;

    public static ItemMaster Entity
    {
        get
        {
            //初アクセス時にロードする
            if (_entity == null)
            {
                _entity = Resources.Load<ItemMaster>("Master/ItemMaster");

                //ロード出来なかった場合はエラーログを表示
                if (_entity == null)
                {
                    Debug.LogError("ItemMaster" + " not found");
                }
            }

            return _entity;
        }
    }

    //public int playerItemCount;
    //public Dictionary<int, ItemParam> ItemParamDatas = new ();
    public List<ItemData> ItemParamDatas = new();
    public ItemData GetItemData(int id)
    {
        return ItemParamDatas.FirstOrDefault(x => x.ID == id);
    }

    public string GetItemName(int id)
    {
        return ItemParamDatas.FirstOrDefault(x => x.ID == id)?.Name;
    }
    
    public ItemData FindItemData(string name)
    {
        return ItemParamDatas.FirstOrDefault(x => x.Name == name);
    }


}



