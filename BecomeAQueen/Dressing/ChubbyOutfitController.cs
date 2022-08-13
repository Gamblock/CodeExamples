using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.LevelLogic.Dressing.Exam;
using DG.Tweening;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class FatOutfitController : MonoBehaviour
{
  public string fatCharacterID = "Story_02_FatCharacter.isFatCharacter";
  public string delayBeforeFatID = "Story_02_FatCharacter.delayBeforeFat";
  public FloatEventChannel OnBlendShapeChangeChannel;
  public List<SkinnedMeshRenderer> skinnedMeshRenderers;
  public DialogueIntegrationManager dialogueIntegrationManager;
  public float durationFatAnimation = 2f;
  
  private const float maxValueBlendShape = 100;
  private bool isStartFat;
  
  private void OnEnable()
  {
    OnBlendShapeChangeChannel.OnEventRaised += SetBlendShape;
    dialogueIntegrationManager.setCharacterFat.OnValueChanged += StartChangeBlendShape;

    StartChangeBlendShape(dialogueIntegrationManager.setCharacterFat.Value);
  }

  private void OnDisable()
  {
    OnBlendShapeChangeChannel.OnEventRaised -= SetBlendShape;
    dialogueIntegrationManager.setCharacterFat.OnValueChanged -= StartChangeBlendShape;
  }

  private void Start()
  {
    if (DialogueLua.GetVariable(fatCharacterID).asBool)
    {
      SetBlendShape(maxValueBlendShape);
    }
  }

  private void StartChangeBlendShape(bool isChangeBlendShape)
  {
    if (isStartFat || !isChangeBlendShape) return;
    isStartFat = isChangeBlendShape;
    DOTween.To(GetBlendShape, SetBlendShape, maxValueBlendShape, durationFatAnimation).SetDelay(DialogueLua.GetVariable(delayBeforeFatID).asInt);
  }
  
  private float GetBlendShape() => skinnedMeshRenderers[0].GetBlendShapeWeight(0);

  private void SetBlendShape(float valueBlendShape)
  {
    foreach (var mesh in skinnedMeshRenderers)
    {
      mesh.SetBlendShapeWeight(0, valueBlendShape);
    }
  }
}