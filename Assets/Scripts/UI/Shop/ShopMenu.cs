using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopMenu : MonoBehaviour
{
    [SerializeField] private GameObject itemPop;

    [SerializeField] private GameObject menuList;

    [SerializeField] private TextMeshProUGUI moneyObj;
    [SerializeField] private TextMeshProUGUI explanationText;
    [SerializeField] private GameObject UpDownCursor;
    [SerializeField] private ExplantionTab expTab;
    [SerializeField] private GameObject StatusMenuWindow;
    [SerializeField] private GameObject CharaStatusParent;
    [SerializeField] private GameObject StatusCharaObj;

    private List<StatusMenu> _statusMenus = new();
    
    private class ItemDataList
    {
        public int ID;
        public string Name;
        public int Count;
        public int Price;
        public GameObject Data;
    }
    
    private List<ItemDataList> itemList = new();

    public void ShowStatusMenu(bool flag)
    {
        if (flag)
        {
            foreach (var obj in _statusMenus)
            {
                Destroy(obj.gameObject);
            }
            _statusMenus.Clear();
            for (int i = 0; i < PlayerDataRepository.Instance.playersState.Count; i++)
            {
                var obj = Instantiate(StatusCharaObj, CharaStatusParent.transform, true).GetComponent<StatusMenu>();
                _statusMenus.Add(obj);
            }
            StatusMenuWindow.SetActive(true);
        }
        else
        {
            StatusMenuWindow.SetActive(false);
        }
    }

    public void SetStatusData(int number)
    {
        int num = 0;
        
        foreach (var status in _statusMenus)
        {
            status.ChangeState(PlayerDataRepository.Instance.GetWeaponStateParam(num,itemList[number].ID));
            num++;
        }
    }
    
    
    public void CreateShopMenu(MenuList mList)
    {
        
        EndShopMenu();
        foreach (var data in NPCManager.Instance.nowTalkItemShopData.ShopItemDatas)
        {
            if (mList != MenuList.Shop_Buy )
            {
                int itemCount = 0;
                if (PlayerDataRepository.Instance.ItemList.Find(x => x.ID == data.ID) is PlayerDataRepository.HubItemData value)
                {
                    itemCount = value.num;
                }
            
                CreateMenuItem(data.ID, data.name,itemCount,data.price);    
            }
            else 
            {
                CreateMenuItem(data.ID, data.name,0,0);
            }
        }

        moneyObj.text = PlayerDataRepository.Instance.gold.ToString().ConvertToFullWidth() + "Ｇ";
    }

    public void ShowCursor(bool flag)
    {
        UpDownCursor.SetActive(flag);
        expTab.SetText(
            flag ?"こうにゅうすう":"もってるかず",
            flag ?"ごうけいがく":"ねだん"
            );
    }

    public void SetCursorPos(Vector3 pos)
    {
        UpDownCursor.transform.position = pos;
    }

    public void SetExplanation(int num)
    {
        var data = ItemMaster.Entity.GetItemData(itemList[num].ID);
        if (data != null)
        {
            explanationText.text = data.Explanation.ConvertToFullWidth();    
        }
        else
        {
            var wData = WeaponArmorMaster.Entity.GetWeaponData(itemList[num].ID);
            explanationText.text = wData != null ? wData.Explanation.ConvertToFullWidth() : "データーベースにありません";
        }
    }

    private void SetBuyItem(int num)
    {
        itemList[num].Data.GetComponent<ItemPop>()
            .SetItemData(itemList[num].Name,itemList[num].Count , itemList[num].Count * itemList[num].Price);
    }

    public (int,int,int) GetBuyItemIDAndCount(int num)
    {
        return (itemList[num].ID, itemList[num].Count,itemList[num].Count * itemList[num].Price);
    }

    public void ChangeBuyItemNum(int num, int upDonw)
    {
        itemList[num].Count += upDonw;
        if (itemList[num].Count <= 0)
            itemList[num].Count = 0;
        else if(itemList[num].Count >= 99)
            itemList[num].Count = 99;

        SetBuyItem(num);
    }

    public void EndShopMenu()
    {
        foreach (var data in itemList)
        {
            Destroy(data.Data);
        }
        itemList.Clear();
    }
    
    private void CreateMenuItem(int id,string itemName,int num,int price)
    {
        var obj = Instantiate(itemPop, menuList.transform, true);
        obj.GetComponent<ItemPop>().SetItemData(itemName, num, price);
        ItemDataList idl = new ItemDataList
        {
            ID = id,
            Name = itemName,
            Count = 0,
            Data = obj,
            Price = price,
        };

        itemList.Add(idl);
    }
}

public struct ShopStateParameter
{
    public int Atk;
    public int Def;
    public int Spd;
    public int Mga;
    public int Mgd;

    public ShopStateParameter(Parameter param)
    {
        Atk = param.Atk;
        Def = param.Def;
        Spd = param.Qui;
        Mga = param.Mga;
        Mgd = param.Mgd;
    }

    public static ShopStateParameter operator -(ShopStateParameter param1, ShopStateParameter param2)
    {
        ShopStateParameter result;
        result.Atk = param1.Atk - param2.Atk;
        result.Def = param1.Def - param2.Def;
        result.Spd = param1.Spd - param2.Spd;
        result.Mga = param1.Mga - param2.Mga;
        result.Mgd = param1.Mgd - param2.Mgd;

        return result;
    }
    public static ShopStateParameter operator +(ShopStateParameter param1, ShopStateParameter param2)
    {
        ShopStateParameter result;
        result.Atk = param1.Atk + param2.Atk;
        result.Def = param1.Def + param2.Def;
        result.Spd = param1.Spd + param2.Spd;
        result.Mga = param1.Mga + param2.Mga;
        result.Mgd = param1.Mgd + param2.Mgd;

        return result;
    }
}


