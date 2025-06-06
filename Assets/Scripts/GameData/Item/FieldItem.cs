using System;
using UnityEngine;

public class FieldItem : MonoBehaviour
{
    // 先頭一桁　：MapID
    // ２～３　：固有ID
    // ４～　　：アイテムID
    [SerializeField] private string mapId;
    [Range(1,99)]
    [SerializeField] private int originalId;
    [SerializeField] private string itemName;
    [SerializeField] private bool isCollected = false;
    
    public string AllID => mapId + originalId + ID;
    public int ID => ItemMaster.Entity.FindItemData(itemName).ID;
    private void Start()
    {
        isCollected = ItemDataBase.Entity.GetItemCollect(AllID);
        gameObject.SetActive(!isCollected);
    }
    public int GetItem()
    {
        gameObject.SetActive(false);
        isCollected = true;
        ItemDataBase.Entity.SetItemCollect(AllID,true);
        return ID;
    }
}
