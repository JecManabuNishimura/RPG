using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugManager : MonoBehaviour
{
    void Start()
    {
        Action action = () =>
        {
            if (GameManager.Instance.mode != Now_Mode.Battle)
            {
				GameManager.Instance.mode = Now_Mode.Battle;
				GameManager.Instance.state = Now_State.Menu;    // バトルを作っても良いかも
				BattleManager.Instance.StartBattle();
                /*
                BattleManager.Instance.SetupBattle();
                LoadSceneManager.LoadScene("Battle");
                
                */
                //SceneManager.LoadSceneAsync("Battle");
            }
            else
            {
                MenuManager.Instance._menuController.ChangeMenu(MenuList.None);
                LoadSceneManager.LoadScene("SampleScene");
                foreach (var t in PlayerDataRepository.Instance.playersState)
                {
                    t.BattleDataReset();
                }
                GameManager.Instance.mode = Now_Mode.Field;
                GameManager.Instance.state = Now_State.Active;
            }
        };
        // 強制バトル開始
        InputManager.Instance.SetKeyboardEvent(UseButtonType.Player,KeyCode.B,action,true);
        InputManager.Instance.SetKeyboardEvent(UseButtonType.Menu,KeyCode.B,action,true);

        Action battleOnOff = () =>
        {
            GameManager.Instance.battleFlag = !GameManager.Instance.battleFlag;
        };
		InputManager.Instance.SetKeyboardEvent(UseButtonType.Player, KeyCode.T, battleOnOff, true);
	}

    private void OnSceneUnloaded(Scene current)
    {
        // 破棄したときに呼ぶ処理
    }
}
