using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Channel;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Events;
using UnlockGames.BA.Analytics;
using UnlockGames.BA.Analytics.DTO;
using UnlockGames.BA.Game.Events;
using UnlockGames.BA.Game.Quests;
using UnlockGames.BA.StorageCamera;

namespace UnlockGames.BA.Game.Consumables
{
    [CreateAssetMenu(menuName = "Game/Consumable/ConsumableStorageController")]
    public class ConsumablesManager : Core.InitSystem.System
    {
        [SerializeField] private List<Consumable> items;
        [SerializeField] private LocationUIViewModel locationUIViewModel;
        [SerializeField] private StringEventChannelSO anyQuestStateChangeChannel;
        [SerializeField] private QuestDatabase questDatabase;
        [SerializeField] private StorageSO storage;
        
        [SerializeField] private ConsumablesPersistentDAO consumablesDAO;
        [SerializeField] private Consumable _liveConsumable;
        
        [Header("Settings")]
        [SerializeField] private int _livesMaxAmounts;
        [SerializeField] private List<ConsumableAmountData> consumableDefaultValues;

        [Space(4f)] [SerializeField] private AnalyticsViewModel _analyticsViewModel;

        private readonly UnityEventConsumableDataAmount _changeConsumableAmountEvent = new();

        public event UnityAction<string, int, int> OnConsumableAmountChanged
        {
            add => _changeConsumableAmountEvent.AddListener(value);
            remove => _changeConsumableAmountEvent.RemoveListener(value);
        }

        public override void Initialize()
        {
            if (storage.IsFirstGameLaunch)
            {
                foreach (ConsumableAmountData consumableDefaultValue in consumableDefaultValues)
                {
                    AddConsumableAmount(consumableDefaultValue.Consumable.ArticyID, consumableDefaultValue.Amount);
                }
            }
        }

        private void OnEnable()
        {
            anyQuestStateChangeChannel.OnEventRaised += OnAnyQuestStateChange;
        }

        private void OnDisable()
        {
            anyQuestStateChangeChannel.OnEventRaised -= OnAnyQuestStateChange;
        }
        
        private void OnAnyQuestStateChange(string questArticyID)
        {
            if (questDatabase.ActiveQuest == null)
            {
                consumablesDAO.ApproveCache();
            }
        }

        public int GetConsumableAmount(string consumableArticyId)
        {
            if (!items.Exists(i => i.ArticyID == consumableArticyId))
            {
                throw new ArgumentException($"Wrong consumable articy id is provided ({consumableArticyId})", nameof(consumableArticyId));
            }

            return consumablesDAO.GetConsumableAmount(consumableArticyId);
        }

        public Consumable GetConsumable(string consumableArticyId)
        {
            return items.FirstOrDefault(i => i.ArticyID == consumableArticyId);
        }

        public bool AddConsumableAmount(string consumableArticyId, int amount)
        {
            return ModifyConsumableAmount(consumableArticyId, amount);
        }

        public bool AddConsumableAmount(ConsumableAmountData amountData)
        {
            return ModifyConsumableAmount(amountData.Consumable.ArticyID, amountData.Amount);
        }

        public bool WithdrawConsumableAmount(string consumableArticyId, int amount)
        {
            return ModifyConsumableAmount(consumableArticyId, -amount);
        }
        
        public bool WithdrawConsumableAmount(ConsumableAmountData amountData)
        {
            return ModifyConsumableAmount(amountData.Consumable.ArticyID, -amountData.Amount);
        }

        public bool HasEnoughConsumablesToWithdraw(string consumableArticyId, int delta)
        {
            Consumable consumable = items.FirstOrDefault(i => i.ArticyID == consumableArticyId);
            if (consumable == null)
            {
                Debug.LogError($"Consumable of type {consumableArticyId} is not found");
                return false;
            }
            int amount = consumablesDAO.GetConsumableAmount(consumable.ArticyID);

            if (amount - delta < 0)
            {
                _analyticsViewModel.CallOnPlayerDidntHaveEnoughResources(new NotEnoughResourcesData(consumable.ArticyName, Mathf.Abs(delta), amount));
                Debug.Log($"Not enough consumables ({consumable.ArticyName}, current: {amount.ToString()}, delta: {delta.ToString()})");
                return false;
            }

            return true;
        }

        private bool ModifyConsumableAmount(string consumableArticyId, int delta)
        {
            Consumable consumable = items.FirstOrDefault(i => i.ArticyID == consumableArticyId);
            if (consumable == null)
            {
                Debug.LogError($"Consumable of type {consumableArticyId} is not found");
                return false;
            }

            int amount = consumablesDAO.GetConsumableAmount(consumable.ArticyID);
            
            if (delta < 0 && !HasEnoughConsumablesToWithdraw(consumableArticyId, delta))
            {
                return false;
            }
            
            if (consumableArticyId == _liveConsumable.ArticyID && amount + delta > _livesMaxAmounts)
            {
                return false;
            }

            amount += delta;

            // We don't want any consumables to be modified instantly while any quest is active.
            // Otherwise, if user leaves the game during an active quest, quest state will be rollbacked to grantable, but resources will be already withdrawn 
            // The amount should be cached and written to persistent data storage after quest is completed
            if (questDatabase.ActiveQuest != null)
            {
                consumablesDAO.CacheConsumableAmount(consumable.ArticyID, amount);
            }
            else
            {
                consumablesDAO.SetConsumableAmount(consumable.ArticyID, amount);
            }
            
            _changeConsumableAmountEvent.Invoke(consumableArticyId, amount, delta);
            UpdateConsumableViewStates();
            return true;
        }

        private void UpdateConsumableViewStates()
        {
            locationUIViewModel.ConsumableViewStates.Value =
                items
                    .Select(i => new ConsumablesViewState(i.ArticyID, consumablesDAO.GetConsumableAmount(i.ArticyID)))
                    .ToList();
        }
    }
}
