using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuData
{
    protected Vector3 InitPos;
    protected int selectedIndex;

    protected int veriIndex;
    protected int horiIndex;
    protected Vector3 offsetCursor;
    
    private GameObject _cursor;
    private float _xOffset;
    private float _yOffset;

    protected MenuObj WindowObj;
    
    public void SetCursorObject(GameObject obj,MenuList menu)
    {
        _cursor = obj;
        if (MenuManager.Instance.cursorPos.Count != 0 && MenuManager.Instance.cursorPos.Peek().Item1 == menu)
        {
            offsetCursor = WindowObj.tranInitPos;
            (_,veriIndex,horiIndex) = MenuManager.Instance.cursorPos.Pop();
        }
        else
        {
            offsetCursor = _cursor.transform.position;
            veriIndex = 0;
        }
    }

    public void InitCursor()
    {
        offsetCursor = WindowObj.tranInitPos;
        veriIndex = 0;
        _cursor.transform.position = offsetCursor;
    }

    public void SetCursorActive(bool flag)
    {
        _cursor.SetActive(flag);
    }

	protected void SetActionCommand(int number)
	{
		switch (number)
		{
			case 0:             // たたかう
				PlayerDataRepository.Instance.PlayerState.ActionCommand = "たたかう";
				break;
			case 2:             // 防御
				PlayerDataRepository.Instance.PlayerState.ActionCommand = "ぼうぎょ";
                PlayerDataRepository.Instance.PlayerState.parameter.DefFlag = true;
				break;
            case 3:             // 道具
                PlayerDataRepository.Instance.PlayerState.ActionCommand = "アイテム";
                break;
		}
	}

	public void Initialize()
    {
        veriIndex = 0;
        horiIndex = 0;
        selectedIndex = 0;
        _cursor.GetComponent<RectTransform>().anchoredPosition  = WindowObj.anchorInitPos; 
    }
    public void UpdateCursorPosition(float x,float y)
    {
        // カーソルの移動処理を実装
        _cursor.transform.position = new Vector3(offsetCursor.x + x, offsetCursor.y - y, _cursor.transform.position.z);
        SoundMaster.Entity.PlaySESound(PlaceOfSound.CursorMove);
    }

    public void UpdateCursorPosition(Vector3 pos)
    {
        _cursor.transform.position = pos;
    }

    public Vector3 GetCursorPosition()
    {
        return _cursor.transform.position;
    }

    
    
    protected virtual void MoveCursor(int verticalDirection, int horizontalDirection)
    {
        
    }

}
