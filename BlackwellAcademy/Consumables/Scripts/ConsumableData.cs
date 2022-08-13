using System;
using PixelCrushers.DialogueSystem;
using UnlockGames.BA.Core.DialogueSystem;

namespace UnlockGames.BA.Game.Consumables
{
    [Serializable, Obsolete("Use ConsumableItem instead. This class will be removed after quests SO importing from articy")]
    public class ConsumableData : ArticyItemData
    {
        public ConsumableData(Asset item) : base(item)
        {
        }
       
    }
}