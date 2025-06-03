using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public GameObject menuObj;
    public MenuController _menuController;
    public Action playerEndMenuHandl;
    public MenuList nowSelect;
    public MenuList prevSelect;

    public Stack<(MenuList,int,int)> cursorPos = new();
    public bool isOpenMenu;
    
    public int selectPlayerNum;
    public bool moveFlag = false;

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

    private void Start()
    {
        InputManager.Instance.SetKeyEvent(UseButtonType.Menu,InputMaster.Entity.FieldKey.Enter,ShowMenu);
        InputManager.Instance.SetKeyEvent(UseButtonType.Menu,InputMaster.Entity.FieldKey.Cancel,CloseMenu);
        InputManager.Instance.SetKeyEvent(UseButtonType.Menu,InputMaster.Entity.FieldKey.Up,CursorUp);
        InputManager.Instance.SetKeyEvent(UseButtonType.Menu,InputMaster.Entity.FieldKey.Down,CursorDown);
        InputManager.Instance.SetKeyEvent(UseButtonType.Menu,InputMaster.Entity.FieldKey.Right,CursorRight);
        InputManager.Instance.SetKeyEvent(UseButtonType.Menu,InputMaster.Entity.FieldKey.Left,CursorLeft);
        InputManager.Instance.SetKeyboardEvent(UseButtonType.Menu,KeyCode.Return,ShowMenu,true);
        InputManager.Instance.SetKeyboardEvent(UseButtonType.Menu,KeyCode.Escape,CloseMenu,true);
        InputManager.Instance.SetKeyboardEvent(UseButtonType.Menu,KeyCode.UpArrow,CursorUp,true);
        InputManager.Instance.SetKeyboardEvent(UseButtonType.Menu,KeyCode.DownArrow,CursorDown,true);
        InputManager.Instance.SetKeyboardEvent(UseButtonType.Menu,KeyCode.RightArrow,CursorRight,true);
        InputManager.Instance.SetKeyboardEvent(UseButtonType.Menu,KeyCode.LeftArrow,CursorLeft,true);
    }

    public GameObject GetCursorObj(MenuList list)
    {
        foreach (var obj in _menuController.menuObjs)
        {
            if (obj.keyList == list)
            {
                return obj.Cursor;
            }
        }

        return null;
    }
    

    public MenuObj GetWindow(MenuList list)
    {
        foreach (var obj in _menuController.menuObjs)
        {
            if (obj.keyList == list)
            {
                return obj;
            }
        }
        return null;
    }

    public void CloseOpenMenu()
    {
        foreach(var obj in _menuController.menuObjs)
        {
            if (obj.openMenu && !obj.notCloseMenu)
            {
                obj.openMenu = false;
                obj.gameObject.SetActive(false);
                obj.Cursor.transform.position = obj.tranInitPos;
            }
            else if(obj.notCloseMenu)
            {
                obj.Cursor.transform.position = obj.tranInitPos;
            }
        }
    }
    public GameObject GetBattleCursorObj(MenuList list)
    {
        foreach (var obj in _menuController.BattleMenuObjs)
        {
            if (obj.keyList == list)
            {
                return obj.Cursor;
            }
        }

        return null;
    }
    public MenuObj GetBattleMenuWindow(MenuList list)
    {
        foreach (var obj in _menuController.BattleMenuObjs)
        {
            if (obj.keyList == list)
            {
                return obj;
            }
        }
        return null;
    }

    public void CloseDialog()
    {
        GameManager.Instance.mode = Now_Mode.Field;
        GameManager.Instance.state = Now_State.Active;
        _menuController.ChangeMenu(MenuList.None);
        // フィールドに戻る
        LoadSceneManager.LoadScene("SampleScene");
        // フィールドが違う場合でも、強制定期に、指定になるので、
        // 戦闘前に流れていたBGMをONにする仕組みに変える必要がある
        SoundMaster.Entity.PlayBGMSound(PlaceOfSound.FieldMusic); 
    }


    public void OpenMenu(MenuList menu)
    {
        isOpenMenu = true;
        GameManager.Instance.state = Now_State.Menu;
        _menuController.ChangeMenu(menu);
    }

    public void OpenBattleMenu()
    {
        //MenuButton.ActiveButton();
        _menuController.ChangeMenu(MenuList.Battle);
    }
    
    public void ShowMenu() => _menuController.SelectMenu();
    public void CloseMenu() =>_menuController.CloseMenu();

    public void CursorUp()
    {
        if (!moveFlag)
        {
            moveFlag = true;
            _menuController.CursorUp();    
        }

        if (!InputManager.Instance.CheckActionPress(UseButtonType.Menu ,InputMaster.Entity.FieldKey.Up.type))
        {
            moveFlag = false;
        }
    }

    public void CursorDown()
    {
        if (!moveFlag)
        {
            moveFlag = true;
            _menuController.CursorDown();    
        }
        if (!InputManager.Instance.CheckActionPress(UseButtonType.Menu ,InputMaster.Entity.FieldKey.Down.type))
        {
            moveFlag = false;
        }
    }

    public void CursorRight()
    {
        
        if (!moveFlag)
        {
            moveFlag = true;
            _menuController.CursorRight();    
        }
        if (!InputManager.Instance.CheckActionPress(UseButtonType.Menu ,InputMaster.Entity.FieldKey.Right.type))
        {
            moveFlag = false;
        }
    }

    public void CursorLeft()
    {
        if (!moveFlag)
        {
            moveFlag = true;
            _menuController.CursorLeft();    
        }
        if (!InputManager.Instance.CheckActionPress(UseButtonType.Menu ,InputMaster.Entity.FieldKey.Left.type))
        {
            moveFlag = false;
        }
    }
}
