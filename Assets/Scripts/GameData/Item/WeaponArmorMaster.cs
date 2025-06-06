using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameData.Item;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "WeaponArmorMaster", menuName="Master/CreateWeaponArmorMaster") ]
public class WeaponArmorMaster : ScriptableObject
{
    private static WeaponArmorMaster _entity;
    public static WeaponArmorMaster Entity
    {
        get
        {
            //初アクセス時にロードする
            if (_entity == null)
            {
                _entity = Resources.Load<WeaponArmorMaster>("Master/WeaponArmorMaster");

                //ロード出来なかった場合はエラーログを表示
                if (_entity == null)
                {
                    Debug.LogError(nameof(WeaponArmorMaster) + " not found");
                }
            }

            return _entity;
        }
    }

    
    public List<InfoWeaponArmor> WeaponArmorDatas = new ();

    public InfoWeaponArmor GetWeaponData(int id)
    {
        return WeaponArmorDatas.FirstOrDefault(x => x.dataParam.ID == id);
    }
    
}

[CustomEditor(typeof(WeaponArmorMaster))] //拡張するクラスを指定
public class WeaponMasterEditor : Editor
{
    private WeaponArmorMaster itemReader;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        itemReader = (WeaponArmorMaster)target;
        if (GUILayout.Button("SetWeaponData"))
        {
            ReadWeaponData();
        }
    }
    
    
    void ReadWeaponData()
    {
        itemReader.WeaponArmorDatas.Clear();
        var data = DataReader.ReadData("WeaponsArmor");
        for (int i = 1; i < data.Count; i++)
        {
            InfoWeaponArmor wad = new();
            wad.UpParam = new();
            wad.dataParam.ID = int.Parse(data[i][0]);
            wad.dataParam.Name = data[i][1];
            wad.equipment = data[i][2] switch
            {
                "頭部" => WeaponArmorEquipment.Part.Head,
                "胴体" => WeaponArmorEquipment.Part.Body,
                "右手" => WeaponArmorEquipment.Part.Hand1,
                "左手" => WeaponArmorEquipment.Part.Hand2,
                _ => throw new ArgumentOutOfRangeException()
            };
            wad.UpParam.Hp = int.Parse(data[i][3]);
            wad.UpParam.Mp = int.Parse(data[i][4]);
            wad.UpParam.Atk = int.Parse(data[i][5]);
            wad.UpParam.Def = int.Parse(data[i][6]);
            wad.UpParam.Mga = int.Parse(data[i][7]);
            wad.UpParam.Mgd = int.Parse(data[i][8]);
            wad.UpParam.Qui = int.Parse(data[i][9]);
            // 運が入る
            if (Enum.TryParse<EffectType>(data[i][11], true, out var parsedEffect))
            {
                wad.dataParam.Effect = parsedEffect;
            }
            else
            {
                // 変換に失敗した場合のフォールバック（例：noneにする）
                wad.dataParam.Effect = EffectType.none;
                Debug.LogWarning($"未定義のEffectType: {data[i][11]} を none に置き換えました。");
            }
            wad.dataParam.Price = int.Parse(data[i][12]);
            
            itemReader.WeaponArmorDatas.Add(wad);
        }
    }
}

[Serializable]
public class InfoWeaponArmor
{
    public WeaponArmorEquipment.Part equipment;
    public ItemData dataParam = new();
    public Parameter UpParam = new();
    public bool isSet = false;

}