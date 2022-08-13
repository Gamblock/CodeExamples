using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using System.Linq;
using PixelCrushers.DialogueSystem.Articy;
using PixelCrushers.DialogueSystem.Articy.Articy_3_1;
using UnityEngine;
using UnlockGames.BA.Core.DialogueSystem;
using UnlockGames.BA.Game.Consumables;
using Location = UnlockGames.BA.Game.Locations.Location;

namespace UnlockGames.BA.Core.DialogueSystem
{
    public static class LuaExtended
    {
        public static DialogueEntry FindDialogueEntry(string dialogueSlotArticyId, out Conversation conversation)
        {
            conversation = DialogueManager.Instance.initialDatabase.conversations.Find(x =>
                x.dialogueEntries.Exists(
                    y => y.fields.Exists(
                        z => z.title == ArticyConstants.FieldTitleArticyId && z.value == dialogueSlotArticyId)));
            return conversation.dialogueEntries.Find(y => y.fields.Exists(z => z.title == ArticyConstants.FieldTitleArticyId && z.value == dialogueSlotArticyId));
        }

        public static Field FindActorField(ref Actor actor, string valueField, string fieldPartName)
        {
            return actor.fields.Find(x => x.value == valueField && x.title.Contains(fieldPartName));
        }

        public static Field FindActorField(this Actor actor, string valueField, string fieldPartName)
        {
            return actor.fields.Find(x => x.value == valueField && x.title.Contains(fieldPartName));
        }

        public static Field FindActorField(string actorArticyId, string valueField, string fieldPartName)
        {
            return FindActorByArticyId(actorArticyId).fields.Find(x => x.value == valueField && x.title.Contains(fieldPartName));
        }

        public static Field FindActorField(string actorArticyId, string fieldName)
        {
            return FindActorByArticyId(actorArticyId).fields.Find(x => x.title.Contains(fieldName));
        }

        public static Actor FindActorByArticyId(ref DialogueDatabase dialogueDatabase, string articyId)
        {
            return dialogueDatabase.actors.Find(x => x.fields.Exists(y => y.title == ArticyConstants.FieldTitleArticyId && y.value == articyId));
        }

        public static Actor FindActorByArticyId(string articyId)
        {
            return FindActorByArticyId(ref DialogueManager.instance.initialDatabase, articyId);
        }

        public static string[] SplitStrip(string subtableStrip)
        {
            return subtableStrip.Split(';').Where(x => !string.IsNullOrEmpty(x)).ToArray();
        }
        public static string GetArticyTechnicalName(this Asset asset, ref DialogueDatabase dialogueDatabase)
        {
            return asset.fields.Find(x => x.title == ArticyConstants.FieldTitleArticyTechnicalName).value;
        }

        public static string GetArticyTechnicalName(this Asset asset)
        {
            return asset.GetArticyTechnicalName(ref DialogueManager.instance.initialDatabase);
        }

  
        public static string GetArticyId(this Asset asset)
        {
            return asset.fields.GetArticyId();
        }

        public static string GetArticyId(this List<Field> fields)
        {
            return fields.Find(x => x.title == ArticyConstants.FieldTitleArticyId).value;
        }

        public static int GetCount(this List<Field> fields, string fieldTitlePrefix)
        {
            int count = 0;
            while (!string.IsNullOrEmpty(fields.Find(x => x.title == fieldTitlePrefix + count)?.value))
            {
                count++;
            }
            return count;
        }

        public static string GetTechnicalPrefixName(this Asset asset)
        {
            return asset.fields.Find(x => x.title == ArticyConstants.FieldTitleTechnicalPrefixName)?.value;
        }
        public static string GetTechnicalPostfixName(this Asset asset)
        {
            return asset.fields.Find(x => x.title == ArticyConstants.FieldTitleTechnicalPostfixName)?.value;
        }
        public static void SetField(this Item item, string titleName, object value)
        {
            DialogueLua.SetItemField(item.Name, titleName, value);
        }

        public static Actor FindActorByArticyTechnicalName(ref DialogueDatabase dialogueDatabase, string technicalName)
        {
            return dialogueDatabase.actors.Find(x => x.fields.Exists(y => y.title == ArticyConstants.FieldTitleArticyTechnicalName && y.value == technicalName));
        }

