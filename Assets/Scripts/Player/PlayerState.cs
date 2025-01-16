using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMenu : IPlayerState
{
    private Player _player;
    public PlayerStatus State { get; }
    public PlayerStateMenu(Player player) => _player = player;
    public void Entry()
    {
        throw new System.NotImplementedException();
    }

    public void Update()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }
}
public class PlayerStateField : IPlayerState
{
    private Player _player;
    
    public PlayerStatus State { get; }
    public PlayerStateField(Player player) => _player = player;
    private bool _menuFlag = false;
    
    public void Entry()
    {

        Action talk = () =>
        {
            if (_player.hitObj != null && !GameManager.Instance.eventFlag)
            {
                //===============================================================================
                // 会話開始
                //===============================================================================
                GameManager.Instance.mode = Now_Mode.Menu;
                GameManager.Instance.eventFlag = true;
                _player.hitObj.GetComponent<INpc>()?.Talking();
            }
            else
            {
                if (!_menuFlag && !GameManager.Instance.eventFlag)
                {
                    if (GameManager.Instance.mode == Now_Mode.Field)
                    {
                        //===============================================================================
                        // メニューオープン
                        //===============================================================================
                        MenuManager.Instance.OpenMenu(MenuList.Main);
                        _menuFlag = true;

                    }
                }
            }
        };
        InputManager.Instance.SetKeyEvent(UseButtonType.Player, InputMaster.Entity.FieldKey.Enter, talk);
        InputManager.Instance.SetKeyboardEvent(UseButtonType.Player, KeyCode.Return, talk,true);
        
        MenuManager.Instance.playerEndMenuHandl = () =>
        {
            GameManager.Instance.state = Now_State.Active;
            _menuFlag = false;
            GameManager.Instance.eventFlag = false;
        };
        var transform = _player.transform;
        transform.position = PlayerDataRepository.Instance.playerPos;
//        transform.GetChild(0).rotation = PlayerDataRepository.Instance.playerRota;
    }

    public void Update()
    {
        if (GameManager.Instance.eventFlag && _player.hitObj != null)
            if(_player.hitObj.TryGetComponent(out Npc npcComponent))
                if (npcComponent.isEnd)
                    GameManager.Instance.eventFlag = false;
        
        _player._playerMovement.isMove = false;
        
        if (_menuFlag || GameManager.Instance.eventFlag) return;
        
        _player._playerMovement.isMove = true;
        
        
        var transform = _player.transform;
        PlayerDataRepository.Instance.playerPos = transform.position;
//        PlayerDataRepository.Instance.playerRota = transform.GetChild(0).rotation;
    }

    public void Exit()
    {
    }
    

}
