using System;

[Serializable]
public class EffectParameterInfo
{
	public EffectParameterType type;
	public string propertyName;
	public string labelName;

	public EffectParameterInfo(
		EffectParameterType type,
		string propertyName,
		string labelName
	)
	{
		this.type = type;
		this.propertyName = propertyName;
		this.labelName = labelName;
	}
}