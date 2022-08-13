using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BuyDressPopupViewModel", menuName = "_Game/_ViewModels/BuyDressPopupViewModel", order = 0)]
public class BuyDressPopupViewModel : ScriptableObject
{
    public event Action OnShowBuyItemPopup = () => { };
    public event Action OnShowBuyItemSetPopup = () => { };
    public event Action<BuyDressSetPopupState> OnUpdatePopup = (x) => { };

    public void UpdatePopup(BuyDressSetPopupState itemSet)
    {
        OnUpdatePopup.Invoke(itemSet);
    }
    
    public void ShowItemPopup()
    {
        OnShowBuyItemPopup.Invoke();
    }
    
    public void ShowItemSetPopup()
    {
        OnShowBuyItemSetPopup.Invoke();
    }
}
