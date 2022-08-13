using System.Collections.Generic;
using ComfortGames.CharacterCustomization;
using UnityEngine;

namespace _Game.Scripts.LevelLogic.Dressing
{
    [CreateAssetMenu(fileName = "OutfitCategoriesSO", menuName = "ScriptableObjects/OutfitCategoriesSO")]
    public class OutfitCategoriesSO : ScriptableObject
    {
        public List<OutfitCategoryScriptableObject> categoriesList;

        public void SetNullsIfNeeded()
        {
            foreach (var category in categoriesList)
            {
                if (category.isDefaultOutfitShouldBeNull)
                {
                    category.defaultOutfitScriptableObject = null;
                }
            }
        }
    }
}