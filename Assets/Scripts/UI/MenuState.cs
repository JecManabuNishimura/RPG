using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using Unity.VisualScripting;
using UnityEngine;

public class MenuStateNone : MenuData, IMenuState
{
    private MenuController _menu;
    public MenuStateNone(MenuController menu) => _menu = menu;
    public MenuList State => MenuList.None;

    public void CloseMenu()
    {
    }

    public void CursorUp()
    {
    }

    public void CursorDown()
    {
    }

    public void CursorRight()
    {
    }

    public void CursorLeft()
    {
    }

    public void Entry()
    {
    }

    public void Exit()
    {
    }

    public void SelectMenu()
    {
    }

    public void Update()
    {

    }
}

// メインメニュー
public class MenuStateMain : MenuData,IMenuState
{
    private MenuController _menu;
    public MenuStateMain(MenuController menu) => _menu = menu;

    public MenuList State => MenuList.Main;

    public void Entry()
    {
        GameManager.Instance.mode = Now_Mode.Menu;
        SoundMaster.Entity.PlaySESound(PlaceOfSound.MenuOpen);
        WindowObj = MenuManager.Instance.GetWindow(State);
        if (WindowObj is not null)
        {
            WindowObj.transform.gameObject.SetActive(true);
            SetCursorObject(MenuManager.Instance.GetCursorObj(State),State);
            selectedIndex = veriIndex * 2 + horiIndex;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            
        }
    }
    
    public void Exit()
    {
        GameManager.Instance.mode = Now_Mode.Field;   
    }

    public void SelectMenu()
    {
        switch(selectedIndex)
        {
            
            case 0:    // メイン
                break;
            case 1:     // 魔法
                MenuManager.Instance.nowSelect = MenuList.Magic;
                MenuManager.Instance.cursorPos.Push((State, veriIndex, horiIndex));
                _menu.ChangeMenu(MenuList.SelectChara);
                break;

            case 2:     // 道具
                MenuManager.Instance.nowSelect = MenuList.ItemList;
                MenuManager.Instance.cursorPos.Push((State, veriIndex, horiIndex));
                _menu.ChangeMenu(MenuList.ItemList);
                break;
            case 3:
                MenuManager.Instance.nowSelect = MenuList.EquipmentMenu;
                MenuManager.Instance.cursorPos.Push((State, veriIndex, horiIndex));
                _menu.ChangeMenu(MenuList.EquipmentMenu);
                break;
            
            case 4:     // 強さ
                MenuManager.Instance.nowSelect = MenuList.Strength;
                MenuManager.Instance.cursorPos.Push((State, veriIndex,horiIndex));
                _menu.ChangeMenu(MenuList.SelectChara);
                break;
        }
    }

    public void CloseMenu()
    {
        Initialize();
        WindowObj.transform.gameObject.SetActive(false);
        //MenuButton.EndButton();
        _menu.ChangeMenu(MenuList.None);
        MenuManager.Instance.playerEndMenuHandl();
    }

    public void CursorUp() => MoveCursor(-1, 0);
    
    public void CursorDown() => MoveCursor(1, 0);

    public void CursorRight() => MoveCursor(0, 1);

    public void CursorLeft() => MoveCursor(0, -1);

    protected override void MoveCursor(int verticalDirection, int horizontalDirection)
    {
        veriIndex = Mathf.Clamp((veriIndex + verticalDirection),0,2);
        horiIndex = Mathf.Clamp((horiIndex + horizontalDirection),0,1);
        // 上下移動と左右移動を組み合わせて扱う場合
        selectedIndex = veriIndex * 2 + horiIndex;
        float yOffset = (selectedIndex / 2) * 50; // カーソル間の垂直な間隔（例として50ピクセル）
        float xOffset = (selectedIndex % 2) * 170;
        UpdateCursorPosition(xOffset,yOffset);
    }
}

// キャラ選択メニュー
public class MenuStateSelectCharacter : MenuData, IMenuState
{
    private MenuController _menu;
    public MenuStateSelectCharacter(MenuController menu) => _menu = menu;
    public MenuList State => MenuList.SelectChara;
    public void Entry()
    {
        WindowObj = MenuManager.Instance.GetWindow(State);
        if (WindowObj is not null)
        {
            WindowObj.transform.gameObject.SetActive(true);
            WindowObj.openMenu = true;
            SetCursorObject(MenuManager.Instance.GetCursorObj(State),State);
        }
    }

    public void Update()
    {
    }

    public void Exit()
    {
        
    }

    public void SelectMenu()
    {
        
        switch(MenuManager.Instance.nowSelect)
        {
            case MenuList.Magic:
                switch(GameManager.Instance.mode)
                {
                    case Now_Mode.Field:
                        _menu.ChangeMenu(MenuManager.Instance.nowSelect);
                        break;
                    case Now_Mode.Battle:
                        MenuManager.Instance.cursorPos.Push((State, veriIndex, horiIndex));
                        MenuManager.Instance.selectPlayerNum = veriIndex;
                        WindowObj.transform.gameObject.SetActive(false);
                        MenuManager.Instance.cursorPos.Pop();
                        // 誰を選択したのか
                        PlayerDataRepository.Instance.PlayerState.to.Add(
                            PlayerDataRepository.Instance.playersState[veriIndex]
                        );

                        PlayerDataRepository.Instance.PlayerState.ActionFlag = true;
                        BattleManager.Instance.BattleDatas.Add(PlayerDataRepository.Instance.PlayerState);
                        PlayerDataRepository.Instance.NextCharacter();
                        MenuManager.Instance.CloseOpenMenu();
                        _menu.ChangeMenu(MenuList.Battle);
                        return;
                }
                break;
            case MenuList.ItemList:
                MenuManager.Instance.cursorPos.Push((State, veriIndex, horiIndex));
                MenuManager.Instance.selectPlayerNum = veriIndex;
                WindowObj.transform.gameObject.SetActive(false);
                MenuManager.Instance.cursorPos.Pop();

                if (GameManager.Instance.mode == Now_Mode.Field)
                {
                    PlayerDataRepository.Instance.UseItem(PlayerDataRepository.Instance.playersState[veriIndex]);
                }
                else if (GameManager.Instance.mode == Now_Mode.Battle)
                {
                    // 誰を選択したのか
                    PlayerDataRepository.Instance.PlayerState.to.Add(
                        PlayerDataRepository.Instance.playersState[veriIndex]
                    );
                    
                    PlayerDataRepository.Instance.PlayerState.ActionFlag = true;
                    BattleManager.Instance.BattleDatas.Add(PlayerDataRepository.Instance.PlayerState);
					PlayerDataRepository.Instance.NextCharacter();
                    MenuManager.Instance.CloseOpenMenu();
                    _menu.ChangeMenu(MenuList.Battle);
                    return;
				}

                break;
        }
        // 呼び出し元に帰る
        _menu.ChangeMenu(MenuManager.Instance.nowSelect);

    }

