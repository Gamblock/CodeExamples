using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnlockGames.BA.Game.Consumables;
using UnlockGames.BA.StorageCamera;
using UnlockGames.BA.UI;

public class OverlayUILivesView : MonoBehaviour
{
    [SerializeField] private BuyLivesViewModel _viewModel;
    [SerializeField] private TextMeshProUGUI _timerLabel;

    private void OnEnable()
    {
        _viewModel.overlayTimer.OnValueChanged += UpdateTimer;
    }
    
    private void OnDestroy()
    {
        _viewModel.overlayTimer.OnValueChanged -= UpdateTimer;
    }

    private void UpdateTimer(string timer)
    {
        _timerLabel.text = timer;
    }

}
