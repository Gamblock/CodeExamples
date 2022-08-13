using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;

public class CameraControllerDressingPopup : MonoBehaviour
{
    public CinemachineBrain mainCamera;
    public DressingPopupViewModel dressingPopupViewModel;

    private void OnEnable()
    {
        dressingPopupViewModel.onShowPopup += CutToDressingCameraImmidiaetly;
    }

    private void OnDisable()
    {
        dressingPopupViewModel.onShowPopup -= CutToDressingCameraImmidiaetly;
    }

    private async void CutToDressingCameraImmidiaetly()
    {
        var startingBlend = mainCamera.m_DefaultBlend;
        mainCamera.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0);
        await Task.Delay(TimeSpan.FromSeconds(0.1));
        mainCamera.m_DefaultBlend = startingBlend;
    }
}
