using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.LevelLogic.Dressing;
using ComfortGames.CharacterCustomization;
using UnityEngine;
using UnityEngine.Playables;

public class DressingLevelHolder : MonoBehaviour
{
    public CharacterAppearanceController characterAppearance;
    public CharacterAppearanceAnimator characterAppearanceAnimator;
    public AppearanceParticleController particleController;
    public CharacterRotator characterRotator;
    public string customizationLevelOutfitsPath;
    public List<ClothesCategory> clothesCategories;
    public PlayableDirector playableDirector;
    public SkinnedMeshRenderer playerBody;
    public DressingViewModelSO ViewModelSo;

    private void Start()
    {
        if (ViewModelSo != null && playerBody != null)
        {
            ViewModelSo.currentBody = playerBody;
        }
    }

    public void Initialize(ClothesGroupConfigSO clothesConfig, StorageSO storage)
    {
        characterAppearance.Initialize(clothesConfig, storage, customizationLevelOutfitsPath);
    }

    public void InitializeSelectionView(OutfitSelectionView selectionView)
    {
        selectionView.Initialize(clothesCategories);
    }
}
