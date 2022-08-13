using System;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.Articy;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnlockGames.BA.AnimationControl;
using UnlockGames.BA.CameraControl;
using UnlockGames.BA.ConversantsRotation;
using UnlockGames.BA.Game.Locations;
using UnlockGames.BA.Game.Quests;
using UnlockGames.BA.MiniGames;
using UnlockGames.BA.NPC;
using UnlockGames.BA.Timeline;
using UnlockGames.BA.DSSequenceCommand;
using System.Text;
using Game.Systems.Articy;
using UnlockGames.BA.Game.Locations.Portal;

namespace UnlockGames.BA.Core.DialogueSystem
{
    public static class ArticyConverterExtended
    {
     
        #region Conversation
        public static void ConvertConversations(List<Conversation> conversations, ArticyConverter articyConverter, ref int itemID, ref Template template)
        {
            for (int i = 0; i < conversations.Count; i++)
            {
                if (conversations[i].fields.Exists(x => x.title == ArticyConstants.FieldTitleNodeName && x.value.Contains("DialogueQuest")))
                {
                   conversations[i] = conversations[i].ConvertConversationToQuest(articyConverter, ref itemID,  ref template);
                    // Debug.LogError($"{conversations[i].fields.GetArticyId()} - out to {conversations.Find(x=> x.ConversantID == conversations[i].dialogueEntries[conversations[i].dialogueEntries.Count - 1].outgoingLinks[0].destinationConversationID).fields.GetArticyId()}");
                    // Debug.LogError($"{conversations[i].fields.GetArticyId()} - out to {conversations[i].dialogueEntries[conversations[i].dialogueEntries.Count - 1].outgoingLinks[0].destinationDialogueID}");

                }
            }
           
        }
        public static Conversation ConvertConversationToQuest(this Conversation conversation, ArticyConverter articyConverter, ref int itemID, ref Template template)
        {
            itemID++;
            Item itemQuest = template.CreateItem(itemID, conversation.Name);
            string articyIdItemQuest = $"9{conversation.fields.GetArticyId().Substring(1)}";
            
            conversation.fields.Add(new Field(ArticyConstants.FieldTitleAllActorsArticyIds, GetAllConversationActorIds(ref conversation), FieldType.Text));
            conversation.fields.Add(new Field(ArticyConstants.FieldTitleQuestGroupId,
                GetQuestGroup(conversation, articyConverter), FieldType.Text));
            itemQuest.fields.CopyFields(conversation.fields);
            Field.SetValue(itemQuest.fields, "Is Item", "False", FieldType.Boolean);
            Field.SetValue(itemQuest.fields, "Is Quest", "True", FieldType.Boolean);
            Field.SetValue(itemQuest.fields, "StartConversation", conversation.fields.GetArticyId(), FieldType.Text);
            Field.SetValue(itemQuest.fields, ArticyConstants.FieldTitleArticyId, conversation.fields.GetArticyId(), FieldType.Text);

            CheckEmptyField(conversation, ArticyConstants.FieldTitlePrefixSubtable + Quest.FieldTitleStripNpc, nameof(ConvertConversationToQuest));

            CheckEmptyField(conversation, Game.Locations.LocationData.FieldTitleLocationSlot, nameof(ConvertConversationToQuest));
            CheckEmptyField(conversation, LocationSpotData.FieldTitleLocationSpotSlot, nameof(ConvertConversationToQuest));
            CheckEmptyField(conversation, Quest.FieldTitleCostConsumable, nameof(ConvertConversationToQuest));

            string[] titles = conversation.Title.Split('/');
            Field.SetValue(itemQuest.fields, "Name", titles[titles.Length - 1], FieldType.Text);
            Field.SetValue(itemQuest.fields, "Title", titles[titles.Length - 1], FieldType.Text);
            articyConverter.database.items.Add(itemQuest);
            
            DialogueEntry finishInstruction = template.CreateDialogueEntry(articyConverter.GetNextConversationEntryID(conversation), conversation.id, "FinishQuestInstruction");
            finishInstruction.outgoingLinks = new List<Link>();

            

            DialogueEntry outputDialogueEntry = conversation.dialogueEntries.Find(x => x.Title == "output");
            if (outputDialogueEntry != null)
            {
                int outgoingDialogueEntryId = outputDialogueEntry.id;
                if (conversation.dialogueEntries == null || conversation.dialogueEntries.Count < 2)
                {
                    ShowError(conversation.fields,conversation,nameof(ConvertConversationToQuest),"conversation.dialogueEntries is Empty");

                    return null;
                }
                List<DialogueEntry> dialogueEntriesForRewriting = conversation.dialogueEntries.FindAll(x => x.outgoingLinks.Exists(y => y.destinationDialogueID == outgoingDialogueEntryId));

                
                for (int i = 0; i < dialogueEntriesForRewriting.Count; i++)
                {
                    for (int j = 0; j < dialogueEntriesForRewriting[i].outgoingLinks.Count; j++)
                    {

                        var link = dialogueEntriesForRewriting[i].outgoingLinks[j];
                        link.destinationConversationID = conversation.id;
                        link.destinationDialogueID = finishInstruction.id;
                        
                        if (conversation?.dialogueEntries[conversation.dialogueEntries.Count - 1]?.outgoingLinks == null 
                            || conversation?.dialogueEntries[conversation.dialogueEntries.Count - 1]?.outgoingLinks.Count <1)
                        {
                            ShowError(conversation?.dialogueEntries[conversation.dialogueEntries.Count - 1]?.fields,conversation,nameof(ConvertConversationToQuest),"conversation?.dialogueEntries[conversation.dialogueEntries.Count - 1]?.outgoingLinks ");
                            continue;
                        }

                        link.originConversationID = conversation?.dialogueEntries[conversation.dialogueEntries.Count - 1]?.outgoingLinks[0]?.destinationConversationID??-1;
                        link.originDialogueID = conversation?.dialogueEntries[conversation.dialogueEntries.Count - 1]?.outgoingLinks[0]?.destinationDialogueID??-1;
                        if (link.originConversationID == -1)
                        {
                            ShowError(dialogueEntriesForRewriting[i].fields,conversation,nameof(ConvertConversationToQuest),"link.originConversationID");
                        }
                    }
                }

                finishInstruction.userScript = $"CompleteQuest(\"{conversation.fields.GetArticyId()}\")";
                finishInstruction.isGroup = true;
                conversation.dialogueEntries.Add(finishInstruction);
            }
            else
            {
                ShowError(conversation.fields,conversation,nameof(ConvertConversationToQuest),"output Node not found");
    
            }
            return conversation;
        }