    public void CloseMenu()
    {
        Initialize();
        
        switch (MenuManager.Instance.nowSelect)
        {
            case MenuList.ItemList:
                WindowObj.transform.gameObject.SetActive(false);
                WindowObj.openMenu = false;
                //MenuManager.Instance.cursorPos.Pop();
                _menu.ChangeMenu(MenuList.ItemList);
                break;
            default:
                WindowObj.transform.gameObject.SetActive(false);
                WindowObj.openMenu = false;
                _menu.ChangeMenu(MenuList.Main);
                break;
        }
        
        
    }

    public void CursorUp() => MoveCursor(-1, 0);
    

    public void CursorDown() => MoveCursor(1, 0);


    public void CursorRight()
    {
        
    }

    public void CursorLeft()
    {
        
    }

    protected override void MoveCursor(int verticalDirection, int horizontalDirection)
    {
        veriIndex = Mathf.Clamp((veriIndex + verticalDirection),0,PlayerDataRepository.Instance.playersState.Count-1);
        // 上下移動と左右移動を組み合わせて扱う場合
        selectedIndex = veriIndex;
        float yOffset = selectedIndex  * 50; // カーソル間の垂直な間隔（例として50ピクセル）
        UpdateCursorPosition(0,yOffset);
    }
}
// 強さメニュー
public class MenuStateStrength : MenuData,IMenuState
{
    public MenuList State => MenuList.Strength;
    private MenuController _menu;
    public MenuStateStrength(MenuController menu) => _menu = menu;
    
    private GameObject _cursor;
    private StrengthMenu strength;
    public void Entry()
    {
        WindowObj = MenuManager.Instance.GetWindow(State);
        if (WindowObj is not null)
        {
            WindowObj.transform.gameObject.SetActive(true);
            strength = WindowObj.gameObject.GetComponent<StrengthMenu>();
            strength.Init(MenuManager.Instance.selectPlayerNum);
        }
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }

    public void SelectMenu()
    {
        
    }

    public void CloseMenu()
    {
        WindowObj.transform.gameObject.SetActive(false);
        _menu.ChangeMenu(MenuList.SelectChara);
    }

    public void CursorUp()
    {
    }

    public void CursorDown()
    {
    }

    public void CursorRight()
    {
    }

    public void CursorLeft()
    {
    }
}
// バトルメニュー
public class MenuStateBattle : MenuData,IMenuState
{
    public MenuList State => MenuList.Battle;
    private MenuController _menu;
    public MenuStateBattle(MenuController menu) => _menu = menu;
    
    public void Entry()
    {
        BattleManager.Instance.ParamChange(); // 表示パラメーター更新
        WindowObj = MenuManager.Instance.GetWindow(State);
        if (WindowObj is not null)
        {
            WindowObj.transform.gameObject.SetActive(true);
            
            SetCursorObject(MenuManager.Instance.GetCursorObj(State),State);
            
        }
    }

    public void Update()
    {
        switch (BattleManager.Instance.State)
        {
            case BattleList.End:
                _menu.ChangeMenu(MenuList.Battle_Result);
                break;
        }

        if (BattleManager.Instance.State != BattleList.Battle)
        {
            SetCursorActive(true);
        }
        else
        {
            SetCursorActive(false);
        }
    }

    public void Exit()
    {
        
    }

    public void SelectMenu()
    {
        if (BattleManager.Instance.State != BattleList.Battle)
        {
            SetActionCommand(selectedIndex);
			switch (selectedIndex)
            {
                case 0:     // こうげき
                    PlayerDataRepository.Instance.PlayerState.attackType = AttackType.Attack;
                    MenuManager.Instance.nowSelect = MenuList.Battle;
                    _menu.ChangeMenu(MenuList.Battle_Enemy);
                    break;
                case 1:     // じゅもん
                    PlayerDataRepository.Instance.PlayerState.attackType = AttackType.Magic;
                    MenuManager.Instance.nowSelect = MenuList.Magic;
                    MenuManager.Instance.cursorPos.Push((State, veriIndex, horiIndex));
                    _menu.ChangeMenu(MenuList.Magic);
                    break;
                case 2:     // ぼうぎょ
                    PlayerDataRepository.Instance.PlayerState.attackType = AttackType.Guard;
                    PlayerDataRepository.Instance.PlayerState.to.Clear();
					PlayerDataRepository.Instance.PlayerState.ActionFlag = true;
					BattleManager.Instance.BattleDatas.Add(PlayerDataRepository.Instance.PlayerState);
                    PlayerDataRepository.Instance.NextCharacter();
                    BattleManager.Instance.ParamChange(); // 表示パラメーター更新
					break;
                case 3:     // どうぐ
                    PlayerDataRepository.Instance.PlayerState.attackType = AttackType.Item;
                    MenuManager.Instance.nowSelect = MenuList.ItemList;
                    MenuManager.Instance.cursorPos.Push((State, veriIndex, horiIndex));
                    _menu.ChangeMenu(MenuList.ItemList);
					break;
            }
        }
    }

    public void CloseMenu()
    {
        PlayerDataRepository.Instance.PrevCharacter();
        BattleManager.Instance.ParamChange(); // 表示パラメーター更新
    }

    public void CursorUp()
    {
        if (BattleManager.Instance.State != BattleList.Battle)
        {
            MoveCursor(-1, 0);
        }
    }

    public void CursorDown()
    {
        if (BattleManager.Instance.State != BattleList.Battle)
        {
            MoveCursor(1, 0);
        }
    }

    public void CursorRight()
    {
    }

    public void CursorLeft()
    {
    }

    protected override void MoveCursor(int verticalDirection, int horizontalDirection)
    {
        veriIndex = Mathf.Clamp((veriIndex + verticalDirection),0,3);
        // 上下移動と左右移動を組み合わせて扱う場合
        selectedIndex = veriIndex;
        float yOffset = selectedIndex  * 50; // カーソル間の垂直な間隔（例として50ピクセル）
        UpdateCursorPosition(0,yOffset);
    }
}

