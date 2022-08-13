using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Playables;

public class PlayBarkSequenceMixerBehaviour : PlayableBehaviour
{
    private HashSet<int> played = new HashSet<int>();

    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        GameObject trackBinding = playerData as GameObject;
        Transform speaker = (trackBinding != null) ? trackBinding.transform : null;
        int inputCount = playable.GetInputCount();

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            if (inputWeight > 0.001f && !played.Contains(i))
            {
                played.Add(i);
                ScriptPlayable<PlayBarkSequenceBehaviour> inputPlayable =
                    (ScriptPlayable<PlayBarkSequenceBehaviour>) playable.GetInput(i);
                PlayBarkSequenceBehaviour input = inputPlayable.GetBehaviour();
                var sequence = input.sequence;
                if (Application.isPlaying)
                {
                    // We need current conversation actor to specify bark UI position near actor's bone.
                    var currentEntry = DialogueManager.instance.currentConversationState.subtitle.dialogueEntry;
                    var dialogueActor = DialogueActor.GetDialogueActorComponent(DialogueManager.currentActor);
                    
                    // Before "Continue()" sequence applied the current entry contains a link to the bark entry.
                    // The entry is a bark entry if there is only one link from current. In other case the entry is
                    // either a Player's choice or the conversation end.
                    if (currentEntry.outgoingLinks.Count == 1)
                    {
                        var targetEntry =
                            DialogueManager.databaseManager.defaultDatabase.GetDialogueEntry(
                                currentEntry.outgoingLinks[0]);
                        var actorId = targetEntry.ActorID;
                        dialogueActor = DialogueActor.GetDialogueActorComponent(DialogueManager.conversationModel
                            .GetCharacterInfo(actorId).transform);
                    }

                    if (dialogueActor != null)
                    {
                        var barkUI = DialogueActor.GetBarkUI(DialogueManager.currentActor) as MultiPanelBarkUI;
                        if (barkUI != null)
                        {
                            barkUI.UIPanels[input.barkUiPanelNumber].holder.useCustomSettings = false;
                            barkUI.UIPanels[input.barkUiPanelNumber].holder.customSettings.hideTrigger = "Hide";
                            barkUI.UIPanels[input.barkUiPanelNumber].holder.customSettings.showTrigger = "Show";
                            barkUI.duration = input.timeSpan;
                            barkUI.SwitchPanel(input.barkUiPanelNumber);
                            RectTransform barkRect =  barkUI.UIPanels[input.barkUiPanelNumber].transform as RectTransform;

                            if (input.barkOffsetType == BarkOffsetType.PIXEL)
                            {
                                barkRect.anchorMax = new Vector2(0.5f, 1f);
                                barkRect.anchorMin = new Vector2(0.5f, 1f);
                                barkRect.anchoredPosition = barkUI.UIPanels[input.barkUiPanelNumber].holder.startPosition +
                                                            input.barkUiOffset;
                            }
                            else
                            {
                                barkRect.anchorMax = new Vector2(0.5f, 0f);
                                barkRect.anchorMin = new Vector2(0.5f, 0f);
                                var worldPosition = dialogueActor.barkUISettings.barkUIPivot.position +
                                                    (input.barkOffsetType == BarkOffsetType.CUSTOM_WORLD ? input.barkOffset : dialogueActor.barkUISettings.barkUIOffset);
                                barkRect.anchoredPosition = new Vector2(barkRect.anchoredPosition.x, Camera.main.WorldToScreenPoint(worldPosition).y);
                            }
                        }

                    }
                    
                    DialogueManager.PlaySequence(sequence, speaker, input.listener);
                }
                else
                {
                    PreviewUI.ShowMessage(sequence, 3, -1);
                }
            }
            else if (inputWeight <= 0.001f && played.Contains(i))
            {
                played.Remove(i);
            }
        }
    }

    public override void OnGraphStart(Playable playable)
    {
        base.OnGraphStart(playable);
        played.Clear();
    }

    public override void OnGraphStop(Playable playable)
    {
        base.OnGraphStop(playable);
        played.Clear();
    }
}