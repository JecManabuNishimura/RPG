using UnityEngine;

public static class EffectProcessor
{
	public static void Apply(EffectRef effectRef, EffectContext context)
	{
		switch (effectRef.effectType)
		{
			case EffectType.HpRecover:
				ApplyHpRecover(effectRef, context);
				break;

			case EffectType.MpRecover:
				ApplyMpRecover(effectRef, context);
				break;

			case EffectType.AddPoison:
				ApplyPoison(effectRef, context);
				break;

			case EffectType.None:
			default:
				break;
		}
	}

	private static void ApplyHpRecover(EffectRef effectRef, EffectContext context)
	{
		if (context.Target == null) return;

		int value = effectRef.power;

		if (effectRef.rate > 0f)
		{
			value += Mathf.RoundToInt(context.Target.parameter.MaxHp * effectRef.rate);
		}

		context.Target.Healing(value);
	}

	private static void ApplyMpRecover(EffectRef effectRef, EffectContext context)
	{
		if (context.Target == null) return;

		int value = effectRef.power;

		if (effectRef.rate > 0f)
		{
			value += Mathf.RoundToInt(context.Target.parameter.MaxMp * effectRef.rate);
		}

		context.Target.RecoverMp(value);
	}

	private static void ApplyPoison(EffectRef effectRef, EffectContext context)
	{
		if (context.Target == null) return;

		if (Random.value > effectRef.successRate)
		{
			Debug.Log("“Å•t—^‚ÉŽ¸”s");
			return;
		}
		/*
		context.Target.AddPoison(
			effectRef.power,
			effectRef.durationTurn
		);*/
	}
}