// ステータス:敵選択
public class MenuStateBattle_EnemySelect : MenuData,IMenuState
{
    public MenuList State => MenuList.Battle_Enemy;
    private MenuController _menu;
    public MenuStateBattle_EnemySelect(MenuController menu) => _menu = menu;
    
    public void Entry()
    {
        SetCursorObject(_menu._BattleCursor.gameObject,State);
        SetCursorActive(true);
        BattleManager.Instance.SortEnemyList();
        MoveCursor(0, 0);
        if (PlayerDataRepository.Instance.PlayerState.attackType == AttackType.Magic)
        {
            if (MagicMaster.Entity.GetMagicData(PlayerDataRepository.Instance.PlayerState.selectMagicName).targetType ==
                MagicTargetType.AllEnemies)
            {
                BattleManager.Instance.SelectEnemy(0, true);
            }
        }
    }

    public void Update()
    {
    }

    public void Exit()
    {
        SetCursorActive(false);
        BattleManager.Instance.SelectEnemy(-1);
    }

    public void SelectMenu()
    {
        PlayerDataRepository.Instance.PlayerState.to.Clear();
        PlayerDataRepository.Instance.PlayerState.ActionFlag = true;
        BattleManager.Instance.BattleDatas.Add(PlayerDataRepository.Instance.PlayerState);
        switch (PlayerDataRepository.Instance.PlayerState.attackType)
        {
            case AttackType.Attack:
                PlayerDataRepository.Instance.PlayerState.to.Add(BattleManager.Instance.EnemyList[selectedIndex]);
                break;
            case AttackType.Magic:
                var magic = MagicMaster.Entity.GetMagicData(PlayerDataRepository.Instance.PlayerState.selectMagicName);
                switch (magic.targetType)
                {
                    case MagicTargetType.SingleEnemy:
                        PlayerDataRepository.Instance.PlayerState.to.Add(BattleManager.Instance.EnemyList[selectedIndex]);
                        break;
                    case MagicTargetType.AllEnemies:
                        for (int i = 0; i < BattleManager.Instance.enemyNum; i++)
                        {
                            PlayerDataRepository.Instance.PlayerState.to.Add(BattleManager.Instance.EnemyList[i]);
                        }
                        break;
                }
                break;
        }
        PlayerDataRepository.Instance.NextCharacter();
        BattleManager.Instance.ParamChange();
        MenuManager.Instance.CloseOpenMenu();
        _menu.ChangeMenu(MenuList.Battle);

        
    }

    public void CloseMenu()
    {
        //WindowObj.transform.gameObject.SetActive(false);
        SetCursorActive(false);
        BattleManager.Instance.SelectEnemy(-1);
        _menu.ChangeMenu(MenuManager.Instance.nowSelect);
    }

    public void CursorUp()
    {
        
    }

    public void CursorDown()
    {
        
    }

    public void CursorRight() => MoveCursor(0, 1);

    public void CursorLeft() => MoveCursor(0, -1);

    protected override void MoveCursor(int verticalDirection, int horizontalDirection)
    {
        switch (PlayerDataRepository.Instance.PlayerState.attackType)
        {
            case AttackType.Attack:
            {
                horiIndex = Mathf.Clamp((horiIndex + horizontalDirection), 0, BattleManager.Instance.enemyNum - 1);
                selectedIndex = horiIndex;
                EnemyCharactor selectEnemy =
                    BattleManager.Instance.EnemyList[selectedIndex].GetComponent<EnemyCharactor>();
                Vector3 enePos = BattleManager.Instance.EnemyList[selectedIndex].transform.position;
                BattleManager.Instance.SelectEnemy(selectedIndex);
                enePos.y += 1.5f;
                UpdateCursorPosition(enePos);
                break;
            }
            case AttackType.Magic:
                var magic = MagicMaster.Entity.GetMagicData(PlayerDataRepository.Instance.PlayerState.selectMagicName);
                switch (magic.targetType)
                {
                    case MagicTargetType.SingleEnemy:
                        horiIndex = Mathf.Clamp((horiIndex + horizontalDirection),0,BattleManager.Instance.enemyNum - 1);
                        selectedIndex = horiIndex;
                        EnemyCharactor selectEnemy = BattleManager.Instance.EnemyList[selectedIndex].GetComponent<EnemyCharactor>();    
                        Vector3 enePos = BattleManager.Instance.EnemyList[selectedIndex].transform.position;
                        BattleManager.Instance.SelectEnemy(selectedIndex);
                        enePos.y += 1.5f; 
                        UpdateCursorPosition(enePos);
                        break;
                    case MagicTargetType.AllEnemies:
                        break;
                }
                break;
        }

    }
}

// バトルリザルト
public class MenuStateBattle_Result : MenuData, IMenuState
{
    
    public MenuList State => MenuList.Battle_Result;
    private MenuController _menu;
    public MenuStateBattle_Result(MenuController menu) => _menu = menu;
    public void Entry()
    {
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }

    public void SelectMenu()
    {
    }

    public void CloseMenu()
    {
    }

    public void CursorUp()
    {
    }

    public void CursorDown()
    {
    }

    public void CursorRight()
    {
    }

    public void CursorLeft()
    {
    }
}

// 道具メニュー
public class MenuStateItemList : MenuData, IMenuState
{
    public MenuList State => MenuList.ItemList;
    private MenuController _menu;
    public MenuStateItemList(MenuController menu) => _menu = menu;
    private int pushFlag =0;
    private bool keyDown;
    private float timer;
    private const float moveTimer = 10;
    private float mTime;

