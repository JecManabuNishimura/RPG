using TMPro;
using UnityEngine;

namespace UI
{
    public class StrengthMenu :StrengthMenuData
    {
        public void Init(int selectPlayer)
        {
            PlayerCharacter state = PlayerDataRepository.Instance.playersState[selectPlayer];


            name.text = state.CharaName;
            hp.text = "ＨＰ：" + state.TotalParam.Hp.ToString().ConvertToFullWidth();
            mp.text = "ＭＰ："+ state.TotalParam.Mp.ToString().ConvertToFullWidth();
            level.text = "ＬＶ："+ state.Level.ToString().ConvertToFullWidth();
            atk.text = "こうげきりょく：" + state.TotalParam.Atk.ToString().ConvertToFullWidth();
            maxHp.text = "さいだいＨＰ：" + state.TotalParam.MaxHp.ToString().ConvertToFullWidth();
            hand1.text =　state.WeaponArmorEauip.Hand1.isSet ? state.WeaponArmorEauip.Hand1.dataParam.Name.ConvertToFullWidth():"すで";
        }
    }
}
