using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentParameter : MonoBehaviour
{
    [SerializeField] private List<BeAfParameter> DataParam;

    public void InitDataParam(int charaNum)
    {
        DataParam[0].InitText(PlayerDataRepository.Instance.playersState[charaNum].TotalParam.str);
        DataParam[1].InitText(PlayerDataRepository.Instance.playersState[charaNum].TotalParam.Res);
        DataParam[2].InitText(PlayerDataRepository.Instance.playersState[charaNum].TotalParam.Mga);
        DataParam[3].InitText(PlayerDataRepository.Instance.playersState[charaNum].TotalParam.Mgd);
        DataParam[4].InitText(PlayerDataRepository.Instance.playersState[charaNum].TotalParam.Atk);
        DataParam[5].InitText(PlayerDataRepository.Instance.playersState[charaNum].TotalParam.Def);
        DataParam[6].InitText(PlayerDataRepository.Instance.playersState[charaNum].TotalParam.Qui);
        DataParam[7].InitText(PlayerDataRepository.Instance.playersState[charaNum].TotalParam.Luc);
    }

    private EquipUpDown GetUpDown(int a, int b)
    {
        if (a < b)
        {
            return EquipUpDown.Up;
        }
        else if (a == b)
        {
            return EquipUpDown.Equal;
        }
        else 
        {
            return EquipUpDown.Down;
        }
    }
    public void SetBeAFParameter(int charaNum,Parameter weaponParam,Parameter setWeaponParam)
    {
        var charaData = PlayerDataRepository.Instance.playersState[charaNum];
        DataParam[0].SetText(
            charaData.TotalParam.str,
            (charaData.TotalParam - setWeaponParam + weaponParam).str,GetUpDown(charaData.TotalParam.str,(charaData.TotalParam - setWeaponParam+ weaponParam).str));
        DataParam[1].SetText(
            charaData.TotalParam.Res,
            (charaData.TotalParam - setWeaponParam+ weaponParam).Res,GetUpDown(charaData.TotalParam.Res,(charaData.TotalParam - setWeaponParam+ weaponParam).Res));
        DataParam[2].SetText(
            charaData.TotalParam.Mga,
            (charaData.TotalParam - setWeaponParam+ weaponParam).Mga,GetUpDown(charaData.TotalParam.Mga,(charaData.TotalParam - setWeaponParam+ weaponParam).Mga));
        DataParam[3].SetText(
            charaData.TotalParam.Mgd,
            (charaData.TotalParam - setWeaponParam+ weaponParam).Mgd,GetUpDown(charaData.TotalParam.Mgd,(charaData.TotalParam - setWeaponParam+ weaponParam).Mgd));
        DataParam[4].SetText(
            charaData.TotalParam.Atk,
            (charaData.TotalParam - setWeaponParam+ weaponParam).Atk,GetUpDown(charaData.TotalParam.Atk,(charaData.TotalParam - setWeaponParam+ weaponParam).Atk));
        DataParam[5].SetText(
            charaData.TotalParam.Def,
            (charaData.TotalParam - setWeaponParam+ weaponParam).Def,GetUpDown(charaData.TotalParam.Def,(charaData.TotalParam - setWeaponParam+ weaponParam).Def));
        DataParam[6].SetText(
            charaData.TotalParam.Qui,
            (charaData.TotalParam - setWeaponParam+ weaponParam).Qui,GetUpDown(charaData.TotalParam.Qui,(charaData.TotalParam - setWeaponParam+ weaponParam).Qui));
        DataParam[7].SetText(
            charaData.TotalParam.Luc,
            (charaData.TotalParam - setWeaponParam+ weaponParam).Luc,GetUpDown(charaData.TotalParam.Luc,(charaData.TotalParam - setWeaponParam+ weaponParam).Luc));
    }
}