    private const int MaxViewCount = 12; // 何個表示されるのか
    private float scrollSize;
    private int _moveCount;
    private float ypos;
    public void Entry()
    {
        // これを何故入れたか忘れた
        //if (!PlayerDataRepository.Instance.PlayerState.ActionFlag)
        {
            WindowObj = MenuManager.Instance.GetWindow(State);
            if (WindowObj is not null)
            {
                WindowObj.openMenu = true;
                WindowObj.transform.gameObject.SetActive(true);
                SetCursorObject(MenuManager.Instance.GetCursorObj(State), State);
                selectedIndex = veriIndex * 2 + horiIndex;
            }

            // 1 / (プレイヤーのアイテム総数 / 2) - 画面のアイテム表示数
            scrollSize = (1.0f / ((PlayerDataRepository.Instance.ItemCount / 2.0f) - MaxViewCount));
            WindowObj.ResetItemData();
            foreach (var item in PlayerDataRepository.Instance.ItemList)
            {
                WindowObj.CreateItemData(item.ID);
            }

            // 一番初めのアイテムを表示
            var data = PlayerDataRepository.Instance.GetItemData(PlayerDataRepository.Instance.ItemList[selectedIndex].ID);
            switch (GameManager.Instance.mode)
            {
                // バトル中は説明なし
                case Now_Mode.Field:
                    WindowObj.SetExpText(data != null ? data.Explanation.ConvertToFullWidth() : "");
                    break;
            }
        }
        /*
        else
        {
            Initialize();
            WindowObj.transform.gameObject.SetActive(false);
            _menu.ChangeMenu(MenuList.Battle);
        }
        Debug.Log(State);
        */
    }

    public void Update()
    {
        if (PlayerDataRepository.Instance.ItemList.Count > selectedIndex)
        {
            var data = PlayerDataRepository.Instance.GetItemData(PlayerDataRepository.Instance.ItemList[selectedIndex]
                .ID);
            WindowObj.SetExpText(data is not null ? data.Explanation.ConvertToFullWidth() : "");
        }

    }

    public void Exit()
    {
    }

    public void SelectMenu()
    {
        var data = PlayerDataRepository.Instance.GetItemData(PlayerDataRepository.Instance.ItemList[selectedIndex].ID);
        if(data != null)
        {
            switch (data.Effect)
            {
                case EffectType.Recovery:
                case EffectType.Weapon:
                    PlayerDataRepository.Instance.selectItemId = data.ID;
                    MenuManager.Instance.nowSelect = MenuList.ItemList;
                    MenuManager.Instance.cursorPos.Push((State, veriIndex, horiIndex));
                    _menu.ChangeMenu(MenuList.SelectChara);
                    break;
            }
        }
    }

    public void CloseMenu()
    {
        switch (GameManager.Instance.mode)
        {
            case Now_Mode.Field:
                Initialize();
                WindowObj.transform.gameObject.SetActive(false);
                WindowObj.openMenu = false;
                _menu.ChangeMenu(MenuList.Main);
                break;
            case Now_Mode.Battle:
                Initialize();
                WindowObj.transform.gameObject.SetActive(false);
                WindowObj.openMenu = false;
                _menu.ChangeMenu(MenuList.Battle);
                break;
        }
    }

    public void CursorUp()
    {
        keyDown = true;
        MoveCursor(-1, 0);
        pushFlag = 1;
        
    }

    public void CursorDown()
    {
        keyDown = true;
        MoveCursor(1, 0);
        pushFlag = 2;
    }

    public void CursorRight()
    {
        keyDown = true;
        MoveCursor(0, 1);
        
    }

    public void CursorLeft()
    {
        keyDown = true;
        MoveCursor(0, -1);
    }

    protected override void MoveCursor(int verticalDirection, int horizontalDirection)
    {
        double maxIndex = PlayerDataRepository.Instance.ItemCount / 2.0;
        if (Math.Abs(maxIndex - Math.Floor(maxIndex)) > 0.01) {
            maxIndex = Math.Ceiling(maxIndex);
        } else {
            maxIndex = Math.Round(maxIndex);
        }

        if (maxIndex == 0) return;
        
        veriIndex = Mathf.Clamp((veriIndex + verticalDirection), 0,  (int)maxIndex - 1);
        ypos = Mathf.Clamp((ypos+ verticalDirection), -1, ypos < veriIndex ? MaxViewCount: veriIndex);
        
        horiIndex = Mathf.Clamp((horiIndex + horizontalDirection), 0, 1);
        switch (ypos)
        {
            case 12 when verticalDirection == 1:
                if (veriIndex < (int)maxIndex - 1)
                {
                    SetScrollPos(verticalDirection);
                    ypos -= 1;
                }
                break;
            case -1 when verticalDirection == -1:
            {
                if (veriIndex > -1)
                {
                    SetScrollPos(verticalDirection);
                    ypos += 1;
                }
                    
                
                break;
            }
        }
        selectedIndex = veriIndex * 2 + horiIndex;
        float yOffset = ypos * 50.0f;
        float xOffset = (selectedIndex % 2) * 440;
        UpdateCursorPosition(xOffset, yOffset);
    }
    
    void SetScrollPos(int VerticalDirection)
    {
        WindowObj.scrollrect.content.anchoredPosition += new Vector2(0, 50 * VerticalDirection);
    }
}


public class ShopMenu_Tool: MenuData, IMenuState
{
    public MenuList State => MenuList.Shop_Tool;
    private MenuController _menu;
    public ShopMenu_Tool(MenuController menu) => _menu = menu;
    private int _itemCount = 5;
    public void Entry()
    {
        GameManager.Instance.mode = Now_Mode.Menu;
        WindowObj = MenuManager.Instance.GetWindow(MenuList.Shop);
        if (WindowObj is not null)
        {
            WindowObj.transform.gameObject.SetActive(true);
            SetCursorObject(MenuManager.Instance.GetCursorObj(MenuList.Shop),State);
            selectedIndex = veriIndex;
        }
        WindowObj.shopMenu.CreateShopMenu(State);
        // 所持数
        _itemCount = NPCManager.Instance.nowTalkItemShopData.ShopItemDatas.Count;
        WindowObj.shopMenu.SetExplanation(selectedIndex);
    }

    public void Update()
    {
    }

    public void Exit()
    {
        
    }

    public void SelectMenu()
    {
        
        WindowObj.shopMenu.ShowCursor(true);
        Vector3 pos = GetCursorPosition();
        pos.x += 550; 
        WindowObj.shopMenu.SetCursorPos(pos);
        MenuManager.Instance.nowSelect = State;
        MenuManager.Instance.cursorPos.Push((State, veriIndex, horiIndex));
        _menu.ChangeMenu(MenuList.Shop_Buy);
    }

    public void CloseMenu()
    {
        Initialize();
        WindowObj.shopMenu.EndShopMenu();
        WindowObj.transform.gameObject.SetActive(false);
        //MenuButton.EndButton();
        _menu.ChangeMenu(MenuList.None);
        MenuManager.Instance.playerEndMenuHandl();
        GameManager.Instance.mode = Now_Mode.Field;
    }

