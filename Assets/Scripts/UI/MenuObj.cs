using System;
using System.Collections.Generic;
using GameData.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuObj : MonoBehaviour
{

    public GameObject Cursor = null;

    public MenuList keyList;
    public Vector3 anchorInitPos;
    public Vector3 tranInitPos;
    public RectTransform rect = null;
    public ScrollRect scrollrect;
    public bool notCloseMenu = false;
    public bool openMenu = false;

    [Header("ショップメニューの場合")] 
    public ShopMenu shopMenu;
    [Header("アイテムリストの場合&装備メニュー")]
    public ItemSetData data;
    
    private List<GameObject> objList = new();

    public EquipmentMenuData equData = new();
    private void Awake()
    {
        if (Cursor == null) return;
        rect = Cursor.GetComponent<RectTransform>();
        anchorInitPos = rect.anchoredPosition;
        tranInitPos = Cursor.transform.position;
    }

    public void CreateItemData(int id)
    {
        var obj =  Instantiate(data.itemDataObj).GetComponent<ItemText>();
        PlayerDataRepository.Instance.ItemList.TryGetValue(id, out var item);
        var  itemdata = ItemMaster.Entity.GetItemData(id);
        obj.CreateText(itemdata.Name,item.num,itemdata.sprite);
        obj.transform.parent = data.itemContent.transform;
        objList.Add(obj.gameObject);
    }
    public void CreateItemData()
    {
        var obj =  Instantiate(data.itemDataObj).GetComponent<ItemText>();
        obj.CreateText("はずす",0);
        obj.transform.parent = data.itemContent.transform;
        objList.Add(obj.gameObject);
    }

    public void CreateMagicData(MagicData magic)
    {
        var obj = Instantiate(data.itemDataObj).GetComponent<ItemText>();
        obj.CreateText(magic.magicName, magic.mpCost);
        obj.transform.parent = data.itemContent.transform;
        objList.Add(obj.gameObject);
    }

    public void ResetItemData()
    {
        foreach (var obj in objList)
        {
            Destroy(obj);
        }
        objList.Clear();
    }

    public void SetExpText(string text)
    {
        if(data.expText != null)
        {
			data.expText.text = text;
		}
        
    }
}
[Serializable]
public class ItemSetData
{
    public GameObject itemContent;
    public GameObject itemDataObj;
    public TextMeshProUGUI expText;
}

[Serializable]
public class EquipmentMenuData
{
    public NowEquipmentData Data;
    public EquipmentSelectMenu Select;
    public EquipmentParameter Parameter;
}

