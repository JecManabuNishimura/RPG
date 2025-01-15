using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateCharacterSelect : MonoBehaviour
{
    [SerializeField]
    private GameObject textObj;

    [SerializeField]
    private GameObject CommandObj;
    private void Start()
    {
        foreach (var ps in PlayerDataRepository.Instance.playersState)
        {
             var obj = Instantiate(textObj).GetComponent<TextMeshProUGUI>();
             obj.text = ps.CharaName;
             obj.gameObject.transform.parent = CommandObj.transform;
        }
    }
}
