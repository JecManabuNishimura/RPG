using System.Collections.Generic;

namespace UI
{
    public class MenuContext
    {
        private IMenuState _currentState;
        private IMenuState _previousState;

        private Dictionary<MenuList, IMenuState> _stateTable;

        public void Init(MenuController menu, MenuList initState)
        {
            if (_stateTable is not null) return;

            Dictionary<MenuList, IMenuState> table = new()
            {
                { MenuList.None, new MenuStateNone(menu) },
                { MenuList.Main, new MenuStateMain(menu) },
                { MenuList.SelectChara, new MenuStateSelectCharacter(menu) },
                { MenuList.Strength, new MenuStateStrength(menu) },
                { MenuList.Battle, new MenuStateBattle(menu) },
                { MenuList.Battle_Enemy, new MenuStateBattle_EnemySelect(menu) },
                { MenuList.Battle_Result, new MenuStateBattle_Result(menu) },
                { MenuList.ItemList, new MenuStateItemList(menu) },
                { MenuList.Shop_Tool, new ShopMenu_Tool(menu) },
                { MenuList.Shop_Buy, new ShopBuyMenu(menu) },
                { MenuList.Shop_Weapon, new ShopMenu_Weapon(menu) },
                { MenuList.EquipmentMenu, new EquipmentMenu(menu) },
                { MenuList.EquipmentMenu1, new EquipmentMenu1(menu) },
                { MenuList.EquipmentMenu2, new EquipmentMenu2(menu) },
            };
            _stateTable = table;
            _currentState = _stateTable[initState];
            ChangeState(initState);
        }
        public void ChangeState(MenuList next)
        {
            if (_stateTable is null) return;
            if (_currentState is null || _currentState.State == next)
            {
                return;
            }

            var nextState = _stateTable[next];
            _previousState = _currentState;
            SoundMaster.Entity.PlaySESound(PlaceOfSound.MenuSelect);
            _previousState?.Exit();
            _currentState = nextState;
            
            _currentState.Entry();
        }
        public void Update() => _currentState?.Update();
        public void SelectMenu() => _currentState?.SelectMenu();
        public void CloseMenu() => _currentState?.CloseMenu();
        
        public void CursorUp() => _currentState?.CursorUp();
        public void CursorDown() => _currentState?.CursorDown();
        public void CursorRight() => _currentState?.CursorRight();
        public void CursorLeft() => _currentState?.CursorLeft();
    }
}
