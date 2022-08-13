using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UMA.CharacterSystem;
using UnityEngine;
using UnlockGames.BA.NPC;
using UnlockGames.BA.Timeline;
using static UnlockGames.BA.MiniGames.DressingUp.UI.DressingUpOutOfNineUIPanel;

namespace UnlockGames.BA.MiniGames.DressingUp
{
    public class NpcClothesChanger : MonoBehaviour
    {
        [SerializeField] private NpcOnLevel character;
        [SerializeField] private DynamicCharacterAvatar _avatar;
        [SerializeField] private DressingUpDatabase _dressingUpDatabase;
        [SerializeField] private DressingUpViewModel _dressingUpViewModel;
        [SerializeField] private ClothesSlotVisibilityController _clothesSlotVisibilityController;
        
        private bool _isMainCharacter = true;
        
        public void SetAsMainCharacter(bool isMainCharacter)
        {
            _isMainCharacter = isMainCharacter;
        }

        private void OnEnable()
        {
            if (_avatar == null)
            {
                _avatar = GetComponentInChildren<DynamicCharacterAvatar>();
            }
            Subscribe();
            LoadClothesPreset();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _dressingUpViewModel.PreviewSelectedClothesElement += WearClothes;
            _dressingUpViewModel.DressingUpChoiceValidated += SaveClothesPreset;
            _dressingUpViewModel.OnQuestEnded += OnQuestEnded;
            _dressingUpViewModel.CancelChosenClothesElements += OnCancelSelectedClothesElements;
            _clothesSlotVisibilityController.ShowSlot += OnShowSlot;
            _clothesSlotVisibilityController.HideSlot += OnHideSlot;
        }

        private void Unsubscribe()
        {
            _dressingUpViewModel.PreviewSelectedClothesElement -= WearClothes;
            _dressingUpViewModel.DressingUpChoiceValidated -= SaveClothesPreset;
            _dressingUpViewModel.OnQuestEnded -= OnQuestEnded;
            _dressingUpViewModel.CancelChosenClothesElements -= OnCancelSelectedClothesElements;
            _clothesSlotVisibilityController.ShowSlot -= OnShowSlot;
            _clothesSlotVisibilityController.HideSlot -= OnHideSlot;
        }
        
        public void WearClothes(string clothesElementArticyId, ClothesType clothesType)
        {
            ClothesElementData clothesElementData = new ClothesElementData(_dressingUpDatabase.clothesSynced[clothesElementArticyId]);
            
            _avatar.SetSlot(clothesElementData.wardrobeRecipe);
            _avatar.BuildCharacter();

            CheckClothesAndLiftCharacterIfNeeded();
        }
        
        public void TakeOffClothesBySlotName(string slotName)
        {
            _avatar.ClearSlot(slotName);
            _avatar.BuildCharacter();

            // probably not needed since this functionality now is only for bag
            CheckClothesAndLiftCharacterIfNeeded();
        }

        private void ClearAllClothes()
        {
            _avatar.ClearSlots();
            _avatar.BuildCharacter();
        }

        public void LoadClothesPreset()
        {
            string filePath = $"{Application.persistentDataPath}/{_avatar.loadPath}/{_avatar.loadFilename}";

            if (File.Exists(filePath))
            {
                Debug.Log($"<b><color=#b5f719>[NpcClothesChanger: Loaded clothes preset]</color></b>");
                _avatar.DoLoad();
            }
            else
            {
                Debug.Log($"<b><color=#b5f719>[NpcClothesChanger: No clothes presets to load yet, loading default recipe]</color></b>");
                _avatar.LoadDefaultWardrobe();
            }
            _avatar.BuildCharacter();
            CheckClothesAndLiftCharacterIfNeeded();
        }
        
        public void SaveClothesPreset()
        {
            if (!_isMainCharacter)
            {
                return;
            }
            
            Debug.Log($"<b><color=#b5f719>[NpcClothesChanger.SaveClothesPreset()]</color></b>");
            PerformClothesSave();
        }

        private void PerformClothesSave()
        {
            // filePath is empty because DynamicCharacterAvatar.DoSave() in that case will use path stored in the component
            _avatar.DoSave(false, "", DynamicCharacterAvatar.SaveOptions.useDefaults);
        }
        
        private void OnQuestEnded()
        {
            CheckClothesAndLiftCharacterIfNeeded(true);
        }

        private void OnCancelSelectedClothesElements()
        {
            _avatar.ClearSlots();
            LoadClothesPreset();
        }

        #region Lift npc
        
        private void CheckClothesAndLiftCharacterIfNeeded(bool delayedLift = false)
        {
            bool liftHasBeenDone = false;
            foreach (var clothes in _avatar.WardrobeRecipes.Values)
            {
                foreach (var clothesItem in _dressingUpDatabase.clothesSynced.Values)
                {
                    if (clothesItem.wardrobeRecipe == clothes && clothesItem.clothesLiftUpOffset > 0)
                    {
                        LiftNpc(character, clothesItem.clothesLiftUpOffset, delayedLift);
                        liftHasBeenDone = true;
                        Debug.Log($"<b><color=#6ffaaa>[{character.npc.name} has been lifted up to {clothesItem.clothesLiftUpOffset} due to clothes item {clothesItem.name} requirement]</color></b>");
                        break;
                    }
                }
                if(liftHasBeenDone)
                    break;
            }

            if (!liftHasBeenDone)
            {
                LiftNpc(character, 0f, delayedLift);
            }
        }

        private async void LiftNpc(NpcOnLevel npc, float liftAmount, bool delayedLift = false)
        {
            // delayed lift is to ensure that timeline won't lock position of Transform with animator
            if (delayedLift)
            {
                await UniTask.DelayFrame(1, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            }
            
            npc.npcAnimator.BodyAnimator.transform.localPosition = new Vector3(0, liftAmount, 0);
        }
        
        #endregion
        
        private void OnShowSlot(string _)
        {
            // we know the slot name we must restore, but it's better to "reload"
            // clothes then just to search for required clothes slot
            LoadClothesPreset();
        }

        private void OnHideSlot(string slotName)
        {
            TakeOffClothesBySlotName(slotName);
        }
    }
}

