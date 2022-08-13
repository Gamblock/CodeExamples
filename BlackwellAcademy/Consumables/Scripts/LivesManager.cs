using System;
using Doozy.Engine.UI;
using Gamelogic.Extensions;
using SweetSugar.Scripts.Core;
using TMPro;
using UnityEngine;
using UnlockGames.BA.Game.Consumables;
using UnlockGames.BA.Game.Match3;
using UnlockGames.BA.StorageCamera;
using UnlockGames.BA.UI;
using UnlockGames.Core.Scenes;

public class LivesManager : MonoBehaviour
{
    
    private const string LIVES_TIMER_FORMAT = @"mm\:ss";
    private const string FULL = "FULL";
    
    private const string BuyLivesPopup = "BuyLivesPopup";
    private const string NotEnoughCrystalsPopup = "NotEnoughCrystalsPopup";
    private const string Match3Scene = "Match3Level";
        
    [SerializeField] private StorageSO _storageSO;
    [SerializeField] private ConsumablesManager _consumablesManager;
    [SerializeField] private Consumable _liveConsumable;
    [SerializeField] private Consumable _crystalConsumable;
    [SerializeField] private LocationUIViewModel _locationUIViewModel;
    [SerializeField] private BuyLivesViewModel livesViewModel;
    [SerializeField] private NotEnoughCrystalsPopupViewModel _notEnoughCrystalsViewModel;
    [SerializeField] private ScenesDirector _sceneDirector;

    [Header("Settings")]
    [SerializeField] private int _liveRefillTime;
    [SerializeField] private int _livesFullThreshold;
    [SerializeField] private int _livesRefillPrice;

    private Clock clock;
    private DateTime lastUpdateTime;
    
    public int LivesFullThreshold => _livesFullThreshold;

    public event Action<int> LivesAmountChanged = (amount) => { };
    
    public int LivesAmount =>  _consumablesManager.GetConsumableAmount(_liveConsumable.ArticyID);

    public void Initialize()
    {
        var startingClocktime = DateTimeOffset.Parse(_storageSO.LivesClockStartTime);
        var totalTimePassed = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - startingClocktime.ToUnixTimeSeconds();
        
        var livesRestored = Mathf.FloorToInt(totalTimePassed/ _liveRefillTime);
        if (livesRestored < 0)
        {
            livesRestored = 0;
        }
        var addingToTimer = totalTimePassed % _liveRefillTime;

        var totalSeconds = _liveRefillTime - addingToTimer  ;
        
        AddLives(livesRestored);
        

        if (_storageSO.IsFirstGameLaunch)
        {
            SetInitCount();
        }
        
        lastUpdateTime = DateTime.Now;
        clock.Reset(totalSeconds);
        clock.Unpause();
    }

    private void Start()
    {
        clock = new Clock();
        clock.OnSecondsChanged += OnSecondsChanged;
        clock.OnClockExpired += OnClockExpired;
        _locationUIViewModel.LivesIndicatorButtonClick += TryShowBuyLivesPopup;
        livesViewModel.BuyLivesClicked += TryRefillLives;
        _sceneDirector.OnSceneLoaded += CheckShowAftermatchPopup;
        _consumablesManager.OnConsumableAmountChanged += OnAnyConsumableAmountChanged;
    }

    private void OnDestroy()
    {
        clock.OnSecondsChanged -= OnSecondsChanged;
        clock.OnClockExpired -= OnClockExpired;
        _locationUIViewModel.LivesIndicatorButtonClick -= TryShowBuyLivesPopup;
        livesViewModel.BuyLivesClicked -= TryRefillLives;
        _sceneDirector.OnSceneLoaded -= CheckShowAftermatchPopup;
        _consumablesManager.OnConsumableAmountChanged += OnAnyConsumableAmountChanged;
    }

    private void Update()
    {
        var currentTime = DateTime.Now;
        var deltaTime = currentTime - lastUpdateTime;
        lastUpdateTime = currentTime;
        if (deltaTime.TotalSeconds > _liveRefillTime)
        {
            AddLives((int)deltaTime.TotalSeconds / _liveRefillTime);
            clock.Update(deltaTime.TotalSeconds % _liveRefillTime);
        }
        else
        {
            clock.Update(deltaTime.TotalSeconds);
        }
    }

