using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Doozy.Engine;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnlockGames.BA.CameraControl;
using UnlockGames.BA.Core.DialogueSystem;
using UnlockGames.BA.DialogueSystem.UI;
using UnlockGames.BA.Game.Consumables;
using UnlockGames.BA.NPC;
using static UnlockGames.BA.MiniGames.DressingUp.UI.DressingUpOutOfNineUIPanel;

namespace UnlockGames.BA.MiniGames.DressingUp
{
    [CreateAssetMenu(menuName = "Game/DressingUp/DressingUpMiniGameController")]
    public class DressingUpMiniGameController : Core.InitSystem.System
    {
        [SerializeField] private DressingUpViewModel _dressingUpViewModel;
        [SerializeField] private DressingUpDatabase _dressingUpDatabase;
        [SerializeField] private ClothesSlotVisibilityController _clothesSlotVisibilityController;
        
        [Space(4f)]
        [SerializeField] private ConsumablesManager consumablesManager;
        [SerializeField] private Consumable consumableForPremiumItems;

        [Space(4f)] [SerializeField] private CameraBlendDelegateController _cameraBlendDelegateController;

        private int _premiumChoices;
        private float _timeDelayForStaging = 3f;
        private List<ClothesElementData> _clothesElementsChosen;
        private DressingUpState _dressingUpState;

        private int dressingUpOutOfNineTotalEmements;

        [Header("Posing Specific")] 
        public float dressingUpCameraBlendTime = 1f;
        
        [HideInInspector] public float recordedDefaultCameraBlendTime;
        [HideInInspector] public Camera mainCamera;
        [HideInInspector] public CinemachineBrain cinemachineBrain;

        #region Clothes Initialization Part
        
        // Called at a start of the application
        public override void Initialize()
        {
            Debug.Log($"<b><color=#12f6f1>[DressingUpMiniGameController.InitializeAsync]</color></b>");
            _premiumChoices = 0;
            _clothesElementsChosen = new List<ClothesElementData>();

            mainCamera = Camera.main;
            cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
            recordedDefaultCameraBlendTime = cinemachineBrain.m_DefaultBlend.m_Time;
            
            _dressingUpDatabase.InitializeDatabase();
            
            base.Initialize();
        }
        
        #endregion End-- Clothes Initialization Part


        #region Subscribe / Unsubscribe
        
        public override void RegisterLuaFunctions()
        {
            Lua.RegisterFunction(nameof(ChangeDressingUpState), this, typeof(DressingUpMiniGameController).GetMethod(nameof(ChangeDressingUpState)));
            Lua.RegisterFunction(nameof(StageNpc), this, typeof(DressingUpMiniGameController).GetMethod(nameof(StageNpc)));
            Lua.RegisterFunction(nameof(ManageStageItems), this, typeof(DressingUpMiniGameController).GetMethod(nameof(ManageStageItems)));
            Lua.RegisterFunction(nameof(UnstageNpc), this, typeof(DressingUpMiniGameController).GetMethod(nameof(UnstageNpc)));
            Lua.RegisterFunction(nameof(CheckDressingUpResult), this, typeof(DressingUpMiniGameController).GetMethod(nameof(CheckDressingUpResult)));
            Lua.RegisterFunction(nameof(RegisterClothesForDressingUpOutOfNine), this, typeof(DressingUpMiniGameController).GetMethod(nameof(RegisterClothesForDressingUpOutOfNine)));
            Lua.RegisterFunction(nameof(ResetPremiumClothesChoicesCounter), this, typeof(DressingUpMiniGameController).GetMethod(nameof(ResetPremiumClothesChoicesCounter)));
            Lua.RegisterFunction(nameof(ChangeNpcClothesPreset), this, typeof(DressingUpMiniGameController).GetMethod(nameof(ChangeNpcClothesPreset)));
        }