    public void CursorUp()
    {
        MoveCursor(-1, 0);
        WindowObj.shopMenu.SetExplanation(selectedIndex);
    }

    public void CursorDown()
    {
        MoveCursor(1, 0);
        WindowObj.shopMenu.SetExplanation(selectedIndex);
    }

    public void CursorRight()
    {
    }

    public void CursorLeft()
    {
    }

    protected override void MoveCursor(int verticalDirection, int horizontalDirection)
    {
        veriIndex = Mathf.Clamp((veriIndex + verticalDirection),0,_itemCount-1);
        selectedIndex = veriIndex;
        float yOffset = (selectedIndex) * 50; // カーソル間の垂直な間隔（例として50ピクセル）
        UpdateCursorPosition(0,yOffset);
    }
}

public class ShopMenu_Weapon: MenuData, IMenuState
{
    public MenuList State => MenuList.Shop_Weapon;
    private MenuController _menu;
    public ShopMenu_Weapon(MenuController menu) => _menu = menu;
    private int _itemCount = 5;
    public void Entry()
    {
        GameManager.Instance.mode = Now_Mode.Menu;
        WindowObj = MenuManager.Instance.GetWindow(MenuList.Shop);
        if (WindowObj is not null)
        {
            WindowObj.transform.gameObject.SetActive(true);
            SetCursorObject(MenuManager.Instance.GetCursorObj(MenuList.Shop),State);
            selectedIndex = veriIndex;
        }
        WindowObj.shopMenu.CreateShopMenu(State);
        // 所持数
        _itemCount = NPCManager.Instance.nowTalkItemShopData.ShopItemDatas.Count;
        WindowObj.shopMenu.SetExplanation(selectedIndex);
        WindowObj.shopMenu.ShowStatusMenu(true);
        WindowObj.shopMenu.SetStatusData(selectedIndex);
        
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            
        }
    }

    public void Exit()
    {
        
    }

    public void SelectMenu()
    {
        WindowObj.shopMenu.ShowCursor(true);
        Vector3 pos = GetCursorPosition();
        pos.x += 550; 
        WindowObj.shopMenu.SetCursorPos(pos);
        MenuManager.Instance.nowSelect = MenuList.Shop_Weapon;
        MenuManager.Instance.cursorPos.Push((State, veriIndex, horiIndex));
        _menu.ChangeMenu(MenuList.Shop_Buy);
    }

    public void CloseMenu()
    {
        Initialize();
        WindowObj.shopMenu.EndShopMenu();
        WindowObj.shopMenu.ShowStatusMenu(false);
        WindowObj.transform.gameObject.SetActive(false);
        //MenuButton.EndButton();
        _menu.ChangeMenu(MenuList.None);
        MenuManager.Instance.playerEndMenuHandl();
        GameManager.Instance.mode = Now_Mode.Field;
    }

    public void CursorUp()
    {
        MoveCursor(-1, 0);
        WindowObj.shopMenu.SetExplanation(selectedIndex);
        WindowObj.shopMenu.SetStatusData(selectedIndex);
    }

    public void CursorDown()
    {
        MoveCursor(1, 0);
        WindowObj.shopMenu.SetExplanation(selectedIndex);
        WindowObj.shopMenu.SetStatusData(selectedIndex);
    }

    public void CursorRight()
    {
    }

    public void CursorLeft()
    {
    }

    protected override void MoveCursor(int verticalDirection, int horizontalDirection)
    {
        veriIndex = Mathf.Clamp((veriIndex + verticalDirection),0,_itemCount-1);
        selectedIndex = veriIndex;
        float yOffset = (selectedIndex) * 50; // カーソル間の垂直な間隔（例として50ピクセル）
        UpdateCursorPosition(0,yOffset);
    }
}

public class ShopBuyMenu : MenuData, IMenuState
{
    public MenuList State => MenuList.Shop_Buy;
    private MenuController _menu;
    public ShopBuyMenu(MenuController menu) => _menu = menu;
    public void Entry()
    {
        WindowObj = MenuManager.Instance.GetWindow(MenuList.Shop);
        WindowObj.shopMenu.ChangeBuyItemNum(MenuManager.Instance.cursorPos.Peek().Item2,0);
    }

    public void Update()
    {
    }

    public void Exit()
    {
        
    }

    public void SelectMenu()
    {
        
        // item1=ID item2=個数 item3=トータル金額
        (int, int,int) data = WindowObj.shopMenu.GetBuyItemIDAndCount(MenuManager.Instance.cursorPos.Peek().Item2);

        if (PlayerDataRepository.Instance.gold >= data.Item3 && data.Item2 > 0)
        {
            WindowObj.shopMenu.ShowCursor(false);
            var iData = ItemMaster.Entity.GetItemData(data.Item1);
            if (iData != null)
            {
                PlayerDataRepository.Instance.GetItem(iData.ID,data.Item2);        
            }
            else
            {
                var wData = WeaponArmorMaster.Entity.GetWeaponData(data.Item1);
                if(wData != null)
                    PlayerDataRepository.Instance.GetItem(wData.ID,data.Item2);
            }
            
            PlayerDataRepository.Instance.ChangeGold(-data.Item3);
            _menu.ChangeMenu(MenuManager.Instance.nowSelect);    
        }
        
    }

    public void CloseMenu()
    {
        WindowObj.shopMenu.ShowCursor(false);
        _menu.ChangeMenu(MenuManager.Instance.nowSelect);
    }

    public void CursorUp() => WindowObj.shopMenu.ChangeBuyItemNum(MenuManager.Instance.cursorPos.Peek().Item2,1);

    public void CursorDown() => WindowObj.shopMenu.ChangeBuyItemNum(MenuManager.Instance.cursorPos.Peek().Item2,-1);

    public void CursorRight()
    {
    }

    public void CursorLeft()
    {
    }
}

public class EquipmentMenu : MenuData, IMenuState
{
    private MenuController _menu;
    public EquipmentMenu(MenuController menu) => _menu = menu;
    public MenuList State => MenuList.EquipmentMenu;
    
    public void Entry()
    {
        WindowObj = MenuManager.Instance.GetWindow(State);
        if (WindowObj is not null)
        {
            WindowObj.transform.gameObject.SetActive(true);
        }
    }
    public void CloseMenu()
    {
        WindowObj.transform.gameObject.SetActive(false);
        _menu.ChangeMenu(MenuList.Main);
    }

