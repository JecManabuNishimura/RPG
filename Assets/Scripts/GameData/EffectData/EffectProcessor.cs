using UnityEngine;

public static class EffectProcessor
{
	public static void Apply(EffectRef effectRef, CharacterState state)
	{
        switch (effectRef.conditionType)
        {
            case ConditionType.HpRecover:
                ApplyHpRecover(effectRef, state);
                break;

            case ConditionType.MpRecover:
                ApplyMpRecover(effectRef, state);
                break;

            case ConditionType.AddPoison:
                ApplyPoison(effectRef, state);
                break;
            case ConditionType.Death:
				state.DethFlag = true;
				state.parameter.Hp = 0;
                break;
            case ConditionType.Paralysis:
                break;
            case ConditionType.Stun:
                break;
            case ConditionType.Burn:
                break;
            case ConditionType.Petrification:
                break;
            case ConditionType.None:
				break;
            default:
                break;
        }
    }

    private static void ApplyHpRecover(EffectRef effectRef, CharacterState state)
	{
		int value = effectRef.power;

		if (effectRef.rate > 0f)
		{
			value += Mathf.RoundToInt(state.parameter.MaxHp * effectRef.rate);
		}

		state.Healing(value);
	}

	private static void ApplyMpRecover(EffectRef effectRef, CharacterState state)
	{
		int value = effectRef.power;

		if (effectRef.rate > 0f)
		{
			value += Mathf.RoundToInt(state.parameter.MaxMp * effectRef.rate);
		}

        state.RecoverMp(value);
	}

	private static void ApplyPoison(EffectRef effectRef, CharacterState state)
	{
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