        private void OnEnable()
        {
            _dressingUpViewModel.PreviewSelectedClothesElement += ClothesElementHasBeenPreviewSelected;
            _dressingUpViewModel.PlayerHasChosenClothesElement += PlayerHasChosenClothesElements;
            _dressingUpViewModel.DressingUpUIInitialized += OnDressingUpOutOfThreeUIInitialized;
            _dressingUpViewModel.CancelChosenClothesElements += OnCancelChosenClothesElements;
        }

        private void OnDisable()
        {
            Lua.UnregisterFunction(nameof(ChangeDressingUpState));
            Lua.UnregisterFunction(nameof(StageNpc));
            Lua.UnregisterFunction(nameof(ManageStageItems));
            Lua.UnregisterFunction(nameof(UnstageNpc));
            Lua.UnregisterFunction(nameof(CheckDressingUpResult));
            Lua.UnregisterFunction(nameof(RegisterClothesForDressingUpOutOfNine));
            Lua.UnregisterFunction(nameof(ResetPremiumClothesChoicesCounter));
            Lua.UnregisterFunction(nameof(ChangeNpcClothesPreset));
            
            _dressingUpViewModel.PreviewSelectedClothesElement -= ClothesElementHasBeenPreviewSelected;
            _dressingUpViewModel.PlayerHasChosenClothesElement -= PlayerHasChosenClothesElements;
            _dressingUpViewModel.DressingUpUIInitialized -= OnDressingUpOutOfThreeUIInitialized;
            _dressingUpViewModel.CancelChosenClothesElements -= OnCancelChosenClothesElements;
        }

        #endregion

        #region Registered Lua Commands
        
        // here you can add some functionality on changing dressingUpState
        // this specific method is for dressing out of three
        public void ChangeDressingUpState(string dressingUpState)
        {
            bool dressingUpStateOutOfThree = LuaExtended.AsBool(dressingUpState);
            _dressingUpState = dressingUpStateOutOfThree ? DressingUpState.ChoiceOutOfThree : DressingUpState.None;
            Debug.Log($"<b><color=#fc7703>[ChangeDressingUpStateByInstruction ({dressingUpStateOutOfThree})]</color></b>");
        }

        public void StageNpc(string npcToStage, string locationSpotToPlace, string animationToPlay, string cameraPreset, string timeDelayForStaging)
        {
            Debug.Log( $"<b><color=#fc7703>[StageNpc({npcToStage}, {locationSpotToPlace}, {animationToPlay}, {cameraPreset}, {timeDelayForStaging})]</color></b>");
            _timeDelayForStaging = Single.Parse(timeDelayForStaging, CultureInfo.InvariantCulture);
            _dressingUpViewModel.CallStagingInstructionReceived(new StageNpcInstruction(npcToStage, locationSpotToPlace, animationToPlay, cameraPreset));
            _clothesSlotVisibilityController.ShowSelectedSlot();
        }

        public void ManageStageItems(string activateStageItems)
        {
            bool _activateStageItems = LuaExtended.AsBool(activateStageItems);
            Debug.Log($"<b><color=#fc7703>[ManageStageItems({_activateStageItems})]</color></b>");
            _dressingUpViewModel.CallManageStageItemsInstructionReceived(_activateStageItems);
        }
        
        public void UnstageNpc()
        {
            Debug.Log( $"<b><color=#fc7703>[UnstageNpc()]</color></b>");
            _dressingUpViewModel.CallStagingInstructionReceived(null);
            _clothesSlotVisibilityController.HideSelectedSlot();
        }

        public bool CheckDressingUpResult(string minPremiumChoices, string maxPremiumChoices)
        {
            Debug.Log( $"<b><color=#fc7703>[Requested amount of premium choices: {_premiumChoices}]</color></b>");
            int minBorder = Int32.Parse(minPremiumChoices);
            int maxBorder = Int32.Parse(maxPremiumChoices);
            return _premiumChoices >= minBorder && _premiumChoices <= maxBorder;
        }

