using System.Collections.Generic;
using _Game.Scripts.LevelLogic.Dressing;
using ComfortGames.CharacterCustomization;
using Doozy.Engine.UI;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public enum ClothesCategory
{
    DEFAULT,
    MAKEUP,
    HAIR,
    BODY,
    PANTS,
    SHOES,
    NECKLACE,
    DRESS,
    FULLBODY,
}

public class DressingLevelManager : MonoBehaviour
{
    [Header("Scriptables")]
    public MainMenuUIViewModelSO mainMenuUIViewModelSO;
    public VoidEventChannelSO OnLevelWonChannel;
    public DressingViewModelSO viewModel;
    public RewardForAdsViewModelSO rewardForAdsViewModel;
    public LevelsFunnelManager levelsFunnel;
    public StorageSO storage;
    public AppSettingsSO settings;
    public AnalyticsManager analyticsManager;
    public HapticsManager hapticsManager;
    public MainSceneLoadingUIViewModel loadingSO;
    public DialogueSystemController dialogueController;
    
    [Header("Character customization")] 
    public BuyVipSetPopupController buyVipSetPopupController;
    public OutfitSelectionView outfitSelectionView;
    public GameObject outfitSelection;

    public ClothesCategory defaultCategory = ClothesCategory.BODY;

    [Header("Camera")] 
    public ClothesCategoryEventChannelSO OnCameraFocusSet;
    public VoidEventChannelSO OnCameraToCompletedPositionSet;

    [Header("Ads Manager")]
    public AdsManagerSO AdsManagerSO;
    public int adRewardValue;
    
    public float timeUntilIdleReminderIsShown;

    public InAppPurchasesManagerSO inAppPurchasesManagerSo;
    
    public PricesContainerHolder pricesContainerHolder;
    private PricesContainerSO pricesContainerSO => pricesContainerHolder.GetPricesContainer();
    
    public RewardedViewModelSO rewardedViewModelSO;
    public LuckyWheelViewModelSO luckyWheelViewModel;

    private DressingLevelSO level;
    private Dictionary<ClothesCategory, string> currentOutfit;
    private Dictionary<ClothesCategory, bool> categoriesChosen;
    private ClothesCategory currentCategory;
    private DressingLevelHolder levelInstance;
    private float timeOfLastAction;
    private string currentSceneName;
    private List<(ClothesCategory, ClothesCategory)> collisionList = new List<(ClothesCategory, ClothesCategory)>();
    private HashSet<string> startDressingsId = new HashSet<string>(); //CharacterCustomization finds clothes that are not in the level directory
    private Dictionary<ClothesCategory, string> outfitOnStart;
    private int diamondsOnStart;

    private void OnEnable()
    {
        viewModel.OnApplyButtonClicked += ApplyDressing;
        viewModel.OnCategoryButtonClicked += ChooseCategory;
        viewModel.OnBuyButtonClicked += BuyOutfitElement;
        viewModel.OnEquipButtonClicked += EquipOutfitElement;
        viewModel.OnBuyVipSetClicked += BuyVipSetButtonClicked;
        viewModel.OnDressingScenePreviewShown += HandleDressingPreviewShown;
        rewardForAdsViewModel.OnGetRewardForAdsButtonClicked += WatchAdButtonClicked;
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        var story = levelsFunnel.GetCurrentStory();
        level = story.GetLevels()[storage.LevelIndex] as DressingLevelSO;
        currentSceneName = story.GetLevels()[storage.LevelIndex].GetSceneName();
        levelInstance = Instantiate(level.levelPrefab, transform);
        BuildCollisionList(level.clothingGroupConfig.collisionMatrix.collisionListFirst, 
            level.clothingGroupConfig.collisionMatrix.collisionListSecond);
        
        if (levelInstance.playableDirector)
        {
            levelInstance.playableDirector.stopped += StartLevelUI;
        }      
    }

    private void OnApplicationQuit()
    {
        ClearOutfitDataAnQuit();
    }

    private void ClearOutfitDataAnQuit()
    {
        storage.SaveCharacterOutfit(outfitOnStart);
        storage.Diamonds = diamondsOnStart;
        inAppPurchasesManagerSo.AddBasicCurrency(0);
    }
   

