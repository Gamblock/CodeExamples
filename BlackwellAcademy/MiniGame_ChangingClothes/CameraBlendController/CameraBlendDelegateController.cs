using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnlockGames.BA.CameraControl;

namespace UnlockGames.BA.MiniGames.DressingUp
{
    [CreateAssetMenu(fileName = "CameraBlendDelegateController", menuName = "Game/DressingUp/CameraBlendDelegateController")]
    public class CameraBlendDelegateController : ScriptableObject
    {
        public Action<CameraPreset, Transform> ChangeCameraByPreset = (_, __) => { };
        
        public Action DecreaseCamerasPriority = ( ) => { };
    }
}

