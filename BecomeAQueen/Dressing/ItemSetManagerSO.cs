using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.LevelLogic.Dressing;
using ComfortGames.CharacterCustomization;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "ItemSetHolder", menuName = "_Game/_ItemSets/ItemSetHolder", order = 0)]
public class ItemSetManagerSO : ScriptableObject
{
    public StorageSO storageSO;
    public List<ItemSetSO> allItemSets;
    public ClothesGroupConfigSO clothesGroupConfig;
    private List<OutfitScriptableObject> allOutfits;
    private List<MakeUpSO> allMakeups;
    private List<HairColorSO> allHairColors;
    
    public List<HairColorSO> GetHairColorsToDisplay()
    {
        List<HairColorSO> availableHairColors = new List<HairColorSO>();
        
        foreach (var hairColorGuid in storageSO.GetAllHairColorGuidsHashSet())
        {
            availableHairColors.Add(allHairColors.Find(x => x.elementGuid == hairColorGuid));
        }
        return availableHairColors;
    }

    public List<HairColorSO> GetAllMatchingHairColorSOs(string guid)
    {
        var currentHairColor = GetHairColor(guid);
        List<HairColorSO> matchingHairColors = new List<HairColorSO>();
        foreach (var hairColorSO in allHairColors)
        {
            if (hairColorSO != null && hairColorSO.dressingPopupIcon != null && hairColorSO.dressingPopupIcon.name == currentHairColor.dressingPopupIcon.name  )
            {
                matchingHairColors.Add(hairColorSO);
            }
        }

        return matchingHairColors;
    }
    public List<MakeUpSO> GetMakeupsToDisplay()
    {
        List<MakeUpSO> availableMakeups = new List<MakeUpSO>();
        
        foreach (var makeupGuid in storageSO.GetAllMakeupGuidsHashSet())
        {
            availableMakeups.Add(allMakeups.Find(x => x.elementGuid == makeupGuid));
        }
        return availableMakeups;
    }
    public List<OutfitScriptableObject> GetAllPieces()
    {
        var piecesList = new List<OutfitScriptableObject>();

        var sets = new List<string>();
        
        sets.AddRange(storageSO.GetAllOutfits());
        var all = GetItems(sets);
        
        foreach (var item in all)
        {
            item.isAvailable = false;
        }
        
        sets.Clear();
        sets.AddRange(storageSO.GetAvailableItemSets());
        var available= GetItems(sets);
        
        foreach (var item in available)
        {
            item.isAvailable = true;
        }
        
        piecesList.AddRange(all);
        piecesList.AddRange(available);

        piecesList = piecesList.Distinct().ToList();

        return piecesList;
    }
    
    private List<OutfitScriptableObject> GetItems(List<string> sets)
    {
        List<OutfitScriptableObject> piecesList = new List<OutfitScriptableObject>();
        foreach (var availableItemSet in sets)
        {
            ItemSetSO set = GetItemSet(availableItemSet);

            if (set == null)
            {
                var piece = GetOutfit(availableItemSet);
                
                if (!piece.addToAvailableItems || piece.outfitIcon == null)
                {
                    continue;
                }

                if (piecesList.Contains(piece))
                {
                    continue;
                }

                piecesList.Add(piece);
                continue;
            }

            foreach (var outfitScriptableObject in set.GetItems())
            {
                if (piecesList.Contains(outfitScriptableObject))
                {
                    continue;
                }

                piecesList.Add(outfitScriptableObject);
            }
        }

        return piecesList;
    }
    
    private void OnEnable()
    {
        InitializeAllOutfits();
        allMakeups = new List<MakeUpSO>();
        allHairColors = new List<HairColorSO>();
        UpdateCustomizationElementsList(clothesGroupConfig.hairColorsPaths);
        UpdateCustomizationElementsList(clothesGroupConfig.makeUpsPaths);
    }
    
    public ItemSetSO GetItemSet(string guid)
    {
        if (string.IsNullOrEmpty(guid))
        {
            return allItemSets.First();
        }
        
        return allItemSets.Find(x => x.id == guid);
    }
    
    public OutfitScriptableObject GetOutfit(string guid)
    {
        return allOutfits.Find(x => x.outfitElementGuid == guid);
    }

    public MakeUpSO GetMakeup(string guid)
    {
        return allMakeups.Find(x => x.elementGuid == guid);
    }
    
    public HairColorSO GetHairColor(string guid)
    {
        return allHairColors.Find(x => x.elementGuid == guid);
    }
    public OutfitScriptableObject GetAvailableOutfit(string guid)
    {
        var items = storageSO.GetAvailableItemSets();
        return GetItems(items).Find(x=>x.outfitElementGuid==guid);
    }
    public ItemSetSO GetNextItemSet()
    {
        var set = allItemSets.Find(x => !storageSO.GetAvailableItemSets().Contains(x.id));

        if (set == null)
        {
            return allItemSets[0];
        }

        return set;
    }
    
    private void UpdateCustomizationElementsList(List<string> paths)
    {
        foreach (var path in paths)
        {
            var elements = CharacterCustomizationAssetManager.GetCustomizationElementSos(path);
            foreach (var element in elements)
            {
                if (element == null) continue;
                switch (element)
                {
                    case MakeUpSO so:
                        allMakeups.Add(so);
                        break;
                    case HairColorSO colorSo:
                        allHairColors.Add(colorSo);
                        break;
                }
            }
        }
    }
    private void InitializeAllOutfits()
    {
        if (allOutfits != null)
        {
            return;
        }
        
        allOutfits = new List<OutfitScriptableObject>();
        
        foreach (var path in clothesGroupConfig.outfitsPaths)
        {
            allOutfits.AddRange(CharacterCustomizationAssetManager.GetOutfitScriptableObjects(path).ToList());
        }
    }
}