        public void RegisterClothesForDressingUpOutOfNine(string clothesItemsString, string dressingUpState)
        {
            Debug.Log($"<b><color=#12f6f1>[RegisterClothesForDressingUpOutOfNine({dressingUpState})]</color></b>");

            bool dressingUpOutOfNineActive = LuaExtended.AsBool(dressingUpState);
            
            _dressingUpState = dressingUpOutOfNineActive ? DressingUpState.ChoiceOutOfNine : DressingUpState.None;

            _dressingUpViewModel.CallDressingUpStateChanged(_dressingUpState);
            
            if (dressingUpOutOfNineActive)
            {
                UpdateKeysAmount();
                _clothesElementsChosen.Clear();
                var clotehsElementIds = clothesItemsString.Split(';').ToList();
                dressingUpOutOfNineTotalEmements = clotehsElementIds.Count;
                _dressingUpViewModel.CallClothesElementDataRetrieved(GetDatasFromIds(clotehsElementIds));
            }
        }

        public void ResetPremiumClothesChoicesCounter()
        {
            Debug.Log( $"<b><color=#fc7703>[Ordered to reset amount of premium choices, prev value: {_premiumChoices}, new value: 0]</color></b>");
            _premiumChoices = 0;
        }

        public void ChangeNpcClothesPreset(string npcArticyId, string clothesPresetIndex)
        {
            Debug.Log( $"<b><color=#fc7703>[ChangeNpcClothesPreset({npcArticyId},{clothesPresetIndex})]</color></b>");
            _dressingUpViewModel.CallChangeNpcClothesPreset(npcArticyId, Int32.Parse(clothesPresetIndex));
        }

        #endregion
        
        #region Subscription methods
        
        private void ClothesElementHasBeenPreviewSelected(string clothesElementArticyId, ClothesType clothesType)
        {
            Debug.Log($"<b><color=#12f6f1>[ClothesElementHasBeenPreviewSelected]</color></b>"); 
            var clothesElementData = new ClothesElementData(_dressingUpDatabase.clothesSynced[clothesElementArticyId]);
            if (_dressingUpState == DressingUpState.ChoiceOutOfThree)
            {
                _clothesElementsChosen.Clear();
                _clothesElementsChosen.Add(clothesElementData);
            }else if (_dressingUpState == DressingUpState.ChoiceOutOfNine)
            {
                // same element and already stored
                if (_clothesElementsChosen.FirstOrDefault(x => x.clothesElementArticyId == clothesElementArticyId) != null)
                {
                    return;
                }
                
                // Player put on alternative to the already chosen clothes item (meaning, by the same slot)
                var elementWithSameSlot = _clothesElementsChosen.FirstOrDefault(x => x.wardrobeRecipe.wardrobeSlot == clothesElementData.wardrobeRecipe.wardrobeSlot);
                if (elementWithSameSlot != null)
                {
                    _clothesElementsChosen.Remove(elementWithSameSlot);
                }
                _clothesElementsChosen.Add(clothesElementData);
            }
            //todo separate
            //_dressingUpViewModel.CallClothesElementsHasBeenChosen(_clothesElementsChosen);
            _dressingUpViewModel.CallSelectedClothesElementDataReceived(_clothesElementsChosen);
        }
        
        private void PlayerHasChosenClothesElements()
        {
            if(!AreAllClothesItemsHaveUniqueType())
            {
                return;
            }
            
            int totalPriceToPay = GetSelectedClothesElementsPrice();
            if (totalPriceToPay > 0 && !consumablesManager.WithdrawConsumableAmount(consumableForPremiumItems.ArticyID, totalPriceToPay))
            {
                _dressingUpViewModel.CallOpenNotEnoughKeysPanel();
            }
            else
            {
                foreach (var clothesElementData in _clothesElementsChosen)
                {
                    _premiumChoices += clothesElementData.isPremium ? 1 : 0;
                }
                OnDressingUpStateChanged();
            }
        }
        
