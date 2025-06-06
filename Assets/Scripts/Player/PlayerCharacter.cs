using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameData.Item;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerCharacter : CharacterState
{
    public int Level;
    public int experience;  // 取得経験値　（レベルアップ時にリセット）
    // レベルアップ系
    public LevelParameter[] levelUpParameter = new LevelParameter[100];
    public LevelParameter LevelUpParameter => levelUpParameter[Level - 2];
    public List<string> magicData = new List<string>(); // 魔法データ
    public bool levelUpFlag;
    public Parameter upParam = new();          // レベルアップ時獲得ステータス
    public int defencePower = 1;
    public WeaponArmorEquipment WeaponArmorEauip = new();
    

    public Parameter TotalParam => WeaponArmorEauip + parameter; // 装備などした時のパラメーター

    public void SetWaeponArmor(WeaponArmorEquipment.Part part, InfoWeaponArmor weapon)
    {
        switch (part)
        {
            case WeaponArmorEquipment.Part.Body:
                WeaponArmorEauip.Body = weapon;
                WeaponArmorEauip.Body.isSet = true;
                break;
            case WeaponArmorEquipment.Part.Hand1:
                WeaponArmorEauip.Hand1 = weapon;
                WeaponArmorEauip.Hand1.isSet = true;
                break;
            case WeaponArmorEquipment.Part.Hand2:
                WeaponArmorEauip.Hand2 = weapon;
                WeaponArmorEauip.Hand2.isSet = true;
                break;
            case WeaponArmorEquipment.Part.Head:
                WeaponArmorEauip.Head = weapon;
                WeaponArmorEauip.Head.isSet = true;
                break;
        }
    }

    public void BattleDataReset()
    {
        to.Clear();
        ActionFlag = false;
    }
    public override IEnumerator PerformAction()
    {
		// １フレーム遅らせないと、メッセージが早くなってしまう
		yield return new WaitForEndOfFrame();
        defencePower = 1;
        parameter.DefFlag = false;
		switch (attackType)
        {
            case AttackType.Attack:
                MessageManager.Instance.StartDialogMessage(CharaName + "のこうげき");
                yield return new WaitUntil(() => MessageManager.Instance.IsEndMessage);
				foreach (var t in to)
                {
                    // ドラクエ風ダメージ計算式
                    int damage = (TotalParam.Atk / 2) - (t.parameter.Def / 4);
                    // クリティカル計算
                    if (parameter.Luc > Random.Range(0, 255))
                    {
                        t.Damage(damage * 2,ElementType.None,true);                            
                    }
                    else
                    {
                        t.Damage(damage,ElementType.None,false);
                    }
					
					yield return new WaitUntil(() => MessageManager.Instance.IsEndMessage);
				}
				break;
            case AttackType.Magic:
                string magicName = selectMagicName;
                MessageManager.Instance.StartDialogMessage(CharaName + "が" + magicName  + "を唱えた");
                yield return new WaitUntil(() => MessageManager.Instance.IsEndMessage);
                foreach (var t in to)
                {
                    var magic = MagicMaster.Entity.GetMagicData(magicName);
                    // ドラクエ風ダメージ計算式
                    int damage =  TotalParam.Mga + (int)magic.power;
                    t.Damage(damage,magic.elementType ,false);
                    yield return new WaitUntil(() => MessageManager.Instance.IsEndMessage);
                }
                break;
            case AttackType.Guard:
				MessageManager.Instance.StartDialogMessage(CharaName + "は　ぼうぎょした");
				defencePower = 2;
				yield return new WaitUntil(() => MessageManager.Instance.IsEndMessage);
				break;
            case AttackType.Item:
                MessageManager.Instance.StartDialogMessage(CharaName + "は" + ItemMaster.Entity.GetItemName(PlayerDataRepository.Instance.selectItemId) +"　しようした");
                yield return new WaitUntil(() => MessageManager.Instance.IsEndMessage);
                foreach (var t in to)
                {
                    PlayerDataRepository.Instance.UseItem(t);    
                    MessageManager.Instance.StartDialogMessage(t.CharaName + "は" + ItemMaster.Entity.GetItemData(PlayerDataRepository.Instance.selectItemId).Power.ToString().ConvertToFullWidth() +"　かいふくした");
                    BattleManager.Instance.ParamChange(); // 表示パラメーター更新
                    yield return new WaitUntil(() => MessageManager.Instance.IsEndMessage);
                }

                // アイテム番号初期化
                PlayerDataRepository.Instance.selectItemId = 0;
                
                yield return new WaitUntil(() => MessageManager.Instance.IsEndMessage);
                break;
        }

        BattleDataReset();
    }
    public override void Damage(int damage,ElementType eType,bool criticalFlag)
    {
        int calcDamage = Mathf.Clamp(damage, 0, damage);
        base.Damage(calcDamage,eType,criticalFlag);
        MessageManager.Instance.StartDialogMessage(CharaName + "は" +
                                                   (criticalFlag ? "クリティカル":"") +
                                                   calcDamage.ToString().ConvertToFullWidth() + "のダメージうけた\n" +
                                                   ((parameter.Hp <= 0) ? CharaName + "は しぼうした" : ""));
        SoundMaster.Entity.PlaySESound(PlaceOfSound.Damage);
		Debug.Log(CharaName + "は" +　calcDamage + "のダメージうけた");
        
        if (parameter.Hp <= 0)
        {
            DethFlag = true;
        }
        BattleManager.Instance.ParamChange(); // 表示パラメーター更新
    }
    
    public void CheckLevelUp()
    {
        var currentLevel = Level;
        var requiredExp = levelUpParameter[currentLevel - 1].Experience - experience;
        var getExp = BattleManager.Instance.GetExp;
		levelUpFlag = false;
		while (getExp > 0)
        {
            getExp--;
            requiredExp--;
            experience++;
            if (requiredExp <= 0)
            {
                Level++;
                currentLevel++;
                requiredExp = levelUpParameter[currentLevel - 1].Experience;
                upParam += levelUpParameter[currentLevel - 2].Parameter;
                parameter += levelUpParameter[currentLevel - 2].Parameter;
                experience = 0;
                levelUpFlag = true;
            }
        }
    }

    // レベル１～指定レベルまで一気にステータスを上げる処理
    public void AdjustmentLevelStatus(int level)
    {
        for(int i = 2; i <= level; i++)
        {
            parameter += levelUpParameter[i - 2].Parameter;
        }
    }
    
    //public void Initialize(string name, int level,Parameter state, MagicData[] magicDatas,int index,string LoadTateName)
    public void Initialize(CharaInitDataParam initDataParam,int index)
    {
        CharaName = initDataParam.name;
        parameter = initDataParam.intParam;
        Level = initDataParam.startLevel;
        
        var playerLevel    = DataReader.ReadData(initDataParam.loadTateName);
        var playerExp = DataReader.ReadData("RequiredExperiencePoints");
        
        
        int counter = 0;
        for(int i=1; i<playerLevel.Count;i++)
        {
            LevelParameter param = new();
            param.Parameter = new();
            param.Level = int.Parse(playerLevel[i][0]);
            param.Parameter.MaxHp = int.Parse(playerLevel[i][1]);
            param.Parameter.MaxMp = int.Parse(playerLevel[i][2]);
            param.Parameter.Atk = int.Parse(playerLevel[i][3]);
            param.Parameter.Def = int.Parse(playerLevel[i][4]);
            param.Parameter.Qui = int.Parse(playerLevel[i][5]);
            param.Parameter.Hp = param.Parameter.MaxHp;
            param.Parameter.Mp = param.Parameter.MaxMp;
            if(i < playerExp.Count - 1 )
                param.Experience = int.Parse(playerExp[i + 1][index + 1]);

            levelUpParameter[counter] = param;
            counter++;
        }

        foreach (var t in initDataParam.itemList)
        {
            PlayerDataRepository.Instance.ItemList.Add(ItemMaster.Entity.FindItemData(t.name).ID,
                new PlayerDataRepository.HubItemData
                {
                    ID = ItemMaster.Entity.FindItemData(t.name).ID,
                    num = t.count
                });
        }
        for(int i=0; i< initDataParam.magicLearning.Length; i++)
        {
            magicData.Add(initDataParam.magicLearning[i].magicName);
        }

        if (playerLevel.Count != 0)
            AdjustmentLevelStatus(Level);
	}
}

