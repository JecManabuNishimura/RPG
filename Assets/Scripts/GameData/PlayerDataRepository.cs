using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameData.Item;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerDataRepository
{
    public static PlayerDataRepository Instance = new();
    public List<PlayerCharacter> playersState = new();

    public PlayerCharacter PlayerState => playersState[SelectIndex];
    public PlayerCharacter RandomPlayer => playersState[Random.Range(0, playersState.Count)];
    public Vector3 playerPos;
    public float PlayerCamPos => (Camera.main.transform.position - playerPos).y;
    public Quaternion playerRota;

    public int ItemCount => ItemList.Count;

    public int SelectIndex;

    [Serializable]
    public class HubItemData
    {
        public int ID;
        public int num;
    }
    public List<HubItemData> ItemList = new();// IDと個数がわかるようにする
    public int selectItemId;
    

    public int gold = 100000;

    public bool DeathCheck()
    {
        foreach (var ps in playersState)
        {
            if (!ps.DethFlag)
            {
                return false;
            }
        }

        return true;
    }

    public int GetWeaponArmorSetCount(WeaponArmorEquipment.Part part, int id)
    {
        switch (part)
        {
            case WeaponArmorEquipment.Part.Head:
                return playersState.Count(player => player.WeaponArmorEauip.Head.ID == id);
            case WeaponArmorEquipment.Part.Hand1:
                return playersState.Count(player => player.WeaponArmorEauip.Hand1.ID == id);
            case WeaponArmorEquipment.Part.Hand2:
                return playersState.Count(player => player.WeaponArmorEauip.Hand2.ID == id);
            case WeaponArmorEquipment.Part.Body:
                return playersState.Count(player => player.WeaponArmorEauip.Body.ID == id);
            case WeaponArmorEquipment.Part.DoubleHand:
                return playersState.Count(player => player.WeaponArmorEauip.DoubleHand.ID == id);
            default:
                break;
        }
        return 0;
    }

    public void PlusGold(int num)
    {
        gold += num;
    }

    public MagicData GetMagicData(int number)
    {
        if(number >= 0 && number < PlayerState.magicData.Count)
        {
            return MagicMaster.Entity.GetMagicData(PlayerState.magicData[number]);
        }
        return null;
    }
    public ItemData GetItemData(int index)
    {
        return ItemMaster.Entity.GetItemData(index);
    }

    public InfoWeaponArmor GetWeaponData(int index)
    {
        if (ItemList.Count > index)
        {
            return WeaponArmorMaster.Entity.GetWeaponData(ItemList[index].ID);
        }

        return null;
    }

    public void NextCharacter()
    {
        SelectIndex++;
        if (SelectIndex > playersState.Count - 1)
        {
            SelectIndex = playersState.Count - 1;
        }
    }
    public void PrevCharacter()
    {
        SelectIndex--;
        if (SelectIndex < 0)
        {
            SelectIndex = 0;
        }
    }

    public ShopStateParameter GetWeaponStateParam(int num, int weaponID)
    {
        ShopStateParameter param = new ShopStateParameter(playersState[num].parameter);
        ShopStateParameter weaponParam = new ShopStateParameter(WeaponArmorMaster.Entity.GetWeaponData(weaponID).UpParam);
        ShopStateParameter attachWeaponParam =
            new ShopStateParameter(playersState[num].WeaponArmorEauip.GetWeaponEquipmentData(weaponID).UpParam);

        return (param + weaponParam) - (param + attachWeaponParam);
    }

    public void GetItem(int id, int num = 1)
    {
        var fItem = ItemList.Find(_ => _.ID == id);

		if (fItem != null)
        {
			fItem.num += num;
        }
        else
        {
            HubItemData item = new HubItemData
            {
                ID = id,
                num = num
            };
            ItemList.Add(item);
        }
    }

    public void ChangeGold(int num)
    {
        gold += num;
    }

    public void UseItem(CharacterState selectChara)
    {
        int index = -1;
        if (selectChara.UseItem(ItemMaster.Entity.GetItemData(selectItemId)) ||
            GameManager.Instance.mode == Now_Mode.Battle)
        {
            index = ItemList.FindIndex(x => x.ID == selectItemId);
        }
        else
        {
            return;
        }

        if (ItemList[index].num == 0)
        {
            ItemList.RemoveAt(index);
        }
    }
    public void Initialize()
    {
        playersState.Clear();

        int counter = 0;
        foreach (var c in CharacterStateSetting.Entity.CharacterParam)
        {
            playersState.Add(new PlayerCharacter());
            playersState[counter].Initialize(c, counter);
            counter++;
        }
    }
}