    private void OnDisable()
    {
        viewModel.OnApplyButtonClicked -= ApplyDressing;
        viewModel.OnCategoryButtonClicked -= ChooseCategory;
        viewModel.OnBuyButtonClicked -= BuyOutfitElement;
        viewModel.OnEquipButtonClicked -= EquipOutfitElement;
        viewModel.OnBuyVipSetClicked -= BuyVipSetButtonClicked;
        viewModel.OnDressingScenePreviewShown -= HandleDressingPreviewShown;
        rewardForAdsViewModel.OnGetRewardForAdsButtonClicked -= WatchAdButtonClicked;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (levelInstance.playableDirector)
        {
            levelInstance.playableDirector.stopped -= StartLevelUI;
        }
    }

    private void HandleDressingPreviewShown()
    {
        Debug.Log("Clear scene");
        Destroy(levelInstance.gameObject);
    }

    private void Start()
    {
        viewModel.SetCurrencyValue(storage.Diamonds);
        outfitOnStart = storage.characterOutfit;
        diamondsOnStart = storage.Diamonds;
    }

    private void StartLevelUI(PlayableDirector playableDirector)
    {
        outfitSelection.SetActive(true);
        levelInstance.InitializeSelectionView(outfitSelectionView);
        buyVipSetPopupController.SetVipSetPopup(level.vipSet);
        viewModel.SetCurrencyValue(storage.Diamonds);
        
        AddItemsToDressPreview();
    }

    private void AddItemsToDressPreview()
    {
        OutfitController outfitController = CharacterCustomizationFinderManager.GetOutfitController();
        
        for (int i = 0; i < outfitController.numOutfitModels; i++)
        {
            OutfitScriptableObject outfit = outfitController.GetOutfitModel(i);
            inAppPurchasesManagerSo.AddToAllAvailableItems(outfit.outfitElementGuid);
        }
    }
    
    private void Update()
    {
        if (outfitSelection.activeSelf && timeOfLastAction + timeUntilIdleReminderIsShown < Time.time)
        {
            outfitSelectionView.ToggleNotSelectedPointer(ChooseNotSelectedCategory());
            SetLastActionTime();
        }

    }

    private void BuildCollisionList(List<ClothesCategory> collisionListFirst, List<ClothesCategory> collisionListSecond)
    {
        for (var i = 0; i < collisionListFirst.Count; i++)
        {
            collisionList.Add((collisionListFirst[i], collisionListSecond[i]));
        }
    }
    
    private ClothesCategory ChooseNotSelectedCategory()
    {
        foreach (var category in categoriesChosen)
        {
            if (!category.Value && category.Key != currentCategory)
            {
                return category.Key;
            }
        }

        return ClothesCategory.DEFAULT;
    }
    
    private void WatchAdButtonClicked()
    {
        AdsManagerSO.LaunchRV(RewardAfterAd,"Get_more");
    }
    
    private void BuyVipSetButtonClicked()
    {
        if (inAppPurchasesManagerSo.IsPremiumCurrencyMode())
        {
            if (luckyWheelViewModel.IsLuckyWheelEnabled.Value)
            {
                if (inAppPurchasesManagerSo.TryBuyWithPremiumCurrency(pricesContainerSO.dressingPremiumItemPrice,false,true))
                {
                    HandlePremiumBuyAndParticles();
                }
            }
            else
            {
                if (inAppPurchasesManagerSo.TryBuyWithPremiumCurrency(pricesContainerSO.dressingPremiumItemPrice))
                {
                    HandlePremiumBuyAndParticles();
                }
            }
        }
        else
        {
            AdsManagerSO.LaunchRV(HandlePremiumBuy,"Dressing_VIP_Offer");
        }
    }

    private void HandlePremiumBuyAndParticles()
    {
        HandlePremiumBuy(true);
        rewardedViewModelSO.AnimatePremiumCurrencySpendParticles();
    }

    private void BuyVipSetAndHandleParticles()
    {
        BuyVipSet();
        rewardedViewModelSO.AnimatePremiumCurrencySpendParticles();
    }

