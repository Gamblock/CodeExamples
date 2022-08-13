using System;
using UnityEngine;

namespace UnlockGames.BA.Game.Consumables
{
    [Serializable]
    public struct ConsumableAmountData
    {
        [SerializeField] private Consumable consumable;
        [SerializeField] private int amount;

        public Consumable Consumable => consumable;
        public int Amount => amount;

        public bool IsEmpty => consumable == null || amount == default;

        public ConsumableAmountData(Consumable consumable, int amount)
        {
            this.consumable = consumable;
            this.amount = amount;
        }
    }
}