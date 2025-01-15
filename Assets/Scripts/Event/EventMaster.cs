using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

[CreateAssetMenu(fileName = "EventMaster", menuName="Master/CreateEventMaster") ]
public class EventMaster : ScriptableObject
{
    private static EventMaster _entity;
    public static EventMaster Entity
    {
        get
        {
            //初アクセス時にロードする
            if (_entity == null)
            {
                _entity = Resources.Load<EventMaster>("Master/EventMaster");

                //ロード出来なかった場合はエラーログを表示
                if (_entity == null)
                {
                    Debug.LogError(nameof(EventMaster) + " not found");
                }
            }

            return _entity;
        }
    }

    public List<EventInfo> _eventInfos = new List<EventInfo>();
    
    public TimelineAsset GetTimelineAsset(int number) => _eventInfos.Find(info => info.number == number).eventTimeLine;
    public bool GetEventFlag(int num)=>  _eventInfos.FirstOrDefault(e => e.number == num)!.flag;
    
    public void CheckEvent(int num)
    {
        var ei = _eventInfos.Find(info => info.number == num);
        ei.flag = true;
    }

    public void AllFlagReset()
    {
        foreach (var data in _eventInfos)
        {
            data.flag = false;
        }
    }
    public void AddData(EventInfo itemData)
    {
        if (!IsOriginalIDExist(itemData.number))
        {
            
            _eventInfos.Add(itemData);
            _eventInfos = _eventInfos.OrderBy(item => item.number).ToList();
        }
    }
    bool IsOriginalIDExist(int id) => _eventInfos.Any(item => string.Equals(item.number, id));
    
}


[Serializable]
public class EventInfo
{
    public int number;
    public string name;
    public TimelineAsset eventTimeLine;
    public bool flag;
}

[CustomEditor(typeof(EventMaster))]//拡張するクラスを指定
public class EventReaderEditor : Editor
{
    private EventMaster itemReader;
    public override void OnInspectorGUI(){
        base.OnInspectorGUI ();
        itemReader = (EventMaster)target;
        
        GUIStyle customButtonStyle = new GUIStyle(GUI.skin.button);
        customButtonStyle.normal.textColor = Color.green; // ボタンのテキストの色を赤に変更
        if (GUILayout.Button("SetEventData",customButtonStyle)){
            
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
        itemReader.AllFlagReset();
    }

    private void RegisterObject()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Event");
        
        foreach (var o in obj)
        {
            EventPoint e = o.GetComponent<EventPoint>();
            EventInfo itData = new();
            itData.number = e.eventNumber;
            itData.name = e.name;
            itData.flag = false;
            itemReader.AddData(itData);
        }
    }
}
