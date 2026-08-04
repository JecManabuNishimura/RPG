using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AllEffectMaster", menuName = "Master/AllEffectMaster")]
public class AllEffectMaster : ScriptableObject
{
    [SerializeField]
    private TextAsset csvFile;

    [SerializeField]
    private List<EffectDefinition> effects = new List<EffectDefinition>();

    public IReadOnlyList<EffectDefinition> Effects => effects;

    public EffectDefinition GetDefinition(string conditionId)
    {
        foreach (var effect in effects)
        {
            if (effect.conditionId == conditionId)
            {
                return effect;
            }
        }

        return null;
    }

#if UNITY_EDITOR
    public void ImportFromCsv()
    {
        effects.Clear();

        if (csvFile == null)
        {
            Debug.LogWarning("CSVÉtÉ@ÉCÉãÇ™ê›íËÇ≥ÇÍÇƒÇ¢Ç‹ÇπÇÒ");
            return;
        }

        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            string[] columns = line.Split(',');

            if (columns.Length < 3)
            {
                Debug.LogWarning($"CSVÇÃóÒêîÇ™ë´ÇËÇ‹ÇπÇÒ: {line}");
                continue;
            }

            EffectDefinition definition = new EffectDefinition
            {
                conditionId = columns[0].Trim(),
                effectName = columns[1].Trim(),
                category = columns[2].Trim()
            };

            effects.Add(definition);
        }

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log("EffectMaster CSVÇì«Ç›çûÇ›Ç‹ÇµÇΩ");
    }
#endif
}
