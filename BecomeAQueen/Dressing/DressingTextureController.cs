using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DressingTextureController : MonoBehaviour
{
    public Texture2D startingTexture;
    public Texture2D newTexture;
    private Material materialToUpdate;
    public DressingViewModelSO viewModel;

    private Renderer bodyMesh;
    private void OnEnable()
    {
        if (viewModel.currentBody != null)
        {
            bodyMesh = viewModel.currentBody;
            materialToUpdate = bodyMesh.materials[0];
            materialToUpdate.mainTexture = newTexture;
        }
        
    }

    public void UpdateTexture(Renderer currentBody)
    {
        bodyMesh = currentBody;
        materialToUpdate = bodyMesh.materials[0];
        materialToUpdate.mainTexture = newTexture;
    }
    private void OnDestroy()
    {
        ResetTexture();
    }

    public void ResetTexture()
    {
        materialToUpdate.mainTexture = startingTexture;
    }
}
