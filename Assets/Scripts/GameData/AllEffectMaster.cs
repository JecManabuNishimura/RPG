using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllEffectMaster", menuName = "Scriptable Objects/AllEffectMaster")]
public class AllEffectMaster : ScriptableObject
{
	[SerializeField]
	private List<EffectDefinition> effects = new List<EffectDefinition>();

	public IReadOnlyList<EffectDefinition> Effects => effects;

	public EffectDefinition GetDefinition(EffectType effectType)
	{
		foreach (var effect in effects)
		{
			if (effect.effectType == effectType)
			{
				return effect;
			}
		}

		return null;
	}
}