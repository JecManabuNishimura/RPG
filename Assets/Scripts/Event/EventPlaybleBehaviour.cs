using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class EventPlaybleBehaviour : PlayableBehaviour
{
    public DialogueEvent Dialogue;
    private bool hasPlayed = false;  // フラグを用意
    // タイムライン開始時に呼ばれる
    public override void OnGraphStart( Playable playable )
    {
        
        
    }
    // タイムライン停止時に呼ばれる
    public override void OnGraphStop( Playable playable )
    {
    }
    // PlayableTrack再生時に呼ばれる
    public override void OnBehaviourPlay( Playable playable, FrameData info )
    {
        if (hasPlayed) return;
        if(ReadEventData.Instance != null)
        {
            ReadEventData.Instance.ReadEvent(Dialogue);
            hasPlayed = true;
        }
    }
    // PlayableTrack停止時に呼ばれる
    public override void OnBehaviourPause( Playable playable, FrameData info )
    {
    }
    // PlayableTrack再生時フレーム毎に呼ばれる
    public override void PrepareFrame(Playable playable, FrameData info)
    {
    }
}