        private static string GetQuestGroup(Conversation conversation, ArticyConverter articyConverter)
        {
            foreach (var questGroup in articyConverter.database.items)
            {
                if (questGroup.fields.Find(x => x.title == "IsQuestGroup") != null)
                {
                    string questGroupStrip = questGroup.fields.Find(x => x.title == ArticyConstants
                                                                             .FieldTitlePrefixSubtable + ArticyConstants.FieldTitleQuestGroupStrip).value;
                    if (questGroupStrip == "")
                    {
                        continue;
                    }

                    string[] groupedQuests = questGroupStrip.Split(";");
                    foreach (var groupedQuest in groupedQuests)
                    {
                        if (!string.IsNullOrEmpty(groupedQuest) && groupedQuest != ArticyConstants.IdEmpty)
                        {
                            if (conversation.GetArticyId() == groupedQuest)
                                return questGroup.GetArticyId();
                        }
                    }

                }
            }
            return "";
        }

        
        public static string ConvertDialogueFragmentFieldsToLua(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string luaScript = "";
            string technicalName = fields.Find(x => x.title == "DialogueFragmentType")?.value ?? "";

            //switch ( "DialogueFragmentType."+ technicalName) {
            //    case "DialogueFragmentType.Dialogue": luaScript = ConvertInstructionChangePropertyIntValue(ref fields, ref articyConverter, ref conversation); break;
            //}
            return luaScript;
        }
        #endregion
        #region Conditions
        
        public static DialogueEntry ConvertConditionToLuaExpression(this DialogueEntry entry, ArticyConverter articyConverter, ref Conversation conversation, ref ArticyData.Condition condition, bool isTruePath)
        {
           
            string pattern = @"//.*";
            Regex rx = new Regex(pattern);
            condition.expression = rx.Replace(condition.expression, System.String.Empty);
            List<Field> fields = entry.fields;
            articyConverter.SetFeatureFields(fields, condition.features);
            string expression = ConvertConditionFieldsToLua(ref fields, ref articyConverter, ref conversation);
            
            
            if (expression == "")
            {
                expression = articyConverter.AddToConditions(entry.conditionsString, ArticyConverter.ConvertExpression(condition.expression));
            }
            
            entry.conditionsString = isTruePath ? expression : $"({expression}) == false";
            
            entry.isGroup = true;
            return entry;
        }

        private static string ConvertConditionFieldsToLua(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string luaExpression = "";

            string nodeName = fields.Find(x => x.title == ArticyConstants.FieldTitleNodeName)?.value ?? "";
            if (nodeName.Contains("ConditionConsumablesAmount"))
            {
                luaExpression += ConvertConditionItemAmount(ref fields, ref articyConverter, ref conversation);
            }
            else if (nodeName.Contains("ConditionCurrentQuizResult"))
            {
                luaExpression += ConvertConditionCurrentQuizResult(ref fields, ref articyConverter, ref conversation);
            }
            else if (nodeName.Contains("ConditionDressingUpResult"))
            {
                luaExpression += ConvertConditionDressingUpResult(ref fields, ref articyConverter, ref conversation);
            }
            
            return luaExpression;
        }

        private static string ConvertConditionCurrentQuizResult(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string luaExpression = "";
            Field quizResult = fields.Find(x => x.title == "QuizResult");
            if (quizResult != null)
            {
                luaExpression += $"GetQuizCurrentResultCondition(\"{quizResult.value}\")";
            }

            return luaExpression;
        }
        
        private static string ConvertConditionDressingUpResult(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string luaExpression = "";
            Field quizResultMin = fields.Find(x => x.title == "PremiumPointsRequiredToPassMin");
            Field quizResultMax = fields.Find(x => x.title == "PremiumPointsRequiredToPassMax");
            if (quizResultMin != null)
            {
                luaExpression += $"CheckDressingUpResult(\"{quizResultMin.value}\",\"{quizResultMax.value}\")";
            }

            return luaExpression;
        }

        private static string ConvertConditionItemAmount(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string luaExpression = "";
            string ownerArticyId = fields.Find(x => x.title == ArticyConstants.FieldTitleOwnerSlot)?.value ?? "";

            if (string.IsNullOrEmpty(ownerArticyId))
            {
                ShowError(fields, conversation, nameof(ConvertConditionItemAmount), nameof(ownerArticyId));
            }

            
            //int count = 0;
            // if (!int.TryParse(fields.Find(x => x.title == "Count").value, out count) || count <= 0)
            // {
            //     ShowError(fields, conversation, nameof(ConvertConditionItemAmount), nameof(count));
            // }
            //
            // for (int i = 0; i < count; i++)
            // {
            string consumableArticyId = fields.Find(x => x.title == "ConsumableSlot").value;
            string typeCompression = fields.Find(x => x.title == "TypeCompression").value;
            int value = fields.Find(x => x.title == "Value").AsInt();
          
            luaExpression += @$"CompareConsumablesAmount(""{consumableArticyId}"",{typeCompression},{value.ToString()})";
            //}
            if (int.Parse(typeCompression) == 0)
            {
                ShowError(fields,conversation,nameof(ConvertConditionItemAmount),luaExpression + "\n TypeCompression is 0 like");
            }

            return luaExpression;
        }

        #endregion

