using System.Collections;
using System.Collections.Generic;
using UMA.CharacterSystem;
using UnityEngine;
using UnlockGames.BA.CameraControl;

namespace UnlockGames.BA.MiniGames.DressingUp
{
    public class ClothesElementData
    {
        public string clothesElementArticyId;
        public Sprite previewImage;
        public bool isPremium;
        public int premiumCost;
        public UMAWardrobeRecipe wardrobeRecipe;
        public CameraPreset cameraPreset;
        public AnimationControl.Animation animationReaction;
        public float animationReactionDuration;
        public float clothesLiftUpOffset;
        public float yAngleOnStage;
        public ClothesType clothesType;

        public ClothesElementData(ClothesElement clothesElement)
        {
            clothesElementArticyId = clothesElement.articyId;
            previewImage = clothesElement.previewImage;
            isPremium = clothesElement.isPremium;
            premiumCost = clothesElement.premiumCost;
            wardrobeRecipe = clothesElement.wardrobeRecipe;
            cameraPreset = clothesElement.cameraPreset;
            animationReaction = clothesElement.animationReaction;
            animationReactionDuration = clothesElement.animationReactionDuration;
            clothesLiftUpOffset = clothesElement.clothesLiftUpOffset;
            yAngleOnStage = clothesElement.yAngleOnStage;
            clothesType = clothesElement.clothesType;
        }
    }
}