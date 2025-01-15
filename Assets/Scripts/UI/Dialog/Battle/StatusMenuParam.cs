using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusMenuParam : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameArea;

    [SerializeField]
    private TextMeshProUGUI hpArea;
    [SerializeField]
    private TextMeshProUGUI mpArea;
    [SerializeField]
    private TextMeshProUGUI levelArea;

    public void UpdateText(string nameArea, int hpArea, int mpArea, int levelArea)
    {
        this.nameArea.text = nameArea.ConvertToFullWidth(4);
        this.hpArea.text = "Ｈ" + hpArea.ToString("D3").ConvertToFullWidth();
        this.mpArea.text = "Ｍ" + mpArea.ToString("D3").ConvertToFullWidth();
        this.levelArea.text = "ＬＶ" + levelArea.ToString("D2").ConvertToFullWidth();
    }
}
