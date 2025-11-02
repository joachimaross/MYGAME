using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MarketHustle.RealEstate;

namespace MarketHustle.UI
{
    /// <summary>
    /// UI component for displaying property information in lists.
    /// Used in Empire View and Real Estate UI.
    /// </summary>
    public class PropertyListItem : MonoBehaviour
    {
        [Header("UI Elements")]
        public TMP_Text propertyNameText;
        public TMP_Text propertyTypeText;
        public TMP_Text propertyValueText;
        public TMP_Text monthlyIncomeText;
        public Image propertyIcon;
        public Button manageButton;

        private PropertyData propertyData;

        public void Setup(PropertyData property)
        {
            propertyData = property;

            if (propertyNameText != null)
                propertyNameText.text = property.displayName;

            if (propertyTypeText != null)
                propertyTypeText.text = property.propertyType.ToString();

            if (propertyValueText != null)
                propertyValueText.text = $"Value: ${property.propertyValue:N0}";

            if (monthlyIncomeText != null)
            {
                string incomeText = property.forRent ?
                    $"Rent: ${property.monthlyRent:N0}/mo" :
                    "No rental income";
                monthlyIncomeText.text = incomeText;
            }

            // Set up manage button
            if (manageButton != null)
            {
                manageButton.onClick.AddListener(OnManageClicked);
            }
        }

        void OnManageClicked()
        {
            // Open property management window
            var empireView = FindObjectOfType<EmpireViewUI>();
            if (empireView != null)
            {
                empireView.ShowDetailedPropertyView(propertyData);
            }
        }

        public void UpdateValueDisplay()
        {
            if (propertyValueText != null && propertyData != null)
            {
                propertyValueText.text = $"Value: ${propertyData.propertyValue:N0}";
            }
        }
    }
}