        private async void OnDressingUpStateChanged()
        {
            Debug.Log($"<b><color=#fc7703>[OnDressingUpStateChanged - False]</color></b>");

            UpdateKeysAmount();

            // Record chosen clothes, play reaction animation, move camera to chosen preview preset and more
            _dressingUpViewModel.CallDressingUpChoiceValidated();    
            
            await UniTask.Delay(TimeSpan.FromSeconds(_timeDelayForStaging));
            
            _dressingUpViewModel.CallClothesElementsHasBeenChosen(_clothesElementsChosen);
            _dressingUpViewModel.CallDressingUpStateChanged(DressingUpState.None);
            
            cinemachineBrain.m_DefaultBlend.m_Time = recordedDefaultCameraBlendTime;
            _cameraBlendDelegateController.DecreaseCamerasPriority();
        }

        // This method is called from choice of 3 UI as a request to get buttons data
        private void OnDressingUpOutOfThreeUIInitialized(List<string> clotehsElementIds)
        {
            _dressingUpViewModel.CallClothesElementDataRetrieved(GetDatasFromIds(clotehsElementIds));
            
            UpdateKeysAmount();
            _dressingUpViewModel.CallDressingUpStateChanged(DressingUpState.ChoiceOutOfThree);
        }

        private void OnCancelChosenClothesElements()
        {
            _clothesElementsChosen.Clear();
            _dressingUpViewModel.CallSelectedClothesElementDataReceived(_clothesElementsChosen);
        }
        
        #endregion
        
        private void UpdateKeysAmount()
        {
            int? keysAmount = consumablesManager.GetConsumableAmount(consumableForPremiumItems.ArticyID);
            if (keysAmount == null)
            {
                Debug.LogError($"{GetType().Name} - keys consumable is not found");

                return;
            }
            _dressingUpViewModel.CallKeysAmountUpdated(keysAmount.Value);
        }

        // Additional supplementary methods below

        private List<ClothesElementData> GetDatasFromIds(List<string> clothesElementIds)
        {
            List<ClothesElementData> datas = new List<ClothesElementData>();

            foreach (var id in clothesElementIds)
            {
                datas.Add(new ClothesElementData(_dressingUpDatabase.clothesSynced[id]));
            }

            return datas;
        }
        
        private int GetSelectedClothesElementsPrice()
        {
            int priceSum = 0;
            foreach (var clothesElement in _clothesElementsChosen)
            {
                priceSum += clothesElement.isPremium ? clothesElement.premiumCost : 0;
            }

            return priceSum;
        }

        public bool AreAllClothesItemsHaveUniqueType()
        {
            if (_dressingUpState == DressingUpState.None)
            {
                Debug.LogError($"Can't continue, selected dressing up state: {_dressingUpState}");
                return false;
            }
            
            if (_dressingUpState == DressingUpState.ChoiceOutOfThree && _clothesElementsChosen.Count == 1)
            {
                return true;
            }
            
            if (!IsChosenElementsEnoughToProceed(_clothesElementsChosen.Count))
            {
                Debug.LogError($"<b><color=#12f6f1>[First of all you need to chose enough clothes elements with unique clothes types]</color></b>");
                return false;
            }
            
            List<string> allUsedClothesTypes = new List<string>();
            foreach (var clothesElement in _clothesElementsChosen)
            {
                if (!allUsedClothesTypes.Contains(clothesElement.wardrobeRecipe.wardrobeSlot))
                {
                    allUsedClothesTypes.Add(clothesElement.wardrobeRecipe.wardrobeSlot);
                }
            }

            Debug.Log($"<b><color=#12f6f1>[Choice out of nine, total unique clothes elements chosen = {allUsedClothesTypes.Count}]</color></b>");
            return allUsedClothesTypes.Count >= GetMinAmountOfChosenElementsToProceed();
        }

        // Now DressingUpOutOfNine takes 3, 6, or 9 elements
        // bool 'true' will be returned if 1, 2, 3 elements chosen
        private bool IsChosenElementsEnoughToProceed(int elementsChosen)
        {
            return elementsChosen >= dressingUpOutOfNineTotalEmements / 3;
        }

        private int GetMinAmountOfChosenElementsToProceed()
        {
            return dressingUpOutOfNineTotalEmements / 3;
        }
    }
}