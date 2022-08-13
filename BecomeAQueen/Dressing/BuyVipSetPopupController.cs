using System;
using Doozy.Engine.UI;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.LevelLogic.Dressing
{
    public class BuyVipSetPopupController : MonoBehaviour
    {
        public DressingViewModelSO viewModelSo;
        public float timeUntilGetVipSetIsShown;
        public float delayBeforeShowingVipPopup = 10f;
        
        private bool isPopupVipOutfitsShown;
        private float timeIsReadyToBeShown;
        private bool isReadyToBeShown;
        UIPopup uiPopup;
        private VipSet? currentVipSet;

        private void OnEnable()
        {
            viewModelSo.VipSetBought.OnValueChanged += SetVipSetStatus;
            viewModelSo.OnEquipButtonClicked += SetReadyToBeShown;
        }
        
        private void OnDisable()
        {
            viewModelSo.VipSetBought.OnValueChanged -= SetVipSetStatus;
            viewModelSo.OnEquipButtonClicked -= SetReadyToBeShown;
        }

        public void SetVipSetPopup(VipSet vipSet)
        {
            currentVipSet = vipSet;
        }

        private void SetReadyToBeShown(string guid)
        {
            isReadyToBeShown = true;
            timeIsReadyToBeShown = Time.time;
            viewModelSo.OnEquipButtonClicked -= SetReadyToBeShown;
        }

        private void SetVipSetStatus(bool isBought)
        {
            isPopupVipOutfitsShown = isBought;
            if (isBought)
            {
                ShowBuyVipSetButton(false);
            }
        }
        
        private void Update()
        {
            if (isPopupVipOutfitsShown || currentVipSet == null) return;

            if (isReadyToBeShown && timeIsReadyToBeShown + timeUntilGetVipSetIsShown < Time.time)
            {
                ShowBuyVipSetButton(true);
            }
            else if (delayBeforeShowingVipPopup <= 0)
            {
                ShowBuyVipSetButton(true);
            }

            delayBeforeShowingVipPopup -= Time.deltaTime;
        }
        
        private void ShowBuyVipSetButton(bool isVisible)
        {
            if (isVisible && UIPopup.VisiblePopups.Count == 0)
            {
                isPopupVipOutfitsShown = true;
                uiPopup = UIPopup.GetPopup("BuyVipSetNotificationUIPopup");
                uiPopup.Data.Labels[0].GetComponent<TextMeshProUGUI>().text = currentVipSet?.textVipSet;
                uiPopup.Data.Images[0].sprite = currentVipSet?.imageVipSet.sprite;
                uiPopup.Show();
            }
            else if (uiPopup)
            {
                uiPopup.Hide();
            }
        }
    }
}