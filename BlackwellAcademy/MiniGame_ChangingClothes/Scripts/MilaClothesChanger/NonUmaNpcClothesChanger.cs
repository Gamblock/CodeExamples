using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnlockGames.BA.NPC;
using UnlockGames.BA.StorageCamera;

namespace UnlockGames.BA.MiniGames.DressingUp
{
    /// <summary>
    /// Use it to change clothes of npcs which are not UMA characters
    /// </summary>
    public class NonUmaNpcClothesChanger : MonoBehaviour
    {
        [SerializeField] private Npc _relatedNpc;
        [SerializeField] private DressingUpViewModel _dressingUpViewModel;
        [SerializeField] private StorageSO _storageSo;

        [Space(5f)]
        [SerializeField] private List<SimpleClothesPreset> _npcClothesPresets;

        private void OnEnable()
        {
            _dressingUpViewModel.ChangeNpcClothesPreset += OnChangeNpcClothesPreset;
        }

        private void OnDisable()
        {
            _dressingUpViewModel.ChangeNpcClothesPreset -= OnChangeNpcClothesPreset;
        }

        private void OnChangeNpcClothesPreset(string npcArticyId, int clothesPresetId)
        {
            if (npcArticyId != _relatedNpc.data.articyId)
            {
                return;
            }

            _storageSo.SaveNpcOutfit(npcArticyId, clothesPresetId);
            ApplyNpcOutfit(clothesPresetId);
        }

        private void Start()
        {
            ApplyNpcOutfit(_storageSo.LoadNpcOutfit(_relatedNpc.data.articyId));
        }

        private void ApplyNpcOutfit(int outfitIndex)
        {
            Debug.Log($"<b><color=#b5f719>[NonUmaNpcClothesChanger.ApplyNpcOutfit({outfitIndex})]</color></b>");
            for (int i = 0; i < _npcClothesPresets.Count; i++)
            {
                var setClothesActive = i == outfitIndex;

                foreach (var clothesItem in _npcClothesPresets[i].clothesPresetItems)
                {
                    clothesItem.SetActive(setClothesActive);
                }
            }
        }
    }

    [Serializable]
    public class SimpleClothesPreset
    {
        public List<GameObject> clothesPresetItems;
    }
}