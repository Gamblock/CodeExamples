using System;
using ComfortGames.CharacterCustomization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSetEquipButton : MonoBehaviour
{
    public Image backGroundImage;
    public Image icon;
    public Image borderImage;
    public Image lockImage;
    public Button button;
    public DressingPopupViewModel dressingPopupViewModel;
    public TextMeshProUGUI priceText;
    public Image premiumCurrencyBackground;
    public Image standardCurrencyBackground;

    private bool isOpen;
    private string ID;
    private int price;

    private void Awake()
    {
        button.onClick.AddListener(SetLastSelectedItemID);
    }

    private void OnEnable()
    {
        dressingPopupViewModel.onItemPurchased += HideLockOnItemPurchased;
    }

    private void OnDisable()
    {
        dressingPopupViewModel.onItemPurchased -= HideLockOnItemPurchased;
    }

    private void SetLastSelectedItemID()
    {
        if (!isOpen)
        {
            dressingPopupViewModel.lastSelectedItemID.Value = ID;
            dressingPopupViewModel.TogglePurchaseItemButton(true);
            dressingPopupViewModel.currentPurchasePrice.Value = price;
        }
        else
        {
            dressingPopupViewModel.TogglePurchaseItemButton(false);
            dressingPopupViewModel.EquipAvailableItem(ID);
        }
    }
    public void ShowButton()
    {
        backGroundImage.enabled = true;
        icon.enabled = true;
        borderImage.enabled = true;
    }

    public void HideButton()
    {
        backGroundImage.enabled = false;
        icon.enabled = false;
        borderImage.enabled = false;
        lockImage.gameObject.SetActive(false);
    }

    public void Initialize(ItemSetEquipButtonData item)
    {
        icon.sprite = item.Sprite;
        ID = item.Id;
        isOpen = item.IsOpen;
        price = item.Price;
        if (!item.IsOpen)
        {
            priceText.text = item.Price.ToString();
        }

        standardCurrencyBackground.gameObject.SetActive(!item.IsPremium);
        premiumCurrencyBackground.gameObject.SetActive(item.IsPremium);
        lockImage.gameObject.SetActive(!item.IsOpen);
    }

    private void HideLockOnItemPurchased(string itemID)
    {
        if (itemID == ID)
        {
            lockImage.gameObject.SetActive(false);
            isOpen = true;
        }
    }
}

public readonly struct ItemSetEquipButtonData
{
    public readonly string Id;
    public readonly Sprite Sprite;
    public readonly bool IsOpen;
    public readonly int Price;
    public readonly bool IsPremium;

    public ItemSetEquipButtonData(string id, Sprite sprite, bool isOpen, int price, bool isPremium)
    {
        Id = id;
        Sprite = sprite;
        IsOpen = isOpen;
        Price = price;
        IsPremium = isPremium;
    }
}
