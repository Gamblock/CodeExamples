using System;
using UnityEngine;
using UnityEngine.UI;

public class ShowItemsCategoryButton : MonoBehaviour
{
    [SerializeField] private ClothesCategory clothesCategory;
    [SerializeField] private DressingPopupViewModel dressingPopupViewModel;

    [SerializeField] private Image backGroundOn;

    public ClothesCategory Category => clothesCategory;
    
    public Button button;

    public void ToggleOff()
    {
        backGroundOn.gameObject.SetActive(false);
    }
    
    public void ToggleOn()
    {
        backGroundOn.gameObject.SetActive(true);
    }
    
    private void Awake()
    {
        button.onClick.AddListener(ShowItemsFromCategory);
    }

    private void ShowItemsFromCategory()
    {
        dressingPopupViewModel.ShowItemsFromCategory(clothesCategory);
    }
}
