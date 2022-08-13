using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public Vector3EventChannelSO OnPositionParticlesSet;
    public VoidEventChannelSO OnCameraToCompletedPositionSet;
    public ClothesCategoryEventChannelSO OnCameraFocusSet;
    public DressingCamera startCamera;
    public DressingCamera completedCamera;
    public DressingCamera faceCamera;
    public DressingCamera headCamera;
    public DressingCamera bodyCamera;
    public DressingCamera pantsCamera;
    public DressingCamera shoesCamera;
    public GameObject virtualCamerasParent;
    public DressingPopupViewModel dressingPopupViewModel;
    
    DressingCamera currentCamera;
    public DressingCamera CurrentCamera
    {
        get => currentCamera;
        set
        {
            currentCamera.virtualCamera.Priority--;
            currentCamera = value;
            currentCamera.virtualCamera.Priority++;
        }
    }

    private void DisableVirtualCameras()
    {
        virtualCamerasParent.SetActive(false);
    }

    private void EnableVirtualCameras()
    {
        virtualCamerasParent.SetActive(true);
    }
    private void OnEnable()
    {
        OnCameraFocusSet.OnEventRaised += SetCameraFocus;
        OnCameraToCompletedPositionSet.OnEventRaised += SetCameraToCompletedPosition;
        dressingPopupViewModel.onClosePopup += DisableVirtualCameras;
        dressingPopupViewModel.onShowPopup += EnableVirtualCameras;
    }

    private void OnDisable()
    {
        dressingPopupViewModel.onClosePopup -= DisableVirtualCameras;
        dressingPopupViewModel.onShowPopup -= EnableVirtualCameras;
        OnCameraFocusSet.OnEventRaised -= SetCameraFocus;
        OnCameraToCompletedPositionSet.OnEventRaised -= SetCameraToCompletedPosition;
    }
    
    private void Start()
    {
        currentCamera = startCamera;
        currentCamera.virtualCamera.Priority += 1;
    }
    
    void SetCameraToCompletedPosition()
    {
        SetCamera(completedCamera);
    }

    void SetCameraFocus(ClothesCategory focus)
    {
        if (!currentCamera.virtualCamera) return;

        switch (focus)
        {
            case var f when f == ClothesCategory.DEFAULT || f == ClothesCategory.HAIR:
                SetCamera(headCamera);
                break;
            case var f when f == ClothesCategory.DRESS || f == ClothesCategory.BODY:
                SetCamera(bodyCamera);
                break;
            case var f when f == ClothesCategory.NECKLACE || f == ClothesCategory.MAKEUP:
                SetCamera(faceCamera);
                break;
            case ClothesCategory.PANTS:
                SetCamera(pantsCamera);
                break;
            case ClothesCategory.SHOES:
                SetCamera(shoesCamera);
                break;
        }
    }

    void SetCamera(DressingCamera camera)
    {
        CurrentCamera = camera;
        OnPositionParticlesSet.RaiseEvent(camera.particlePlace.position);
    }
}


[Serializable]
public struct DressingCamera
{
    public CinemachineVirtualCamera virtualCamera;
    public Transform particlePlace;
}


