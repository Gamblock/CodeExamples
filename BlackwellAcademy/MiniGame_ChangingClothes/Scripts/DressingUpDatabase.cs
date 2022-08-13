using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnlockGames.BA.AnimationControl;
using UnlockGames.BA.CameraControl;
using UnlockGames.BA.Core.DialogueSystem;

namespace UnlockGames.BA.MiniGames.DressingUp
{
    [CreateAssetMenu(menuName = "Game/DressingUp/DressingUpDatabase")]
    public class DressingUpDatabase : ScriptableObject
    {
        [SerializeField] private ExpositionCameraController _cameraController;
        [SerializeField] private AnimationController _animationController;
        
        private const string FieldTitle_IsClothesElement = "Is_Clothes";
        private const string FieldTitle_IsClothesPremium = "Is_Clothes_Premium";
        private const string FieldTitle_ClothesPremiumCost = "Clothes_Premium_Cost";
        private const string FieldTitle_AnimationReaction = "AnimationReaction";
        private const string FieldTitle_AnimationReactionDuration = "AnimationReactionDuration";
        private const string FieldTitle_ClothesLiftUpOffset = "ClothesLiftOffset";
        
        [Header("Database fields")]
        [Tooltip("Fill in this list, once game is launched, successfully synced elements will fall into the dictionary below")]
        [ShowInInspector] public List<ClothesElement> clothesList = new List<ClothesElement>();
        [Space(5f)]
        [Tooltip("Verified ClotheElement's fall into this dictionary")]
        [ShowInInspector] public Dictionary<string, ClothesElement> clothesSynced = new Dictionary<string, ClothesElement>();

        public void InitializeDatabase()
        {
            foreach (var clothesElement in clothesList)
            {
                clothesElement.ResetVerified();
            }

            var articyItemClothesElements = LuaExtended.FindItemsByProperty(FieldTitle_IsClothesElement);
            Debug.Log($"<b><color=#12f6f1>[DressingUpMiniGameController - Found {articyItemClothesElements.Count} elements in articy database]</color></b>");
            
            foreach (var articyItem in articyItemClothesElements)
            {
                ArticyItemData articyItemData = new ArticyItemData(articyItem);

                var clothesElement = clothesList.FirstOrDefault(x => x.articyId == articyItemData.articyId);
                
                if (clothesElement == null)
                {
                    Debug.LogError($"Didn't find clothes item in project, relatable to ArticyDatabase item, searched by ArcityId ({articyItemData.articyId})");
                    continue;
                }

                InitializeClothesElementValues(clothesElement, articyItem);

                bool resultAdd = clothesSynced.TryAdd(articyItemData.articyId, clothesElement);
                if (!resultAdd)
                {
                    Debug.LogWarning($"For some reason, couldn't copy clothesItem from clothesList to clothesSynced ({articyItemData.articyId})");
                }
            }
        }
        
        private void InitializeClothesElementValues(ClothesElement clothesElement, Item articyItem)
        {
            clothesElement.lastRevisionVerified = true;

            clothesElement.isPremium = Convert.ToBoolean(articyItem.fields.FirstOrDefault(a => a.title == FieldTitle_IsClothesPremium).value);
            if (clothesElement.isPremium)
            {
                clothesElement.premiumCost = Int32.Parse(articyItem.fields.FirstOrDefault(a => a.title == FieldTitle_ClothesPremiumCost).value);
                if (clothesElement.premiumCost <= 0)
                {
                    Debug.LogError($"Clothes item is marked as 'premium' but has cost '{clothesElement.premiumCost}', making it non-premium explicitly");
                    clothesElement.isPremium = false;
                }
            }
            else
            {
                clothesElement.premiumCost = 0;
            }

            string cameraPresetArticyId = articyItem.fields.FirstOrDefault(a => a.title == ExpositionCameraController.FieldTitleCameraPresets).value;
            clothesElement.cameraPreset = _cameraController.GetCameraPreset(cameraPresetArticyId);

            AnimationControl.Animation animationReaction = _animationController.GetAnimationByArticyId(articyItem.fields
                    .FirstOrDefault(a => a.title == FieldTitle_AnimationReaction).value);
            clothesElement.animationReaction = animationReaction;
            
            string animationTimeToPlayStr = articyItem.fields
                .FirstOrDefault(a => a.title == FieldTitle_AnimationReactionDuration).value;
            clothesElement.animationReactionDuration = Single.Parse(animationTimeToPlayStr, CultureInfo.InvariantCulture);
            
            string clothesLiftUpOffsetStr = articyItem.fields
                .FirstOrDefault(a => a.title == FieldTitle_ClothesLiftUpOffset).value;
            clothesElement.clothesLiftUpOffset = Single.Parse(clothesLiftUpOffsetStr, CultureInfo.InvariantCulture);
        }
    }
}