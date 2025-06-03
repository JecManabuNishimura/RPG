using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameData.Item;
using UnityEngine;

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
    public readonly Dictionary<int, ItemParam> ItemList = new();
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
                return playersState.Count(player => player.WeaponArmorEauip.Head.dataParam.ID == id);
            case WeaponArmorEquipment.Part.Hand1:
                return playersState.Count(player => player.WeaponArmorEauip.Hand1.dataParam.ID == id);
            case WeaponArmorEquipment.Part.Hand2:
                return playersState.Count(player => player.WeaponArmorEauip.Hand2.dataParam.ID == id);
            case WeaponArmorEquipment.Part.Body:
                return playersState.Count(player => player.WeaponArmorEauip.Body.dataParam.ID == id);
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
        return MagicMaster.Entity.GetMagicData(PlayerState.magicData[number]);
    }
    public ItemParam GetItemList(int index)
    {
        var data = ItemList.Values.ToList();
        if (data.Count > index)
            return data[index];
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

    public void GetItem(ItemParam item, int num = 1)
    {
        if (ItemList.ContainsKey(item.ID))
        {
            ItemParam param = ItemList[item.ID];
            param.num += num;
            ItemList[item.ID] = param;
        }
        else
        {
            item.num = num;
            ItemList.Add(item.ID, item);
        }
    }

    public void ChangeGold(int num)
    {
        gold += num;
    }

    public void UseItem(CharacterState selectChara)
    {
        if (selectChara.UseItem(ItemList[selectItemId]) || GameManager.Instance.mode == Now_Mode.Battle)
            ItemList[selectItemId].num--;
        if (ItemList[selectItemId].num == 0)
        {
            ItemList.Remove(selectItemId);
        }
    }
    public void Initialize()
    {
        playersState.Clear();

        int counter = 0;
        foreach (var c in CharacterStateSetting.Entity.CharacterParam)
        {
            playersState.Add(new PlayerCharacter());
            playersState[counter].Initialize(c.name, c.startLevel, c.intParam,c.magicLearning, counter, c.loadTateName);
            counter++;
        }
    }
}
