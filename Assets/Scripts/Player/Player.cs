using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    public PlayerMovement _playerMovement;
    private PlayerContext _context;
    public GameObject hitObj;

    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        
        _context = new PlayerContext();
        _context.Init(this,PlayerStatus.Field);
    }
    
    void Update() => _context.Update();

    private void OnTriggerEnter2D(Collider2D other)
    {
        hitObj = other.gameObject;
        var itemdata = other.GetComponent<IItem>();
        if(itemdata != null)
            PlayerDataRepository.Instance.GetItem(itemdata.GetItem());

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        hitObj = null;
    }

}
