using System;
using _Game.Scripts.LevelLogic.LevelReward;
using Doozy.Engine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DressingVipButtonController : MonoBehaviour
{
  public InAppPurchaseViewModel inAppPurchaseViewModel;
  public DressingViewModelSO dressingViewModelSO;
  public UIPopup uIPopup;
  public Button vipButton;
  public Button premiumVIPButton;
  public TextMeshProUGUI premiumPrice;
  public PricesContainerHolder pricesContainerHolder;
  private PricesContainerSO pricesContainerSO => pricesContainerHolder.GetPricesContainer();

  private void Awake()
  {
    vipButton.enabled = false;
    premiumVIPButton.enabled = false;
  }

  private void OnEnable()
  {
    uIPopup.ShowBehavior.OnFinished.Event.AddListener(EnableVipButton);
    
    vipButton.onClick.AddListener(OnVipButtonClick);
    premiumVIPButton.onClick.AddListener(OnVipButtonClick);
    
    vipButton.gameObject.SetActive(!inAppPurchaseViewModel.IsPremiumCurrencyMode());
    premiumVIPButton.gameObject.SetActive(inAppPurchaseViewModel.IsPremiumCurrencyMode());
    premiumPrice.text = pricesContainerSO.dressingPremiumItemPrice.ToString();
  }

  private void OnDisable()
  {
    uIPopup.HideBehavior.OnStart.Event.RemoveListener(EnableVipButton);
    
    vipButton.onClick.RemoveListener(OnVipButtonClick);
    premiumVIPButton.onClick.RemoveListener(OnVipButtonClick);
  }

  void EnableVipButton()
  {
    vipButton.enabled = true; //Doozy enables on button incorrectly on some devices. This  insures correct button activation.
    premiumVIPButton.enabled = true;
  }
  
  void OnVipButtonClick()
  {
    dressingViewModelSO.BuyVipSetClicked();
  }
  
}