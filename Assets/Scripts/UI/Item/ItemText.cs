using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemText : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI numberText;
    public Image icon;

    public void CreateText(string itemName, int num,Sprite sprite = null)
    {
        nameText.text = itemName;
        numberText.text = num.ToString().ConvertToFullWidth();
        icon.sprite = sprite;
        icon.color = Color.white;
        if (sprite == null)
        {
            icon.color = Color.clear;
        }
    }
}
