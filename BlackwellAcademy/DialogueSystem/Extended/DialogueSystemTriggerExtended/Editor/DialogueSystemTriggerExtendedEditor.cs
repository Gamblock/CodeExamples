using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueSystemTriggerExtended), true)]
public class DialogueSystemTriggerExtendedEditor : DialogueSystemTriggerEditor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_dressingUpViewModel"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_clothesSlotVisibilityController"), true);
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }
}
