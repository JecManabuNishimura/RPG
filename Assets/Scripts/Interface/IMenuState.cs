
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMenuState
{
    
    MenuList State { get; }
    void Entry();
    void Update();
    void Exit();
    void SelectMenu();

    void CloseMenu();

    void CursorUp();
    void CursorDown();
    void CursorRight();
    void CursorLeft();
}

public enum MenuList : int
{
    None,
    Main,
    SelectChara,
    Talk,
    Spell,
    ItemList,
    Check,
    Strength,
    Tactics,
    Battle,
    Battle_Enemy,
    Battle_Result,
    Shop,
    Shop_Tool,
    Shop_Weapon,
    Shop_Buy,
    EquipmentMenu,
    EquipmentMenu1,
    EquipmentMenu2,
}
