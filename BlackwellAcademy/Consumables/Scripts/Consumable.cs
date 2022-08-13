using UnityEngine;
using UnlockGames.BA.Core;

namespace UnlockGames.BA.Game.Consumables
{
    [CreateAssetMenu(menuName = "Game/Consumable/ConsumableData")]
    public class Consumable : ArticyItem
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private Sprite spriteForRewardPanel;
        

        public Sprite Sprite => sprite;
        public Sprite SpriteForRewardPanel
        {
            get
            {
                if (spriteForRewardPanel != null)
                {
                    return spriteForRewardPanel;
                }
                else
                {
                    return sprite;
                }
            }
        }
    }
}