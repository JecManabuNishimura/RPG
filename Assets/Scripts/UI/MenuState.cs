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
        MenuManager.Instance.cursorPos.Push((State, veriIndex,horiIndex));
        MenuManager.Instance.selectPlayerNum = veriIndex;
        switch(MenuManager.Instance.nowSelect)
        {
            case MenuList.ItemList:
                if (GameManager.Instance.mode == Now_Mode.Field)
                {
                    WindowObj.transform.gameObject.SetActive(false);
                    MenuManager.Instance.cursorPos.Pop();
                    PlayerDataRepository.Instance.UseItem(PlayerDataRepository.Instance.playersState[veriIndex]
                    );
                }
                else if (GameManager.Instance.mode == Now_Mode.Battle)
                {
                    WindowObj.transform.gameObject.SetActive(false);
                    MenuManager.Instance.cursorPos.Pop();       // キャラ選択からの抜け出し
                    // 誰を選択したのか
                    PlayerDataRepository.Instance.PlayerState.to.Add(
                        PlayerDataRepository.Instance.playersState[veriIndex]
                    );
                    if (MenuManager.Instance.nowSelect == MenuList.ItemList)
                    {
                        //PlayerDataRepository.Instance.PlayerState.UseItemParam = ItemMaster.Instance.selectItem;
                        //PlayerDataRepository.Instance.selectItemId = ItemMaster.Instance.;
                    }

                    PlayerDataRepository.Instance.PlayerState.ActionFlag = true;
                    BattleManager.Instance.BattleDatas.Add(PlayerDataRepository.Instance.PlayerState);
                    MenuManager.Instance.cursorPos.Pop();       // アイテムリストからの抜け出し
					PlayerDataRepository.Instance.NextCharacter();
					var itemwindow = MenuManager.Instance.GetWindow(MenuList.ItemList);
					itemwindow.transform.gameObject.SetActive(false);
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
                //MenuManager.Instance.cursorPos.Pop();
                _menu.ChangeMenu(MenuList.ItemList);
                break;
            default:
                WindowObj.transform.gameObject.SetActive(false);
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
            /*
            // カーソルの移動
            if (Input.GetKeyDown(KeyCode.W))
            {
                MoveCursor(-1, 0);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                MoveCursor(1, 0);
            }*/
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
                    _menu.ChangeMenu(MenuList.Battle_Enemy);
                    break;
                case 1:     // じゅもん
                    break;
                case 2:     // ぼうぎょ
					PlayerDataRepository.Instance.PlayerState.to.Clear();
					PlayerDataRepository.Instance.PlayerState.ActionFlag = true;
					BattleManager.Instance.BattleDatas.Add(PlayerDataRepository.Instance.PlayerState);
                    PlayerDataRepository.Instance.NextCharacter();
                    BattleManager.Instance.ParamChange(); // 表示パラメーター更新
					break;
                case 3:     // どうぐ
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
		PlayerDataRepository.Instance.PlayerState.to.Add(BattleManager.Instance.EnemyList[selectedIndex]);
        BattleManager.Instance.BattleDatas.Add(PlayerDataRepository.Instance.PlayerState);
        PlayerDataRepository.Instance.NextCharacter();
        BattleManager.Instance.ParamChange();
        _menu.ChangeMenu(MenuList.Battle);
    }

    public void CloseMenu()
    {
        //WindowObj.transform.gameObject.SetActive(false);
        SetCursorActive(false);
        BattleManager.Instance.SelectEnemy(-1);
        _menu.ChangeMenu(MenuList.Battle);
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
       
        horiIndex = Mathf.Clamp((horiIndex + horizontalDirection),0,BattleManager.Instance.enemyNum - 1);
        selectedIndex = horiIndex;
        EnemyCharactor selectEnemy = BattleManager.Instance.EnemyList[selectedIndex].GetComponent<EnemyCharactor>();    
        Vector3 enePos = BattleManager.Instance.EnemyList[selectedIndex].transform.position;
        BattleManager.Instance.SelectEnemy(selectedIndex);
        enePos.y += 1.5f; 
        UpdateCursorPosition(enePos);
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
                WindowObj.transform.gameObject.SetActive(true);
                SetCursorObject(MenuManager.Instance.GetCursorObj(State), State);
                selectedIndex = veriIndex * 2 + horiIndex;
            }

            // 1 / (プレイヤーのアイテム総数 / 2) - 画面のアイテム表示数
            scrollSize = (1.0f / ((PlayerDataRepository.Instance.ItemCount / 2.0f) - MaxViewCount));
            WindowObj.ResetItemData();
            foreach (var item in PlayerDataRepository.Instance.ItemList)
            {
                WindowObj.CreateItemData(item.Value);
            }

            // 一番初めのアイテムを表示
            var data = PlayerDataRepository.Instance.GetItemList(selectedIndex);
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
        var data = PlayerDataRepository.Instance.GetItemList(selectedIndex);
        WindowObj.SetExpText(data is not null ? data.Explanation.ConvertToFullWidth() : "");
        /*
        switch (GameManager.Instance.mode)
        {
            case Now_Mode.Menu:
            {
                // 選択したアイテムの説明を表示
                var data = PlayerDataRepository.Instance.GetItemList(selectedIndex);
                WindowObj.SetExpText(data is not null ? data.Explanation.ConvertToFullWidth() : "");
                break;
            }
        }*/
        /*
        if (Input.GetKeyDown(KeyCode.W))
        {
            keyDown = true;
            MoveCursor(-1, 0);
            pushFlag = 1;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            keyDown = true;
            MoveCursor(1, 0);
            pushFlag = 2;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            keyDown = true;
            MoveCursor(0, -1);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            keyDown = true;
            MoveCursor(0, 1);
        }*/
        // 後で考える
        /*
        if(Input.GetKeyUp(KeyCode.W)||Input.GetKeyUp(KeyCode.S))
        {
            pushFlag = 0;
            timer = 0;
        }

        if (keyDown)
        {
            keyDown = false;
            switch (GameManager.Instance.mode)
            {
                case Now_Mode.Field:
                {
                    // 選択したアイテムの説明を表示
                    var data = PlayerDataRepository.Instance.GetItemList(selectedIndex);
                    WindowObj.SetExpText(data is not null ? data.Explanation.ConvertToFullWidth() : "");
                    break;
                }
            }
        }

        // 連続移動
        if(pushFlag != 0)
        {
            timer += Time.deltaTime;
            switch (timer)
            {
                case >= 0.5f:
                {
                    mTime++;
                    switch (mTime)
                    {
                        case >= moveTimer:
                            mTime = 0;
                            MoveCursor(pushFlag == 1 ? -1 : 1, 0);
                            break;
                    }

                    break;
                }
            }
        }
        */
    }

    public void Exit()
    {
    }

    public void SelectMenu()
    {
        var data = PlayerDataRepository.Instance.GetItemList(selectedIndex);
        if(data != null)
        {
            switch (data.Effect)
            {
                case "recovery":
                case "Weapon":
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
                _menu.ChangeMenu(MenuList.Main);
                break;
            case Now_Mode.Battle:
                Initialize();
                WindowObj.transform.gameObject.SetActive(false);
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
        MenuManager.Instance.nowSelect = MenuList.Shop_Tool;
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
                PlayerDataRepository.Instance.GetItem(iData,data.Item2);        
            }
            else
            {
                var wData = WeaponArmorMaster.Entity.GetWeaponData(data.Item1);
                if(wData != null)
                    PlayerDataRepository.Instance.GetItem(wData.dataParam,data.Item2);
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
    public void Entry()
    {
        WindowObj = MenuManager.Instance.GetWindow(State);
        if (WindowObj is not null)
        {
            WindowObj.transform.gameObject.SetActive(true);
            SetCursorObject(MenuManager.Instance.GetCursorObj(State), State);
            selectedIndex = veriIndex * 2 + horiIndex;
        }

        // 1 / (プレイヤーのアイテム総数 / 2) - 画面のアイテム表示数
        WindowObj.ResetItemData();
        foreach (var item in PlayerDataRepository.Instance.ItemList)
        {
            WindowObj.CreateItemData(item.Value);
        }

        // 一番初めのアイテムを表示
        var data = PlayerDataRepository.Instance.GetItemList(selectedIndex);
        switch (GameManager.Instance.mode)
        {
            // バトル中は説明なし
            case Now_Mode.Field:
                WindowObj.SetExpText(data != null ? data.Explanation.ConvertToFullWidth() : "");
                break;
        }
    }

    public void Update()
    {
        throw new NotImplementedException();
    }

    public void Exit()
    {
        throw new NotImplementedException();
    }

    public void SelectMenu()
    {
        throw new NotImplementedException();
    }

    public void CloseMenu()
    {
        Initialize();
        MenuManager.Instance.prevSelect = State;
    }

    public void CursorUp()
    {
        throw new NotImplementedException();
    }

    public void CursorDown()
    {
        throw new NotImplementedException();
    }

    public void CursorRight()
    {
        throw new NotImplementedException();
    }

    public void CursorLeft()
    {
        throw new NotImplementedException();
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
            PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.Hand1.dataParam.Name,
            PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.Hand2.dataParam.Name,
            PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.Head.dataParam.Name,
            PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.Body.dataParam.Name
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
        var oneDigitItems = PlayerDataRepository.Instance.ItemList
            .Where(item => item.Value.ID is >= 0 and < 10)
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
        
        foreach (var item in oneDigitItems)
        {
            if (WeaponArmorMaster.Entity.GetWeaponData(item.Value.ID).equipment == part)
            {
                int num = PlayerDataRepository.Instance.GetWeaponArmorSetCount(part, item.Value.ID);
                if (num >= item.Value.num) return;
                itemCounter++;
                WindowObj.CreateItemData(item.Value);
                weaponData.Add(WeaponArmorMaster.Entity.GetWeaponData(item.Value.ID));
            }
        }

        // 一番初めのアイテムを表示
        var data = PlayerDataRepository.Instance.GetItemList(selectedIndex);
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
        string hand1 = PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.Hand1.dataParam.Name;
        string hand2 = PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.Hand2.dataParam.Name;
        string head = PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.Head.dataParam.Name;
        string body = PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.Body.dataParam.Name;
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
            PlayerDataRepository.Instance.PlayerState.SetWaeponArmor(part, new InfoWeaponArmor());
            WindowObj.equData.Parameter.SetBeAFParameter(
                PlayerDataRepository.Instance.SelectIndex,
                new Parameter(),
                PlayerDataRepository.Instance.PlayerState.WeaponArmorEauip.GetEquipmentUpParameter(part));
        }
        else
        {
            PlayerDataRepository.Instance.PlayerState.SetWaeponArmor(part,WeaponArmorMaster.Entity.GetWeaponData(weaponData[selectedIndex - 1 ].dataParam.ID));
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

