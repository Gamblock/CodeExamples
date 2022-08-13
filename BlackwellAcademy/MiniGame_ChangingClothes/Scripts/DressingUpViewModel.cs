using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnlockGames.BA.DialogueSystem.UI;
using static UnlockGames.BA.MiniGames.DressingUp.UI.DressingUpOutOfNineUIPanel;

namespace UnlockGames.BA.MiniGames.DressingUp
{
    [CreateAssetMenu(fileName = "DressingUpViewModel", menuName = "Game/DressingUp/DressingUpViewModel")]
    public class DressingUpViewModel : ScriptableObject
    {
        public event Action<DressingUpState> DressingUpStateChanged = _ => { };
        public event Action DressingUpChoiceValidated = () => { };
        public event Action<int> KeysAmountUpdated = (_) => { };
        public event Action<string, ClothesType> PreviewSelectedClothesElement = (s, type) => { };
        public event Action<StageNpcInstruction> StagingInstructionReceived = (_) => {}; // DTO
        public event Action<bool> ManageStageItemsInstructionReceived = (_) => {}; // DTO
        public event Action PlayerHasChosenClothesElement = () => {};
        public event Action CancelChosenClothesElements = () => {};
        public event Action<List<ClothesElementData>> ClothesElementsHaveBeenChosen = (_) => {};
        public event Action<List<ClothesElementData>> SelectedClothesElementsDataReceived = (_) => { }; // DTO
        public event Action OpenNotEnoughKeysPanel = () => { };
        public event Action<List<string>> DressingUpUIInitialized = _ => { };
        public event Action<List<ClothesElementData>> ClothesElementDataRetrieved = _ => { };
        public event Action OnQuestEnded = () => { };
        
        // this is for non-uma characters to change clothes presets (just enable/disable skinned-to-them clothes)
        public event Action<string, int> ChangeNpcClothesPreset = (npcArticyId, clothesIndex) => { };
        
        public void CallDressingUpStateChanged(DressingUpState dressingUpState)
        {
            DressingUpStateChanged.Invoke(dressingUpState);
        }

        public void CallDressingUpChoiceValidated()
        {
            DressingUpChoiceValidated.Invoke();
        }
        
        public void CallKeysAmountUpdated(int keysUmount)
        {
            KeysAmountUpdated.Invoke(keysUmount);
        }
        
        public void CallPreviewSelectedClothesElement(string clothesElement, ClothesType clothesType)
        {
            PreviewSelectedClothesElement.Invoke(clothesElement, clothesType);
        }
        
        public void CallStagingInstructionReceived(StageNpcInstruction stageNpcInstruction)
        {
            StagingInstructionReceived.Invoke(stageNpcInstruction);
        }
        
        public void CallManageStageItemsInstructionReceived(bool itemsVisible)
        {
            ManageStageItemsInstructionReceived.Invoke(itemsVisible);
        }

        public void CallPlayerHasChosenClothesElement()
        {
            PlayerHasChosenClothesElement.Invoke();
        }
        
        public void CallCancelChosenClothesElements()
        {
            CancelChosenClothesElements.Invoke();
        }

        public void CallClothesElementsHasBeenChosen(List<ClothesElementData> clothesElementsChosen)
        {
            ClothesElementsHaveBeenChosen.Invoke(clothesElementsChosen);
        }

        public void CallSelectedClothesElementDataReceived(List<ClothesElementData> clothesElementsData)
        {
            SelectedClothesElementsDataReceived.Invoke(clothesElementsData);
        }

        public void CallOpenNotEnoughKeysPanel()
        {
            OpenNotEnoughKeysPanel.Invoke();
        }

        public void CallDressingUpUIInitialized(List<string> clothesArticyIds)
        {
            DressingUpUIInitialized.Invoke(clothesArticyIds);
        }

        public void CallClothesElementDataRetrieved(List<ClothesElementData> retrievedClothesDatas)
        {
            ClothesElementDataRetrieved.Invoke(retrievedClothesDatas);
        }

        public void CallOnQuestEnded()
        {
            OnQuestEnded.Invoke();
        }

        public void CallChangeNpcClothesPreset(string npcArticyId, int clothesPresetIndex)
        {
            ChangeNpcClothesPreset.Invoke(npcArticyId, clothesPresetIndex);
        }
    }
}

