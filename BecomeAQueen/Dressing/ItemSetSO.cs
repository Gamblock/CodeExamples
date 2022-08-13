using System;
using System.Collections.Generic;
using System.Linq;
using ComfortGames.CharacterCustomization;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "ItemSetSO", menuName = "_Game/_ItemSets/ItemSetSO", order = 0)]
public class ItemSetSO : ScriptableObject
{
    public string id => Title;
    
    public string Title;
    public Sprite Sprite;
    
    [SerializeField] private List<OutfitScriptableObject> Items;
    
    public IReadOnlyCollection<OutfitScriptableObject> GetItems() => Items.AsReadOnly();
}