        public static string GetLocalizedDescription(string articyIdItem)
        {
            return DialogueLua.GetLocalizedItemField(
                FindItemByArticyId(ref DialogueManager.Instance.initialDatabase,
                    FindItemByArticyId(ref DialogueManager.Instance.initialDatabase, articyIdItem).fields.Find(x => x.title == "Description1").value).Name, "LocalizedText").asString;
        }

        public static string GetLocalizedContent(string articyIdItem)
        {
            return DialogueLua.GetLocalizedItemField(
                FindItemByArticyId(ref DialogueManager.Instance.initialDatabase,
                    FindItemByArticyId(ref DialogueManager.Instance.initialDatabase, articyIdItem).fields.Find(x => x.title == "Content").value).Name, "LocalizedText").asString;
        }

        public static bool AsBool(this Field field)
        {
            return field.value == "True" || field.value == "1";
        }

        public static string ConvertToLuaStringBool(this bool value)
        {
            return value ? "True" : "False";
        }

        public static bool AsBool(this string value)
        {
            return value.ToLower() == "True".ToLower() || value == "1";
        }

        public static bool AsBool(this double value)
        {
            return (int) value == 1;
        }

        public static int AsInt(this Field field)
        {
            return int.Parse(field.value);
        }

        public static float AsFloat(this Field field)
        {
            return int.Parse(field.value);
        }

        public static float PercentAsFloat(this Field field)
        {
            return int.Parse(field.value) / 100;
        }

        public static int AsInt(string value)
        {
            return int.Parse(value);
        }

        public static Item FindItemByArticyId(ref DialogueDatabase dialogueDatabase, string articyId)
        {
            return dialogueDatabase.items.Find(x => x.fields.Exists(y => y.title == ArticyConstants.FieldTitleArticyId && y.value == articyId));
        }

        public static Item FindItemByArticyId(string articyId)
        {
            return FindItemByArticyId(ref DialogueManager.Instance.initialDatabase, articyId);
        }

  
        public static Item FindItemByTechnicalName(ref DialogueDatabase dialogueDatabase, string technicalName)
        {
            return dialogueDatabase.items.Find(x => x.fields.Exists(y => y.title == ArticyConstants.FieldTitleArticyTechnicalName && y.value == technicalName));
        }

        public static Item FindItemByTechnicalName(string technicalName)
        {
            return FindItemByTechnicalName(ref DialogueManager.Instance.initialDatabase, technicalName);
        }
        public static Item FindItemByFullTechnicalName(ref DialogueDatabase dialogueDatabase, string technicalPrefixName, string technicalPostfixName)
        {
            return dialogueDatabase.items.Find(x => x.fields.Exists(y => (y.title == ArticyConstants.FieldTitleTechnicalPrefixName && y.value == technicalPrefixName) 
                                                                         && (y.title == ArticyConstants.FieldTitleTechnicalPostfixName && y.value == technicalPostfixName)));
        }

        public static Item FindItemByFullTechnicalName(string technicalPrefixName, string technicalPostfixName)
        {
            return FindItemByFullTechnicalName(ref DialogueManager.Instance.initialDatabase, technicalPrefixName,technicalPostfixName);
        }
      
        #region Location
        public static PixelCrushers.DialogueSystem.Location FindLocationByArticyId(ref DialogueDatabase dialogueDatabase, string articyId)
        {
            return dialogueDatabase.locations.Find(x => x.fields.Exists(y => y.title == ArticyConstants.FieldTitleArticyId && y.value == articyId));
        }
        public static PixelCrushers.DialogueSystem.Location FindLocationByArticyId(string articyId)
        {
            return FindLocationByArticyId(ref DialogueManager.Instance.initialDatabase, articyId);
        }
        public static List<PixelCrushers.DialogueSystem.Location> FindLocationsByProperty(string propertyName)
        {
            return FindLocationsByProperty(ref DialogueManager.instance.initialDatabase, propertyName);
        }
        public static List<PixelCrushers.DialogueSystem.Location> FindLocationsByProperty(ref DialogueDatabase dialogueDatabase, string propertyName)
        {
           
            return dialogueDatabase.locations.FindAll(x => x.fields.Exists(y => y.title == propertyName && y.value.AsBool()));
        }
        #endregion
        #region Conversation
        public static Conversation FindConversationByArticyId(ref DialogueDatabase dialogueDatabase, string articyId)
        {
            return dialogueDatabase.conversations.Find(x => x.fields.Exists(y => y.title == ArticyConstants.FieldTitleArticyId && y.value == articyId));
        }
        public static Conversation FindConversationByArticyId(string articyId)
        {
            return FindConversationByArticyId(ref DialogueManager.Instance.initialDatabase, articyId);
        }
      
