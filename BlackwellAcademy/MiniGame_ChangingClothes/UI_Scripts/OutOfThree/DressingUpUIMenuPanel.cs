using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.UI;
using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using UnlockGames.BA.Awards;
using UnlockGames.BA.DSSequenceCommand;
using UnlockGames.BA.MiniGames.DressingUp;
using static UnlockGames.BA.MiniGames.DressingUp.UI.DressingUpOutOfNineUIPanel;

namespace UnlockGames.BA.DialogueSystem.UI
{
    public class DressingUpUIMenuPanel : StandardUIMenuPanel
    {
        private const string PopupNameNotEnoughKeys = "DressingUpNotEnoughKeys";
        
        [Header("Common")] 
        [SerializeField] private DressingUpViewModel _dressingUpViewModel;

        [Header("DressingUp OutOfThree")] 
        [SerializeField] private Button _lockClothesChoiceButton;

        [SerializeField] private ProceduralImage _clothesBtn;
        [SerializeField] private GameObject _premiumClothesPriceGobj;
        [SerializeField] private TMP_Text _premiumClothesPriceTxt;
        
        [Space(5f)]
        [SerializeField] private GameObject _currentAmountOfKeysPanel;
        [SerializeField] private TMP_Text _currentAmountOfKeysText;

        [Space(6f)]
        [SerializeField] private Color _noClothesSelectedColor;
        [SerializeField] private Color _defaultSelectedColor;
        [SerializeField] private Color _premiumSelectedColor;

        private List<DressingUpUIResponseButton> responseButtons = new List<DressingUpUIResponseButton>();

        private bool _dressingUpActive;
        private bool _selectedClothesElementHasBeenSet;
        
        /// You can consider this method as initialization method for buttons.
        /// It's called when Dialogue system has passed the last node before 'logic split' and is considering which node out of 3 (or more) to choose
        protected override void SetResponseButtons(Response[] responses, Transform target)
        {
            Debug.Log($"<b><color=#12f6f1>[CUSTOM BUTTONS - SetResponseButtons()]</color></b>");

            SubscribeToEvents();
                
            base.SetResponseButtons(responses, target);
            responseButtons = new List<DressingUpUIResponseButton>();

            List<string> clothesElementIds = new List<string>();
            
            for (int i = 0; i < responses.Length; i++)
            {
                if (i < buttons.Length)
                {
                    var dressingUpButton = (DressingUpUIResponseButton) buttons[i];
                    responseButtons.Add(dressingUpButton);
                    clothesElementIds.Add(dressingUpButton.InitializeButton(_dressingUpViewModel));
                }
            }
            
            OnSelectedClothesElementsDataReceived(null);
            _dressingUpViewModel.CallDressingUpUIInitialized(clothesElementIds);
        }

        // Can't call from OnEnable because it's called right after SetResponseButtons()
        private void SubscribeToEvents()
        {
            Debug.Log($"<b><color=#12f6f1>[CUSTOM BUTTONS - Subscribe to events]</color></b>");
            _dressingUpViewModel.PreviewSelectedClothesElement += OnPlayerHasChosenClothesElement;
            _dressingUpViewModel.ClothesElementsHaveBeenChosen += OnClothesElementsHaveBeenChosen;
            
            _dressingUpViewModel.DressingUpStateChanged += OnDressingUpStateChangedChanged;
            _dressingUpViewModel.SelectedClothesElementsDataReceived += OnSelectedClothesElementsDataReceived;

            _lockClothesChoiceButton.onClick.AddListener(PlayerClickedOnConfirmClothesButton);
            _dressingUpViewModel.OpenNotEnoughKeysPanel += OpenNotEnoughKeysPanelStateChanged;
            
            _dressingUpViewModel.KeysAmountUpdated += OnKeysAmountUpdatedChanged;

            _dressingUpViewModel.ClothesElementDataRetrieved += OnClothesElementDataRetrieved;
        }
        
        protected override void OnDisable()
        {
            Debug.Log($"<b><color=#12f6f1>[CUSTOM BUTTONS - OnDisable]</color></b>");
            base.OnDisable();
            _dressingUpViewModel.PreviewSelectedClothesElement -= OnPlayerHasChosenClothesElement;
            _dressingUpViewModel.ClothesElementsHaveBeenChosen -= OnClothesElementsHaveBeenChosen;
            
            _dressingUpViewModel.DressingUpStateChanged -= OnDressingUpStateChangedChanged;
            _dressingUpViewModel.SelectedClothesElementsDataReceived -= OnSelectedClothesElementsDataReceived;
            
            _lockClothesChoiceButton.onClick.RemoveAllListeners();
            _dressingUpViewModel.OpenNotEnoughKeysPanel -= OpenNotEnoughKeysPanelStateChanged;
            
            _dressingUpViewModel.KeysAmountUpdated -= OnKeysAmountUpdatedChanged;

            _dressingUpViewModel.ClothesElementDataRetrieved -= OnClothesElementDataRetrieved;
        }

        private void OnClothesElementDataRetrieved(List<ClothesElementData> datas)
        {
            for (int i = 0; i < responseButtons.Count; i++)
            {
                if (i < datas.Count)
                {
                    responseButtons[i].SetUiData(datas[i]);
                }
            }
        }

        private void OnPlayerHasChosenClothesElement(string _, ClothesType clothesType)
        {
            _selectedClothesElementHasBeenSet = true;
        }
        
        private void OnClothesElementsHaveBeenChosen(List<ClothesElementData> clothesElementDatas)
        {
            var firstClothesItem = clothesElementDatas.First();
            foreach (var button in responseButtons)
            {
                if (button.clothesItemSlotArticyId == firstClothesItem.clothesElementArticyId)
                {
                    button.ContinueOnClick();
                    break;
                }
            }
            //SetDialogueUIVisible(true);
        }

        void OnSelectedClothesElementsDataReceived(List<ClothesElementData> selectedClothesElements)
        {
            ClothesElementData clothesElementData = selectedClothesElements?.FirstOrDefault();
            _clothesBtn.color = clothesElementData == null ? _noClothesSelectedColor : clothesElementData.isPremium? _premiumSelectedColor : _defaultSelectedColor;
            _premiumClothesPriceGobj.SetActive(clothesElementData != null && (clothesElementData.isPremium));
            _premiumClothesPriceTxt.text = clothesElementData == null ? "" : clothesElementData.premiumCost.ToString();
            
            _currentAmountOfKeysPanel.SetActive(clothesElementData != null && clothesElementData.isPremium);
            
            foreach (var button in responseButtons)
            {
                button.SetUiData(clothesElementData, false);
            }
        }

        private void PlayerClickedOnConfirmClothesButton()
        {
            if (!_selectedClothesElementHasBeenSet)
            {
                Debug.LogWarning("Chose the element first");
                return;
            }

            _dressingUpViewModel.CallPlayerHasChosenClothesElement();
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

        private void OnDressingUpStateChangedChanged(DressingUpState dressingUpState)
        {
            _dressingUpActive = dressingUpState == DressingUpState.ChoiceOutOfThree;

            _selectedClothesElementHasBeenSet = false;
            OnDressingUpButtonsStateChanged(_dressingUpActive);
        }
        
        private void OnDressingUpButtonsStateChanged(bool setButtonsEnabled)
        {
            _lockClothesChoiceButton.interactable = setButtonsEnabled;

            foreach (var button in buttons)
            {
                button.button.interactable = setButtonsEnabled;
            }
        }
    }
}

