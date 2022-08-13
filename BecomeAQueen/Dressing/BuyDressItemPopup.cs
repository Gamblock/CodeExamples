using Doozy.Engine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyDressItemPopup : MonoBehaviour
{
    [SerializeField] private UIPopup popup;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button dressPreviewButton;
    [SerializeField] private TextMeshProUGUI itemTitleText;
    [SerializeField] private PopupType popupType = PopupType.Item;
    
    public BuyDressPopupViewModel buyDressPopupViewModel;
    public DressingPopupViewModel dressingPopupViewModel;
    
    public Image panelImage;

    private string currentSetID;

    private void Awake()
    {
        closeBtn.onClick.AddListener(()=>popup.Hide());
        dressPreviewButton.onClick.AddListener(OpenDressPreviewPopup);
    }

    private void OnEnable()
    {
        buyDressPopupViewModel.OnUpdatePopup += UpdatePopup;
    }

    private void OnDisable()
    {
        buyDressPopupViewModel.OnUpdatePopup -= UpdatePopup;
    }

    private void UpdatePopup(BuyDressSetPopupState state)
    {
        panelImage.sprite = state.Sprite;
        currentSetID = state.ItemSetID;

        if (popupType == PopupType.Set)
        {
            itemTitleText.text = state.Title;
        }
    }

    private void OpenDressPreviewPopup()
    {
        popup.Hide();

        if (popupType == PopupType.Set)
        {
            dressingPopupViewModel.UpdateCharacter(currentSetID);
            dressingPopupViewModel.ShowPopupDefaultState();
        }
        else
        {
            dressingPopupViewModel.UpdateCharacter(currentSetID);
        }
    }
}

public enum PopupType
{
    Item,
    Set
}

public class BuyDressSetPopupState
{
    public BuyDressSetPopupState(string id, string title, Sprite sprite)
    {
        ItemSetID = id;
        Title = title;
        Sprite = sprite;
    }
    
    public string ItemSetID;
    public string Title;
    public Sprite Sprite;
}
