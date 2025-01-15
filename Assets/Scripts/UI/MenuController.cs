using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class MenuController : MonoBehaviour
    {
        public GameObject selectMenuObj;

        private MenuContext _context;
        public List<MenuObj> menuObjs = new ();

        public List<MenuObj> BattleMenuObjs = new();
        
        public BattleCursor _BattleCursor;

        private void Start()
        {
            _context = new MenuContext();
            _context.Init(this,MenuList.None);
            MenuManager.Instance._menuController = this;
            MenuManager.Instance.menuObj = selectMenuObj;
            selectMenuObj.SetActive(false);
        }

        public GameObject GetSelectMenuObj()
        {
            return selectMenuObj;
        }

        void Update() => _context.Update();
        public void SelectMenu() => _context.SelectMenu();
        public void CloseMenu() => _context.CloseMenu();
        public void ChangeMenu(MenuList next) => _context.ChangeState(next);

        public void CursorUp() => _context.CursorUp();
        public void CursorDown()=> _context.CursorDown();
        public void CursorRight()=> _context.CursorRight();
        public void CursorLeft()=> _context.CursorLeft();

    }
}
