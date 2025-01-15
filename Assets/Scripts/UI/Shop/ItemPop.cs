using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemPop : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;           // 名前
    [SerializeField]
    private TextMeshProUGUI NoPosseText;        // 所持数
    [SerializeField]
    private TextMeshProUGUI PriceText;        // 価格


    public void SetItemData(string name,int num,int price)
    {
        nameText.text = name.ConvertToFullWidth();
        NoPosseText.text = num.ToString().ConvertToFullWidth();
        PriceText.text = price.ToString().ConvertToFullWidth() + "Ｇ";
    }
}
