using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BeAfParameter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI BeforeText;
    [SerializeField] private TextMeshProUGUI AfterText;

    public void InitText(int before)
    {
        BeforeText.text = before.ToString().ConvertToFullWidth();
        AfterText.text = "";
    }
    public void SetText(int before, int after,EquipUpDown upDown)
    {
        BeforeText.text = before.ToString().ConvertToFullWidth();
        AfterText.text = after.ToString().ConvertToFullWidth();
        AfterText.color = upDown switch
        {
            EquipUpDown.Up => Color.green,
            EquipUpDown.Down => Color.red,
            EquipUpDown.Equal => Color.white
        };
    }
}

public enum EquipUpDown
{
    Up,
    Down,
    Equal,
}
