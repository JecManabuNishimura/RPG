using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour 
{
    public static InputManager Instance;

    private List<KeyCode> downKeyCode = new();
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

    private class ActionDataTable
    {
        public Dictionary<ButtonType, Action> ButtonData = new();
        public Dictionary<AxisType, Action> AxisData = new();
        public Dictionary<KeyCode, Action> KeyDownData = new();
        public Dictionary<KeyCode, Action> KeyData = new();
    }
    

    private ActionDataTable MenuAction = new ();
    private ActionDataTable MessageAction = new ();
    private ActionDataTable PlayerAction = new ();
    private ActionDataTable DebugAction = new();

    private Dictionary<(UseButtonType, ButtonType), bool> _actionOnOffs = new ();

    void ExecuteKeyboardAction(KeyCode code,bool downFlag)
    {
        ActionDataTable table = GameManager.Instance.state switch
        {
            Now_State.Active => PlayerAction,
            Now_State.Menu => MenuAction,
            Now_State.Shop => MenuAction,
            Now_State.Message => MessageAction,
        };
        if (downFlag)
        {
            if (table.KeyDownData.ContainsKey(code)) 
            {
                table.KeyDownData[code]?.Invoke();
            }       
        }
        else
        {
            if (table.KeyData.ContainsKey(code)) 
            {
                table.KeyData[code]?.Invoke();
            }
        }
         
    }
    void ExecuteButtonAction(ButtonType buttonType)
    {
        ActionDataTable table = GameManager.Instance.state switch
        {
            Now_State.Active => PlayerAction,
            Now_State.Menu => MenuAction,
            Now_State.Shop => MenuAction,
            Now_State.Message => MessageAction,
        };
        if(table != null && table.ButtonData.ContainsKey(buttonType))
            table.ButtonData[buttonType]?.Invoke();
    }
    void ExecuteAxisAction(ButtonType buttonType,bool negative)
    {
        AxisType type;
        type.Negative = negative;
        type.type = buttonType;
        if (MenuAction.AxisData.ContainsKey(type))
            MenuAction.AxisData[type]?.Invoke();
        if (PlayerAction.AxisData.ContainsKey(type))
            PlayerAction.AxisData[type]?.Invoke();
        
    }

    public bool CheckActionPress(UseButtonType useTYpe, ButtonType type)
    {
        if (!_actionOnOffs.ContainsKey((useTYpe, type)))
            return false;
        return _actionOnOffs[(useTYpe,type)];
    }
    private void Update()
    {
        if (Input.anyKey)
        {
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode))) {
                if (Input.GetKey(code))
                {
                    ExecuteKeyboardAction(code,false);
                }
            }
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode))) {
                if (Input.GetKeyDown(code))
                {
                    ExecuteKeyboardAction(code,true);
                }
            }
        }
        if (Gamepad.current == null) return;
        if (Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            ExecuteButtonAction(ButtonType.A);
        }
        if (Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            ExecuteButtonAction(ButtonType.B);
        }
        if (Gamepad.current.buttonNorth.wasPressedThisFrame)
        {
            ExecuteButtonAction(ButtonType.X);
        }
        if (Gamepad.current.buttonWest.wasPressedThisFrame)
        {
            ExecuteButtonAction(ButtonType.Y);
        }
        if (Gamepad.current.leftShoulder.wasPressedThisFrame)
        {
            ExecuteButtonAction(ButtonType.LS);
        }
        if (Gamepad.current.rightShoulder.wasPressedThisFrame)
        {
            ExecuteButtonAction(ButtonType.RS);
        }
        if (Gamepad.current.rightTrigger.wasPressedThisFrame)
        {
            ExecuteButtonAction(ButtonType.RT);
        }
        if (Gamepad.current.leftTrigger.wasPressedThisFrame)
        {
            ExecuteButtonAction(ButtonType.LT);
        }
        if (Gamepad.current.leftStick.ReadValue().x >= 0.8f)
        {
            ExecuteAxisAction(ButtonType.LX, false);
            _actionOnOffs[(UseButtonType.Menu,ButtonType.LX)] = true;
        }
        if (Gamepad.current.leftStick.ReadValue().x <= -0.8f)
        {
            ExecuteAxisAction(ButtonType.LX, true);
            _actionOnOffs[(UseButtonType.Menu,ButtonType.LX)] = true;
        }
        if (Gamepad.current.leftStick.ReadValue().y >= 0.8f)
        {
            ExecuteAxisAction(ButtonType.LY, false);
            _actionOnOffs[(UseButtonType.Menu,ButtonType.LY)] = true;
        }
        if (Gamepad.current.leftStick.ReadValue().y <= -0.8f)
        {
            ExecuteAxisAction(ButtonType.LY, true);
            _actionOnOffs[(UseButtonType.Menu,ButtonType.LY)] = true;
        }

        if (Gamepad.current.leftStick.ReadValue().magnitude <= 0.4f)
        {
            _actionOnOffs[(UseButtonType.Menu,ButtonType.LX)] = false;
            _actionOnOffs[(UseButtonType.Menu,ButtonType.LY)] = false;
        }
    }

    public void SetKeyboardEvent(UseButtonType useButtonType,KeyCode code, Action action,bool downFlag)
    {
        ActionDataTable table = useButtonType switch
        {
            UseButtonType.Player => PlayerAction,
            UseButtonType.Menu => MenuAction,
            UseButtonType.Message => MessageAction,
            UseButtonType.Debug => DebugAction,
        } ;
        if(downFlag)
        {
            if (!table.KeyDownData.TryAdd(code, action))
            {
                table.KeyDownData.Remove(code);
                table.KeyDownData.Add(code,action);
            }
        }
        else
            if (!table.KeyData.TryAdd(code, action))
            {
                table.KeyData.Remove(code);
                table.KeyData.Add(code,action);
            }
    }

    public void SetKeyEvent(UseButtonType useButtonType, ButtonType buttonType, Action action,bool deleteFlag = false)
    {
        ActionDataTable table = useButtonType switch
        {
            UseButtonType.Player => PlayerAction,
            UseButtonType.Menu => MenuAction,
            UseButtonType.Message => MessageAction,
        } ;
        
        if (!table.ButtonData.ContainsKey(buttonType))
        {
            table.ButtonData.Add(buttonType,action);
            
            _actionOnOffs.Add((useButtonType,buttonType),false);
        }
        else
        {
            if (deleteFlag)
            {
                // 削除の場合
                table.ButtonData.Remove(buttonType);
                _actionOnOffs.Remove((useButtonType, buttonType));
            }
            else
            {
                table.ButtonData.Remove(buttonType);
                table.ButtonData.Add(buttonType,action);
            }
        }
            
    }
    public void SetKeyEvent(UseButtonType useButtonType, AxisType buttonType, Action action)
    {
        ActionDataTable table = useButtonType switch
        {
            UseButtonType.Player => PlayerAction,
            UseButtonType.Menu => MenuAction,
        } ;
        if (!table.AxisData.ContainsKey(buttonType))
            table.AxisData.Add(buttonType,action);
        else
        {
            table.AxisData.Remove(buttonType);
            table.AxisData.Add(buttonType,action);
        }
    }
}
public enum UseButtonType
{
    Player,
    Menu,
    Message,
    Debug,
}


