using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleState
{
    BattleList State { get; }
    void Entry();
    void Update();
    void Exit();
}

public enum BattleList : int
{
    None,
    Start,
    PlayerCommand,
    Battle,
    End,
}
