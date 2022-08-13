using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.UI;
using UnityEngine;


[CreateAssetMenu(fileName = "BuyDressSetPopupManager", menuName = "_Game/_Managers/BuyDressSetPopupManager", order = 0)]
public class BuyDressSetPopupManagerSO : ScriptableObject
{
    [SerializeField] private BuyDressPopupViewModel buyDressPopupViewModel;

    private void OnEnable()
    {
        buyDressPopupViewModel.OnShowBuyItemPopup += ShowItemPopup;
        buyDressPopupViewModel.OnShowBuyItemSetPopup += ShowItemSetPopup;
    }
    
    private void OnDisable()
    {
        buyDressPopupViewModel.OnShowBuyItemPopup -= ShowItemPopup;
        buyDressPopupViewModel.OnShowBuyItemSetPopup -= ShowItemSetPopup;
    }

    private void ShowItemPopup()
    {
        var uiPopup = UIPopup.GetPopup("BuyDressItemPopup");
        uiPopup.Show();
    }
    
    private void ShowItemSetPopup()
    {
        var uiPopup = UIPopup.GetPopup("BuyDressItemSetPopup");
        uiPopup.Show();
    }
}
