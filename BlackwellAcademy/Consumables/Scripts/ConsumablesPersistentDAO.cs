using System.Collections.Generic;
using UnityEngine;

namespace UnlockGames.BA.Game.Consumables
{
    [CreateAssetMenu(fileName = nameof(ConsumablesPersistentDAO), menuName = "Game/Consumable/" + nameof(ConsumablesPersistentDAO))]
    public sealed class ConsumablesPersistentDAO : ScriptableObject
    {
        private const string CONSUMABLES_DATA_KEY_PREFIX = "CONSUMABLES_";

        private const string CONSUMABLES_AMOUNT_KEY = CONSUMABLES_DATA_KEY_PREFIX + "amount_";

        // if any problems with consumable amounts will appear, possibly we should cache delta than all amount entirely
        private readonly Dictionary<string, int> _consumableAmountCache = new();
        
        public void SetConsumableAmount(string consumableArticyID, int amount)
        {
            ES3.Save($"{CONSUMABLES_AMOUNT_KEY}{consumableArticyID}", amount);
        }

        public void CacheConsumableAmount(string consumableArticyID, int amount)
        {
            _consumableAmountCache[consumableArticyID] = amount;
        }

        public int GetConsumableAmount(string consumableArticyID)
        {
            if (_consumableAmountCache.TryGetValue(consumableArticyID, out int cachedValue))
            {
                return cachedValue;
            }
            return ES3.Load($"{CONSUMABLES_AMOUNT_KEY}{consumableArticyID}", 0);
        }

        public void ApproveCache()
        {
            foreach (KeyValuePair<string, int> deltaCachePair in _consumableAmountCache)
            {
                SetConsumableAmount(deltaCachePair.Key, deltaCachePair.Value);
            }
            
            _consumableAmountCache.Clear();
        }
    }
}