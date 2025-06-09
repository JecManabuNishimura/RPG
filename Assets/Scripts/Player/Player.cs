using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    public PlayerMovement _playerMovement;
    private PlayerContext _context;
    public List<PlayerDataRepository.HubItemData> hubItems = new ();
    public GameObject hitObj;

    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        
        _context = new PlayerContext();
        _context.Init(this,PlayerStatus.Field);
    }
    
    void Update() => _context.Update();
    void FixedUpdate()
    {
        hubItems = PlayerDataRepository.Instance.ItemList;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        hitObj = other.gameObject;

        var fieldItem = other.GetComponent<FieldItem>();
        if (fieldItem != null)
        {
            int itemId = fieldItem.GetItem();
            PlayerDataRepository.Instance.GetItem(itemId);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        hitObj = null;
    }

}
