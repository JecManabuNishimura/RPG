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

        SerializedProperty conditionIdProp = property.FindPropertyRelative("conditionId");
        SerializedProperty targetTypeProp = property.FindPropertyRelative("targetType");
        SerializedProperty parameterTypesProp = property.FindPropertyRelative("parameterTypes");

        Rect lineRect = new Rect(
            position.x,
            position.y,
            position.width,
            EditorGUIUtility.singleLineHeight
        );

        if (conditionIdProp == null)
        {
            EditorGUI.HelpBox(
                lineRect,
                "EffectRef に conditionId がありません",
                MessageType.Error
            );

            EditorGUI.EndProperty();
            return;
        }

        DrawConditionDropdown(lineRect, conditionIdProp);

        NextLine(ref lineRect);

        if (targetTypeProp != null)
        {
            EditorGUI.PropertyField(
                lineRect,
                targetTypeProp,
                new GUIContent("対象")
            );
        }

        NextLine(ref lineRect);

        if (parameterTypesProp != null)
        {
            EditorGUI.PropertyField(
                lineRect,
                parameterTypesProp,
                new GUIContent("使用パラメータ")
            );
        }

        EffectParameterType paramTypes = EffectParameterType.None;

        if (parameterTypesProp != null)
        {
            paramTypes = (EffectParameterType)parameterTypesProp.intValue;
        }

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

    private void DrawConditionDropdown(Rect rect, SerializedProperty conditionIdProp)
    {
        if (conditionIdProp == null)
        {
            EditorGUI.HelpBox(rect, "conditionId が見つかりません", MessageType.Error);
            return;
        }

        AllEffectMaster master = FindAllEffectMaster();

        if (master == null)
        {
            EditorGUI.PropertyField(rect, conditionIdProp, new GUIContent("効果ID"));
            return;
        }

        var effects = master.Effects;

        if (effects == null)
        {
            EditorGUI.PropertyField(rect, conditionIdProp, new GUIContent("効果ID"));
            return;
        }

        string currentId = conditionIdProp.stringValue;
        string currentLabel = "なし";

        foreach (var effect in effects)
        {
            if (effect == null) continue;

            if (effect.conditionId == currentId)
            {
                currentLabel = string.IsNullOrEmpty(effect.effectName)
                    ? effect.conditionId
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
                string.IsNullOrEmpty(currentId),
                "",
                conditionIdProp
            );

            menu.AddSeparator("");

            foreach (var effect in effects
                         .Where(e => e != null)
                         .OrderBy(e => e.category)
                         .ThenBy(e => e.effectName))
            {
                string category = string.IsNullOrEmpty(effect.category)
                    ? "その他"
                    : effect.category;

                string effectName = string.IsNullOrEmpty(effect.effectName)
                    ? effect.conditionId
                    : effect.effectName;

                string menuPath = $"{category}/{effectName}";

                AddConditionMenuItem(
                    menu,
                    menuPath,
                    currentId == effect.conditionId,
                    effect.conditionId,
                    conditionIdProp
                );
            }

            menu.DropDown(buttonRect);
        }
    }

    private void AddConditionMenuItem(
        GenericMenu menu,
        string menuPath,
        bool isSelected,
        string selectedId,
        SerializedProperty conditionIdProp
    )   
    {
        if (conditionIdProp == null)
        {
            return;
        }

        SerializedObject serializedObject = conditionIdProp.serializedObject;
        string propertyPath = conditionIdProp.propertyPath;

        menu.AddItem(
            new GUIContent(menuPath),
            isSelected,
            () =>
            {
                serializedObject.Update();

                SerializedProperty property = serializedObject.FindProperty(propertyPath);

                if (property != null)
                {
                    property.stringValue = selectedId;
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