using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerState
{
    PlayerStatus State { get; }
    void Entry();
    void Update();
    void Exit();
}

public enum PlayerStatus{Field,Battle,}