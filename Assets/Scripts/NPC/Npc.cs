using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Npc : MonoBehaviour,INpc
{
    public NPCType npcType;
    public int id;
    public bool isEnd = true;
    public ItemShopData data;

    public void Talking()
    {
        switch (npcType)
        {
            case NPCType.ToolShop:
            case NPCType.WeaponShop:
                GameManager.Instance.state = Now_State.Shop;
                NPCManager.Instance.nowTalkItemShopData = data;
                MenuManager.Instance.OpenMenu(npcType == NPCType.ToolShop?  MenuList.Shop_Tool : MenuList.Shop_Weapon);
                isEnd = false;
                break;
            case NPCType.Villager:
                if(isEnd)
                {
                    MessageManager.Instance.ClearDialogMessage();
                    isEnd = false;
                    StartCoroutine(StartTolk());
                }        
                break;
        }
        
        
    }

    private IEnumerator StartTolk()
    {
		
		yield return new WaitForEndOfFrame();
        var foundSerif = NpcSerifEditor.Entity.serifData.FirstOrDefault(_ => _.id == id);
        if(foundSerif == null)
        {
			MessageManager.Instance.StartDialogMessage("アイデーがみつかりませんでした。"+id.ToString().ConvertToFullWidth());
		}
        else
        {
			MessageManager.Instance.StartDialogMessage(foundSerif.serif);
		}
		
        yield return new WaitUntil(() => MessageManager.Instance.IsEndMessage);
        isEnd = true;
        // 状態リセット
		GameManager.Instance.mode = Now_Mode.Field;
		GameManager.Instance.state = Now_State.Active;
	}
}
