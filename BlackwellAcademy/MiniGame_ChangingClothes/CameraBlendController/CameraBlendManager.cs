using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnlockGames.BA.CameraControl;

namespace UnlockGames.BA.MiniGames.DressingUp
{
    public class CameraBlendManager : MonoBehaviour
    {
        [SerializeField] private CameraBlendDelegateController _cameraBlendDelegateController;
        
        [Space(5f)]
        [SerializeField] private CinemachineVirtualCamera _cameraA;
        [SerializeField] private CinemachineVirtualCamera _cameraB;
        [SerializeField] private Transform _handCameraA;
        [SerializeField] private Transform _handCameraB;
        
        private static int highPriority = 20000;
        private static int lowPriority = 1;

        private void OnEnable()
        {
            _cameraBlendDelegateController.ChangeCameraByPreset += SwitchCameras;
            _cameraBlendDelegateController.DecreaseCamerasPriority += DecreaseCamerasPriority;
        }

        private void OnDisable()
        {
            _cameraBlendDelegateController.ChangeCameraByPreset -= SwitchCameras;
            _cameraBlendDelegateController.DecreaseCamerasPriority -= DecreaseCamerasPriority;
        }

        public void SwitchCameras(CameraPreset preset, Transform targetForCamera)
        {
            DetectCamerasByPriority(out CinemachineVirtualCamera highest, out CinemachineVirtualCamera lowest, out Transform lowestHand);
            
            lowestHand.position = targetForCamera.position;
            lowestHand.rotation = targetForCamera.rotation;
            lowest.transform.localPosition = preset.transform.position;
            lowest.transform.localRotation = preset.transform.rotation;

            lowest.Priority = highPriority;
            highest.Priority = lowPriority;
        }

        private void DetectCamerasByPriority(out CinemachineVirtualCamera currentHighest,
            out CinemachineVirtualCamera currentLowest, out Transform currentLowestHand)
        {
            if (_cameraA.Priority >= _cameraB.Priority)
            {
                currentHighest = _cameraA;
                currentLowest = _cameraB;
                currentLowestHand = _handCameraB;
            }
            else
            {
                currentHighest = _cameraB;
                currentLowest = _cameraA;
                currentLowestHand = _handCameraA;
            }
        }

        public void DecreaseCamerasPriority()
        {
            _cameraA.Priority = lowPriority;
            _cameraB.Priority = lowPriority;
        }
    }
}

