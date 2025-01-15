using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject sb;
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
            
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        ReadEventData.Instance = new ReadEventData();
        
        FieldIn();
    }

    private void Start()
    {
        sb.SetActive(false);
		BattleManager.Instance.screenBreak = sb;
		state = Now_State.Active;
        
        SoundMaster.Entity.SetSEAudio(gameObject.AddComponent<AudioSource>());
        SoundMaster.Entity.SetBGMAudio(gameObject.AddComponent<AudioSource>());
        
        SoundMaster.Entity.PlayBGMSound(PlaceOfSound.FieldMusic);
        
        
    }

    private void FieldIn()
    {
        PlayerObj = Instantiate(player,PlayerDataRepository.Instance.playerPos,Quaternion.identity);
        
        cCam.Follow = PlayerObj.GetComponent<PlayerMovement>().gameObject.transform;
        if (BattleManager.Instance != null)
        {
			BattleManager.Instance.screenBreak = sb;
		}
        
		//PlayerDataRepository.Instance.playersState[0] = obj.AddComponent<Player>();
	}

    void OnApplicationQuit()
    {
        if(ItemDataBase.Entity != null)
            ItemDataBase.Entity.AllCollectReset();
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
