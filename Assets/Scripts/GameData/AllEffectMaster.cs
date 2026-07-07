using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllEffectMaster", menuName = "Scriptable Objects/AllEffectMaster")]
public class AllEffectMaster : ScriptableObject
{
	[SerializeField]
	private List<EffectDefinition> effects = new List<EffectDefinition>();

	public IReadOnlyList<EffectDefinition> Effects => effects;

	public EffectDefinition GetDefinition(ConditionType effectType)
	{
		foreach (var effect in effects)
		{
			if (effect.conditionType == effectType)
			{
				return effect;
			}
		}

		return null;
	}
}

[Serializable]
public enum ConditionType
{
    [InspectorName("‚È‚µ")]
    None,

    [InspectorName("HP‰ñ•œ")]
    HpRecover,

    [InspectorName("MP‰ñ•œ")]
    MpRecover,

    [InspectorName("“Å•t—^")]
    AddPoison,

    [InspectorName("€–S")]
    Death,

    [InspectorName("–ƒáƒ")]
    Paralysis,

    [InspectorName("ƒXƒ^ƒ“")]
    Stun,

    [InspectorName("‰Î")]
    Burn,

    [InspectorName("Î‰»")]
    Petrification,

}