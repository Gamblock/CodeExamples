using System;
using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.LevelLogic.Dressing
{
    public class ApplyButtonController : MonoBehaviour
    {
        public CharacterCustomizationViewModelSO viewModelSo;
        public UIButton applyButton;
        private void OnEnable()
        {
            viewModelSo.isApplyButtonActive.OnValueChanged += HandleApplyButton;
            HandleApplyButton(viewModelSo.isApplyButtonActive.Value);
        }
        
        private void OnDisable()
        {
            viewModelSo.isApplyButtonActive.OnValueChanged -= HandleApplyButton;
        }        

        private void HandleApplyButton(bool isEnable)
        { 
            if (isEnable)
            {
                applyButton.gameObject.SetActive(true);
                applyButton.EnableButton(); 
            }
            else
            {
                applyButton.DisableButton();
                applyButton.gameObject.SetActive(false);
            }
        }

    }
}