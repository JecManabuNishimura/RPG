using UnityEngine;
using Cinemachine;
using UnityEditor;
using Unity.Multiplayer.PlayMode;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int playerMemberNum = 2;
    public Now_Mode mode;
    public Now_State state = Now_State.Active;
    public GameObject player;
    public int countStep;
    public int MassCountStep;
    public bool battleFlag = true;
    public bool eventFlag = false;
	public CinemachineVirtualCamera cCam;
    public GameObject playerStartPos;
    public GameObject currentPlayer;

    private Action<Scene> onSceneLoadedAction;

    private async void Update()
    {
        MassCountStep = countStep / 30; // 仮
        // 歩数が一定数を超すと戦闘開始

        if ((MassCountStep >= BattleManager.Instance._BattleController.nextEnemyEncount)
            && battleFlag)
        {
            if (BattleManager.Instance.CheckBattleStart())
            {
                cCam.Follow = null;
                GameManager.Instance.mode = Now_Mode.Battle;
                GameManager.Instance.state = Now_State.Menu; // バトルを作っても良いかも    

                BattleManager.Instance.StartBattle();
            }
            countStep = 0;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            PlayerDataRepository.Instance.Initialize();
            NPCManager.Instance.Initialize();
			PlayerDataRepository.Instance.playerPos = playerStartPos.transform.position;
			Debug.Log("プレイヤー位置初期化");
			DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ReadEventData.Instance = new ReadEventData();
        foreach(var  ev in EventMaster.Entity._eventInfos)
        {
            // イベントフラグをすべて初期状態にする
            // セーブ実装時にはここを変更
            ev.flag = false;
        }

        Destroy(playerStartPos);
        FieldIn();
        SceneManager.sceneLoaded += OnSceneLoaded;

    }
    private void OnDestroy()
    {
        if (Instance != this)
            return;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        Instance = null;
    }

    private void Start()
    {
		state = Now_State.Active;
        
        SoundMaster.Entity.SetSEAudio(gameObject.AddComponent<AudioSource>());
        SoundMaster.Entity.SetBGMAudio(gameObject.AddComponent<AudioSource>());
        
        SoundMaster.Entity.PlayBGMSound(PlaceOfSound.FieldMusic);
        
    }

    private void FieldIn()
    {

        currentPlayer = Instantiate(player,PlayerDataRepository.Instance.playerPos,Quaternion.identity);
        cCam = FindFirstObjectByType<CinemachineVirtualCamera>();
        cCam.Follow = currentPlayer.GetComponent<PlayerMovement>().gameObject.transform;
        
		//PlayerDataRepository.Instance.playersState[0] = obj.AddComponent<Player>();
	}

    void OnApplicationQuit()
    {
        if(ItemDataBase.Entity != null)
            ItemDataBase.Entity.AllCollectReset();
    }

    // シーンロード後に実行する処理を予約
    public void ReserveSceneLoaded(Action<Scene> action)
    {
        onSceneLoadedAction = action;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 予約された処理を取り出してクリア
        Action<Scene> action = onSceneLoadedAction;
        onSceneLoadedAction = null;

        action?.Invoke(scene);
    }
    public void ReservePlayerSpawn(string spawnPointId)
    {
        ReserveSceneLoaded(scene =>
        {
            PlayerSpawn(scene, spawnPointId);
        });
    }

    private void PlayerSpawn(Scene scene, string spawnPointId)
    {
        SpawnTilemap spawnTilemap = null;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            spawnTilemap =
                root.GetComponentInChildren<SpawnTilemap>(true);

            if (spawnTilemap != null)
                break;
        }

        if (spawnTilemap == null)
        {
            Debug.LogError("SpawnTilemapが見つかりません");
            return;
        }

        if (!spawnTilemap.TryGetSpawnPosition(
            spawnPointId, out Vector3 spawnPosition))
        {
            Debug.LogError(
                $"出現地点が見つかりません: {spawnPointId}");
            return;
        }

        // Repositoryを先に更新
        PlayerDataRepository.Instance.playerPos = spawnPosition;
        
        Destroy(currentPlayer);
        // プレイヤー生成
        currentPlayer = Instantiate(
            player,
            spawnPosition,
            Quaternion.identity
        );
        cCam = FindFirstObjectByType<CinemachineVirtualCamera>();
        cCam.Follow = currentPlayer.GetComponent<PlayerMovement>().gameObject.transform;

        // 座標を反映
        PlayerDataRepository.Instance.playerPos = spawnPosition;
        
        // スポーン地点のイベントを無効化
        MapEventTilemap mapEvent =
            FindFirstObjectByType<MapEventTilemap>();

        if (mapEvent != null)
        {
            mapEvent.IgnoreSpawnCell();
        }
    }
}

public enum Now_Mode
{
    Field,
    Battle,
    Menu,
}

public enum Now_State
{
    Menu,
    Message,
    Active,
    Shop,
}
