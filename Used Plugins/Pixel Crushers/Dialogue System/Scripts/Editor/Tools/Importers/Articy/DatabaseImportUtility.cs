using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PixelCrushers.DialogueSystem.Articy
{
  public class DatabaseImportUtility
  {
    public static void CopyCallbacksInConversations(
      DialogueDatabase fromDatabase,
      DialogueDatabase toDatabase,
      string fieldsPropertyName,
      string fieldNamePropertyName,
      string fieldValuePropertyName,
      string keyFieldName,
      string conversationsPropertyName,
      string entriesPropertyName,
      string callbacksPropertyName)
    {
      var fromObject = new SerializedObject(fromDatabase);
      var toObject = new SerializedObject(toDatabase);

      var fromConversations = fromObject.FindProperty(conversationsPropertyName);
      var toConversations = toObject.FindProperty(conversationsPropertyName);

      for (int i = 0; i < fromConversations.arraySize; i++)
      {
        var fromConversation = fromConversations.GetArrayElementAtIndex(i);
        var fromKeyFieldValue = GetKeyFieldValue(fromConversation, fieldsPropertyName,
          fieldNamePropertyName, fieldValuePropertyName, keyFieldName);

        var toConversation = fromConversation;

        for (int j = 0; j < toConversations.arraySize; j++)
        {
          var conversation = toConversations.GetArrayElementAtIndex(j);
          var keyFieldValue = GetKeyFieldValue(conversation, fieldsPropertyName,
            fieldNamePropertyName, fieldValuePropertyName, keyFieldName);
          if (keyFieldValue == fromKeyFieldValue)
          {
            toConversation = conversation;
            break;
          }
        }

        if (toConversation == fromConversation)
        {
          continue;
        }

        CopySingleConversationCallbacks(
          fromObject,
          toObject,
          fromConversation,
          toConversation,
          fieldsPropertyName,
          fieldNamePropertyName,
          fieldValuePropertyName,
          keyFieldName,
          entriesPropertyName,
          callbacksPropertyName);
      }
    }

    private static string GetKeyFieldValue(
      SerializedProperty property,
      string fieldsPropertyName,
      string fieldNamePropertyName,
      string fieldValuePropertyName,
      string keyFieldName)
    {
      var fields = property.FindPropertyRelative(fieldsPropertyName);
      for (int i = 0; i < fields.arraySize; i++)
      {
        var field = fields.GetArrayElementAtIndex(i);
        var fieldName = field.FindPropertyRelative(fieldNamePropertyName).stringValue;
        if (fieldName != keyFieldName)
        {
          continue;
        }

        return field.FindPropertyRelative(fieldValuePropertyName).stringValue;
      }

      return null;
    }

    private static void CopySingleConversationCallbacks(
      SerializedObject fromDatabase,
      SerializedObject toDatabase,
      SerializedProperty fromConversation,
      SerializedProperty toConversation,
      string fieldsPropertyName,
      string fieldNamePropertyName,
      string fieldValuePropertyName,
      string keyFieldName,
      string entriesPropertyName,
      string callbacksPropertyName)
    {
      var fromEntries = fromConversation.FindPropertyRelative(entriesPropertyName);
      var toEntries = toConversation.FindPropertyRelative(entriesPropertyName);

      for (int i = 0; i < fromEntries.arraySize; i++)
      {
        var fromEntry = fromEntries.GetArrayElementAtIndex(i);
        var fromKeyFieldValue = GetKeyFieldValue(fromEntry, fieldsPropertyName,
          fieldNamePropertyName, fieldValuePropertyName, keyFieldName);

        var toEntry = fromEntry;
        for (int j = 0; j < toEntries.arraySize; j++)
        {
          var entry = toEntries.GetArrayElementAtIndex(j);
          var keyFieldValue = GetKeyFieldValue(entry, fieldsPropertyName,
            fieldNamePropertyName, fieldValuePropertyName, keyFieldName);
          if (keyFieldValue == fromKeyFieldValue)
          {
            toEntry = entry;
            break;
          }
        }

        if (toEntries == fromEntry)
        {
          continue;
        }

        PersistentCallUtils.PersistentCallUtils.TransferPersistentCalls(
          fromDatabase,
          toDatabase,
          fromEntry.FindPropertyRelative(callbacksPropertyName),
          toEntry.FindPropertyRelative(callbacksPropertyName),
          callbacksPropertyName,
          callbacksPropertyName,
          false,
          false);
      }
    }

    public static void CopyFieldsInConversations(DialogueDatabase fromDatabase, DialogueDatabase toDatabase,
      string keyFieldName)
    {
      foreach (var fromConversation in fromDatabase.conversations)
      {
        if (!fromConversation.FieldExists(keyFieldName))
        {
          Debug.LogError($"Conversation named {fromConversation.Name} " +
                         $"in local database does not contain key field named {keyFieldName}.");
          return;
        }

        var toConversation = toDatabase.conversations.Find(el =>
          el.AssignedField(keyFieldName).value == fromConversation.AssignedField(keyFieldName).value);
        if (toConversation == null)
        {
          continue;
        }

        CopySingleConversationFields(fromConversation, toConversation, keyFieldName);
      }
    }

    private static void CopySingleConversationFields(Conversation fromConversation, Conversation toConversation,
      string keyFieldName)
    {
      foreach (var fromEntry in fromConversation.dialogueEntries)
      {
        var fromKeyField = fromEntry.fields.Find(field => field.title == keyFieldName);
        if (fromKeyField == null)
        {
          Debug.LogError($"Entry with id {fromEntry.id} " +
                         $"in conversation {fromConversation.Name}" +
                         $"in local database does not contain key field named {keyFieldName}.");
          return;
        }

        var toEntry = toConversation.dialogueEntries.Find(entry =>
          entry.fields.Find(field =>
            field.title == keyFieldName && field.value == fromKeyField.value) != null);
        if (toEntry == null)
        {
          continue;
        }

        CopyUniqueFieldsToEntry(fromEntry, toEntry);
      }
    }

    private static void CopyUniqueFieldsToEntry(DialogueEntry fromEntry, DialogueEntry toEntry)
    {
      foreach (var field in fromEntry.fields)
      {
        if (toEntry.fields.Select(el => el.title).Contains(field.title))
        {
          continue;
        }

        toEntry.fields.Add(field);
      }
    }
  }
}