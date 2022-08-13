
using PixelCrushers.DialogueSystem;
using UnityEditor;
using UnlockGames.BA.DialogueSystem.UI;

namespace UnlockGames.BA.MiniGames.DressingUp.UI
{
    [CustomEditor(typeof(DressingUpUIMenuPanel), true)]
    public class DressingUpUIMenuPanelEditor : StandardUIMenuPanelEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            AddCustomUIElements();
            serializedObject.ApplyModifiedProperties();
            
            base.OnInspectorGUI();
        }

        // Add here all fields for DressingUpUIMenuPanel that need to be serialized
        private void AddCustomUIElements()
        {
            EditorGUILayout.LabelField("Custom Elements", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_dressingUpViewModel"), true);
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_lockClothesChoiceButton"), true);
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_clothesBtn"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_premiumClothesPriceGobj"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_premiumClothesPriceTxt"), true);
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentAmountOfKeysPanel"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentAmountOfKeysText"), true);
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_noClothesSelectedColor"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_defaultSelectedColor"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_premiumSelectedColor"), true);
            
            // add here more elements
        }
    }
}