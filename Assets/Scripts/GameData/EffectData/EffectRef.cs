using System;
using UnityEngine;

[Serializable]
public class EffectRef
{
    public ConditionType conditionType = ConditionType.None;

    public EffectParameterType parameterTypes = EffectParameterType.None;

    public int power = 0;

    public float rate = 0f;

    public int durationTurn = 0;

    [Range(0f, 1f)]
    public float successRate = 1f;

    
}