public class WeaponArmorEquipment
{

    public InfoWeaponArmor Head = new();
    public InfoWeaponArmor Hand1 = new();
    public InfoWeaponArmor Body = new();
    public InfoWeaponArmor Hand2 = new();

    public enum Part
    {
        Head,
        Hand1,
        Body,
        Hand2
    }

    public Parameter GetEquipmentUpParameter(Part part)
    {
        return part switch
        {
            Part.Hand1 => Hand1.UpParam,
            Part.Hand2 => Hand2.UpParam,
            Part.Head => Head.UpParam,
            Part.Body => Body.UpParam,
        };
    }

    public InfoWeaponArmor GetWeaponEquipmentData(int itemID)
    {
        return WeaponArmorMaster.Entity.GetWeaponData(itemID).equipment switch
        {
            Part.Body => Body,
            Part.Hand1 => Hand1,
            Part.Hand2 => Hand2,
            Part.Head => Head,
        };
    }

    public static Parameter SumParamter(WeaponArmorEquipment param)
    {
        return param.Head.UpParam + param.Hand1.UpParam + param.Hand2.UpParam + param.Body.UpParam;
    }

    public static Parameter operator +(WeaponArmorEquipment param1, Parameter param2)
    {
        var result = SumParamter(param1) + param2;
        return result;
    }
    public static Parameter operator +( Parameter param1,WeaponArmorEquipment param2)
    {
        var result = SumParamter(param2) + param1;
        return result;
    }
}

public class LevelParameter
{
    public int Level;
    public Parameter Parameter;
    public int Experience;
}