    public int GetAllLivesRestoreTimeSeconds()
    {
        return clock.TimeInSeconds + (_livesFullThreshold - LivesAmount - 1) * _livesRefillPrice;
    }
    
    public void TryRefillLives()
    {
        var crystalAmount = _consumablesManager.GetConsumableAmount(_crystalConsumable.ArticyID);
        if (crystalAmount < _livesRefillPrice)
        {
            ShowNotEnoughCrystalsPopup(_livesRefillPrice - crystalAmount);
        }
        else
        {
            RefillLives();
        }
    }

    private void CheckShowAftermatchPopup(string sceneName)
    {
        if (sceneName != Match3Scene)
        {
            return;
        }
        
        if (LivesAmount <= 0)
        {
            ShowBuyLivesPopup();
        }
    }

    private void OnClockExpired()
    {
        AddLives(1);
        
        clock.Reset(_liveRefillTime);
    }

    private void OnSecondsChanged()
    {
        var time = TimeSpan.FromSeconds(clock.Time);
        var formattedTime = time.ToString(LIVES_TIMER_FORMAT);
        livesViewModel.buyLivesState.Value = new BuyLivesState(formattedTime, LivesAmount, _livesRefillPrice, _livesFullThreshold);

        var overlayTimer = LivesAmount >= _livesFullThreshold ? FULL : formattedTime;
        livesViewModel.overlayTimer.Value = overlayTimer;
    }
    
    private void AddLives(int amount)
    {
        if (LivesAmount + amount >= _livesFullThreshold)
        {
            amount = _livesFullThreshold - LivesAmount;
        }
        var consumableData = new ConsumableAmountData(_liveConsumable, amount);
        _consumablesManager.AddConsumableAmount(consumableData);
    }
    
    private void TryShowBuyLivesPopup()
    {
        if (LivesAmount >= _livesFullThreshold)
        {
            return;
        }

        ShowBuyLivesPopup();
    }

    private void ShowBuyLivesPopup()
    {
        UpdateBuyLivesPopupState();
        UIPopup.GetPopup(BuyLivesPopup).Show();
    }

    private void UpdateBuyLivesPopupState()
    {
        var time = TimeSpan.FromSeconds(clock.Time);
        var formattedTime = time.ToString(LIVES_TIMER_FORMAT);
        livesViewModel.buyLivesState.Value = new BuyLivesState(formattedTime, LivesAmount, _livesRefillPrice, _livesFullThreshold);
    }

    private void ShowNotEnoughCrystalsPopup(int crystalsCount)
    {
        UIPopup.GetPopup(NotEnoughCrystalsPopup).Show();
        _notEnoughCrystalsViewModel.OnNotEnoughCrystalsPopupShow(crystalsCount);
    }
    
    private void RefillLives()
    {
        var amountToAdd = _livesFullThreshold - LivesAmount;
        AddLives(amountToAdd);
        
        var crystalsData = new ConsumableAmountData(_crystalConsumable, _livesRefillPrice);
        _consumablesManager.WithdrawConsumableAmount(crystalsData);
    }

    private void SetInitCount()
    {
        var consumableData = new ConsumableAmountData(_liveConsumable, _livesFullThreshold);
        _consumablesManager.AddConsumableAmount(consumableData);
    }
    
    private void OnAnyConsumableAmountChanged(string consumableArticyID, int amount, int delta)
    {
        if (consumableArticyID == _liveConsumable.ArticyID && delta != 0)
        {
            var prevValue = amount - delta;
            var wasGreaterThenTreshold = prevValue >= _livesFullThreshold;
            var becameLessThenTreshold = amount < _livesFullThreshold;
            
            if ( wasGreaterThenTreshold && becameLessThenTreshold)
            {
                clock.Reset(_liveRefillTime);
                clock.Unpause();
            }

            if (amount < _livesFullThreshold)
            {
                _storageSO.LivesClockStartTime = DateTimeOffset.UtcNow.ToString();
            }

            LivesAmountChanged.Invoke(amount);
        }
    }
    
}
