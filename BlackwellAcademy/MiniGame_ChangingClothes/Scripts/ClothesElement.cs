using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UMA.CharacterSystem;
using UnityEngine;
using UnlockGames.BA.CameraControl;

namespace UnlockGames.BA.MiniGames.DressingUp
{
    [CreateAssetMenu(menuName = "Game/DressingUp/ClothesElement", fileName = "ClothesElement")]
    public class ClothesElement : ScriptableObject
    {
        [ReadOnly] public bool lastRevisionVerified;
        
        [Header("Set in Editor")]
        public UMAWardrobeRecipe wardrobeRecipe;
        public string articyId;
        public Sprite previewImage;
        public ClothesType clothesType;
        [Space(4f)] public float yAngleOnStage;
        
        [Header("Set from Articy")]
        // for convenience
        //public SlotDestination slotDestination;

        // read-only, set from articy
        public bool isPremium;
        public int premiumCost;
        public CameraPreset cameraPreset;
        public AnimationControl.Animation animationReaction;
        public float animationReactionDuration;
        public float clothesLiftUpOffset;

        
        // this is to make sure when we hit "Play" we can observe which ClothesElements are backed up by Articy
        public void ResetVerified()
        {
            lastRevisionVerified = false;
        }
    }
}

