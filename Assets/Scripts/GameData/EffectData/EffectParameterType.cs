using System;
using UnityEngine;

[Flags]
public enum EffectParameterType
{
    [InspectorName("‚È‚µ")]
    None = 0,

    [InspectorName("Œø‰Ê—Í")]
    Power = 1 << 0,

    [InspectorName("Š„‡")]
    Rate = 1 << 1,

    [InspectorName("Ž‘±ƒ^[ƒ“")]
    DurationTurn = 1 << 2,

    [InspectorName("¬Œ÷—¦")]
    SuccessRate = 1 << 3,
}