    public void CursorUp()
    {
    }

    public void CursorDown()
    {
    }

    public void CursorRight()
    {
    }

    public void CursorLeft()
    {
    }

    public void Exit()
    {
    }

    public void SelectMenu()
    {
    }

    public void Update()
    {
        // 前の状態で終了していたら、終了処理へと移行
        if (MenuManager.Instance.prevSelect == MenuList.EquipmentMenu1)
        {
            WindowObj.transform.gameObject.SetActive(false);
            MenuManager.Instance.prevSelect = MenuList.None;
            _menu.ChangeMenu(MenuList.Main);    
        }
        else
        {
            _menu.ChangeMenu(MenuList.EquipmentMenu1);
        }
    }
}

public class MagicMenu : MenuData, IMenuState
{
    private MenuController _menu;
    public MagicMenu(MenuController menu) => _menu = menu;
    public MenuList State => MenuList.Magic;
    private float ypos;
    private const int MaxViewCount = 12; // 何個表示されるのか
    public void Entry()
    {
        WindowObj = MenuManager.Instance.GetWindow(State);
        if (WindowObj is not null)
        {
            WindowObj.transform.gameObject.SetActive(true);
            WindowObj.openMenu = true;
            SetCursorObject(MenuManager.Instance.GetCursorObj(State), State);

            selectedIndex = veriIndex * 2 + horiIndex;
        }
        MenuManager.Instance.nowSelect = State;
        // 1 / (プレイヤーのアイテム総数 / 2) - 画面のアイテム表示数
        WindowObj.ResetItemData();
        foreach (var item in PlayerDataRepository.Instance.PlayerState.magicData)
        {
            if(item == null)
            {
                break;
            }
            WindowObj.CreateMagicData(MagicMaster.Entity.GetMagicData(item));
        }

        // 一番初めのアイテムを表示
        var data = PlayerDataRepository.Instance.GetMagicData(selectedIndex);
        switch (GameManager.Instance.mode)
        {
            // バトル中は説明なし
            case Now_Mode.Field:
                WindowObj.SetExpText(data != null ? data.explanation.ConvertToFullWidth() : "");
                break;
        }
    }

    public void Update()
    {
        var data = PlayerDataRepository.Instance.GetMagicData(selectedIndex);
        WindowObj.SetExpText(data != null ? data.explanation.ConvertToFullWidth() : "");
    }

    public void Exit()
    {
        //MenuManager.Instance.cursorPos.Pop();
    }

    public void SelectMenu()
    {
        switch(GameManager.Instance.mode)
        {
            case Now_Mode.Field:
                /*
                WindowObj.transform.gameObject.SetActive(false);
                MenuManager.Instance.cursorPos.Push((State, veriIndex, horiIndex));
                _menu.ChangeMenu(MenuList.SelectChara);
                */
                break;
            case Now_Mode.Battle:
                var magic = PlayerDataRepository.Instance.GetMagicData(selectedIndex);
                if(PlayerDataRepository.Instance.PlayerState.parameter.Mp < magic.mpCost)
                {
                    return;
                }
                MenuManager.Instance.cursorPos.Push((State, veriIndex, horiIndex));
                PlayerDataRepository.Instance.PlayerState.selectMagicName = magic.magicName;
                switch (magic.targetType)
                {
                    case MagicTargetType.Self:
                        break;
                    case MagicTargetType.SingleAlly:
                        _menu.ChangeMenu(MenuList.SelectChara);
                        break;
                    case MagicTargetType.AllAllies:
                        break;
                    case MagicTargetType.AllEnemies:
                    case MagicTargetType.SingleEnemy:
                        _menu.ChangeMenu(MenuList.Battle_Enemy);
                        break;
                }
                
                break;
        }
    }

    public void CloseMenu()
    {
        switch (GameManager.Instance.mode)
        {
            case Now_Mode.Field:
                Initialize();
                WindowObj.transform.gameObject.SetActive(false);
                _menu.ChangeMenu(MenuList.SelectChara);
                break;
            case Now_Mode.Battle:
                Initialize();
                WindowObj.transform.gameObject.SetActive(false);
                WindowObj.openMenu = false;
                _menu.ChangeMenu(MenuList.Battle);
                break;
        }
        WindowObj.openMenu = false;
    }

    public void CursorUp()
    {
        MoveCursor(-1, 0);
    }

    public void CursorDown()
    {
        MoveCursor(1, 0);
    }

    public void CursorRight()
    {
        MoveCursor(0, 1);
    }

    public void CursorLeft()
    {
        MoveCursor(0, -1);
    }
    protected override void MoveCursor(int verticalDirection, int horizontalDirection)
    {
        double maxIndex = PlayerDataRepository.Instance.PlayerState.magicData.Count / 2.0;
        if (Math.Abs(maxIndex - Math.Floor(maxIndex)) > 0.01)
        {
            maxIndex = Math.Ceiling(maxIndex);
        }
        else
        {
            maxIndex = Math.Round(maxIndex);
        }

        if (maxIndex == 0) return;

        veriIndex = Mathf.Clamp((veriIndex + verticalDirection), 0, (int)maxIndex - 1);
        ypos = Mathf.Clamp((ypos + verticalDirection), -1, ypos < veriIndex ? MaxViewCount : veriIndex);

        horiIndex = Mathf.Clamp((horiIndex + horizontalDirection), 0, 1);
        switch (ypos)
        {
            case 12 when verticalDirection == 1:
                if (veriIndex < (int)maxIndex - 1)
                {
                    SetScrollPos(verticalDirection);
                    ypos -= 1;
                }
                break;
            case -1 when verticalDirection == -1:
                {
                    if (veriIndex > -1)
                    {
                        SetScrollPos(verticalDirection);
                        ypos += 1;
                    }


                    break;
                }
        }
        selectedIndex = veriIndex * 2 + horiIndex;
        float yOffset = ypos * 50.0f;
        float xOffset = (selectedIndex % 2) * 440;
        UpdateCursorPosition(xOffset, yOffset);
    }

    void SetScrollPos(int VerticalDirection)
    {
        WindowObj.scrollrect.content.anchoredPosition += new Vector2(0, 50 * VerticalDirection);
    }
}

