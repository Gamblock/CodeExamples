using Doozy.Engine.UI;
using UnityEngine;
using UnlockGames.BA.MiniGames.DressingUp;
using static UnlockGames.BA.MiniGames.DressingUp.UI.DressingUpOutOfNineUIPanel;

namespace UnlockGames.BA.DialogueSystem
{
    public class DialogueBlindView : MonoBehaviour
    {
        [SerializeField] private DialogueSystemController _dialogueSystemController;
        [SerializeField] private DressingUpViewModel _dressingUpViewModel;
        [SerializeField] private UIView [] _uiView;

        private void Start()
        {
            if (_dialogueSystemController.isConversationActive)
            {
                ShowBinds(true);
            }
            else
            {
                HideBinds(true);
            }
        }

        private void OnEnable()
        {
            _dialogueSystemController.OnConversationTurnOn += HandleConversionTurnOn;
            _dialogueSystemController.OnConversationTurnOff += HandleConversationTurnOff;
            _dressingUpViewModel.DressingUpStateChanged += OnDressingUpStateChanged;
        }
        
        private void OnDisable()
        {
            _dialogueSystemController.OnConversationTurnOn -= HandleConversionTurnOn;
            _dialogueSystemController.OnConversationTurnOff -= HandleConversationTurnOff;
            _dressingUpViewModel.DressingUpStateChanged -= OnDressingUpStateChanged;
        }

        private void OnDressingUpStateChanged(DressingUpState dressingUpState)
        {
            if (dressingUpState is DressingUpState.ChoiceOutOfNine or DressingUpState.ChoiceOutOfNine)
            {
                HideBinds();
            }
            else
            {
                ShowBinds();
            }
        }
        
        private void HandleConversionTurnOn()
        {
            ShowBinds();
        }

        private void HandleConversationTurnOff()
        {
            HideBinds();
        }

        private void ShowBinds(bool instant = false)
        {
            foreach (var uiView in _uiView)
            {
                uiView.Show();
            }
        }

        private void HideBinds(bool instant = false)
        {
            foreach (var uiView in _uiView)
            {
                uiView.Hide();
            }
        }
    }
}