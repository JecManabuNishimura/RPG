using System;
using Interface;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GameData.Item;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using JetBrains.Annotations;

public class CharacterState: MonoBehaviour, ICharacter
{
    public Parameter parameter = new();
    public string CharaName;
    
    // 攻撃系
    public List<CharacterState> to = new ();
    public AttackType attackType;
    public string ActionCommand;
    public bool DethFlag;
    public bool ActionFlag;
    public string selectMagicName;

    public virtual void Damage(int damage,ElementType eType, bool criticalFlag)
    {
        parameter.Hp = Mathf.Max(parameter.Hp - damage, 0);
    }
    public virtual void Healing(int healing)
    {
        parameter.Hp = Mathf.Min(parameter.Hp + healing, parameter.MaxHp);
    }

    public bool UseItem(ItemData item)
    {
        switch (item.Subject)
        {
            case SubjectType.HP:
                if (parameter.Hp == parameter.MaxHp)
                {
                    return false;
                }
                parameter.Hp = Mathf.Clamp(parameter.Hp + item.Power,0,parameter.MaxHp);
                
                break;
            case SubjectType.MP:
                parameter.Mp += item.Power;
                break;
        }

        return true;
    }

    public virtual IEnumerator PerformAction()
    {
        yield break ;
    }
}

[Serializable]
public class AttackPattern
{
    [Attack]
    public int id;

    public int weight;
}
[Serializable]
public enum AttackType
{
    Attack,
    Magic,
    Item,
    Guard,
}

[Serializable]
public class Parameter
{
    string Gender = "";
    public int MaxHp;
    public int MaxMp;
	public int Hp;
	public int Mp;
    public int str;
    public int Res;
    public int Mga;
    public int Mgd;
    public int Atk;
    public int Def;
    public int Qui;
    public int Luc;
    public ElementType attribute;
    public int Exp;
    public bool DefFlag;     // 防御
    
    public static Parameter operator +(Parameter param1, Parameter param2)
    {
        Parameter result = new Parameter();
        result.Hp = param1.Hp + param2.Hp;
        result.MaxHp = param1.MaxHp + param2.MaxHp;
        result.Mp = param1.Mp + param2.Mp;
        result.Atk = param1.Atk + param2.Atk;
        result.Def = param1.Def + param2.Def;
        result.Qui = param1.Qui + param2.Qui;
        result.Mga = param1.Mga + param2.Mga;
        result.Mgd = param1.Mgd + param2.Mgd;
        result.str = param1.str + param2.str;
        result.Res = param1.Res + param2.Res;
        result.Luc = param1.Luc + param2.Luc;
        return result;
    }
    public static Parameter operator -(Parameter param1, Parameter param2)
    {
        Parameter result = new Parameter();
        result.Hp = param1.Hp - param2.Hp;
        result.MaxHp = param1.MaxHp - param2.MaxHp;
        result.Mp = param1.Mp - param2.Mp;
        result.Atk = param1.Atk - param2.Atk;
        result.Def = param1.Def - param2.Def;
        result.Qui = param1.Qui - param2.Qui;
        result.Mga = param1.Mga - param2.Mga;
        result.Mgd = param1.Mgd - param2.Mgd;
        result.str = param1.str - param2.str;
        result.Res = param1.Res - param2.Res;
        result.Luc = param1.Luc - param2.Luc;
        return result;
    }
}

public class DataReader
{
    public static List<string[]> ReadData(string fileName)
    {
        List<string[]> csvDatas = new();
        TextAsset csvData = Resources.Load(fileName) as TextAsset;
        if (csvData != null)
        {
            StringReader reader = new StringReader(csvData.text);
            while (reader.Peek() != -1)
            {
                string line = reader.ReadLine();
                csvDatas.Add(line.Split(','));
            }
        }

        return csvDatas;
    }
}

