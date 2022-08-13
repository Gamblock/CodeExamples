namespace UnlockGames.BA.Game.Consumables
{
    public readonly struct ConsumablesViewState
    {
        public readonly string ConsumableArticyID;
        public readonly int Amount;

        public ConsumablesViewState(string consumableArticyID, int amount)
        {
            ConsumableArticyID = consumableArticyID;
            Amount = amount;
        }
    }
}