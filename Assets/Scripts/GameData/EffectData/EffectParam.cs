using System;
using UnityEngine;

[Serializable]
public class EffectParam
{
	[EffectId]
	[InspectorName("効果")]
	public string effectId;

	[Header("基本値")]
	public int value = 100;

	[Header("割合")]
	public float rate = 0f;

	[Header("継続ターン")]
	public int durationTurn = 0;

	[Header("成功率")]
	[Range(0f, 1f)]
	public float successRate = 1f;
}