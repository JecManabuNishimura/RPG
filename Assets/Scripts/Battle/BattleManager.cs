using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public GameObject screenBreak;
    public List<MapEnemyData> EnemyDataList;
    public BattleController _BattleController;

    public List<EnemyCharactor> EnemyList = new();

    public List<CharacterState> BattleDatas = new();
    private GameObject battleEnemy;
    public BattleList State;
    public int GetExp;
    public int GetGold;

    public event Action OnParamChange;

    public bool IsEndMotion;
	//public event Action<string,string> OnDialogMessage;
	//public bool IsEndMessage = false;


	public int enemyNum
    {
        get { return EnemyList.Count; }
    }
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }        
    }

    public void SetupBattle()
    {
        //OnDialogMessage = null;
        battleEnemy = _BattleController.SelectEnemy();
    }

    public void SetDatasList()
    {
        BattleDatas.Clear();
        foreach (var enemy in EnemyList)
        {
            BattleDatas.Add(enemy);
        }
    }

    public void ParamChange()
    {
        OnParamChange?.Invoke();
    }

    public void TurnOrder()
    {
        BattleDatas = BattleDatas
            .OrderByDescending(_ => _.parameter.DefFlag)    // 防御が先頭
            .ThenByDescending(_ => _.parameter.Qui)
            .ToList();
    }

    public void SelectEnemy(int index,bool all = false)
    {
        if (all)
        {
            foreach (var t in EnemyList)
            {
                t.GetComponent<EnemyCharactor>().Select(true);
            }

            return;
        }
        for (int i = 0; i < EnemyList.Count; i++)
        {
            EnemyList[i].GetComponent<EnemyCharactor>().Select(index == i);
        }
    }
    
    
    public void BattleDataSetup()
    {
        // 次のエンカウント設定
        _BattleController.SetNextEnemyEncounter();
        EnemyList.Clear();
        MenuManager.Instance.OpenBattleMenu();
        Instantiate(battleEnemy,Vector3.zero, Quaternion.identity);
    }


    public void SortEnemyList()
    {
        EnemyList = EnemyList.OrderBy(e => e.transform.position.x).ToList();
    }

    public bool CheckPlayerCommandEnd()
    {
        return EnemyList.Count + PlayerDataRepository.Instance.playersState.Count == BattleDatas.Count;
    }

    public IEnumerator DestroyEnemy()
    {
        if (EnemyList.Count != 0)
        {
            /*
            foreach (var enemy in EnemyList)
            {
                //if(enemy.DethFlag)
                    //yield return StartCoroutine(enemy.DethAction());
            }*/
               
            EnemyList = EnemyList.Where(_ => !_.DethFlag).ToList();
        }
        yield break;
    }

    public async void StartBattle()
    {
		ScreenShot cam = Camera.main.GetComponent<ScreenShot>();
		SoundMaster.Entity.PlaySESound(PlaceOfSound.BattleInSe);
		SoundMaster.Entity.StopBgmSound();
		cam.StartSnap();
		await Task.Delay(1);

		screenBreak.SetActive(true);
		await screenBreak.GetComponent<ScreenBreak>().FastBreak();

		BattleManager.Instance.SetupBattle();
		LoadSceneManager.LoadScene("Battle");

		SoundMaster.Entity.PlayBGMSound(PlaceOfSound.BattleMusic);
		await screenBreak.GetComponent<ScreenBreak>().StartBreak(2.5f);
		//await sb.GetComponent<ScreenBreak>().StartBreak(3.0f);
	}

    public bool CheckBattleStart()
    {
        return _BattleController.FindRegionEnemy();
    }
}