        public static DialogueEntry ConvertDialogueFragmentToLuaExpression(this DialogueEntry entry, ArticyConverter articyConverter, ref Conversation conversation, ref ArticyData.DialogueFragment fragment)
        {
            List<Field> fields = entry.fields;
            entry.fields.Add(new Field(ArticyConstants.FieldTitleActorArticyId, fragment.speakerIdRef, FieldType.Text));
            string nodeName = fields.Find(x => x.title == ArticyConstants.FieldTitleNodeName)?.value ?? "";
            if (nodeName.Contains("DialogueFragment") || nodeName.Contains("DialogChoose"))
            {
                string userScript = ConvertDialogueFragmentFieldsToLuaExpression(ref fields, ref articyConverter, ref conversation, entry.id);
                if (nodeName.Contains("QuizResponse"))
                {
                    userScript += ConvertDialogueQuizResponseFieldsToLuaExpression(ref fields, ref articyConverter, ref conversation, entry.id);
                }
                if (!string.IsNullOrEmpty(entry.userScript) && !string.IsNullOrEmpty(userScript))
                {
                    entry.userScript += ";\n";
                }
                
                entry.userScript += userScript;

                if (nodeName.Contains("SetQuizMenuPanel"))
                {
                    entry.currentSequence += ConvertDialogueEntrySequenceToLuaExpression(ref fields, ref articyConverter, ref conversation, entry.id);
                }
                if (nodeName.Contains("SetDressingUpMenuPanel"))
                {
                    entry.currentSequence += ConvertDialogueDressingUpEntrySequenceToLuaExpression(ref fields, ref articyConverter, ref conversation, entry.id);
                }
                if (nodeName.Contains("SetPaintStatueMenuPanel"))
                {
                    entry.currentSequence += ConvertDialoguePaintStatueEntrySequenceToLuaExpression(ref fields, ref articyConverter, ref conversation, entry.id);
                }
                
            }
            return entry;
        }

        public static string ConvertDialogueFragmentFieldsToLuaExpression(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation, int id)
        {
            string luaExpression = "";
            string cameraPresets = FindProperty(ref articyConverter, fields, ExpositionCameraController.FieldTitleCameraPresets);
            string cameraToTarget = FindActor(ref articyConverter, fields, ExpositionCameraController.FieldTitleTargetForCamera);
            if (cameraToTarget != "" || cameraPresets != "")
            {
                luaExpression = $"SetCameraSettings(\"{cameraPresets}\", \"{cameraToTarget}\");";
            }

            string animation = FindProperty(ref articyConverter, fields, AnimationController.FieldTitleAnimation);
            string animationTarget = FindActor(ref articyConverter, fields, AnimationController.FieldTitleAnimationTarget);
            string attachable = FindProperty(ref articyConverter, fields, AnimationController.FieldTitleAttachable);
            if (animation != "")
            {
                luaExpression += $"PlayAnimation(\"{animation}\", \"{animationTarget}\", \"{attachable}\");";
            }

            Field rotateToSpeakerField = fields.Find(el => el.title == ConversantsRotationController.FieldTitleNeedRotateToSpeaker);
            string speakerArticyId = fields.Find(el => el.title == ArticyConstants.FieldTitleActorArticyId)?.value;
            if (rotateToSpeakerField != null && LuaExtended.AsBool(rotateToSpeakerField) && speakerArticyId != ArticyConstants.IdEmpty && !string.IsNullOrEmpty(speakerArticyId))
            {
                luaExpression += $"RotateConversantsToSpeaker(\"{speakerArticyId}\", \"{conversation.id}\");";
            }
                                    
            Field showThinkingBubble = fields.Find(el => el.title == "ShowThinkingBubble");
            if (showThinkingBubble != null && LuaExtended.AsBool(showThinkingBubble))
            {
                luaExpression += "SetThinkingBubble(\"1\");";
            }
            else
            {
                luaExpression += "SetThinkingBubble(\"0\");";
            }

            return luaExpression;
        }

        public static string ConvertDialogueQuizResponseFieldsToLuaExpression(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation, int id)
        {
            string luaExpression = "";
            Field isQuizResponse = fields.Find(x => x.title == "IsQuizResponse");
            Field isCorrect = fields.Find(x => x.title == "QuizAnswerCorrectness");
            if (isQuizResponse != null && isCorrect != null)
            {
                luaExpression += $"AnswerQuizQuestion(\"{isCorrect.value}\");";
            }
            return luaExpression;
        }

        private static string ConvertDialogueEntrySequenceSetQuizMenuPanel(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string luaExpression = "";
            string quizState = fields.Find(x => x.title == "QuizState")?.value ?? "";
            if (string.IsNullOrEmpty(quizState) || quizState == ArticyConstants.IdEmpty)
            {
                ShowError(fields, conversation, nameof(ConvertInstructionChangeQuizState), nameof(quizState));
            }

            int menuPanelNumber = quizState == "1" ? 1 : 0;
            luaExpression = $"SetMenuPanel(Player, {menuPanelNumber});";
            return luaExpression;
        }
        
        // '2' Means index, 0 - default panel, 2 - panel for dressing up. 3 - dressing up counter overrides. Check the root object in 'VN.. .prefab'
        private static string ConvertDialogueEntrySequenceSetDressingUpMenuPanel(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string luaExpression = "";
            string dressingUpState = fields.Find(x => x.title == "DressingUpState")?.value ?? "";
            if (string.IsNullOrEmpty(dressingUpState) || dressingUpState == ArticyConstants.IdEmpty)
            {
                ShowError(fields, conversation, nameof(ConvertDialogueEntrySequenceSetDressingUpMenuPanel), nameof(dressingUpState));
            }
            
            int menuPanelNumber = LuaExtended.AsBool(dressingUpState) ? 2 : 0;
            luaExpression = $"SetMenuPanel(Player, {menuPanelNumber});";
            return luaExpression;
        }
        // '2' Means index, 0 - default panel, 3 - panel for 'paint statue'. Check the root object in 'VN.. .prefab'
        private static string ConvertDialogueEntrySequenceSetPaintStatueMenuPanel(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string luaExpression = "";
            string paintStatueState = fields.Find(x => x.title == "ChangePaintStatueState")?.value ?? "";
            if (string.IsNullOrEmpty(paintStatueState) || paintStatueState == ArticyConstants.IdEmpty)
            {
                ShowError(fields, conversation, nameof(ConvertDialogueEntrySequenceSetPaintStatueMenuPanel), nameof(paintStatueState));
            }
            
            int menuPanelNumber = LuaExtended.AsBool(paintStatueState) ? 3 : 0;
            luaExpression = $"SetMenuPanel(Player, {menuPanelNumber});";
            return luaExpression;
        }

