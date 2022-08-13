using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnlockGames.BA.Game.Consumables;

namespace UnlockGames.BA.UI.Overlay
{
    public class OverlayUIConsumableView : MonoBehaviour
    {
        [SerializeField] private Consumable _consumableData;
        [SerializeField] protected TextMeshProUGUI _valueText;
        [SerializeField] private TextMeshProUGUI _nameConsumableText;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _iconShadow;
        [SerializeField] private LocationUIViewModel locationUIViewModel;

        private void Start()
        {
            if (_icon != null)
            {
                _icon.sprite = _consumableData.Sprite;
                if (_iconShadow != null)
                {
                    _iconShadow.sprite = _consumableData.Sprite;
                }
            }
            if (_nameConsumableText != null)
            {
                _nameConsumableText.text = _consumableData.name;
            } 
        }
        private void OnEnable()
        {
            locationUIViewModel.ConsumableViewStates.OnValueChanged += OnConsumableViewStateUpdate;
            
            OnConsumableViewStateUpdate(locationUIViewModel.ConsumableViewStates.Value);
        }

        private void OnDisable()
        {
            locationUIViewModel.ConsumableViewStates.OnValueChanged -= OnConsumableViewStateUpdate;
        }

        private void OnConsumableViewStateUpdate(List<ConsumablesViewState> consumablesViewStates)
        {
            ConsumablesViewState viewState =
                consumablesViewStates.FirstOrDefault(i => i.ConsumableArticyID == _consumableData.ArticyID);
            if (viewState.ConsumableArticyID == null)
            {
                return;
            }
            
            SetValueText(viewState.Amount);
        }

        protected virtual void SetValueText(int value)
        {
            _valueText.text = value.ToString();
        }
    }
}