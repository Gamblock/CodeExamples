using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Articy.Unity.Utils;
using UnityEngine;
#if UNITY_EDITOR
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
#endif


[CreateAssetMenu(fileName = "AddressablesBuildLauncherSO", menuName = "_Game/Addressables/AddressablesBuildLauncherSO", order = 0)]
public class AddressablesBuildLauncherSO : ScriptableObject
{
  public List<StorySO> stories;
  public string profileName = "Default";
//#if UNITY_EDITOR
  public AddressableAssetGroupSchema defaultGroup_PlayAssetDeliverySchema;
  public BuildScriptPackedMode buildScriptPackedMode;
  
  private AddressableAssetSettings settings;

  public void BuildAddressables()
  {
    // settings cannot be assigned manually - this SO is not in the project.
    // the AddressableAssetSettings prefab in the project is fake - you can't serialize changes through it!
    // need a default setting from the package cache  
    settings = AddressableAssetSettingsDefaultObject.Settings;

    RemoveAllLabels(settings);
    SetAddressablesGroups(settings);
    RemoveAllEmptyGroups(settings);
    SetProfile(profileName, settings);
    SetBuilder(buildScriptPackedMode, settings);
    BuildAddressableContent();
  }

  private void SaveEditor([NotNull] UnityEngine.Object target) //required for serialization. analog ctrl+s
  {
    EditorUtility.SetDirty(target);
    AssetDatabase.SaveAssets(); 
    AssetDatabase.Refresh(); 
  }

  private void RemoveAllLabels(AddressableAssetSettings settings)
  {
    var labels = settings.GetLabels();
    foreach (var label in labels)
    {
      settings.RemoveLabel(label);
    }

    SaveEditor(settings);
  }

  private void SetAddressablesGroups(AddressableAssetSettings settings)
  {
    foreach (var story in stories)
    {
      foreach (var segments in story.levelsOrdered)
      {
        var label = $"{story.name}_{segments.addressableSegmentLabel}";
        settings.AddLabel(label); 

        foreach (var levelCase in segments.segment)
        {
          var levelPrefab = levelCase.levelSO.GetLevelPrefabReference();
          
          if (levelPrefab != null && !levelPrefab.AssetGUID.IsNullOrEmpty()) //empty field in inspector is not null value for addressables! only GUID gives the right information
          {
            
            if (levelCase.levelSO.TryRemoveLevelPrefab()) // remove the non addressable prefab
            {
              EditorUtility.SetDirty(levelCase.levelSO); //required for serialization.
            }

            var entry = settings.FindAssetEntry(levelPrefab.AssetGUID);

            //move entry to group
            var group = settings.FindGroup(levelCase.levelSO.GetSceneName());
            if (group == null)
            {
              group = settings.CreateGroup(levelCase.levelSO.GetSceneName(), false, false, false, settings.DefaultGroup.Schemas, defaultGroup_PlayAssetDeliverySchema.GetType());
            }
            settings.MoveEntry(entry, group);

            if (entry == null)
            {
              Debug.LogError($" no prefab assigned : {levelCase.levelSO.name}");
            }

            entry.SetLabel(label, true, true, true);
          }
        }
      }
    }

    SaveEditor(settings);
  }  
  
  private void RemoveAllEmptyGroups(AddressableAssetSettings settings) //deleting old empty groups
  {
    for (int i = 0; i < settings.groups.Count; i++) 
    {
      var group = settings.groups[i];
      if (group.entries.Count == 0 && group.Name != "Default Local Group")
      {
        settings.RemoveGroup(group);
      }
    }
    SaveEditor(settings);
  }

  private void SetProfile(string profile, AddressableAssetSettings settings)
  {
    string profileId = settings.profileSettings.GetProfileId(profile);
    if (String.IsNullOrEmpty(profileId))
      Debug.LogWarning($"Couldn't find a profile named, {profile}, " +
                       $"using current profile instead.");
    else
      settings.activeProfileId = profileId;
  }

  private void SetBuilder(IDataBuilder builder, AddressableAssetSettings settings)
  {
    int index = settings.DataBuilders.IndexOf((ScriptableObject) builder);

    if (index > 0)
      settings.ActivePlayerDataBuilderIndex = index;
    else
      Debug.LogWarning($"{builder} must be added to the " +
                       $"DataBuilders list before it can be made " +
                       $"active. Using last run builder instead.");
  }

  private void BuildAddressableContent()
  {
    AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);
    bool success = string.IsNullOrEmpty(result.Error);
    if (!success)
    {
      Debug.LogError("Addressables build error encountered: " + result.Error);
    }
  }

  //#endif 
}