        private static string ConvertDialogueEntrySequenceToLuaExpression(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation, int entryID)
        {
            var luaExpression = "";
            string quizState = fields.Find(x => x.title == "QuizState")?.value ?? "";
            if (quizState != "")
            {
                luaExpression = "Continue();" + ConvertDialogueEntrySequenceSetQuizMenuPanel(ref fields, ref articyConverter, ref conversation);
            }
            return luaExpression;
        }
        
        // Even though in DressingUpMiniGame we don't use generic UI provided by DialogueSystem (we hide it temporarily with CanvasGroup),
        // we need to get callbacks from DialogueSystem to get 'result nodes' of player choice, to perform mini game.
        private static string ConvertDialogueDressingUpEntrySequenceToLuaExpression(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation, int entryID)
        {
            var luaExpression = "";
            string dressingUpState = fields.Find(x => x.title == "DressingUpState")?.value ?? "";
            if (dressingUpState != "")
            {
                luaExpression = "Continue();" + ConvertDialogueEntrySequenceSetDressingUpMenuPanel(ref fields, ref articyConverter, ref conversation);
            }
            return luaExpression;
        }
        
        private static string ConvertDialoguePaintStatueEntrySequenceToLuaExpression(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation, int entryID)
        {
            var luaExpression = "";
            string paintStatueState = fields.Find(x => x.title == "ChangePaintStatueState")?.value ?? "";
            if (paintStatueState != "")
            {
                luaExpression = "Continue();" + ConvertDialogueEntrySequenceSetPaintStatueMenuPanel(ref fields, ref articyConverter, ref conversation);
            }
            return luaExpression;
        }

        #region Instructions

        public static DialogueEntry ConvertInstructionToLuaExpression(this DialogueEntry entry, ArticyConverter articyConverter, ref Conversation conversation, ref ArticyData.Instruction instruction)
        {
            string pattern = @"//.*";
            Regex cutPattern = new Regex(pattern);
            instruction.expression = cutPattern.Replace(instruction.expression, System.String.Empty);
            List<Field> fields = entry.fields;

            articyConverter.SetFeatureFields(fields, instruction.features);
            entry.userScript = ConvertInstructionFieldsToLuaExpression(ref fields, ref articyConverter, ref conversation, entry.id);

            if (entry.userScript == "")
            {
                entry.userScript = articyConverter.AddToUserScript(entry.userScript, ArticyConverter.ConvertExpression(instruction.expression));
            }

            string sequence = ConvertInstructionFieldsToSequence(entry, ref fields, ref articyConverter, ref conversation, entry.id);
            if (!string.IsNullOrEmpty(sequence))
            {
                entry.Sequence = sequence;
            }
            entry.isGroup = string.IsNullOrEmpty(sequence);

            return entry;
        }

        private static string ConvertInstructionFieldsToSequence(this DialogueEntry entry, ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation, int id)
        {
            string nodeName = fields.Find(x => x.title == ArticyConstants.FieldTitleNodeName)?.value;
            if (string.IsNullOrEmpty(nodeName))
            {
                ShowError(fields, conversation, nameof(ConvertInstructionFieldsToSequence), nameof(nodeName));
                return null;
            }

            string sequence;
            if (nodeName.Contains(ArticyConstants.FieldInstructionTimelineStop, StringComparison.Ordinal))
            {
                sequence = ConvertInstructionTimelineStop(ref fields, ref articyConverter, ref conversation);
            }
            else if (nodeName.Contains(ArticyConstants.FieldInstructionTimeline, StringComparison.Ordinal))
            {
                sequence = ConvertInstructionTimeline(entry, ref fields, ref articyConverter, ref conversation);
            }
            else if (nodeName.Contains(ArticyConstants.FieldInstructionSequence))
            {
                sequence = ConvertInstructionSequence(entry, ref fields, ref articyConverter, ref conversation);
            }
            else
            {
                sequence = null;
            }

            return sequence;
        }

