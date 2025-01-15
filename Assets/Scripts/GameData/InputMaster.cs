using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InputMaster", menuName="Master/CreateInputMaster") ]
public class InputMaster : ScriptableObject
{
    
    private static InputMaster _entity;
    public static InputMaster Entity
    {
        get
        {
            //初アクセス時にロードする
            if (_entity == null)
            {
               
                _entity = Resources.Load<InputMaster>("Master/InputMaster");

                //ロード出来なかった場合はエラーログを表示
                if (_entity == null)
                {
                    Debug.LogError(nameof(InputMaster) + " not found");
                }
            }

            return _entity;
        }
    }

    public FieldActionType FieldKey;
    public MenuActionType MenuKey;
}

[Serializable]
public class ActionType
{
    public ButtonType Enter;
    public ButtonType Cancel;
    public AxisType Up;
    public AxisType Down;
    public AxisType Left;
    public AxisType Right;
}
[Serializable]
public class MenuActionType : ActionType
{
    public ButtonType CharaSelectRight;
    public ButtonType CharaSelectLeft;
}
[Serializable]
public class FieldActionType : ActionType
{
    
}
[Serializable]
public struct AxisType
{
    public ButtonType type;
    public bool Negative;
}
public enum ButtonType
{
    A,
    B,
    X,
    Y,
    L1,
    R1,
    LS,
    RS,
    Start,
    Back,
    LX,
    LY,
    RX,
    RY,
    DX,
    DY,
    LR,
    LT,
    RT,
}
