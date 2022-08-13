using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine;
using Doozy.Engine.UI;
using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using UnlockGames.BA.DialogueSystem.UI;

namespace UnlockGames.BA.MiniGames.DressingUp.UI
{
    public class DressingUpOutOfNineUIPanel : MonoBehaviour
    {
        private const string PopupNameNotEnoughKeys = "DressingUpNotEnoughKeys";
        
        [Header("Common")] 
        [SerializeField] private DressingUpViewModel _dressingUpViewModel;

        [Header("DressingUp OutOfNine")] 
        [SerializeField] private Button _lockClothesChoiceButton;
        [SerializeField] private Button _resetClothesChoiceButton;
        [SerializeField] private GameObject _panelSelectClothes;

        [Space(5f)]
        [SerializeField] private GameObject _currentAmountOfKeysPanel;
        [SerializeField] private TMP_Text _currentAmountOfKeysText;
        
        [Space(5f)]
        [SerializeField] private GameObject _clothesBtnRegular;
        [SerializeField] private GameObject _clothesBtnPremium;
        [SerializeField] private GameObject _premiumClothesPriceGobj;
        [SerializeField] private TMP_Text _premiumClothesPriceTxt;

        [SerializeField] private List<DressingUpOutOfNineButton> responseButtons = new List<DressingUpOutOfNineButton>();

        [Space(5f)] [SerializeField] private GridLayoutGroup _gridLayoutGroup;
        [Space(3f)] [SerializeField] private DressingUpGridLayoutHelper _settingOutOfSix;
        [Space(3f)] [SerializeField] private DressingUpGridLayoutHelper _settingOutOfNine;
        
        private List<ClothesElementData> _lastClothesElementsChosen;
        private bool _dressingUpOfNineActive;
        
        private int _dressingUpOutOfNineTotalEmements;
        private int _dressingUpOutOfNineMinElementsToProceed;
        
        private DressingUpState _previousDressingUpState;

        // 1) Subscribe and unsubscribe only to methods on changing DressingUp2 state to true_false
        private void OnEnable()
        {
            _dressingUpViewModel.DressingUpStateChanged += OnDressingUpStateChanged;
        }
        
        private void OnDisable()
        {
            _dressingUpViewModel.DressingUpStateChanged -= OnDressingUpStateChanged;
        }
        
        private void OnDressingUpStateChanged(DressingUpState dressingUpState)
        {
            _dressingUpOfNineActive = dressingUpState == DressingUpState.ChoiceOutOfNine;
            OnDressingUpButtonsStateChanged(_dressingUpOfNineActive);

            if (dressingUpState == DressingUpState.ChoiceOutOfNine)
            {
                SubscribeAndInit();
            }
            else if (_previousDressingUpState == DressingUpState.ChoiceOutOfNine && dressingUpState == DressingUpState.None)
            {
                Unsubscribe();
            }
            _previousDressingUpState = dressingUpState;
        }

        private void OnClothesChoiceValidated()
        {
            GameEventMessage.SendEvent("CloseDressingUpOutOfNine");
        }
        
        private void SubscribeAndInit()
        {
            Debug.Log($"<b><color=#12f6f1>[Dressing up out of nine - Subscribe to events]</color></b>");

            GameEventMessage.SendEvent("OpenDressingUpOutOfNine");
            _lastClothesElementsChosen = new List<ClothesElementData>();
            
            _lockClothesChoiceButton.onClick.AddListener(PlayerClickedOnConfirmClothesButton);
            _resetClothesChoiceButton.onClick.AddListener(PlayerPressedResetClothes);
            
            _dressingUpViewModel.ClothesElementsHaveBeenChosen += OnClothesElementsHaveBeenChosen;
            
            _dressingUpViewModel.SelectedClothesElementsDataReceived += OnClothesElementsesHaveBeenPreviewSelected;
            
            
            _dressingUpViewModel.OpenNotEnoughKeysPanel += OpenNotEnoughKeysPanelStateChanged;
            
            _dressingUpViewModel.KeysAmountUpdated += OnKeysAmountUpdatedChanged;
            
            _dressingUpViewModel.ClothesElementDataRetrieved += OnClothesElementDataRetrieved;
            
            _dressingUpViewModel.DressingUpChoiceValidated += OnClothesChoiceValidated;
        }

        private void Unsubscribe()
        {
            Debug.Log($"<b><color=#12f6f1>[Dressing up out of nine - unsubscribe from events]</color></b>");
            
            _lockClothesChoiceButton.onClick.RemoveAllListeners();
            _resetClothesChoiceButton.onClick.RemoveAllListeners();
            
            _dressingUpViewModel.ClothesElementsHaveBeenChosen -= OnClothesElementsHaveBeenChosen;
            
            _dressingUpViewModel.SelectedClothesElementsDataReceived -= OnClothesElementsesHaveBeenPreviewSelected;
            
            _dressingUpViewModel.OpenNotEnoughKeysPanel -= OpenNotEnoughKeysPanelStateChanged;
            
            _dressingUpViewModel.KeysAmountUpdated -= OnKeysAmountUpdatedChanged;
            
            _dressingUpViewModel.ClothesElementDataRetrieved -= OnClothesElementDataRetrieved;
            
            _dressingUpViewModel.DressingUpChoiceValidated -= OnClothesChoiceValidated;
        }
        