        private static string ConvertInstructionFieldsToLuaExpression(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation, int id)
        {
            string luaExpression = "";
            string nodeName = fields.Find(x => x.title == ArticyConstants.FieldTitleNodeName)?.value ?? "";
            if (nodeName.Contains("ChangeConsumablesAmount"))
            {
                luaExpression = ConvertInstructionChangeConsumablesAmount(ref fields, ref articyConverter, ref conversation);
            }
            else if (nodeName.Contains("InstructionShowInGameNotification"))
            {
                luaExpression = ConvertInstructionShowInGameNotification(ref fields, ref conversation);
            }
            else if (nodeName.Contains("InstructionSpawnStatist"))
            {
                luaExpression = ConvertInstructionSpawnStatist(ref fields, ref articyConverter, ref conversation);
            }
            else if (nodeName.Contains("InstructionSpawnTalkStatist"))
            {
                luaExpression = ConvertInstructionSpawnTalkStatist(ref fields, ref articyConverter, ref conversation);
            }
            else if (nodeName.Contains("InstructionRemoveAllStatist"))
            {
                luaExpression = "RemoveAllStatist()";
            }
            else if (nodeName.Contains("InstructionRemoveAttachable"))
            {
                luaExpression = ConvertInstructionRemoveAttachable(ref fields, ref articyConverter, ref conversation);
            }else if (nodeName.Contains("InstructionNewQuestUnlocked"))
            {
                luaExpression = ConvertInstructionNewQuestUnlocked(ref fields, ref articyConverter, ref conversation);
            }
            else if (nodeName.Contains("InstructionChangeQuizState"))
            {
                luaExpression = ConvertInstructionChangeQuizState(ref fields, ref articyConverter, ref conversation);
            }
            else if (nodeName.Contains("InstructionPortalCommand"))
            {
                luaExpression = ConvertInstructionPortalCommand(ref fields, ref articyConverter, ref conversation);
            }
            else if (nodeName.Contains("InstructionChangeDressingUpState"))
            {
                luaExpression = ConvertInstructionChangeDressingUpState(ref fields, ref articyConverter, ref conversation);
            }
            else if (nodeName.Contains("InstructionToStageNpc"))
            {
                luaExpression = ConvertInstructionStageNpc(ref fields, ref articyConverter, ref conversation);
            }
            else if (nodeName.Contains("InstructionManageStageItems"))
            {
                luaExpression = ConvertInstructionManageStageItems(ref fields, ref articyConverter, ref conversation);
            }
            else if (nodeName.Contains("InstructionToUnstageNpc"))
            {
                luaExpression = ConvertInstructionUnstageNpc(ref fields, ref articyConverter, ref conversation);
            }
            else if (nodeName.Contains("InstructionChangeName"))
            {
                luaExpression = ConvertInstructionChangeName(ref fields, ref articyConverter, ref conversation);
            }
            else if (nodeName.Contains("InstructionDressingUpOutOfNine"))
            {
                luaExpression = ConvertInstructionDressingUpOutOfNine(ref fields, ref articyConverter, ref conversation);
            }
            else if (nodeName.Contains("InstructionResetPremiumClothesCounter"))
            {
                luaExpression = ConvertInstructionResetPremiumClothesCounter(ref fields, ref articyConverter, ref conversation);
            }
            else if (nodeName.Contains("InstructionNpcChoseClothesPreset"))
            {
                luaExpression = ConvertInstructionNpcChoseClothesPreset(ref fields, ref articyConverter, ref conversation);
            }
            else if (nodeName.Contains("InstructionAnalyticsPlayerChoice"))
            {
                luaExpression = ConvertInstructionAnalyticsPlayerChoice(ref fields, ref articyConverter, ref conversation);
            }
            else if (nodeName.Contains("InstructionSetMetaTutorialStep"))
            {
                luaExpression = ConvertInstructionSetMetaTutorialStep(ref fields);
            }
            return luaExpression;
        }


        private static string ConvertInstructionChangeName(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string luaExpression = $"ChangeNameShowPopup();";
            return luaExpression;
        }

        private static string ConvertInstructionChangeQuizState(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string luaExpression = "";
            string quizState = fields.Find(x => x.title == "QuizState")?.value ?? "";
            string quiz = fields.Find(x => x.title == "Quiz")?.value ?? "";
            if (string.IsNullOrEmpty(quizState) || quizState == ArticyConstants.IdEmpty
                || string.IsNullOrEmpty(quiz) || quiz == ArticyConstants.IdEmpty)
            {
                ShowError(fields, conversation, nameof(ConvertInstructionChangeQuizState), nameof(quizState));
            }
            luaExpression = $"ChangeQuizState(\"{quizState}\", \"{quiz}\");";
            return luaExpression;
        }

        private static string ConvertInstructionPortalCommand(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string luaExpression = "";
            
            string targetPortalArticyId = fields.Find(x => x.title == LocationPortalsManager.FieldTargetPortalSlot)?.value;
            string command = fields.Find(x => x.title == LocationPortalsManager.FieldCommand)?.value;

            if (string.IsNullOrEmpty(targetPortalArticyId) || targetPortalArticyId == ArticyConstants.IdEmpty)
            {
                ShowError(fields, conversation, nameof(LocationPortalsManager), nameof(targetPortalArticyId));
            }
            else if (string.IsNullOrEmpty(command))
            {
                ShowError(fields, conversation, nameof(LocationPortalsManager), nameof(command));
            }
            else
            {
                luaExpression = $"PortalCommand(\"{targetPortalArticyId}\", \"{command}\")";                
            }
            return luaExpression;    
        }
        private static string ConvertInstructionChangeDressingUpState(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string dressingUpState = fields.Find(x => x.title == "DressingUpState")?.value ?? "";

            if (string.IsNullOrEmpty(dressingUpState) || dressingUpState == ArticyConstants.IdEmpty)
            {
                ShowError(fields, conversation, nameof(ConvertInstructionChangeDressingUpState), nameof(dressingUpState));
            }
            
            string luaExpression = $"ChangeDressingUpState(\"{dressingUpState}\");";
            return luaExpression;
        }
        
        private static string ConvertInstructionStageNpc(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string npcToStage = fields.Find(x => x.title == "NpcToStage")?.value ?? "";
            string locationSpotToPlace = fields.Find(x => x.title == "LocationSpotToPlace")?.value ?? "";
            string animationToPlay = fields.Find(x => x.title == "DefaultAnimationToPlay")?.value ?? "";
            string cameraPreset = fields.Find(x => x.title == "CameraPresetForStaging")?.value ?? "";
            string timeDelayForStaging = fields.Find(x => x.title == "TimeDelayForStaging")?.value ?? "";

            if (IsStringValueWrong(npcToStage) || IsStringValueWrong(locationSpotToPlace) || IsStringValueWrong(animationToPlay))
            {
                ShowError(fields, conversation, nameof(ConvertInstructionChangeDressingUpState), nameof(npcToStage));
            }
            
            string luaExpression = $"StageNpc(\"{npcToStage}\",\"{locationSpotToPlace}\",\"{animationToPlay}\",\"{cameraPreset}\",\"{timeDelayForStaging}\");";
            return luaExpression;
        }
        private static string ConvertInstructionManageStageItems(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string activateStageItems = fields.Find(x => x.title == "ActivateStageItems")?.value ?? "";

            if (IsStringValueWrong(activateStageItems))
            {
                ShowError(fields, conversation, nameof(ConvertInstructionManageStageItems), nameof(activateStageItems));
            }
            
            string luaExpression = $"ManageStageItems(\"{activateStageItems}\");";
            return luaExpression;
        }
        
