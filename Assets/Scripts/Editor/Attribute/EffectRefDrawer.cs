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
        int lineCount = 3; // 効果 + 対象 + 使用パラメータ

        SerializedProperty parameterTypesProp = property.FindPropertyRelative("parameterTypes");

        if (parameterTypesProp != null)
        {
            EffectParameterType paramTypes =
                (EffectParameterType)parameterTypesProp.intValue;

            foreach (var info in ParamDrawInfos)
            {
                if (paramTypes.HasFlag(info.Type))
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

        SerializedProperty conditionTypeProp = property.FindPropertyRelative("conditionType");
        SerializedProperty parameterTypesProp = property.FindPropertyRelative("parameterTypes");

        Rect lineRect = new Rect(
            position.x,
            position.y,
            position.width,
            EditorGUIUtility.singleLineHeight
        );

        DrawConditionDropdown(lineRect, conditionTypeProp);

        // 使用パラメータ
        NextLine(ref lineRect);

        EditorGUI.PropertyField(
            lineRect,
            parameterTypesProp,
            new GUIContent("使用パラメータ")
        );

        EffectParameterType paramTypes =
            (EffectParameterType)parameterTypesProp.intValue;

        EditorGUI.indentLevel++;

        foreach (var info in ParamDrawInfos)
        {
            if (!paramTypes.HasFlag(info.Type))
            {
                continue;
            }

            SerializedProperty paramProp = property.FindPropertyRelative(info.PropertyName);

            if (paramProp == null)
            {
                continue;
            }

            NextLine(ref lineRect);

            EditorGUI.PropertyField(
                lineRect,
                paramProp,
                new GUIContent(info.Label)
            );
        }

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }

    private void DrawConditionDropdown(Rect rect, SerializedProperty conditionTypeProp)
    {
        AllEffectMaster master = FindAllEffectMaster();

        if (master == null)
        {
            EditorGUI.PropertyField(rect, conditionTypeProp, new GUIContent("効果"));
            return;
        }

        var effects = master.Effects
            .Where(e => e != null)
            .ToList();

        ConditionType currentType = (ConditionType)conditionTypeProp.intValue;

        string currentLabel = "なし";

        foreach (var effect in effects)
        {
            if (effect.conditionType == currentType)
            {
                currentLabel = string.IsNullOrEmpty(effect.effectName)
                    ? effect.conditionType.ToString()
                    : effect.effectName;

                break;
            }
        }

        Rect buttonRect = EditorGUI.PrefixLabel(rect, new GUIContent("効果"));

        if (EditorGUI.DropdownButton(buttonRect, new GUIContent(currentLabel), FocusType.Keyboard))
        {
            GenericMenu menu = new GenericMenu();

            AddConditionMenuItem(
                menu,
                "なし",
                currentType == ConditionType.None,
                ConditionType.None,
                conditionTypeProp
            );

            menu.AddSeparator("");

            foreach (var effect in effects.OrderBy(e => e.category).ThenBy(e => e.effectName))
            {
                string category = string.IsNullOrEmpty(effect.category)
                    ? "その他"
                    : effect.category;

                string effectName = string.IsNullOrEmpty(effect.effectName)
                    ? effect.conditionType.ToString()
                    : effect.effectName;

                string menuPath = $"{category}/{effectName}";

                AddConditionMenuItem(
                    menu,
                    menuPath,
                    currentType == effect.conditionType,
                    effect.conditionType,
                    conditionTypeProp
                );
            }

            menu.DropDown(buttonRect);
        }
    }

    private void AddConditionMenuItem(
    GenericMenu menu,
    string menuPath,
    bool isSelected,
    ConditionType selectedType,
    SerializedProperty conditionTypeProp
)
    {
        SerializedObject serializedObject = conditionTypeProp.serializedObject;
        string propertyPath = conditionTypeProp.propertyPath;

        menu.AddItem(
            new GUIContent(menuPath),
            isSelected,
            () =>
            {
                serializedObject.Update();

                SerializedProperty property = serializedObject.FindProperty(propertyPath);

                if (property != null)
                {
                    property.intValue = (int)selectedType;
                }

                serializedObject.ApplyModifiedProperties();
            }
        );
    }

    private void NextLine(ref Rect rect)
    {
        rect.y += EditorGUIUtility.singleLineHeight + Space;
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