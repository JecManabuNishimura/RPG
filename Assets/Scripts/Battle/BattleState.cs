using System.Collections;
using System.Linq;
using UnityEngine;

public class BattleStart : IBattleState
{
    public BattleList State => BattleList.Start;
    private BattleStateController _battle;
    public BattleStart(BattleStateController battle) => _battle = battle;
    public void Entry()
    {
        BattleManager.Instance.GetExp = 0;
        BattleManager.Instance.GetGold = 0;
        BattleManager.Instance.EnemyList.Clear();
        Debug.Log("start");
        
    }

    public void Update()
    {
        BattleManager.Instance.SetDatasList();
        if (BattleManager.Instance.BattleDatas.Count != 0)
        {
            _battle.ChangeBattleState(BattleList.PlayerCommand);    
        }
        
    }

    public void Exit()
    {
        
    }
}

public class BattlePlayerCommand : IBattleState
{
    public BattleList State => BattleList.PlayerCommand;
    private BattleStateController _battle;
    public BattlePlayerCommand(BattleStateController battle) => _battle = battle;
    public void Entry()
    {
        PlayerDataRepository.Instance.SelectIndex = 0;
        MessageManager.Instance.ClearDialogMessage();
        BattleManager.Instance.ParamChange(); // 表示パラメーター更新
        Debug.Log(State);
    }

    public void Update()
    {
        bool allFlg = PlayerDataRepository.Instance.playersState.All(_ => _.ActionFlag);
        if (allFlg)
        {
            Debug.Log("行動へ");
            _battle.ChangeBattleState(BattleList.Battle);
        }
    }

    public void Exit()
    {
    }
}

public class BttleStep : IBattleState
{
    public BattleList State => BattleList.Battle;

    private BattleStateController _battle;
    public BttleStep(BattleStateController battle) => _battle = battle;
    private bool endFlag = false;
    public void Entry()
    {
        // 素早さ順に変える
        BattleManager.Instance.TurnOrder();
        BattleManager.Instance.StartCoroutine(ExecuteTurns());
        
    }

    public void Update()
    {
        if (endFlag)
        {
            _battle.ChangeBattleState(BattleList.End);
        }
    }

    public void Exit()
    {

    }
    
    private IEnumerator ExecuteTurns()
    {
        foreach (var currentCharacter in BattleManager.Instance.BattleDatas)
        {
            Debug.Log("行動:" + currentCharacter.CharaName);
            // キャラクターの行動を待つ
            yield return BattleManager.Instance.StartCoroutine(currentCharacter.PerformAction());
            
            yield return  BattleManager.Instance.StartCoroutine(BattleManager.Instance.DestroyEnemy());
            if (BattleManager.Instance.EnemyList.Count == 0)
            {
                // 戦い終了
                endFlag = true;
                
                foreach (var t in PlayerDataRepository.Instance.playersState)
                {
                    t.BattleDataReset();
                }
                PlayerDataRepository.Instance.PlusGold(BattleManager.Instance.GetGold);
                yield break;
            }
        }
        
        Debug.Log("行動終了");
        BattleManager.Instance.SetDatasList();
        
        // すべてのキャラクターの行動が終わったら次のステートに遷移
        _battle.ChangeBattleState(BattleList.PlayerCommand);
    }
}

public class BttleEnd : IBattleState
{
    public BattleList State => BattleList.End;
    private BattleStateController _battle;
    public BttleEnd(BattleStateController battle) => _battle = battle;
    public void Entry()
    {
        SoundMaster.Entity.StopBgmSound();
        SoundMaster.Entity.PlaySESound(PlaceOfSound.BattleEndSe);
        // リザルトメッセージ
        MessageManager.Instance.StartDialogMessage("", "ResultMessage");
    }
    
    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }
}