        private static string ConvertInstructionDressingUpOutOfNine(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string clothesItemsForDressingUpOutOfNine = fields.Find(x => x.title == "SUBTABLE__DressingUpOutOfNineClothesItems")?.value ?? "";
            string isDressingUpActive = fields.Find(x => x.title == "DressingUpState")?.value ?? "";
            string luaExpression = $"RegisterClothesForDressingUpOutOfNine(\"{clothesItemsForDressingUpOutOfNine}\",\"{isDressingUpActive}\");";
            return luaExpression;
        }
        private static string ConvertInstructionResetPremiumClothesCounter(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            return $"ResetPremiumClothesChoicesCounter();";
        }
        // We can change instruction and the way it works to change clothes to any npc
        private static string ConvertInstructionNpcChoseClothesPreset(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string npcArticyId = fields.Find(x => x.title == "NpcToManage")?.value ?? "";
            string clothesPresetIndex = fields.Find(x => x.title == "NpcPresetToChose")?.value ?? "";
            string luaExpression = $"ChangeNpcClothesPreset(\"{npcArticyId}\",\"{clothesPresetIndex}\");";
            return luaExpression;
        }
        private static string ConvertInstructionAnalyticsPlayerChoice(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string playerChoice = fields.Find(x => x.title == "PlayerChoice")?.value ?? "";
            string luaExpression = $"AnalyticsPlayerChoice(\"{playerChoice}\");";
            return luaExpression;
        }

        private static string ConvertInstructionSetMetaTutorialStep(ref List<Field> fields)
        {
            int tutorialStep = fields.Find(i => i.title == "MetaTutorialStep")?.AsInt() ?? -1;
            string luaExpression = $"SetTutorialStep({tutorialStep.ToString()});";

            return luaExpression;
        }
        
        private static string ConvertInstructionUnstageNpc(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string luaExpression = $"UnstageNpc();";
            return luaExpression;
        }

        private static string ConvertInstructionSpawnStatist(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string luaExpression = "";

            string spawnStatistArticyId = fields.Find(x => x.title == NpcSpawner.FieldTitleSpawnStatist)?.value ?? "";
            string spawnLocationArticyId = fields.Find(x => x.title == NpcSpawner.FieldTitleSpawnLocation)?.value ?? "";
            string spawnSpotArticyId = fields.Find(x => x.title == NpcSpawner.FieldTitleSpawnSpot)?.value ?? "";

            if (string.IsNullOrEmpty(spawnStatistArticyId) || spawnStatistArticyId == ArticyConstants.IdEmpty)
            {
                ShowError(fields, conversation, nameof(ConvertInstructionSpawnStatist), nameof(spawnStatistArticyId));
            }
            else if (string.IsNullOrEmpty(spawnLocationArticyId) || spawnLocationArticyId == ArticyConstants.IdEmpty)
            {
                ShowError(fields, conversation, nameof(ConvertInstructionSpawnStatist), nameof(spawnLocationArticyId));
            }
            else if (string.IsNullOrEmpty(spawnSpotArticyId) || spawnSpotArticyId == ArticyConstants.IdEmpty)
            {
                ShowError(fields, conversation, nameof(ConvertInstructionSpawnStatist), nameof(spawnSpotArticyId));
            }
            else
            {
                luaExpression = $"AddStatist(\"{spawnStatistArticyId}\", \"{spawnLocationArticyId}\", \"{spawnSpotArticyId}\")";
            }
            return luaExpression;
        }

        private static string ConvertInstructionSpawnTalkStatist(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string luaExpression = "";

            string spawnStatistArticyId = fields.Find(x => x.title == NpcSpawner.FieldTitleSpawnStatist)?.value ?? "";
            string spawnLocationArticyId = fields.Find(x => x.title == NpcSpawner.FieldTitleSpawnLocation)?.value ?? "";
            string spawnSpotArticyId = fields.Find(x => x.title == NpcSpawner.FieldTitleSpawnSpot)?.value ?? "";
            string smallTalk = fields.Find(x => x.title == NpcSpawner.FieldTitleSmallTalk)?.value ?? ArticyConstants.IdEmpty;

            if (string.IsNullOrEmpty(spawnStatistArticyId) || spawnStatistArticyId == ArticyConstants.IdEmpty)
            {
                ShowError(fields, conversation, nameof(ConvertInstructionSpawnTalkStatist), nameof(spawnStatistArticyId));
            }
            else if (string.IsNullOrEmpty(spawnLocationArticyId) || spawnLocationArticyId == ArticyConstants.IdEmpty)
            {
                ShowError(fields, conversation, nameof(ConvertInstructionSpawnTalkStatist), nameof(spawnLocationArticyId));
            }
            else if (string.IsNullOrEmpty(spawnSpotArticyId) || spawnSpotArticyId == ArticyConstants.IdEmpty)
            {
                ShowError(fields, conversation, nameof(ConvertInstructionSpawnTalkStatist), nameof(spawnSpotArticyId));
            }
            else
            {
                luaExpression = $"AddTalkStatist(\"{spawnStatistArticyId}\", \"{spawnLocationArticyId}\", \"{spawnSpotArticyId}\", \"{smallTalk}\")";
            }
            return luaExpression;
        }

        private static string ConvertInstructionShowInGameNotification(ref List<Field> fields, ref Conversation conversation)
        {
            string luaExpression = "";
            string inGameNotificationTechnicalPostfixName = fields.Find(x => x.title == ArticyConstants.FieldTitleTechnicalPostfixName)?.value ?? "";
            string inGameNotificationTechnicalPrefixName = fields.Find(x => x.title == ArticyConstants.FieldTitleTechnicalPrefixName)?.value ?? "";
            string inGameNotificationTechnicalName = ArticyNamingUtility.MakePath(inGameNotificationTechnicalPostfixName,inGameNotificationTechnicalPrefixName);
            if (string.IsNullOrEmpty(inGameNotificationTechnicalPostfixName) || string.IsNullOrEmpty(inGameNotificationTechnicalPrefixName))
            {
                ShowError(fields, conversation, nameof(ConvertInstructionShowInGameNotification), nameof(inGameNotificationTechnicalName));
            }
            else
            {
                luaExpression = $"ShowInGameNotification(\"{inGameNotificationTechnicalPostfixName}\")";
            }

            return luaExpression;
        }

