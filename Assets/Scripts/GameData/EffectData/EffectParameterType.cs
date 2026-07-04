using System;

[Flags]
public enum EffectParameterType
{
	None = 0,

	Power = 1 << 0,
	Rate = 1 << 1,
	DurationTurn = 1 << 2,
	SuccessRate = 1 << 3,
}