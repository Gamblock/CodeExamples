using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine;
using UnityEngine;

[CreateAssetMenu(fileName = "DressingViewModelSO", menuName = "_Game/DressingViewModelSO", order = 0)]
public class DressingViewModelSO : CharacterCustomizationViewModelSO
{
    public event Action<ClothesCategory> OnCategoryButtonClicked = category => { };
    public event Action OnBuyVipSetClicked = () => { };
    public event Action OnDressingScenePreviewShown = () => { };
    
    public ObservableVariable<bool> VipSetBought = new ObservableVariable<bool>(false);

    public SkinnedMeshRenderer currentBody;

    public void CategoryButtonClicked(ClothesCategory category)
    {
        OnCategoryButtonClicked.Invoke(category);
    }

    public void BuyVipSetClicked()
    {
        OnBuyVipSetClicked.Invoke();
    }

    public void HandleDressingScenePreviewShown()
    {
        OnDressingScenePreviewShown.Invoke();
    }

    public void SetState(DressingViewModelState state, string gameEventMessage = "")
    {
        Debug.Log(gameEventMessage);
        var uiState = GetUIState(state);

        isApplyButtonActive.Value = uiState.isApplyButtonActive;
        
        GameEventMessage.SendEvent(gameEventMessage);
    }

    private DressingViewModelUIState GetUIState(DressingViewModelState state)
    {
        switch (state)
        {
            case DressingViewModelState.Completed completed:
                return new DressingViewModelUIState();
            case DressingViewModelState.Undressed undressed:
                return new DressingViewModelUIState();
            case DressingViewModelState.Dressed dressed:
                return new DressingViewModelUIState(true);
            default:
                throw new ArgumentOutOfRangeException(nameof(state));
        }
    }
    
}

public abstract class DressingViewModelState
{
    protected DressingViewModelState() { }

    public class Undressed: DressingViewModelState
    {
        public Undressed() { }
    }
    
    public class Dressed: DressingViewModelState
    {
        public Dressed() { }
    }
    
    public class Completed: DressingViewModelState
    {
        public Completed() { }
    }
}

struct DressingViewModelUIState
{
    public bool isApplyButtonActive;

    public DressingViewModelUIState(bool isApplyButtonActive = false)
    {
        this.isApplyButtonActive = isApplyButtonActive;
    }
}