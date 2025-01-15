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
    public List<ItemParam> ItemParamDatas = new();
    public ItemParam GetItemData(int id)
    {
        return ItemParamDatas.FirstOrDefault(x => x.ID == id);
    }

    public string GetItemName(int id)
    {
        return ItemParamDatas.FirstOrDefault(x => x.ID == id)?.Name;
    }


}



[CustomEditor(typeof(ItemMaster))] //拡張するクラスを指定
public class ItemMasterEditer : Editor
{
    private ItemMaster itemReader;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        itemReader = (ItemMaster)target;
        if (GUILayout.Button("SetItemData"))
        {
            ReadItemData();
        }
    }
    void ReadItemData()
    {
        itemReader.ItemParamDatas.Clear();
        var data = DataReader.ReadData("ItemData");
        for (int i = 1; i < data.Count; i++)
        {
            ItemParam item = new();
            item.ID = int.Parse(data[i][0]);
            item.Name =data[i][1];
            item.Effect = data[i][2];
            item.Subject = data[i][3];
            item.Power = int.Parse(data[i][4]);
            item.Price = int.Parse(data[i][5]);
            item.Explanation = data[i][6];
            
            itemReader.ItemParamDatas.Add(item);
        }
    }
}