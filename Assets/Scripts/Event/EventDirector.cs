using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class EventDirector : MonoBehaviour
{
    private static EventDirector _instance;
    public static EventDirector Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EventDirector>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("EventDirector");
                    _instance = obj.AddComponent<EventDirector>();
                }
            }
            return _instance;
        }
    }

    private PlayableDirector _director;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // シングルトンを破棄しない
        }
        else
        {
            Destroy(gameObject); // 既に存在する場合は破棄
        }

        _director = GetComponent<PlayableDirector>();
    }

    public void Play(TimelineAsset asset)
    {
        _director.playableAsset = asset;
        TimelineAsset timelineAsset = (TimelineAsset)_director.playableAsset;
        var tracks = timelineAsset.GetOutputTracks();
        TransformTweenTrack targetTrack = null;
        foreach (var track in tracks)
        {
            if (track is TransformTweenTrack)
            {
                targetTrack = (TransformTweenTrack)track;
                break;
            }
        }

        if (targetTrack != null)
        {
            _director.SetGenericBinding(targetTrack,GameManager.Instance.PlayerObj.transform);
            
        }

        _director.stopped += OnPlayableDirectorStopped;
        _director.Play();
        
    }
    void OnPlayableDirectorStopped(PlayableDirector director)
    {
        GameManager.Instance.eventFlag = false;
        _director.stopped -= OnPlayableDirectorStopped;
    }
    public void Play() => _director!.Play();
    public void Stop() => _director.Stop();
    public void Pause() => _director.Pause();
    
}
