using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NowEquipmentData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI partText;
    [SerializeField] private TextMeshProUGUI weaponText;
    [SerializeField] private TextMeshProUGUI equipment1Text;
    [SerializeField] private TextMeshProUGUI equipment2Text;
    [SerializeField] private TextMeshProUGUI equipment3Text;

    public void SetEquipmentData(string part, string nowWeapon, string equi1, string equi2, string equi3)
    {
        partText.text = part.ConvertToFullWidth();
        weaponText.text = nowWeapon.ConvertToFullWidth();
        equipment1Text.text = equi1.ConvertToFullWidth();
        equipment2Text.text = equi2.ConvertToFullWidth();
        equipment3Text.text = equi3.ConvertToFullWidth();
    }
}
