using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EquipmentSelectMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI hand1Text;
    [SerializeField] private TextMeshProUGUI hand2Text;
    [SerializeField] private TextMeshProUGUI headText;
    [SerializeField] private TextMeshProUGUI bodyText;

    public void SetText(string charaName, string hand1, string hand2, string head, string body)
    {
        nameText.text = charaName;
        hand1Text.text = (hand1 != "" ? hand1 : "すで");
        hand2Text.text = (hand2 != "" ? hand2 : "すで");
        headText.text = (head != "" ? head : "そうびなし");
        bodyText.text = (body != "" ? body : "そうびなし");
    }
}