    private void RewardAfterAd(bool adWatched)
    {
        Debug.Log($"Ad watched: {adWatched}");
        
        if (adWatched)
        {
            inAppPurchasesManagerSo.AddBasicCurrency(adRewardValue);
            viewModel.SetCurrencyValue(storage.Diamonds);
        }
    }

    private void HandlePremiumBuy(bool adWatched)
    {
        if (adWatched)
        {
            BuyVipSet();
        }
    }
    
    private void SetLastActionTime()
    {
        timeOfLastAction = Time.time;
    }

    private void BuyVipSet()
    {
        var clickedCategory = currentCategory;
        foreach (var outfitView in outfitSelectionView.outfitViewList)
        {
            if (outfitView.outfitScriptableObject.isPremium && !startDressingsId.Contains(outfitView.outfitScriptableObject.outfitElementGuid)) //CharacterCustomization finds clothes that are not in the level directory
            {
                levelInstance.particleController.EmitPremiumParticles();
                levelInstance.characterAppearanceAnimator.AnimatePremiumPurchased();
                currentCategory = outfitView.outfitScriptableObject.outfitCategoryScriptableObject.clothesCategory;
                outfitSelectionView.BuyOutfit(outfitView.outfitScriptableObject.outfitElementGuid);
            }
        }

        currentCategory = clickedCategory;

        var keys = new List<ClothesCategory>(categoriesChosen.Keys);
        foreach(var category in keys)
        {
            outfitSelectionView.SetVisibleSelectedCheckmark(category);
            categoriesChosen[category] = true;
        }

        viewModel.VipSetBought.Value = true;
        
        if (CheckOutfitCompleted())
        {
            viewModel.SetState(new DressingViewModelState.Dressed());
        }
    }
    
    private void BuyOutfitElement(string targetOutfitGuid, bool isPremium, int cost)
    {
        if (isPremium)
        {
            if (inAppPurchasesManagerSo.IsPremiumCurrencyMode())
            {
                if (luckyWheelViewModel.IsLuckyWheelEnabled.Value)
                {
                    if(inAppPurchasesManagerSo.TryBuyWithPremiumCurrency(inAppPurchasesManagerSo.pricesContainerHolder.GetPricesContainer().dressingPremiumItemPrice,false,true))
                    {
                        BuyVipSetAndHandleParticles();
                    }
                }
                else
                {
                    if(inAppPurchasesManagerSo.TryBuyWithPremiumCurrency(inAppPurchasesManagerSo.pricesContainerHolder.GetPricesContainer().dressingPremiumItemPrice))
                    {
                        BuyVipSetAndHandleParticles();
                    }
                }
            }
            else
            {
                AdsManagerSO.LaunchRV(HandlePremiumBuy,"VIP_Premium");
            }
            
            return;
        }

        if (storage.Diamonds - cost < 0)
        {
            if (luckyWheelViewModel.IsLuckyWheelEnabled.Value)
            {
                luckyWheelViewModel.ShowNotEnoughResourcesPopup();
            }
            else
            {
                rewardForAdsViewModel.ShowRewardOffer();
            }
            
            return;
        }
        
        inAppPurchasesManagerSo.AddBasicCurrency(-cost);
        viewModel.SetCurrencyValue(storage.Diamonds);
        levelInstance.characterAppearanceAnimator.AnimateSimplePurchased();
        levelInstance.particleController.EmitPurchaseParticles();
        outfitSelectionView.BuyOutfit(targetOutfitGuid);
    }

    private void EquipOutfitElement(string elementGuid)
    {
        RemoveCollidedOutfits();
        if (currentCategory == ClothesCategory.FULLBODY)
        {
            currentOutfit[ClothesCategory.NECKLACE] = null;
            currentOutfit[ClothesCategory.SHOES] = null;
        }
        currentOutfit[currentCategory] = elementGuid;
        categoriesChosen[currentCategory] = true;
        SetLastActionTime();
        outfitSelectionView.DisableNotSelectedPointers();
        outfitSelectionView.SetVisibleSelectedCheckmark(currentCategory);
        if (CheckOutfitCompleted())
        {
            viewModel.SetState(new DressingViewModelState.Dressed());
        }
        hapticsManager.PlayDressingHaptic();
        
        inAppPurchasesManagerSo.HandleAddItemSet(elementGuid);
    }
    
