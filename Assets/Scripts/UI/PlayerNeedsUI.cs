using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MarketHustle.Player;

namespace MarketHustle.UI
{
    /// <summary>
    /// UI component to display and manage player needs.
    /// Shows bars for Energy, Hunger, Social, Hygiene.
    /// </summary>
    public class PlayerNeedsUI : MonoBehaviour
    {
        [Header("Need Bars")]
        public Slider energyBar;
        public Slider hungerBar;
        public Slider socialBar;
        public Slider hygieneBar;

        [Header("Need Texts")]
        public TMP_Text energyText;
        public TMP_Text hungerText;
        public TMP_Text socialText;
        public TMP_Text hygieneText;

        [Header("Warning Panel")]
        public GameObject warningPanel;
        public TMP_Text warningText;

        private PlayerNeeds playerNeeds;

        void Start()
        {
            playerNeeds = FindObjectOfType<PlayerNeeds>();
            if (playerNeeds != null)
            {
                playerNeeds.OnNeedsCritical.AddListener(ShowWarning);
                playerNeeds.OnNeedsRestored.AddListener(HideWarning);
            }
            UpdateUI();
        }

        void Update()
        {
            if (playerNeeds != null)
            {
                UpdateUI();
            }
        }

        void UpdateUI()
        {
            energyBar.value = playerNeeds.GetEnergy() / 100f;
            hungerBar.value = playerNeeds.GetHunger() / 100f;
            socialBar.value = playerNeeds.GetSocial() / 100f;
            hygieneBar.value = playerNeeds.GetHygiene() / 100f;

            energyText.text = $"{playerNeeds.GetEnergy():F0}%";
            hungerText.text = $"{playerNeeds.GetHunger():F0}%";
            socialText.text = $"{playerNeeds.GetSocial():F0}%";
            hygieneText.text = $"{playerNeeds.GetHygiene():F0}%";
        }

        void ShowWarning()
        {
            warningPanel.SetActive(true);
            warningText.text = "Your needs are critical! Find food, rest, or socialize.";
        }

        void HideWarning()
        {
            warningPanel.SetActive(false);
        }
    }
}