public class EquipmentMenu1 : MenuData, IMenuState
{
    private MenuController _menu;
    public EquipmentMenu1(MenuController menu) => _menu = menu;
    public MenuList State => MenuList.EquipmentMenu1;
    
    public void Entry()
    {
        MenuManager.Instance.nowSelect = State;
        InputManager.Instance.SetKeyEvent(UseButtonType.Menu,InputMaster.Entity.MenuKey.CharaSelectLeft,
            SelectPrevChara);
        InputManager.Instance.SetKeyEvent(UseButtonType.Menu,InputMaster.Entity.MenuKey.CharaSelectRight,
            SelectNextChara);
        
        WindowObj = MenuManager.Instance.GetWindow(State);
        if (WindowObj is not null)
        {
            WindowObj.transform.gameObject.SetActive(true);
            WindowObj.openMenu = true;
            SetCursorObject(MenuManager.Instance.GetCursorObj(State),State);
            selectedIndex = veriIndex;
        }
        UpdateText();
        // キャラのステータス
        WindowObj.equData.Parameter.InitDataParam(PlayerDataRepository.Instance.SelectIndex);
    }
    void UpdateText()
    {
        WindowObj.equData.Select.SetText(
            PlayerDataRepository.Instance.PlayerState.CharaName,
            PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.Hand1.name,
            PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.Hand2.name,
            PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.Head.name,
            PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.Body.name
        );
    }
    public void CloseMenu()
    {
        Initialize();
        MenuManager.Instance.prevSelect = State;
        _menu.ChangeMenu(MenuList.EquipmentMenu);
        PlayerDataRepository.Instance.SelectIndex = 0;
        InputManager.Instance.SetKeyEvent(UseButtonType.Menu,InputMaster.Entity.MenuKey.CharaSelectLeft,
            SelectPrevChara,true);
        InputManager.Instance.SetKeyEvent(UseButtonType.Menu,InputMaster.Entity.MenuKey.CharaSelectRight,
            SelectNextChara,true);
        WindowObj.openMenu = false;
    }

    public void CursorUp()
    {
        MoveCursor(-1, 0);
    }

    public void CursorDown()
    {
        MoveCursor(1, 0);
    }

    public void CursorRight()
    {
    }

    public void CursorLeft()
    {
    }

    public void Exit()
    {
    }

    public void SelectMenu()
    {
        WindowObj.transform.gameObject.SetActive(false);
        MenuManager.Instance.nowSelect = MenuList.EquipmentMenu2;
        MenuManager.Instance.cursorPos.Push((State, veriIndex, horiIndex));
        _menu.ChangeMenu(MenuList.EquipmentMenu2);

    }

    public void Update()
    {
    }

    public void SelectNextChara()
    {
        if (!WindowObj.transform.gameObject.activeInHierarchy) return;
        PlayerDataRepository.Instance.NextCharacter();
        UpdateText();
        WindowObj.equData.Parameter.InitDataParam(PlayerDataRepository.Instance.SelectIndex);
    }
    public void SelectPrevChara()
    {
        if (!WindowObj.transform.gameObject.activeInHierarchy) return;
        PlayerDataRepository.Instance.PrevCharacter();
        UpdateText();
        WindowObj.equData.Parameter.InitDataParam(PlayerDataRepository.Instance.SelectIndex);
    }
    
    protected override void MoveCursor(int verticalDirection, int horizontalDirection)
    {
        veriIndex = Mathf.Clamp((veriIndex + verticalDirection),0,3);
        selectedIndex = veriIndex;
        float yOffset = (selectedIndex) * 118; // カーソル間の垂直な間隔（例として50ピクセル）
        UpdateCursorPosition(0,yOffset);
    }
}

public class EquipmentMenu2 : MenuData, IMenuState
{
    private MenuController _menu;
    public EquipmentMenu2(MenuController menu) => _menu = menu;
    public MenuList State => MenuList.EquipmentMenu2;
    private int itemCounter = 0;
    private float ypos;

    private List<InfoWeaponArmor> weaponData = new ();
    private WeaponArmorEquipment.Part part;
    private List<PlayerDataRepository.HubItemData> itemIdList = new ();
    public void Entry()
    {
        MenuManager.Instance.nowSelect = State;
        WindowObj = MenuManager.Instance.GetWindow(State);
        if (WindowObj is not null)
        {
            WindowObj.transform.gameObject.SetActive(true);
            SetCursorObject(MenuManager.Instance.GetCursorObj(State),State);
            selectedIndex = veriIndex;
        }
        
        // 1 / (プレイヤーのアイテム総数 / 2) - 画面のアイテム表示数

        WindowObj.ResetItemData();
        itemIdList = PlayerDataRepository.Instance.ItemList
            .Where(item => item.ID <= 99)
            .ToList();
        
        // はずすを追加
        WindowObj.CreateItemData();
        itemCounter = 0;
        itemCounter++;
        part = MenuManager.Instance.cursorPos.Peek().Item2 switch
        {
            0 => WeaponArmorEquipment.Part.Hand1,
            1 => WeaponArmorEquipment.Part.Hand2,
            2 => WeaponArmorEquipment.Part.Head,
            3 => WeaponArmorEquipment.Part.Body,
        };
        
        foreach (var item in itemIdList)
        {
            if (WeaponArmorMaster.Entity.GetWeaponData(item.ID).equipment == part)
            {
                int num = PlayerDataRepository.Instance.GetWeaponArmorSetCount(part, item.ID);
                if (num >= item.num) return;
                itemCounter++;
                WindowObj.CreateItemData(item.ID);
                weaponData.Add(WeaponArmorMaster.Entity.GetWeaponData(item.ID));
            }
            if (WeaponArmorMaster.Entity.GetWeaponData(item.ID).equipment == WeaponArmorEquipment.Part.DoubleHand && ((part == WeaponArmorEquipment.Part.Hand1) || (part == WeaponArmorEquipment.Part.Hand2)))
            {
                int num = PlayerDataRepository.Instance.GetWeaponArmorSetCount(WeaponArmorEquipment.Part.DoubleHand, item.ID);
                if (num >= item.num) return;
                itemCounter++;
                WindowObj.CreateItemData(item.ID);
                weaponData.Add(WeaponArmorMaster.Entity.GetWeaponData(item.ID));
            }
        }

        // 一番初めのアイテムを表示
        var data = PlayerDataRepository.Instance.GetWeaponData(selectedIndex);
        switch (GameManager.Instance.mode)
        {
            // バトル中は説明なし
            case Now_Mode.Field:
                WindowObj.SetExpText(data != null ? data.Explanation.ConvertToFullWidth() : "");
                break;
        }

        SetEquipmentData();
        WindowObj.equData.Parameter.SetBeAFParameter(PlayerDataRepository.Instance.SelectIndex, new Parameter(),
            PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.GetEquipmentUpParameter(part));
    }

