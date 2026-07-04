#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EffectRef))]
public class EffectRefDrawer : PropertyDrawer
{
	private const float Space = 2f;

	private struct ParamDrawInfo
	{
		public EffectParameterType Type;
		public string PropertyName;
		public string Label;

		public ParamDrawInfo(EffectParameterType type, string propertyName, string label)
		{
			Type = type;
			PropertyName = propertyName;
			Label = label;
		}
	}

	private static readonly ParamDrawInfo[] ParamDrawInfos =
	{
		new ParamDrawInfo(EffectParameterType.Power, "power", "基本値"),
		new ParamDrawInfo(EffectParameterType.Rate, "rate", "割合"),
		new ParamDrawInfo(EffectParameterType.DurationTurn, "durationTurn", "継続ターン"),
		new ParamDrawInfo(EffectParameterType.SuccessRate, "successRate", "成功率"),
	};

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		int lineCount = 1;

		EffectDefinition selectedDefinition = FindSelectedDefinition(property);

		if (selectedDefinition != null)
		{
			foreach (var info in ParamDrawInfos)
			{
				if (selectedDefinition.parameterTypes.HasFlag(info.Type))
				{
					lineCount++;
				}
			}
		}

		return lineCount * EditorGUIUtility.singleLineHeight
			   + (lineCount - 1) * Space;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		SerializedProperty effectTypeProp = property.FindPropertyRelative("effectType");

		AllEffectMaster master = FindAllEffectMaster();

		Rect lineRect = new Rect(
			position.x,
			position.y,
			position.width,
			EditorGUIUtility.singleLineHeight
		);

		if (master == null)
		{
			EditorGUI.PropertyField(lineRect, effectTypeProp, new GUIContent("効果"));
			EditorGUI.EndProperty();
			return;
		}

		var effects = master.Effects
			.Where(e => e != null)
			.ToList();

		EffectType[] effectTypes = new EffectType[effects.Count + 1];
		string[] displayNames = new string[effects.Count + 1];

		effectTypes[0] = EffectType.None;
		displayNames[0] = "なし";

		for (int i = 0; i < effects.Count; i++)
		{
			EffectDefinition definition = effects[i];

			effectTypes[i + 1] = definition.effectType;

			if (!string.IsNullOrEmpty(definition.effectName))
			{
				displayNames[i + 1] = definition.effectName;
			}
			else
			{
				displayNames[i + 1] = definition.effectType.ToString();
			}
		}

		EffectType currentType = (EffectType)effectTypeProp.enumValueIndex;

		int currentIndex = Array.IndexOf(effectTypes, currentType);

		if (currentIndex < 0)
		{
			currentIndex = 0;
		}

		int selectedIndex = EditorGUI.Popup(
			lineRect,
			"効果",
			currentIndex,
			displayNames
		);

		effectTypeProp.enumValueIndex = (int)effectTypes[selectedIndex];

		EffectDefinition selectedDefinition = null;

		if (selectedIndex > 0)
		{
			selectedDefinition = effects[selectedIndex - 1];
		}

		if (selectedDefinition != null)
		{
			EditorGUI.indentLevel++;

			foreach (var info in ParamDrawInfos)
			{
				if (!selectedDefinition.parameterTypes.HasFlag(info.Type))
				{
					continue;
				}

				SerializedProperty paramProp = property.FindPropertyRelative(info.PropertyName);

				if (paramProp == null)
				{
					continue;
				}

				NextLine(ref lineRect);
				EditorGUI.PropertyField(lineRect, paramProp, new GUIContent(info.Label));
			}

			EditorGUI.indentLevel--;
		}

		EditorGUI.EndProperty();
	}

	private void NextLine(ref Rect rect)
	{
		rect.y += EditorGUIUtility.singleLineHeight + Space;
	}

	private EffectDefinition FindSelectedDefinition(SerializedProperty property)
	{
		SerializedProperty effectTypeProp = property.FindPropertyRelative("effectType");

		if (effectTypeProp == null)
		{
			return null;
		}

		AllEffectMaster master = FindAllEffectMaster();

		if (master == null)
		{
			return null;
		}

		EffectType currentType = (EffectType)effectTypeProp.enumValueIndex;

		foreach (EffectDefinition definition in master.Effects)
		{
			if (definition == null) continue;

			if (definition.effectType == currentType)
			{
				return definition;
			}
		}

		return null;
	}

	private AllEffectMaster FindAllEffectMaster()
	{
		string[] guids = AssetDatabase.FindAssets("t:AllEffectMaster");

		if (guids == null || guids.Length == 0)
		{
			return null;
		}

		string path = AssetDatabase.GUIDToAssetPath(guids[0]);
		return AssetDatabase.LoadAssetAtPath<AllEffectMaster>(path);
	}
}
#endif