        private void OnClothesElementDataRetrieved(List<ClothesElementData> datas)
        {
            Debug.Log($"<b><color=#12f6f1>[OnClothesElementDataRetrieved]</color></b>");

            if (datas.Count > responseButtons.Count)
            {
                Debug.LogError($"DressingUpOutOfNine supports up to 9 elements, you are trying to pass {datas.Count}");
            }

            _dressingUpOutOfNineTotalEmements = datas.Count;
            _dressingUpOutOfNineMinElementsToProceed = GetMinAmountOfChosenElementsToProceed();
            
            for (int i = 0; i < responseButtons.Count; i++)
            {
                responseButtons[i].gameObject.SetActive(i < datas.Count);
                if (i < datas.Count)
                {
                    responseButtons[i].InitializeButton(_dressingUpViewModel, datas[i]);
                }
            }

            ManageUiDependingOnClothesElementsChosen(0, 0);

            DressingUpGridLayoutHelper correctHelper = null;
            if (datas.Count == 6)
            {
                _settingOutOfSix.ApplySettingForGridLayoutGroup(_gridLayoutGroup);
                correctHelper = _settingOutOfSix;
            }else if (datas.Count == 9)
            {
                _settingOutOfNine.ApplySettingForGridLayoutGroup(_gridLayoutGroup);
                correctHelper = _settingOutOfNine;
            }

            if (correctHelper != null)
            {
                foreach (var button in responseButtons)
                {
                    button._clothesItemImage.rectTransform.sizeDelta = correctHelper.imageSize;
                }
            }
            
            else
            {
                Debug.LogError("Grid layout group took wrong amount of items");
            }
        }
        
        private void OnClothesElementsesHaveBeenPreviewSelected(List<ClothesElementData> clothesElementsChosen)
        {
            _lastClothesElementsChosen = clothesElementsChosen ?? new List<ClothesElementData>();
            
            bool containsAnyPremiumItems = _lastClothesElementsChosen.FirstOrDefault(x => x.isPremium) != null;
            int combinedPrice = _lastClothesElementsChosen.Sum(x => x.premiumCost);
            
            _clothesBtnRegular.SetActive(!containsAnyPremiumItems);
            _clothesBtnPremium.SetActive(containsAnyPremiumItems);
            
            _premiumClothesPriceGobj.SetActive(combinedPrice > 0);
            _premiumClothesPriceTxt.text = combinedPrice.ToString();

            ManageUiDependingOnClothesElementsChosen(_lastClothesElementsChosen.Count, combinedPrice);
            
            foreach (var button in responseButtons)
            {
                button.SetSelected(_lastClothesElementsChosen);
            }
        }
        
        private void ManageUiDependingOnClothesElementsChosen(int chosenElementsAmount, int combinedPrice)
        {
            Debug.Log($"<b><color=#12f6f1>[ManageUiDependingOnClothesElementsChosen({chosenElementsAmount},{combinedPrice})]</color></b>");
            
            _resetClothesChoiceButton.gameObject.SetActive(chosenElementsAmount > 0);
            _lockClothesChoiceButton.gameObject.SetActive(chosenElementsAmount >= _dressingUpOutOfNineMinElementsToProceed);
            _panelSelectClothes.gameObject.SetActive(chosenElementsAmount < _dressingUpOutOfNineMinElementsToProceed);
            
            // previously there were a condition to turn on keys panel only if all elements chosen and at least one of them is previous
            //_currentAmountOfKeysPanel.gameObject.SetActive(chosenElementsAmount >= _dressingUpOutOfNineMinElementsToProceed && combinedPrice > 0);
            //_currentAmountOfKeysPanel.gameObject.SetActive(true);
        }

        private void OnClothesElementsHaveBeenChosen(List<ClothesElementData> obj)
        {
            DialogueManager.instance.SendMessage(DialogueSystemMessages.OnConversationContinueAll, null, SendMessageOptions.DontRequireReceiver);
        }

        private void PlayerClickedOnConfirmClothesButton()
        {
            if (_dressingUpOfNineActive)
            {
                _dressingUpViewModel.CallPlayerHasChosenClothesElement();
            }
        }
        
        private void OpenNotEnoughKeysPanelStateChanged()
        {
            UIPopup popup = UIPopupManager.GetPopup(PopupNameNotEnoughKeys);
            popup.Show();
        }
        
        private void OnKeysAmountUpdatedChanged(int keysAmount)
        {
            _currentAmountOfKeysText.text = keysAmount.ToString();
        }
        
        private void OnDressingUpButtonsStateChanged(bool setButtonsEnabled)
        {
            _lockClothesChoiceButton.interactable = setButtonsEnabled;
        
            foreach (var button in responseButtons)
            {
                button._button.interactable = setButtonsEnabled;
            }
        }

        private void PlayerPressedResetClothes()
        {
            _dressingUpViewModel.CallCancelChosenClothesElements();
        }

        public enum DressingUpState
        {
            None,
            ChoiceOutOfThree,
            ChoiceOutOfNine
        }
        
        // Now DressingUpOutOfNine takes 3, 6, or 9 elements
        // bool 'true' will be returned if 1, 2, 3 elements chosen
        private bool IsChosenElementsEnoughToProceed(int elementsChosen)
        {
            return elementsChosen >= _dressingUpOutOfNineTotalEmements / 3;
        }

        private int GetMinAmountOfChosenElementsToProceed()
        {
            return _dressingUpOutOfNineTotalEmements / 3;
        }
    }
}