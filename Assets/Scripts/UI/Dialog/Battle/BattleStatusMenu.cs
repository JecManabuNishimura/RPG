using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleStatusMenu : MonoBehaviour
{
    [SerializeField] GameObject ParamObj;
    [SerializeField] private Transform ParamBoxParent;
    [SerializeField] private RectTransform FreamTran;
    [SerializeField] private TextMeshProUGUI nameText;
    
    private List<StatusMenuParam> ParamList = new();
    private const float oneSize = 105;
    
    
    void Start()
    {
        BattleManager.Instance.OnParamChange += UpdateText;
        foreach (var data in PlayerDataRepository.Instance.playersState)
        {
            var obj = Instantiate(ParamObj, ParamBoxParent, true);
            var param = obj.GetComponent<StatusMenuParam>();
            param.UpdateText(data.CharaName, data.parameter.Hp, data.parameter.Mp, data.Level);
            ParamList.Add(param);
        }
        FreamTran.sizeDelta = new Vector2(145f + (oneSize * (PlayerDataRepository.Instance.playersState.Count - 1)), FreamTran.sizeDelta.y);
    }

    public void UpdateText()
    {
        int count = 0;
        foreach (var data in PlayerDataRepository.Instance.playersState)
        {
            ParamList[count].UpdateText(data.CharaName, data.parameter.Hp, data.parameter.Mp,data.Level);
            count++;
        }

        nameText.text = PlayerDataRepository.Instance.PlayerState.CharaName.ConvertToFullWidth();
    }
}
