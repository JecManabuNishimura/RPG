using System;
using UnityEngine;

namespace GameData.Item
{
    public class ItemParamData : MonoBehaviour,IItem
    {
        // 先頭一桁　：MapID
        // ２～３　：固有ID
        // ４～　　：アイテムID
        public string ID;
        public ItemParam param = new();
        public bool isCollected = false;
        public ItemParam GetItem()
        {
            gameObject.SetActive(false);
            isCollected = true;
            ItemDataBase.Entity.SetItemCollect(ID,true);
            return param;
        }
    }

    [Serializable]
    public class ItemParam
    {
        public string Name = "";
        public int ID ;
        public string Effect = "";
        public string Subject = "";
        public int Power;
        public string Explanation = ""; // 説明
        public int Price;
        public Sprite sprite;
        public int num;             // 所持数
    }
}