        private static string ConvertInstructionSequence(this DialogueEntry entry, ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            StringBuilder sequence = new StringBuilder();

            SetDialoguePanelType setDialoguePanel = (SetDialoguePanelType)(fields.Find(x => x.title == DSSequenceCommands.FieldTitleSetDialoguePanel)?.AsInt() ?? (int)SetDialoguePanelType.DoNotUse);
            SetContinueMode setContinueMode = (SetContinueMode)(fields.Find(x => x.title == DSSequenceCommands.FieldTitleSetContinueMode)?.AsInt() ?? (int)SetContinueMode.DoNotUse);
            bool continueCommand = fields.Find(x => x.title == DSSequenceCommands.FieldTitleContinue)?.AsBool() ?? false;

            if (setDialoguePanel != SetDialoguePanelType.DoNotUse)
            {
                bool setDialoguePanelValue = setDialoguePanel == SetDialoguePanelType.True;
                sequence.Append($"SetDialoguePanel({setDialoguePanelValue});\n");
            }

            if (setContinueMode != SetContinueMode.DoNotUse)
            {
                bool setContinueModeValue = setContinueMode == SetContinueMode.True;
                sequence.Append($"SetContinueMode({setContinueModeValue});\n");
            }

            if (continueCommand)
            {
                sequence.Append($"Continue();\n");
            }
            return sequence.ToString();
        }

        private static string ConvertInstructionTimeline(this DialogueEntry entry, ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            bool nowait = fields.Find(x => x.title == TimelineController.FieldTitleNowait)?.AsBool() ?? false;
            bool nostop = fields.Find(x => x.title == TimelineController.FieldTitleNostop)?.AsBool() ?? false;
            bool continueCommand = fields.Find(x => x.title == TimelineController.FieldTitleContinue)?.AsBool() ?? false;
            string nameTimeline = fields.Find(x => x.title == ArticyConstants.FieldTitleTechnicalPostfixName)?.value;

            SetDialoguePanelType setDialoguePanel = (SetDialoguePanelType)(fields.Find(x => x.title == DSSequenceCommands.FieldTitleSetDialoguePanel)?.AsInt() ?? (int)SetDialoguePanelType.DoNotUse);
            SetContinueMode setContinueMode = (SetContinueMode)(fields.Find(x => x.title == DSSequenceCommands.FieldTitleSetContinueMode)?.AsInt() ?? (int)SetContinueMode.DoNotUse);

            if (!string.IsNullOrEmpty(nameTimeline))
            {
                StringBuilder sequence = new StringBuilder($"Timeline(play, {nameTimeline}");
                if (nowait)
                {
                    sequence.Append(", nowait");
                }
                if (nostop)
                {
                    sequence.Append(", nostop");
                }
                sequence.Append(");\n");

                if (setDialoguePanel != SetDialoguePanelType.DoNotUse)
                {
                    bool setDialoguePanelValue = setDialoguePanel == SetDialoguePanelType.True;
                    sequence.Append($"SetDialoguePanel({setDialoguePanelValue});\n");
                }

                if (setContinueMode != SetContinueMode.DoNotUse)
                {
                    bool setContinueModeValue = setContinueMode == SetContinueMode.True;
                    sequence.Append($"SetContinueMode({setContinueModeValue});\n");
                }
                if (continueCommand)
                {
                    sequence.Append($"Continue();\n");
                }

                return sequence.ToString();
            }
            else
            {
                ShowError(fields, conversation, nameof(ConvertInstructionTimeline), nameof(nameTimeline));
                return null;
            }            
        }

        private static string ConvertInstructionTimelineStop(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string nameTimeline = fields.Find(x => x.title == ArticyConstants.FieldTitleTechnicalPostfixName)?.value;
            bool continueCommand = fields.Find(x => x.title == TimelineController.FieldTitleContinue)?.AsBool() ?? false;
            SetDialoguePanelType setDialoguePanel = (SetDialoguePanelType)(fields.Find(x => x.title == DSSequenceCommands.FieldTitleSetDialoguePanel)?.AsInt() ?? (int)SetDialoguePanelType.DoNotUse);
            SetContinueMode setContinueMode = (SetContinueMode)(fields.Find(x => x.title == DSSequenceCommands.FieldTitleSetContinueMode)?.AsInt() ?? (int)SetContinueMode.DoNotUse);

            if (!string.IsNullOrEmpty(nameTimeline))
            {
                StringBuilder sequence = new StringBuilder($"Timeline(stop, {nameTimeline});\n");
                if (setDialoguePanel != SetDialoguePanelType.DoNotUse)
                {
                    bool setDialoguePanelValue = setDialoguePanel == SetDialoguePanelType.True;
                    sequence.Append($"SetDialoguePanel({setDialoguePanelValue});\n");
                }

                if (setContinueMode != SetContinueMode.DoNotUse)
                {
                    bool setContinueModeValue = setContinueMode == SetContinueMode.True;
                    sequence.Append($"SetContinueMode({setContinueModeValue});\n");
                }
                if (continueCommand)
                {
                    sequence.Append($"Continue();\n");
                }
                return sequence.ToString();
            }
            else
            {
                ShowError(fields, conversation, nameof(ConvertInstructionTimelineStop), nameof(nameTimeline));
                return null;
            }
        }

        private static string ConvertInstructionRemoveAttachable(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string luaExpression = "";

            string ownerArticyId = fields.Find(x => x.title == ArticyConstants.FieldTitleOwnerSlot)?.value ?? "";

            if (string.IsNullOrEmpty(ownerArticyId) || ownerArticyId == ArticyConstants.IdEmpty)
            {
                ShowError(fields, conversation, nameof(ConvertInstructionChangeConsumablesAmount), nameof(ownerArticyId));
            }
            
            luaExpression += $"RemoveAttachable(\"{ownerArticyId}\");";
            return luaExpression;
        }
        
