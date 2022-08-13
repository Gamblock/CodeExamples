using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(AddressablesBuildLauncherSO))]
public class AddressablesBuildLauncherSOEditor : OdinEditor
{
  public override void OnInspectorGUI()
  {
    base.OnInspectorGUI();

    var buildLauncher = (AddressablesBuildLauncherSO) target;
    if(GUILayout.Button("BuildAddressables"))
    {
      buildLauncher.BuildAddressables();
    }
  }
}

  
