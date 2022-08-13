using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnlockGames.BA.MiniGames.DressingUp;

public class DialogueSystemTriggerExtended : DialogueSystemTrigger
{
    [Header("CustomFields")]
    [SerializeField] private DressingUpViewModel _dressingUpViewModel;
    [SerializeField] private ClothesSlotVisibilityController _clothesSlotVisibilityController;

    // This method is called from DialogueSystem from ConversationModel.InformParticipants()
    // This MonoBehaviour replaced DialogueSystem basic mono in the Systems.prefab
    // You need to use custom editor (DialogueSystemTriggerExtendedEditor) to add more serialized fields
    public void OnConversationStart()
    {
        _clothesSlotVisibilityController.HideSelectedSlot();
    }
    
    public void OnConversationEnd()
    {
        _dressingUpViewModel.CallOnQuestEnded();
        _clothesSlotVisibilityController.ShowSelectedSlot();
    }
}
