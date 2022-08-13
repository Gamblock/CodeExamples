using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using UnlockGames.BA.Core.DialogueSystem;
using UnlockGames.BA.MiniGames.DressingUp;

namespace UnlockGames.BA.DialogueSystem.UI
{
    public class DressingUpUIResponseButton : StandardUIResponseButton
    {
        [Header("CustomElements")]
        [Space(4f)]
        [SerializeField] private ProceduralImage _clothesItemImage;
        [SerializeField] private ProceduralImage _clothesItemFrameImage;
        [SerializeField] private GameObject _clothesItemPremiumSign;

        [Space(4f)]
        [SerializeField] private Color _colorNotSelected;
        [SerializeField] private Color _colorDefaultClothesChoice;
        [SerializeField] private Color _colorPremiumClothesChoice;

        [Space(4f)] 
        [SerializeField] private Button _button;

        private DressingUpViewModel _dressingUpViewModel;
        [SerializeField] private bool _assignedClothesElementIsPremium;
        
        [Space(5f)]
        [SerializeField] private DressingUpMiniGameController _dressingUpController;

        [HideInInspector] public string clothesItemSlotArticyId;


        public override void OnClick()
        {
            _dressingUpViewModel.CallPreviewSelectedClothesElement(clothesItemSlotArticyId, ClothesType.None);
        }

        // call this when clothes item has been chosen and some time passed to review it by player, maybe
        // player played some animation and let's say golden particles had been emitted
        public void ContinueOnClick()
        {
            base.OnClick(); 
        }

        public string InitializeButton(DressingUpViewModel viewModel)
        {
            _dressingUpViewModel = viewModel;
            
            clothesItemSlotArticyId = response.destinationEntry.fields.Find(x => x.title == "ClothesItemSlot").value;
            return clothesItemSlotArticyId;
        }

        public void SetUiData(ClothesElementData clothesElementData, bool initial = true)
        {
            if (clothesElementData != null && clothesItemSlotArticyId == clothesElementData.clothesElementArticyId)
            {
                _assignedClothesElementIsPremium = clothesElementData.isPremium;

                _clothesItemImage.sprite = clothesElementData.previewImage;
                _clothesItemPremiumSign.SetActive(clothesElementData.isPremium);
                
                SetSelected(!initial);
            }
            else
            {
                SetSelected(false);
            }
        }

        private void SetSelected(bool selected)
        {
            if (selected)
            {
                _clothesItemFrameImage.color =
                    _assignedClothesElementIsPremium ? _colorPremiumClothesChoice : _colorDefaultClothesChoice;
            }
            else
            {
                _clothesItemFrameImage.color = _colorNotSelected;
            }
        }
    }
}

