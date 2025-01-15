using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class BaseAttribute : PropertyAttribute
{
    public string DataType { get; private set; }

    protected BaseAttribute(string dataType)
    {
        DataType = dataType;
    }
}

public class AttackAttribute : BaseAttribute
{
    public AttackAttribute() : base("Attack") { }
}

public class EffectAttribute : BaseAttribute
{
    public EffectAttribute() : base("Effect") { }
}
public class EventAttribute : BaseAttribute
{
    public EventAttribute() : base("Event") { }
}

[CustomPropertyDrawer(typeof(BaseAttribute), true)]
public class GeneralAttributeDrawer : PropertyDrawer
{
    private bool isInitialized = false;
    private int[] itemIds = null;
    private string[] itemLabels = null;

    // 属性ごとの処理をマッピング
    private readonly Dictionary<Type, Func<Dictionary<int, string>>> _attributeProcessors = new Dictionary<Type, Func<Dictionary<int, string>>>();

    public GeneralAttributeDrawer()
    {
        // 属性とその処理をマッピング
        _attributeProcessors[typeof(AttackAttribute)] = GetAttackItemModelLabels;
        _attributeProcessors[typeof(EffectAttribute)] = GetEffectItemModelLabels;
        _attributeProcessors[typeof(EventAttribute)] = GetEventItemModelLabels;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!isInitialized)
        {
            Initialize();
            isInitialized = true;
        }

        property.intValue = EditorGUI.IntPopup(position, label.text, property.intValue, itemLabels, itemIds);
    }

    private void Initialize()
    {
        var attribute = (BaseAttribute)base.attribute;
        Dictionary<int, string> items = GetItemModelLabels(attribute);
        itemIds = new int[items.Count];
        itemLabels = new string[items.Count];
        items.Keys.CopyTo(itemIds, 0);
        items.Values.CopyTo(itemLabels, 0);
    }

    private Dictionary<int, string> GetItemModelLabels(BaseAttribute attribute)
    {
        if (_attributeProcessors.TryGetValue(attribute.GetType(), out var processor))
        {
            return processor();
        }
        return new Dictionary<int, string>();
    }

    private Dictionary<int, string> GetAttackItemModelLabels()
    {
        Dictionary<int, string> result = new Dictionary<int, string>();

        string[] guids = AssetDatabase.FindAssets("t:AttackMaster");
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            AttackMaster model = AssetDatabase.LoadAssetAtPath<AttackMaster>(assetPath);

            int targetId = 0;
            foreach (var data in model.AttackDatas)
            {
                result[targetId++] = $"{targetId}: {data.name}";
            }
        }

        return result;
    }

    private Dictionary<int, string> GetEffectItemModelLabels()
    {
        Dictionary<int, string> result = new Dictionary<int, string>();

        string[] guids = AssetDatabase.FindAssets("t:EffectMaster");
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            EffectMaster model = AssetDatabase.LoadAssetAtPath<EffectMaster>(assetPath);

            int targetId = 0;
            foreach (var data in model.effectData)
            {
                result[targetId++] = $"{targetId}: {data.name}";
            }
        }

        return result;
    }
    
    private Dictionary<int, string> GetEventItemModelLabels()
    {
        Dictionary<int, string> result = new Dictionary<int, string>();

        string[] guids = AssetDatabase.FindAssets("t:EventMaster");
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            EventMaster model = AssetDatabase.LoadAssetAtPath<EventMaster>(assetPath);

            int targetId = 0;
            foreach (var data in model._eventInfos)
            {
                result[targetId++] = $"{targetId}: {data.number}:{data.name}";
            }
        }

        return result;
    }
}