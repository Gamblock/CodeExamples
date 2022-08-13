using System.Collections.Generic;
using PixelCrushers.DialogueSystem;

namespace UnlockGames.BA.Core.DialogueSystem
{
    public static class DialogueDatabaseExtended
    {
        public static DialogueEntry GetOriginDialogueEntry(this Link link, List<Conversation> conversations)
        {
            if (link != null)
            {
                Conversation conversation = GetConversation(link.originConversationID, conversations);
                if ((conversation != null) && (conversation.dialogueEntries != null))
                {
                    return conversation.dialogueEntries.Find(e => e.id == link.destinationDialogueID);
                }
            }

            return null;
        }
        public static Conversation GetConversation(int conversationID, List<Conversation> conversations)
        {
            return conversations.Find(c => c.id == conversationID);
        }
        public static bool CompareValuesByType(ArticyConstants.CompareType compareType, int value, int originValue)
        {
            bool result = false;
            switch (compareType)
            {
                case ArticyConstants.CompareType.Less:
                    result = originValue < value;
                    break;
                case ArticyConstants.CompareType.LessEqually:
                    result = originValue <=  value;
                    break;
                case ArticyConstants.CompareType.Equally:
                    result = originValue ==  value;
                    break;
                case ArticyConstants.CompareType.MoreEqually:
                    result = originValue >= value;
                    break;
                case ArticyConstants.CompareType.More:
                    result = originValue > value;
                    break;
                case ArticyConstants.CompareType.None:
                default: break;
            }

            return result;
        }
    }

   

}
