using UnlockGames.BA.SO;
using UnlockGames.BA.UI;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace UnlockGames.BA.DialogueSystem
{
    public class NpcConversationController : MonoBehaviour
    {
        public Vector3 PlayerStartPosition { get { return playerStartPosition.position; } }
        [SerializeField] private Transform playerStartPosition;
        [SerializeField] private Transform npcStartPosition;
        [SerializeField] private DialogueSystemTrigger bark;
        [SerializeField] private DialogueSystemTrigger mainDialogue;
        [SerializeField] private GameObject indicator;

        private ExtendedInvitationBarkUI extendedInvitationBark;
        [SerializeField] private string questVariableName;
        [SerializeField] private int maxQuestVariableValue;

        private IBarkUI barkUI;

        private void Start()
        {
            barkUI = GetComponentInChildren<IBarkUI>();
            extendedInvitationBark = GetComponentInChildren<ExtendedInvitationBarkUI>();
        }

        public void ShowDialogue()
        {
            HideBark();
            mainDialogue.OnUse();
            if (DialogueManager.currentActor != null)
            {
                DialogueManager.currentActor.LookAt(playerStartPosition);
            }
        }

        public void ShowBark()
        {
            indicator.SetActive(false);
            bark.OnUse();
            if (extendedInvitationBark != null && questVariableName != "" && maxQuestVariableValue > 0)
            {
                extendedInvitationBark.Init(questVariableName, maxQuestVariableValue);
            }
        }

        public void HideBark()
        {
            barkUI.Hide();
        }

        public void ShowIndicator()
        {
            indicator.SetActive(true);
        }
    }
}