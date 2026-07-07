using System;
using UnityEngine;

[Serializable]
public class EffectDefinition
{
    public ConditionType conditionType = ConditionType.None;

    public string effectName;
    public string category;
}