using UnityEngine;
using Cinemachine;
using UnityEditor;
using Unity.Multiplayer.PlayMode;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int playerMemberNum = 2;
    public Now_Mode mode;
    public Now_State state = Now_State.Active;
    public GameObject player;
    public GameObject PlayerObj;
    public int countStep;
    public int MassCountStep;
    public bool battleFlag = true;
    public bool eventFlag = false;
	public CinemachineVirtualCamera cCam;
    public GameObject playerStartPos;
    public GameObject currentPlayer;

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
        PlayerObj = Instantiate(player,PlayerDataRepository.Instance.playerPos,Quaternion.identity);
        
        cCam.Follow = PlayerObj.GetComponent<PlayerMovement>().gameObject.transform;
        
		//PlayerDataRepository.Instance.playersState[0] = obj.AddComponent<Player>();
	}

    void OnApplicationQuit()
    {
        if(ItemDataBase.Entity != null)
            ItemDataBase.Entity.AllCollectReset();
    }
    private void SpawnPlayer(Scene scene)
    {
        if (player == null)
        {
            Debug.LogError("Player Prefabが未設定です");
            return;
        }

        string spawnId = SceneTransition.NextSpawnPointId;

        Vector3 spawnPosition =
            PlayerDataRepository.Instance.playerPos;

        // 出現地点が指定されている場合
        if (!string.IsNullOrEmpty(spawnId))
        {
            MapSpawnPoint point = FindSpawnPoint(scene, spawnId);

            if (point == null)
            {
                Debug.LogWarning(
                    $"SpawnPointが見つかりません: {spawnId}");
                return;
            }

            spawnPosition = point.transform.position;
        }

        // 既存プレイヤーがいるなら削除
        if (currentPlayer != null)
            Destroy(currentPlayer);

        // Playerを生成
        currentPlayer = Instantiate(
            player,
            spawnPosition,
            Quaternion.identity
        );

        // Repositoryに座標を反映
        PlayerDataRepository.Instance.playerPos = spawnPosition;

        // 正常に生成できたらリセット
        SceneTransition.NextSpawnPointId = null;
    }

    private MapSpawnPoint FindSpawnPoint(Scene scene, string spawnId)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            MapSpawnPoint[] points =
                root.GetComponentsInChildren<MapSpawnPoint>(true);

            foreach (MapSpawnPoint point in points)
            {
                if (point.SpawnPointId == spawnId)
                    return point;
            }
        }

        return null;
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
