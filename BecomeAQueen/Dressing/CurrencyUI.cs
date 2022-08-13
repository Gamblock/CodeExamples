using System;
using Dreamteck.Splines.Primitives;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.LevelLogic.Dressing
{
    public class CurrencyUI : MonoBehaviour
    {
        public CharacterCustomizationViewModelSO viewModelSo;
        public TextMeshProUGUI currencyValue;

        private void OnEnable()
        {
            viewModelSo.CurrencyValue.OnValueChanged += HandleCurrencyChange;
        }

        private void OnDisable()
        {
            viewModelSo.CurrencyValue.OnValueChanged -= HandleCurrencyChange;
        }

        private void HandleCurrencyChange(int value)
        {
            currencyValue.text = value.ToString();
        }

    }
}