using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace UnlockGames.BA.MiniGames.DressingUp.UI
{
    public class DressingUpOutOfNineButton : MonoBehaviour
    {
        public ProceduralImage _clothesItemImage;

        [Space(4f)]
        [SerializeField] private GameObject _frameRegularNotSelected;
        [SerializeField] private GameObject _frameRegularSelected;
        [Space(2f)]
        [SerializeField] private GameObject _framePremiumNotSelected;
        [SerializeField] private GameObject _framePremiumSelected;
        [Space(2f)]
        [SerializeField] private GameObject _selectedCheckMark;
        [Space(2f)]
        [SerializeField] private GameObject _itemPriceHolder;
        [SerializeField] private TMP_Text _itemPriceTxt;

        [Space(4f)] 
        public Button _button;

        private DressingUpViewModel _dressingUpViewModel;
        private bool _assignedClothesElementIsPremium;
        private ClothesType _clothesType;
        [HideInInspector] public string clothesItemSlotArticyId;
        

        public void InitializeButton(DressingUpViewModel viewModel, ClothesElementData clothesElementData)
        {
            _dressingUpViewModel = viewModel;
            
            if (clothesElementData != null)
            {
                clothesItemSlotArticyId = clothesElementData.clothesElementArticyId;
                _assignedClothesElementIsPremium = clothesElementData.isPremium;
                _clothesItemImage.sprite = clothesElementData.previewImage;
                _clothesType = clothesElementData.clothesType;
                
                if (_assignedClothesElementIsPremium && clothesElementData.premiumCost > 0)
                {
                    _itemPriceHolder.SetActive(true);
                    _itemPriceTxt.text = clothesElementData.premiumCost.ToString();
                }
                else
                {
                    _itemPriceHolder.SetActive(false);
                }
            }
            SetSelected(false);
            
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(PlayerClickedOnButton);
        }

        public void SetSelected(List<ClothesElementData> clothesElementDatas)
        {
            foreach (var clothesElementData in clothesElementDatas)
            {
                if (clothesElementData.clothesElementArticyId == clothesItemSlotArticyId)
                {
                    SetSelected(true);
                    return;
                }
            }
            SetSelected(false);
        }

        private void SetSelected(bool selected)
        {
            _frameRegularNotSelected.SetActive(!_assignedClothesElementIsPremium && !selected);
            _frameRegularSelected.SetActive(!_assignedClothesElementIsPremium && selected);
            _framePremiumNotSelected.SetActive(_assignedClothesElementIsPremium && !selected);
            _framePremiumSelected.SetActive(_assignedClothesElementIsPremium && selected);
            
            _selectedCheckMark.SetActive(selected);
        }

        private void PlayerClickedOnButton()
        {
            _dressingUpViewModel.CallPreviewSelectedClothesElement(clothesItemSlotArticyId, _clothesType);
        }
    }
}

