using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager 
{
    public static NPCManager Instance = new();
    public ItemShopData nowTalkItemShopData = new();

    public void Initialize()
    {
        Instance ??= this;
    }
}


public enum NPCType
{
    Villager,
    ToolShop,
    WeaponShop,
}