using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class DialogMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI MessageText;

    [SerializeField] private GameObject window;

    private const float CharacterIntervalDefault = 0.1f; // 1文字表示する間隔
    private static float _characterInterval = 0.1f; // 1文字表示する間隔

    private static bool _waitForEnterInput = false; // Enterキー入力待ちかどうかのフラグ

    private bool levelFlag = false;
    private PlayerCharacter state;
    private int messageLine = 0;

    [SerializeField]
    private Transform boxPosTop;
    [SerializeField]
    private Transform boxPosButtom;

    [SerializeField] private GameObject NameTextBox;
    [SerializeField] private TextMeshProUGUI NameText;

    void Start()
    {
        levelFlag = false;
        window.SetActive(false);
        MessageManager.Instance.SetMessage(this);
        MessageManager.Instance.OnDialogMessage += StartMessage;
        MessageManager.Instance.OnSetNameDialogMessage += SetCharaName;

        InputManager.Instance.SetKeyEvent(UseButtonType.Message, InputMaster.Entity.FieldKey.Enter, NextMessage);
        InputManager.Instance.SetKeyboardEvent(UseButtonType.Message,KeyCode.Return,NextMessage,true);

    }

    private void OnDestroy()
    {
        MessageManager.Instance.OnDialogMessage -= StartMessage;
    }

    public void SetCharaName(string name)
    {
        NameTextBox.SetActive(true);
        NameText.text = name;
    }
    public void StartMessage(string message, string fileName)
    {
        
        if (message == "" && fileName == "")
        {
            MessageText.text = "";
            messageLine = 0;
            return;
        }
        if(boxPosButtom != null && boxPosTop != null)
            window.transform.position = PlayerDataRepository.Instance.PlayerCamPos < 1 ? boxPosButtom.position : boxPosTop.position;
        if (NameText.text == "")
        {
            NameTextBox.SetActive(false);
            
        }
		StartCoroutine(CreateMessage(message,fileName));
    }
    
    private IEnumerator CreateMessage(string message, string fileName)
    {
        GameManager.Instance.state = Now_State.Message;
        window.SetActive(true);

        if(fileName == "")
        {
			yield return StartCoroutine(ShowTextLineByLine(message));
		}
        else
        {
			yield return StartCoroutine(DialogSetup(fileName));
		}
		MessageManager.Instance.IsEndMessage = true;
        GameManager.Instance.state = Now_State.Menu;
        if (GameManager.Instance.mode == Now_Mode.Battle && BattleManager.Instance.State == BattleList.End)
        {
            MenuManager.Instance.CloseDialog();
            GameManager.Instance.state = Now_State.Active;
            MessageManager.Instance.OnDialogMessage -= StartMessage;
        }

        NameText.text = "";
        window.SetActive(false);
    }

    private void NextMessage()
    {
        if (_waitForEnterInput)
        {
            // 残りの行を一気に表示
            _waitForEnterInput = false; // フラグをリセット    
            _characterInterval = CharacterIntervalDefault;
        }
        else
        {
            //if (MessageText.text.Length != 0)
            {
                _characterInterval = 0.01f;
            }
        }
    }

    void Update()
    {
    }

    // テキストを1行読み込んで、一文字ずつ表示するコルーチン
    private IEnumerator ShowTextLineByLine(string fileContent)
    {
		
		using (StringReader reader = new StringReader(fileContent))
        {
            string line;
            int counter = 0;
            if(messageLine == 2)
            {
                messageLine = 0;

				MessageText.text = "";
            }
            // 各行を読み込む
            while ((line = reader.ReadLine()) != null)
            {
                if (levelFlag)
                {
                    levelFlag = false;
                    yield return StartCoroutine(LevelUpCheck());
					messageLine = 0;
					MessageText.text = "";
                }
                // 1行読み込んで表示
                yield return StartCoroutine(ShowTextOneByOne(line));
                counter++;
                if (counter == 2)
                {
                    // Enterキーが押されるまで待機
                    _waitForEnterInput = true;
                    yield return new WaitUntil(() => !_waitForEnterInput);
                    counter = 0;
					messageLine = 0;
					MessageText.text = "";
                }
            }
        }
        _waitForEnterInput = true;
        if (MessageText.text != "")
        {
            yield return new WaitUntil(() => !_waitForEnterInput);    
        }
        
    }

    // テキストを一文字ずつ表示するコルーチン
    private IEnumerator ShowTextOneByOne(string line)
    {
		line = ChengeText(line);
        for (int i = 0; i < line.Length; i++)
        {
            char character = line[i];
            if (character == '%' && line[i + 1] == 's')
            {
                levelFlag = true;
                i++;
                continue;
            }

            MessageText.text += character;
			
            // 表示のための待ち時間
            yield return new WaitForSeconds(_characterInterval);
        }
        messageLine++;
		MessageText.text += "\n";
    }

    private string ChengeText(string line)
    {
        line = line.ReplaceString("exp", BattleManager.Instance.GetExp.ToString().ConvertToFullWidth());
        line = line.ReplaceString("gold", BattleManager.Instance.GetGold.ToString().ConvertToFullWidth());
        if (state is not null)
        {
            line = line.ReplaceString("name", state.CharaName);
            line = line.ReplaceString("level", state.Level.ToString().ConvertToFullWidth());
            line = line.ReplaceString("atk", state.upParam.Atk .ToString().ConvertToFullWidth());
            line = line.ReplaceString("def", state.upParam.Def .ToString().ConvertToFullWidth());
            line = line.ReplaceString("qui",  state.upParam.Qui .ToString().ConvertToFullWidth());    
        }

        return line;
    }

    private IEnumerator LevelUpCheck()
    {
        foreach (var chara in PlayerDataRepository.Instance.playersState)
        {
            chara.CheckLevelUp();
            if (chara.levelUpFlag)
            {
                state = chara;
                
                yield return StartCoroutine(DialogSetup("Levelup"));
            }
        }
    }

    private IEnumerator DialogSetup(string fileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);

        if (textAsset != null)
        {
            string fileContent = textAsset.text;

            // 使用例：1行読み込んで一文字ずつ表示
            yield return StartCoroutine(ShowTextLineByLine(fileContent));
        }
        else
        {
            Debug.LogError("Failed to load text file: " + fileName);
        }
    }
}
