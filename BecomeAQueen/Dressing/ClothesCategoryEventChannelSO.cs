using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/ClothesCategoryEventChannel")]
public class ClothesCategoryEventChannelSO : ScriptableObject
{
  public UnityAction<ClothesCategory> OnEventRaised = category => { };
  public void RaiseEvent(ClothesCategory value)
  {
    OnEventRaised.Invoke(value);
  }
}