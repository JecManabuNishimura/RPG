using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class MyCustomClip : PlayableAsset, ITimelineClipAsset
{
    public EventPlaybleBehaviour template = new EventPlaybleBehaviour();
    public DialogueEvent Dialogue;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<EventPlaybleBehaviour>.Create(graph, template);
        EventPlaybleBehaviour behaviour = playable.GetBehaviour();
        behaviour.Dialogue = Dialogue;
        return playable;
    }

    public ClipCaps clipCaps => ClipCaps.None;
}