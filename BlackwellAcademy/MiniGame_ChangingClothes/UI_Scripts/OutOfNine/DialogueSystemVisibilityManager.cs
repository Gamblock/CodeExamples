using System.Collections.Generic;
using UnityEngine;
using UnlockGames.BA.MiniGames.PaintTheStatue;
using static UnlockGames.BA.MiniGames.DressingUp.UI.DressingUpOutOfNineUIPanel;

namespace UnlockGames.BA.MiniGames.DressingUp
{
    public class DialogueSystemVisibilityManager : MonoBehaviour
    {
        [SerializeField] private DressingUpViewModel _dressingUpViewModel;
        [SerializeField] private PaintStatueViewModel _paintStatueViewModel;

        [Space(5f)] 
        [SerializeField] private CanvasGroup _dialogueSystemCanvasGrpup;
        [SerializeField] private CanvasGroup _subtitlePanelCanvasGroup;
        private void OnEnable()
        {
            _dressingUpViewModel.DressingUpStateChanged += OnDressingUpStateChanged;
            _dressingUpViewModel.DressingUpChoiceValidated += OnDressingUpChoiceValidated;
            _paintStatueViewModel.PaintTheStatueUIInitialized += OnPaintUIInitialized;
            _paintStatueViewModel.PaintElementHasBeenChosen += OnPaintItemHasBeenChosen;
        }

        private void OnDisable()
        {
            _dressingUpViewModel.DressingUpStateChanged -= OnDressingUpStateChanged;
            _dressingUpViewModel.DressingUpChoiceValidated -= OnDressingUpChoiceValidated;
            _paintStatueViewModel.PaintTheStatueUIInitialized -= OnPaintUIInitialized;
            _paintStatueViewModel.PaintElementHasBeenChosen += OnPaintItemHasBeenChosen;
        }

        private void OnDressingUpStateChanged(DressingUpState dressingUpState)
        {
            if (dressingUpState == DressingUpState.ChoiceOutOfThree)
            {
                _dialogueSystemCanvasGrpup.alpha = 1f;
                _subtitlePanelCanvasGroup.alpha = 0f;
                _dialogueSystemCanvasGrpup.interactable = true;
            }
            else if (dressingUpState == DressingUpState.ChoiceOutOfNine)
            {
                _dialogueSystemCanvasGrpup.alpha = 0f;
                _dialogueSystemCanvasGrpup.interactable = false;
            }
            else if (dressingUpState == DressingUpState.None)
            {
                _dialogueSystemCanvasGrpup.alpha = 1f;
                _subtitlePanelCanvasGroup.alpha = 1f;
                _dialogueSystemCanvasGrpup.interactable = true;
            }
        }

        private void SetBubbleTextVisibility(bool isBubbleTextVisible)
        {
            Debug.Log($"<b><color=#6ffaaa>[DialogueSystemVisibilityManager.SetBubbleTextVisibility({isBubbleTextVisible})]</color></b>");
            _subtitlePanelCanvasGroup.alpha = isBubbleTextVisible? 1f : 0f;
        }

        private void OnPaintUIInitialized(List<string> _)
        {
            SetBubbleTextVisibility(false);
        }

        private void OnPaintItemHasBeenChosen(PaintStatueBaseData _)
        {
            SetBubbleTextVisibility(true);
        }
        
        private void OnDressingUpChoiceValidated()
        {
            Debug.Log($"<b><color=#12f6f1>[!!!!!OnDressingUpChoiceValidated()]</color></b>");
            _dialogueSystemCanvasGrpup.alpha = 0f;
            _subtitlePanelCanvasGroup.alpha = 1f;
            _dialogueSystemCanvasGrpup.interactable = false;
        }
    }
}

