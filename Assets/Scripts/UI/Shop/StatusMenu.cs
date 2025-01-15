using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI charaName;
    [SerializeField] private TextMeshProUGUI attack;
    [SerializeField] private TextMeshProUGUI defence;
    [SerializeField] private TextMeshProUGUI speed;
    [SerializeField] private TextMeshProUGUI magicAtk;
    [SerializeField] private TextMeshProUGUI magicDef;

    public void ChangeName(string charaName)
    {
        this.charaName.text = charaName.ConvertToFullWidth();
    }
    public void ChangeState(ShopStateParameter state)
    {
        
        attack.text = state.Atk == 0 ? "ー" :　state.Atk.ToString().ConvertToFullWidth();
        defence.text =　state.Def == 0 ? "ー" : state.Def.ToString().ConvertToFullWidth();
        speed.text =　state.Spd == 0 ? "ー" : state.Spd.ToString().ConvertToFullWidth();
        magicAtk.text =　state.Mga == 0 ? "ー" : state.Mga.ToString().ConvertToFullWidth();
        magicDef.text =　state.Mgd == 0 ? "ー" : state.Mgd.ToString().ConvertToFullWidth();
    }
}
