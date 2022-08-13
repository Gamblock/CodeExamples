using System;
using System.Collections.Generic;
using System.Linq;
using ComfortGames.CharacterCustomization;
using UnityEngine;

public class CharacterAppearanceController : MonoBehaviour
{
    public StringEventChannelSO onMakeupApplied;
    public StringEventChannelSO onHairColorApplied;
    public CharacterBuilder characterBuilder;
    public FaceMakeUpController makeUpController;
    public HairColorController hairColorController;

    private List<OutfitScriptableObject> equippedOutfitsList = new List<OutfitScriptableObject>();
    public void Initialize(ClothesGroupConfigSO clothesGroupConfig, StorageSO storage, string customizationLevelOutfitsPath = null)
    {
        SetDefaultOutfit(clothesGroupConfig, storage);
        
        characterBuilder.Initialize(
            clothesGroupConfig.categoriesPath,
            clothesGroupConfig.collisionMatrix,
            equippedOutfitsList,
            customizationLevelOutfitsPath,
            storage);
        
        makeUpController.Initialize(clothesGroupConfig);
        makeUpController.SetMakeUpById(storage.MakeUpGuid);
        
        InitializeHairColorController();
        hairColorController.InitializeHairColors(clothesGroupConfig);
        hairColorController.SetHairColorByGuid(storage.HairColorGuid);
    }

    private void OnEnable()
    {
        if (onMakeupApplied != null && onHairColorApplied != null)
        {
            onMakeupApplied.OnEventRaised += SetMakeupByGuid;
            onHairColorApplied.OnEventRaised += SetHairColorByGuid;
        }
        
    }

    private void OnDisable()
    {
        if (onMakeupApplied != null && onHairColorApplied != null)
        {
            onMakeupApplied.OnEventRaised -= SetMakeupByGuid;
            onHairColorApplied.OnEventRaised -= SetHairColorByGuid;
        }
    }

    public void InitializeHairColorController()
    {
        var hairHolder = characterBuilder.GetComponentInChildren<HairRendererHolder>();
        if (hairHolder == null)
        {
            return;
        }
        hairColorController.InitializeMaterial(hairHolder.hairRenderer);
    }

    private void SetMakeupByGuid(string guid)
    {
        makeUpController.SetMakeUpById(guid);
    }
    private void SetHairColorByGuid(string guid)
    {
        InitializeHairColorController();
        hairColorController.SetHairColorByGuid(guid);
    }
    public List<CustomizationElementSO> GetSuitableHairColors(StorageSO storage)
    {
        var hairShapeGuid = storage.characterOutfit[ClothesCategory.HAIR];
        return hairColorController.hairColors
            .FindAll(el => el.hairSO.outfitElementGuid == hairShapeGuid)
            .Select(el => el as CustomizationElementSO)
            .ToList();
    }

    private void SetDefaultOutfit(ClothesGroupConfigSO clothesGroupConfig, StorageSO storage)
    {
        var outfits = new List<OutfitScriptableObject>();
        foreach (var path in clothesGroupConfig.outfitsPaths)
        {
            outfits.AddRange(CharacterCustomizationAssetManager.GetOutfitScriptableObjects(path).ToList());
        }
        
        var categories = 
            CharacterCustomizationAssetManager.GetOutfitCategoryScriptableObject(clothesGroupConfig.categoriesPath).ToList();

        foreach (var item in storage.characterOutfit)
        {
            var category = categories.Find(el => el.clothesCategory == item.Key);
            if (category == null)
            {
                continue;
            }
            
            var outfit = outfits.Find(el => el.outfitElementGuid == item.Value);
            
            category.defaultOutfitScriptableObject = outfit;
            if (outfit != null)
            {
                equippedOutfitsList.Add(outfit);
            }
        }
    }
    
    
}