        #endregion
        public static Item FindItemByProperty(ref DialogueDatabase dialogueDatabase, string propertyName)
        {
            return dialogueDatabase.items.Find(x => x.fields.Exists(y => y.title == propertyName && y.value.AsBool()));
        }
        public static Actor FindActorByProperty(ref DialogueDatabase dialogueDatabase, string propertyName)
        {
            return dialogueDatabase.actors.Find(x => x.fields.Exists(y => y.title == propertyName && y.value.AsBool()));
        }
        public static Actor FindActorByProperty(string propertyName)
        {
           return FindActorByProperty(ref DialogueManager.instance.initialDatabase, propertyName);
        }
        public static List<Item> FindItemsByPropertyValue(ref DialogueDatabase dialogueDatabase, string property)
        {
            return dialogueDatabase.items.FindAll(x => x.fields.Exists(y => y.title == property));
        }

        public static List<Item> FindItemsByProperty(ref DialogueDatabase dialogueDatabase, string propertyName)
        {
            return dialogueDatabase.items.FindAll(x => x.fields.Exists(y => y.title == propertyName && y.value.AsBool()));
        }

        
        public static List<Item> FindItemsByPropertyValue(string property)
        {
            return FindItemsByPropertyValue(ref DialogueManager.instance.initialDatabase, property);
        }

        public static List<Item> FindItemsByProperty(string propertyName)
        {
            return FindItemsByProperty(ref DialogueManager.instance.initialDatabase, propertyName);
        }

        public static Item FindItemByProperty(string propertyName)
        {
            return FindItemByProperty(ref DialogueManager.instance.initialDatabase, propertyName);
        }

        public static void SetTablePair(this Item item, string key, string value, string tableName)
        {
            SetTablePair(key, value, tableName, item.Name);
        }

        public static void SetTablePair(string key, string value, string tableName, string itemName)
        {
            if (string.IsNullOrEmpty(DialogueLua.GetItemField(itemName, $"{ArticyConstants.FieldTitlePrefixSubtable}{tableName}").asString))
            {
                string pairKeyValue = $"{key}:{value};";
                DialogueLua.SetItemField(itemName, $"{ArticyConstants.FieldTitlePrefixSubtable}{tableName}", pairKeyValue);
                Debug.Log($"{nameof(LuaExtended)}.{nameof(SetTablePair)}() itemName: {itemName}, fullTableName: {ArticyConstants.FieldTitlePrefixSubtable}{tableName}, pairKeyValue {pairKeyValue}");
            }
            else
            {
                bool isPairAdded = false;
                string table = DialogueLua.GetItemField(itemName, $"{ArticyConstants.FieldTitlePrefixSubtable}{tableName}").asString;
                string[] tableElements =  SplitStrip(table);
                
                for (int i = 0; i < tableElements.Length; i++)
                {
                    string[] pairKeyValueSplitted = tableElements[i].Split(':');
                    if (pairKeyValueSplitted[0] == key)
                    {
                        string v = value;
                        string k = key;
                        tableElements[i] = $"{k}:{v};";
                        isPairAdded = true;
                    }
                }

                string listPair = "";
                for (int i = 0; i < tableElements.Length; i++)
                {
                    if (tableElements[i].Last().ToString() != ";")
                    {
                        tableElements[i] = $"{tableElements[i]};";
                    }

                    listPair += tableElements[i];
                }

                if (!isPairAdded)
                {
                    string v = value;
                    string k = key;
                    listPair += $"{k}:{v};";
                }

                DialogueLua.SetItemField(itemName, $"{ArticyConstants.FieldTitlePrefixSubtable}{tableName}", listPair);
            }
        }

        public static QuestState AsQuestState(this Field field)
        {
            return QuestLog.StringToState(field.value);
        }
    }
}
