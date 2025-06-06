using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameData.Item;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "ItemDataBase", menuName="CreateItemDataBase")]
public class ItemDataBase : ScriptableObject
{
    private static ItemDataBase _entity;

    public static ItemDataBase Entity
    {
        get
        {
            //初アクセス時にロードする
            if (_entity == null)
            {
                _entity = Resources.Load<ItemDataBase>("Master/ItemDataBase");

                //ロード出来なかった場合はエラーログを表示
                if (_entity == null)
                {
                    Debug.LogError("ItemDataBase" + " not found");
                }
            }

            return _entity;
        }
    }

    public List<DropItemData> itemParams = new();
    
    public void AddData(FieldItem itemData)
    {
        if (!IsOriginalIDExist(itemData.AllID))
        {
            DropItemData data = new();
            data.ItemName = itemData.name;
            data.originalID = itemData.AllID;
            data.isCollection = false;
            itemParams.Add(data);
            itemParams = itemParams.OrderBy(item => item.originalID, StringComparer.OrdinalIgnoreCase).ToList();
        }
    }

    public void AllCollectReset()
    {
        foreach (var data in itemParams)
        {
            data.isCollection = false;
        }
    }

    public void SetItemCollect(string id,bool flag)
    {
        DropItemData item = itemParams.FirstOrDefault(x => x.originalID == id);
        if (item != null) item.isCollection = flag;
    }

    public bool GetItemCollect(string id)
    {
        DropItemData item = itemParams.FirstOrDefault(x => x.originalID == id);
        return item is { isCollection: true };
    }
    
    public bool IsOriginalIDExist(string id)
    {
        return itemParams.Any(item => Equals(item.originalID,id));
    }
}

[Serializable]
public class DropItemData
{
    public string ItemName;
    // 先頭一桁　：MapID
    // ２～３　：固有ID
    // ４～　　：アイテムID
    public string originalID;
    public bool isCollection;
}

[CustomEditor(typeof(ItemDataBase))]//拡張するクラスを指定
public class ItemReaderEditor : Editor
{
    private ItemDataBase itemReader;
    public override void OnInspectorGUI(){
        base.OnInspectorGUI ();
        itemReader = (ItemDataBase)target;
        
        GUIStyle customButtonStyle = new GUIStyle(GUI.skin.button);
        customButtonStyle.normal.textColor = Color.green; // ボタンのテキストの色を赤に変更
        if (GUILayout.Button("SetItemData",customButtonStyle)){
            
            RegisterObject();
        }
        customButtonStyle.normal.textColor = Color.red; // ボタンのテキストの色を赤に変更
        if (GUILayout.Button("Reset",customButtonStyle))
        {
            AllResetData();
        }
    }

    private void AllResetData()
    {
        //ItemDataBase idb = (ItemDataBase);
        itemReader.AllCollectReset();
    }

    private void RegisterObject()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Item");
        itemReader.itemParams.Clear();
        foreach (var o in obj)
        {
            FieldItem itData = o.GetComponent<FieldItem>();
            itemReader.AddData(itData);
        }
    }

    /*
    // 重複しているoriginalIDを取得するメソッド
    public IEnumerable<string> GetDuplicateOriginalIDs(ItemData[] obj)
    {
        return obj
            .GroupBy(item => item.ID, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key);
    }*/
}