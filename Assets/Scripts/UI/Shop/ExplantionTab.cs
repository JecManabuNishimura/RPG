using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExplantionTab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI getNum;
    [SerializeField] private TextMeshProUGUI price;

    public void SetText(string getName, string priceName)
    {
        getNum.text = getName.ConvertToFullWidth();
        price.text = priceName.ConvertToFullWidth();
        
    }
}
