using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ReadEventData
{
    public static ReadEventData Instance;

    public async void ReadEvent(DialogueEvent eventData)
    {
        EventDirector.Instance.Pause();
        foreach (var list in eventData.data)
        {
            MessageManager.Instance.ClearDialogMessage();
            MessageManager.Instance.SetCharaName(list.name);
            MessageManager.Instance.StartDialogMessage(list.messages);
            await WaitForMessageEndAsync();
        }
        await WaitForMessageEndAsync();
        GameManager.Instance.mode = Now_Mode.Field;
        GameManager.Instance.state = Now_State.Active;
        EventDirector.Instance.Play();
    }
    async Task WaitForMessageEndAsync()
    {
        while (!MessageManager.Instance.IsEndMessage)
        {
            await Task.Yield();
        }
    }
    
}
