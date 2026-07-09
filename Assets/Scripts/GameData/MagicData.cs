using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMagic", menuName = "Magic")]
public class MagicData : ScriptableObject
{
    public string magicName;
    public string explanation;

    public int mpCost;
    public int power;
    public float castTime;
    public List<EffectRef> Effect;      // 効果の種類
    public ElementType elementType; 
    public TargetType targetType;

    public GameObject effectPrefab;
    [SpriteSearch("Assets/Sprites/GameData/Icon"), Preview]
    public Sprite icon;
}

public enum ElementType
{
    None,
    Fire,
    Ice,
    Lightning,
    Heal,
    Wind,
    Earth,
    Light,
    Dark,
    Poison,
    Holy,
    Curse,
    Physical,
    Water,
}

[Serializable]
public enum TargetType {
    [InspectorName("敵単体")]
    SingleEnemy,
    [InspectorName("敵全体")]
    AllEnemies,
    [InspectorName("味方単体")]
    SingleAlly,
    [InspectorName("味方全体")]
    AllAllies,
    [InspectorName("使用者")]
    Self,
    None,
}