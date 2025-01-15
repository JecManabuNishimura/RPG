using System.Collections.Generic;
using UnityEngine;

public class EventPoint : MonoBehaviour
{
    public int eventNumber;
    [Event]
    [SerializeField] private List<int> RequiredEvent;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!EventMaster.Entity.GetEventFlag(eventNumber))
            {
                foreach (var re in RequiredEvent)
                {
                    if (EventMaster.Entity.GetEventFlag(re))
                    {
                        // 一つでも実行ノーチェックがある場合は実行しない
                        return;
                    }
                }

                GameManager.Instance.eventFlag = true;
                EventDirector.Instance.Play(EventMaster.Entity.GetTimelineAsset(eventNumber));
                EventMaster.Entity.CheckEvent(eventNumber);
            }
        }
    }
}
