using System.Collections;
using System.Collections.Generic;
using GameData.Item;
using UnityEngine;

public class ItemData : ItemParamData
{
    void Start()
    {
        ItemParam data = ItemMaster.Entity.GetItemData(int.Parse(ID.Substring(3)));
        int num = param.num;
        param = data;
        param.num = num;
        isCollected = ItemDataBase.Entity.GetItemCollect(ID);
        gameObject.SetActive(!isCollected);
        //ItemMaster.Instance.ItemDatas.Add(this);
    }
}
