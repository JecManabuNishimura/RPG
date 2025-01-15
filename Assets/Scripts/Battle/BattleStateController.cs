using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;

public class BattleStateController : MonoBehaviour
{
    public BattleContext _context;
    void Start()
    {
        _context = new ();
        _context.Init(this,BattleList.Start);
    }
    
    void Update() => _context.Update();
    public void ChangeBattleState(BattleList next) => _context.ChangeState(next);
}
