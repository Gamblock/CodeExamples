using System;
using Doozy.Engine.UI;
using UnityEngine;

namespace _Game.Scripts.LevelLogic.Dressing
{
    public class RotationTipController : MonoBehaviour
    {
        public UIView rotationTipUIView;
        public DressingViewModelSO viewModel;
        public VoidEventChannelSO voidEventChannelSo;
        private bool wasShown;
        
        private void Start()
        {
            wasShown = false;
            viewModel.OnEquipButtonClicked += ShowRotationTip;
            voidEventChannelSo.OnEventRaised += HideRotationTip;
        }

        private void ShowRotationTip(string foo)
        {
            if (wasShown)
            {
                return;
            }
            rotationTipUIView.Show();
        }

        private void HideRotationTip()
        {
            rotationTipUIView.Hide();
            wasShown = true;
        }

        private void OnDisable()
        {
            viewModel.OnEquipButtonClicked -= ShowRotationTip;
            voidEventChannelSo.OnEventRaised -= HideRotationTip;
        }
    }
}