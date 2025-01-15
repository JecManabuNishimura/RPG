using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Debugger : MonoBehaviour
{
	bool enable = false;


	public Rect rectSize = new Rect(0, 0, 400, 500);
	public int fontSize = 18;
	public int oneHeight = 80;
	public int initYPos = 10;

	public bool KeyOnOffFlag = false;

	public enum DebugType
	{
		Selector,
		Player,
		OnOff,
		Enemy,
		Reset,
	}
	public DebugType type = DebugType.Selector;
	private void Start()
	{


	}
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.F12))
		{
			enable = !enable;
			type = DebugType.Selector;
		}
		switch(type)
		{
			case DebugType.OnOff:
			case DebugType.Player:
			case DebugType.Enemy:
			case DebugType.Reset:
				if (IsKeyPressed(KeyCode.Keypad0,KeyCode.Alpha0))
				{
					type = DebugType.Selector;
				}

				if (type == DebugType.Enemy)
				{
					if (GameManager.Instance.mode != Now_Mode.Battle)
					{
						type = DebugType.Selector;
					}
				}
				break;
			case DebugType.Selector:
				if (IsKeyPressed(KeyCode.Keypad1,KeyCode.Alpha1))
				{
					type = DebugType.Player; 
				}
				if (IsKeyPressed(KeyCode.Keypad2,KeyCode.Alpha2))
				{
					type = DebugType.OnOff;
				}

				if (GameManager.Instance.mode == Now_Mode.Battle)
				{
					if (IsKeyPressed(KeyCode.Keypad3,KeyCode.Alpha3))
					{
						type = DebugType.Enemy;
					}	
				}

				if (IsKeyPressed(KeyCode.Keypad4, KeyCode.Alpha4))
				{
					type = DebugType.Reset; 
				}
				break;
			
		}
	}

	private bool IsKeyPressed(params KeyCode[] keys)
	{
		foreach (var key in keys)
		{
			if (Input.GetKeyDown(key))
				return true;
		}
		return false;
	}
	private bool IsKeyRelease(params KeyCode[] keys)
	{
		foreach (var key in keys)
		{
			if (Input.GetKeyUp(key))
				return true;
		}
		return false;
	}
	void OnGUI()
	{
		GUIStyle style = GUI.skin.GetStyle("label");
		style.fontSize = fontSize;
		if (enable)
		{
			switch (type)
			{
				case DebugType.Enemy:
				case DebugType.Player:
				{
					//　各パラメーターの取得
					Type parameterType = typeof(Parameter);
					FieldInfo[] fields = parameterType.GetFields(BindingFlags.Public | BindingFlags.Instance);
					PropertyInfo[] properties =
						parameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
					int dataCount = type == DebugType.Player
						? PlayerDataRepository.Instance.playersState.Count
						: BattleManager.Instance.EnemyList.Count;
					float boxWidth = rectSize.width * dataCount;
					Rect boxRect = rectSize;
					boxRect.height = oneHeight * (fields.Length + 6);
					boxRect.width = boxWidth;

					// 背景
					GUI.Label(boxRect, "", "box");
					GUI.Label(new Rect(10, initYPos, rectSize.width, oneHeight), 
						(type == DebugType.Player ? "プレイヤー情報" : "敵情報"));
					int counter = 1;
					
					for (int i = 0; i < dataCount; i++)
					{
						GUI.Label(
							new Rect(10 + (rectSize.width * i), (oneHeight * counter) + initYPos, rectSize.width,
								oneHeight), "名前:" + 
								            (type == DebugType.Player 
									            ? PlayerDataRepository.Instance.playersState[i].CharaName
									            : BattleManager.Instance.EnemyList[i].CharaName));
						counter++;
						GUI.Label(
							new Rect(10 + (rectSize.width * i), (oneHeight * counter) + initYPos, rectSize.width,
								oneHeight),  
								            (type == DebugType.Player 
									            ? "Level:" + PlayerDataRepository.Instance.playersState[i].Level
									            :"ゴールド:" +BattleManager.Instance.EnemyList[i].gold));
						counter++;
						foreach (var field in fields)
						{
							GUI.Label(
								new Rect(10 + (rectSize.width * i), (oneHeight * counter) + initYPos, rectSize.width,
									oneHeight),
								$"{field.Name}:{field.GetValue( type == DebugType.Player ? PlayerDataRepository.Instance.playersState[i].parameter : BattleManager.Instance.EnemyList[i].parameter)}");
							counter++;
						}

						counter = 1;
					}

					GUI.Label(new Rect(10, (oneHeight * (fields.Length + 4)) + initYPos, rectSize.width, oneHeight),
						$"0:戻る");
					break;
				}
				case DebugType.OnOff:
					GUI.Label(rectSize, "", "box");
					int layerMask = 1 << LayerMask.NameToLayer("RegionMap");

					// Upを入れて、フラグ制御しないと何度も呼ばれてしまう
					if (IsKeyPressed(KeyCode.Keypad1,KeyCode.Alpha1))
					{
						if(!KeyOnOffFlag)
						{
							if ((Camera.main.cullingMask & layerMask) != 0)
							{
								Camera.main.cullingMask &= ~(layerMask);
							}
							else
							{
								Camera.main.cullingMask |= (layerMask);
							}
							KeyOnOffFlag = true;
						}		
					}
					if (IsKeyRelease(KeyCode.Keypad1,KeyCode.Alpha1))
					{
						KeyOnOffFlag = false;
					}
					if (IsKeyPressed(KeyCode.Keypad2,KeyCode.Alpha2))
					{
						if(!KeyOnOffFlag)
						{
							GameManager.Instance.battleFlag = !GameManager.Instance.battleFlag;
							KeyOnOffFlag = true;
						}
					}
					if (IsKeyRelease(KeyCode.Keypad2,KeyCode.Alpha2))
					{
						KeyOnOffFlag = false;
					}

					GUI.Label(new Rect(10, initYPos, rectSize.width, oneHeight),
						"OnOffData:");
					GUI.Label(new Rect(10, (oneHeight * 2) + initYPos, rectSize.width, oneHeight),
						"1->CameraRegionMap:" + ((Camera.main.cullingMask & layerMask) != 0 ? "On" : "Off"));
					GUI.Label(new Rect(10, (oneHeight * 3) + initYPos, rectSize.width, oneHeight),
						"2->Encount:" + (GameManager.Instance.battleFlag ? "On" : "Off"));
					break;

				case DebugType.Selector:
					// 背景
					GUI.Label(rectSize, "", "box");

					GUI.Label(new Rect(10, initYPos, rectSize.width, oneHeight), "1:PlayerData");
					GUI.Label(new Rect(10, (oneHeight * 1) + initYPos, rectSize.width, oneHeight), "2:OnOffData");
					GUI.Label(new Rect(10, (oneHeight * 2) + initYPos, rectSize.width, oneHeight), "3:EnemyData ※バトル中のみ");
					GUI.Label(new Rect(10, (oneHeight * 3) + initYPos, rectSize.width, oneHeight), "4:Reset");
					break;
				case DebugType.Reset:
					if (IsKeyRelease(KeyCode.Keypad1, KeyCode.Alpha1))
					{
						EventMaster.Entity.AllFlagReset();
					}
					GUI.Label(rectSize, "", "box");
					GUI.Label(new Rect(10, initYPos, rectSize.width, oneHeight), "1:EventFlagAllReset");
					break;
			}
		}
	}
}


