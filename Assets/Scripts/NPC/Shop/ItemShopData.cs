using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemShopData", menuName="CreateItemShopData")]
public class ItemShopData :ScriptableObject
{
    public List<ShopItemData> ShopItemDatas = new();
}

[Serializable]
public class ShopItemData
{
    public int ID;
    public string name;
    public int price;
}

[CustomEditor(typeof(ItemShopData))] //拡張するクラスを指定
public class ItemShopDataEditor : Editor
{
    private ItemShopData itemReader;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        itemReader = (ItemShopData)target;

        GUIStyle customButtonStyle = new GUIStyle(GUI.skin.button);
        customButtonStyle.normal.textColor = Color.green; // ボタンのテキストの色を赤に変更
        if (GUILayout.Button("ReadItemData", customButtonStyle))
        {
            ReadItemData();
        }
    }

    private void ReadItemData()
    {
        for (int i = 0; i < itemReader.ShopItemDatas.Count; i++)
        {
            var iData = ItemMaster.Entity.GetItemData(itemReader.ShopItemDatas[i].ID);
            if ( iData != null)
            {
                itemReader.ShopItemDatas[i].name = iData.Name;
                itemReader.ShopItemDatas[i].price = iData.Price;
            }
            else
            {
                var wData = WeaponArmorMaster.Entity.GetWeaponData(itemReader.ShopItemDatas[i].ID); 
                if (wData != null)
                {
                    itemReader.ShopItemDatas[i].name = wData.name;
                    itemReader.ShopItemDatas[i].price = wData.price;
                }
            }
        }
    }

    
}
