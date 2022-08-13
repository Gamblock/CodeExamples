using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace UnlockGames.BA.DialogueSystem
{
    public class BuildingConversationController : MonoBehaviour
    {
        [SerializeField] private DialogueSystemTrigger bark;
        [SerializeField] private string sceneName;
        [SerializeField] private StandardBarkUI _barkUI;
        public string SceneName { get { return sceneName; } }

        public void ShowBark()
        {
            bark.OnUse();
        }

        public void HideBark()
        {
            if (_barkUI != null)
            {
                _barkUI.Hide();
            }
        }
    }
}