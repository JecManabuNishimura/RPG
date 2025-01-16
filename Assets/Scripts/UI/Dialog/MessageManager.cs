using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance;
    public DialogMessage Message;

    public event Action<string, string> OnDialogMessage;
    public event Action<string> OnSetNameDialogMessage;
    public bool IsEndMessage = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMessage(DialogMessage dm)
    {
        Message = dm;
    }
    public void ClearDialogMessage()
    {
        OnDialogMessage?.Invoke("", "");
    }
    public void StartDialogMessage(string message, string fileName = "")
    {
        IsEndMessage = false;
        OnDialogMessage?.Invoke(message, fileName);
    }

    public void SetCharaName(string name)
    {
        string newName = name switch
        {
            "p1" => CharacterStateSetting.Entity.CharacterParam.Count > 0 ? CharacterStateSetting.Entity.CharacterParam[0].name : name,
            "p2" => CharacterStateSetting.Entity.CharacterParam.Count > 1 ? CharacterStateSetting.Entity.CharacterParam[1].name : name,
            "p3" => CharacterStateSetting.Entity.CharacterParam.Count > 2 ? CharacterStateSetting.Entity.CharacterParam[2].name : name,
            "p4" => CharacterStateSetting.Entity.CharacterParam.Count > 3 ? CharacterStateSetting.Entity.CharacterParam[3].name : name,
            "p5" => CharacterStateSetting.Entity.CharacterParam.Count > 4 ? CharacterStateSetting.Entity.CharacterParam[4].name : name,
            _ => name,
        };
        OnSetNameDialogMessage?.Invoke(newName.ConvertToFullWidth());
    }

}
