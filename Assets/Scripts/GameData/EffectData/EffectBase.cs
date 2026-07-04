using UnityEngine;

public abstract class EffectBase : ScriptableObject
{
	[SerializeField]
	private string effectName;

	public string EffectName => effectName;

	public abstract EffectParameterType ParameterTypes { get; }

	public abstract void Apply(EffectContext context, EffectRef effectRef);
}