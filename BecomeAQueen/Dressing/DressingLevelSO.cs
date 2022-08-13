using System;
using System.Collections;
using System.Collections.Generic;
using ComfortGames.CharacterCustomization;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "DressingLevelSO", menuName = "_Game/Levels/DressingLevelSO", order = 0)]
public class DressingLevelSO : LevelBaseSO
{
    public DressingLevelHolder levelPrefab;
    public ClothesGroupConfigSO clothingGroupConfig;
    public List<StorageSO.ClothesElement> customStartOutfit;
    public string conversationName;
    
    public VipSet vipSet;

    public override string GetSceneName() => "Dressing";
}


[Serializable]
public struct VipSet
{
    public string textVipSet;
    public Image imageVipSet;
}
