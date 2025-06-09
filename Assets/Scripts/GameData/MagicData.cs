using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMagic", menuName = "Magic")]
public class MagicData : ScriptableObject
{
    public string magicName;
    public string explanation;

    public int mpCost;
    public float power;
    public float castTime;

    public ElementType elementType; 
    public MagicTargetType targetType;

    public GameObject effectPrefab;
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

public enum MagicTargetType { 
    SingleEnemy, 
    AllEnemies, 
    SingleAlly,
    AllAllies,
    Self 
}