    private void SetEquipmentData()
    {
        string hand1 = PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.Hand1.name;
        string hand2 = PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.Hand2.name;
        string head = PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.Head.name;
        string body = PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.Body.name;
        // 現在の装備品の表示更新
        WindowObj.equData.Data.SetEquipmentData(
            part switch
            {
                WeaponArmorEquipment.Part.Hand1 => "みぎて",
                WeaponArmorEquipment.Part.Hand2 => "ひだりて",
                WeaponArmorEquipment.Part.Head => "あたま",
                WeaponArmorEquipment.Part.Body => "からだ",
            },
            part switch
            {
                WeaponArmorEquipment.Part.Hand1 =>
                    hand1 == "" ? "すで"　: hand1,
                WeaponArmorEquipment.Part.Hand2 =>
                    hand2 == "" ? "すで"　: hand2,
                WeaponArmorEquipment.Part.Head =>
                    head == "" ? "そうびなし"　: head,
                WeaponArmorEquipment.Part.Body =>
                    body == "" ? "そうびなし"　: body,
            },
            part switch
            {
                WeaponArmorEquipment.Part.Hand1 => hand2 == "" ? "すで" : hand2,
                _ => hand1 == "" ? "すで" : hand1,
            },
            part switch
            {
                WeaponArmorEquipment.Part.Head => hand2 == "" ? "すで" : hand2,
                WeaponArmorEquipment.Part.Body => hand2 == "" ? "すで" : hand2,
                _ => head == "" ? "そうびなし" : head,
            },
            part switch
            {
                WeaponArmorEquipment.Part.Body => head == "" ? "そうびなし" : head,
                _ => body == "" ? "そうびなし" : body,
            }
        );
    }

    public void CloseMenu()
    {
        Initialize();
        MenuManager.Instance.prevSelect = State;
        WindowObj.transform.gameObject.SetActive(false);
        _menu.ChangeMenu(MenuList.EquipmentMenu1);
        
    }

    public void CursorUp()
    {
        if (weaponData.Count == 0 || veriIndex == 0) return;
        MoveCursor(-1, 0);
            
        WindowObj.equData.Parameter.SetBeAFParameter(PlayerDataRepository.Instance.SelectIndex,
            veriIndex - 1 == -1 ? new Parameter() : weaponData[veriIndex - 1].UpParam
            ,PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.GetEquipmentUpParameter(part));
    }

    public void CursorDown()
    {
        if (weaponData.Count == 0) return; 
        MoveCursor(1, 0);
        WindowObj.equData.Parameter.SetBeAFParameter(PlayerDataRepository.Instance.SelectIndex,
            weaponData[veriIndex-1].UpParam,
            PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.GetEquipmentUpParameter(part));
    }

    public void CursorRight()
    {
    }

    public void CursorLeft()
    {
    }

    public void Exit()
    {
        weaponData.Clear();
    }

    public void SelectMenu()
    {
        // はずすを選択した場合
        if (selectedIndex == 0)
        {
            if(part == WeaponArmorEquipment.Part.Hand1 || part == WeaponArmorEquipment.Part.Hand2)
            {
                PlayerDataRepository.Instance.PlayerState.SetWaeponArmor(WeaponArmorEquipment.Part.Hand1, new InfoWeaponArmor());
                PlayerDataRepository.Instance.PlayerState.SetWaeponArmor(WeaponArmorEquipment.Part.Hand2, new InfoWeaponArmor());
            }
            else
            {
                PlayerDataRepository.Instance.PlayerState.SetWaeponArmor(part, new InfoWeaponArmor());
            }
                
            WindowObj.equData.Parameter.SetBeAFParameter(
                PlayerDataRepository.Instance.SelectIndex,
                new Parameter(),
                PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.GetEquipmentUpParameter(part));
        }
        else
        {
            if(weaponData[selectedIndex - 1].equipment == WeaponArmorEquipment.Part.DoubleHand)
            {
                PlayerDataRepository.Instance.PlayerState.SetWaeponArmor(WeaponArmorEquipment.Part.Hand1, WeaponArmorMaster.Entity.GetWeaponData(weaponData[selectedIndex - 1].ID));
                PlayerDataRepository.Instance.PlayerState.SetWaeponArmor(WeaponArmorEquipment.Part.Hand2, WeaponArmorMaster.Entity.GetWeaponData(weaponData[selectedIndex - 1].ID));
            }
            else
            {
                PlayerDataRepository.Instance.PlayerState.SetWaeponArmor(part, WeaponArmorMaster.Entity.GetWeaponData(weaponData[selectedIndex - 1].ID));
            }
                
            WindowObj.equData.Parameter.SetBeAFParameter(
                PlayerDataRepository.Instance.SelectIndex,
                weaponData[selectedIndex -1].UpParam,
                PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.GetEquipmentUpParameter(part));
        }
        SetEquipmentData();
        
        
    }

    public void Update()
    {
       
    }
    
    protected override void MoveCursor(int verticalDirection, int horizontalDirection)
    {
        
        veriIndex = Mathf.Clamp((veriIndex + verticalDirection), 0, itemCounter-1);
        ypos = Mathf.Clamp( (ypos + verticalDirection ), -1, 7);
        if (veriIndex < ypos)
            ypos = veriIndex;
        switch (ypos)
        {
            case 7 when verticalDirection == 1:
                if (veriIndex < itemCounter)
                {
                    SetScrollPos(verticalDirection);
                    ypos -= 1;
                }
                break;
            case -1 when verticalDirection == -1:
            {
                if (veriIndex > -1)
                {
                    SetScrollPos(verticalDirection);
                    ypos += 1;
                }
                    
                
                break;
            }
        }
        selectedIndex = veriIndex ;
        float yOffset = ypos * 50.0f;
        
        UpdateCursorPosition(0, yOffset);
    }
    void SetScrollPos(int VerticalDirection)
    {
        WindowObj.scrollrect.content.anchoredPosition += new Vector2(0, 50 * VerticalDirection);
    }
}