    private void InitOutfitChoice()
    {
        foreach (var outfit in storage.characterOutfit)
        {
            startDressingsId.Add(outfit.Value);
        }

        currentOutfit = storage.characterOutfit;
        if (currentOutfit == null)
        {
            currentOutfit = new Dictionary<ClothesCategory, string>();
            foreach (var category in levelInstance.clothesCategories)
                currentOutfit.Add(category, "");
        }
        ChooseCategory(defaultCategory);
        
        categoriesChosen = new Dictionary<ClothesCategory, bool>();
        foreach (var category in levelInstance.clothesCategories)
            categoriesChosen.Add(category, false);
        viewModel.SetState(new DressingViewModelState.Undressed());
    }

    private void RemoveCollidedOutfits()
    {
        foreach (var collision in collisionList)
        {
            if (collision.Item1 == currentCategory)
            {
                RemoveOnCollisionForTargetCategory(collision.Item2);
            }
                
            if (collision.Item2 == currentCategory)
            {
                RemoveOnCollisionForTargetCategory(collision.Item1);
            }
        }
    }

    private void RemoveOnCollisionForTargetCategory(ClothesCategory target)
    {
        currentOutfit[target] = null;
        foreach (var col in collisionList)
        {
            if (!currentOutfit.ContainsKey(col.Item1) || !currentOutfit.ContainsKey(col.Item2))
                continue;
            if (col.Item1 == target
                && col.Item2 != currentCategory)
            {
                currentOutfit[col.Item2] = storage.characterOutfit[col.Item2];
            }
            if (col.Item2 == target 
                && col.Item1 != currentCategory)
            {
                currentOutfit[col.Item1] = storage.characterOutfit[col.Item1];
            }
        }
    }

    private void ApplyDressing()
    {
        Debug.Log("dressingApplied");
        storage.SaveCharacterOutfit(currentOutfit);
        OnCameraToCompletedPositionSet.RaiseEvent();
        levelInstance.characterAppearanceAnimator.AnimateOutfitCompleted();
        levelInstance.particleController.EmitOutfitCompleted();
        OnLevelWonChannel.RaiseEvent();
        viewModel.SetState(new DressingViewModelState.Completed(), settings.winNode);
        analyticsManager.ProvideWinAnalytics();
    }
    
    private bool CheckOutfitCompleted()
    {
        foreach (var item in categoriesChosen)
        {
            if (!item.Value)
            {
                return false;
            }
        }

        return true;
    }

    private void ChooseCategory(ClothesCategory category)
    {
        currentCategory = category;
        levelInstance.characterAppearanceAnimator.SetIdle(category == ClothesCategory.SHOES);
        SetLastActionTime();
        outfitSelectionView.DisableNotSelectedPointers();
        if (levelInstance.playableDirector != null && levelInstance.playableDirector.state == PlayState.Playing)
        {
            return;
        }
        OnCameraFocusSet.RaiseEvent(currentCategory);
        hapticsManager.PlayDressingHaptic();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != currentSceneName)
        {
            SceneManager.UnloadSceneAsync(scene.name);
            return;
        }
        analyticsManager.ProvideStartAnalytics();
        mainMenuUIViewModelSO.SetStartLevelMenuState(MainMenuUIState.LEVELS_DRESSING);
        loadingSO.OnSceneLoad();
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.name));

        if (level.customStartOutfit.Count > 0)
        {
            storage.SaveCharacterOutfit(storage.GetCharacterOutfit(level.customStartOutfit));
        }
        InitOutfitChoice();
        levelInstance.Initialize(level.clothingGroupConfig, storage);

        if (levelInstance.playableDirector)
        {
            dialogueController.StartConversation(level.conversationName);
        }
        else
        {
            StartLevelUI(levelInstance.playableDirector);
        }

        SetLastActionTime();
    }

}
