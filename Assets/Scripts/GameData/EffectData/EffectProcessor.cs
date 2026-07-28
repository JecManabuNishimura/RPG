using UnityEngine;

public static class EffectProcessor
{
	public static void Apply(EffectRef effectRef, CharacterState state)
	{
        switch (effectRef.conditionId)
        {
            case "hp_recover":
                ApplyHpRecover(effectRef, state);
                break;

            case "mp_recover":
                ApplyMpRecover(effectRef, state);
                break;

            case "add_poison":
                ApplyPoison(effectRef, state);
                break;
            case "death":
                state.DethFlag = true;
				state.parameter.Hp = 0;
                break;
            case "paralysis":
                break;
            case "stun":
                break;
            case "burn":
                break;
            case "petrification":
                break;
            case "none":
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