using UnityEngine;
using UnityEngine.Playables;
using System;
using System.Collections.Generic;
using System.Linq;
using PixelCrushers.DialogueSystem;
using UnityEngine.Timeline;

[Serializable]
public class PlayBarkSequenceClip : PlayableAsset, ITimelineClipAsset
{
    public PlayBarkSequenceBehaviour template = new PlayBarkSequenceBehaviour();
    public ExposedReference<Transform> listener;

    private PlayBarkSequenceTrack sequenceTrack;

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var timeline = owner.GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
        var playable = ScriptPlayable<PlayBarkSequenceBehaviour>.Create(graph, template);
        PlayBarkSequenceBehaviour instance = playable.GetBehaviour();
        foreach (var track in timeline.GetOutputTracks())
        {
            if (track is PlayBarkSequenceTrack)
            {
                sequenceTrack = track as PlayBarkSequenceTrack;
                if (Application.isPlaying)
                {
                    foreach (var clip in sequenceTrack.GetClips())
                    {
                        var asset = clip.asset as PlayBarkSequenceClip;
                        if (asset.template.id == template.id)
                        {
                            instance.timeSpan = (float) clip.duration;
                            break;
                        }
                    }
                }
            }
        }

        instance.listener = listener.Resolve(graph.GetResolver());

        return playable;
    }
}