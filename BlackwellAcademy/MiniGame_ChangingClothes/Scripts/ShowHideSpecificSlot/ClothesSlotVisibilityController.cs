using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnlockGames.BA.MiniGames.DressingUp
{
    [CreateAssetMenu(fileName = "ClothesSlotVisibilityController", menuName = "Game/DressingUp/ClothesSlotVisibilityController")]
    public class ClothesSlotVisibilityController : ScriptableObject
    {
        [SerializeField] private string _slotToManage;

        public event Action<string> ShowSlot = (_) => { };
        
        public event Action<string> HideSlot = (_) => { };

        public void ShowSelectedSlot()
        {
            Debug.Log($"<b><color=#6ffaaa>[ClothesSlotVisibilityController.ShowSelectedSlot()]</color></b>");
            ShowSlot.Invoke(_slotToManage);
        }
        
        public void HideSelectedSlot()
        {
            Debug.Log($"<b><color=#6ffaaa>[ClothesSlotVisibilityController.HideSelectedSlot()]</color></b>");
            HideSlot.Invoke(_slotToManage);
        }
    }
}