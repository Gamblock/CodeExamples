using UnityEngine;
using UnityEngine.Playables;
using System;
using Sirenix.OdinInspector;
using UnityEngine.Timeline;

public enum BarkOffsetType
{
     DEFAULT_WORLD,
     CUSTOM_WORLD,
     PIXEL
}

[Serializable]
public class PlayBarkSequenceBehaviour : PlayableBehaviour
{
     [ReadOnly]
     public string id = Guid.NewGuid().ToString();

     [Tooltip("DEFAULT_WORLD -- Use bark offset from DialogueActor component in world coordinates\n" +
              "CUSTOM_WORLD -- Specify bark offset from current actor in world coordinates\n" +
              "PIXEL -- Specify bark offset from Canvas top in pixels")]
     public BarkOffsetType barkOffsetType = BarkOffsetType.PIXEL;
     
     [ShowIf("barkOffsetType", BarkOffsetType.PIXEL)]
     public Vector3 barkUiOffset;
     
     [ShowIf("barkOffsetType", BarkOffsetType.CUSTOM_WORLD)]
     public Vector3 barkOffset = new Vector3(0, 0, 0);
     
     public int barkUiPanelNumber;
     
     [Tooltip("Play this sequence.")]
     [TextArea(5, 5)]
     public string sequence;
     [Tooltip("(Optional) The other subject in the sequence.")]
     public Transform listener;
     [HideInInspector]
     public float timeSpan = 0;

}