        private static string ConvertInstructionNewQuestUnlocked(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string luaExpression = "";

            string questArticyId = fields.Find(x => x.title == ArticyConstants.FieldTitleDialogue)?.value ?? "";

            if (string.IsNullOrEmpty(questArticyId) || questArticyId == ArticyConstants.IdEmpty)
            {
                ShowError(fields, conversation, nameof(ConvertInstructionChangeConsumablesAmount), nameof(questArticyId));
            }
            
            luaExpression += $"InstructionNewQuestUnlocked(\"{questArticyId}\");";
            return luaExpression;
        }

        public static string ConvertInstructionChangeConsumablesAmount(ref List<Field> fields, ref ArticyConverter articyConverter, ref Conversation conversation)
        {
            string luaExpression = "";
            string ownerArticyId = fields.Find(x => x.title == ArticyConstants.FieldTitleOwnerSlot)?.value ?? "";

            if (string.IsNullOrEmpty(ownerArticyId) || ownerArticyId == ArticyConstants.IdEmpty)
            {
                ShowError(fields, conversation, nameof(ConvertInstructionChangeConsumablesAmount), nameof(ownerArticyId));
            }

            int count = fields.GetCount("Consumable_");

            for (int i = 0; i < count; i++)
            {
                string consumableArticyId = fields.Find(x => x.title == "Consumable_" + i).value;
                if (consumableArticyId != ArticyConstants.IdEmpty)
                {
                    string typeChangeValue = fields.Find(x => x.title == "TypeChangeValue_" + i).value;
                    string value = fields.Find(x => x.title == "Value_" + i).value;
                    luaExpression += @$"ChangeConsumablesAmount(""{consumableArticyId}"",{typeChangeValue},{value});";
                }
            }

            return luaExpression;
        }

        private static void ShowError(List<Field> fields, Conversation conversation, string nameMethod, string luaExpression)
        {
            Debug.LogWarning(
                $"{luaExpression} is null"+ 
                $"\n{nameMethod}.conversation - {conversation.fields.Find(x => x.title == ArticyConstants.FieldTitleTitle).value}" +
                "\nTitle - " + fields.Find(x => x.title == ArticyConstants.FieldTitleTitle).value +
                "\nArticyId - " + fields.Find(x => x.title == ArticyConstants.FieldTitleArticyId).value 
                );
        }
        private static bool IsStringValueWrong(string val)
        {
            return string.IsNullOrEmpty(val) || val == ArticyConstants.IdEmpty;
        }

        #endregion

        #region Support

        private static string FindProperty(ref ArticyConverter articyConverter, List<Field> fields, string propertyTitle)
        {
            string id = fields.Find(x => x.title == propertyTitle)?.value ?? "";
            if (id != ArticyConstants.IdEmpty)
            {
                Item item = FindItem(ref articyConverter, id);
                if (item != null)
                {
                    List<Field> fieldProperty = item.fields;
                    return fieldProperty.Find(x => x.title == ArticyConstants.FieldTitleArticyId)?.value ?? "";
                }
                else
                {
                    Debug.LogError($"[exposition settings] Item {propertyTitle} with id {id} not found");
                }
            }
            return "";
        }

        private static string FindActor(ref ArticyConverter articyConverter, List<Field> fields, string propertyTitle)
        {
            string id = fields.Find(x => x.title == propertyTitle)?.value ?? "";
            if (!string.IsNullOrEmpty(id) && id != ArticyConstants.IdEmpty)
            {
              
                Actor actor = FindActor(ref articyConverter, id);
                return actor.GetArticyId();
            }
            return "";
        }

        private static void CopyFields(this List<Field> fields, List<Field> copyingFields)
        {
            foreach (Field field in copyingFields)
            {
               Field.SetValue(fields,field.title,field.value,field.type);
            }
        }

        private static Actor FindActor(ref ArticyConverter articyConverter, string actorArticyId)
        {
            return articyConverter.database.actors.Find(x => x.fields.Find(y => y.title == ArticyConstants.FieldTitleArticyId)?.value == actorArticyId);
        }

        private static Item FindItem(ref ArticyConverter articyConverter, string itemArticyId)
        {
            return articyConverter.database.items.Find(x => x.fields.Find(y => y.title == ArticyConstants.FieldTitleArticyId)?.value == itemArticyId);
        }

        private static string ConvertTypeCompareToLua(ArticyConstants.CompareType value)
        {
            string luaExpression = "";
            switch (value)
            {
                case ArticyConstants.CompareType.Less:
                    luaExpression = "<";
                    break;
                case ArticyConstants.CompareType.LessEqually:
                    luaExpression = "<=";
                    break;
                case ArticyConstants.CompareType.Equally:
                    luaExpression = "==";
                    break;
                case ArticyConstants.CompareType.MoreEqually:
                    luaExpression = ">";
                    break;
                case ArticyConstants.CompareType.More:
                    luaExpression = ">=";
                    break;
                default: break;
            }

            return luaExpression;
        }
        
        private static string GetAllConversationActorIds(ref Conversation conversation)
        {
            var conversationEntries = conversation.dialogueEntries;
            string allConversationActorIds = "";
            for (int i = 0; i < conversationEntries.Count; i++)
            {
                Field actorIdField = conversationEntries[i].fields.Find(el => el.title == ArticyConstants.FieldTitleActorArticyId);
                if (actorIdField == null || string.IsNullOrEmpty(actorIdField.value))
                {
                    continue;
                }
                allConversationActorIds += actorIdField.value + ", ";
            }
            
            return allConversationActorIds;
        }
        
        private static void CheckEmptyField(Conversation conversation, string fieldName, string methodName)
        {
            Field field = conversation.fields.Find(x => x.title == fieldName);
            if (field == null || string.IsNullOrEmpty(field.value) || field.value == ArticyConstants.IdEmpty)
            {
                ShowError(conversation.fields,conversation,methodName,fieldName);
            }
        }

        #endregion
    }
}
