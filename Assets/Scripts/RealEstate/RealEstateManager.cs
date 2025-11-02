using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MarketHustle.RealEstate
{
    public enum PropertyType { Apartment, Condo, Villa, Mansion }

    [Serializable]
    public class PropertyData
    {
        public string id;
        public PropertyType propertyType;
        public string displayName;
        public string sceneName; // Scene to load when entering the property
        public long price;
        public long monthlyRent;
        public bool owned = false;
        public bool forRent = false;

        // Enhanced gameplay mechanics
        public float energyRecoveryRate = 1f; // How fast energy recovers at home
        public float skillGainBonus = 0f; // Bonus to skill experience gain
        public bool hasHomeOffice = false; // Can manage business from home
        public bool hasMasterBedroom = false; // Better energy recovery
        public int luxuryLevel = 1; // Affects social needs and reputation
        public float propertyValue = 0f; // Current market value
        public float appreciationRate = 0.001f; // Daily appreciation
    }

    public class RealEstateManager : MonoBehaviour
    {
        public static RealEstateManager Instance { get; private set; }

        // Optional: author properties as ScriptableObjects for designer workflow
        public RealEstatePropertySO[] propertyAssets;

        public List<PropertyData> availableProperties = new List<PropertyData>();

        public event Action<PropertyData> OnPropertyPurchased;
        public event Action<PropertyData> OnPropertyValueChanged;

        private PropertyData currentHome;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            // If ScriptableObject assets are provided, use them as the authoritative list
            if (propertyAssets != null && propertyAssets.Length > 0)
            {
                availableProperties.Clear();
                foreach (var a in propertyAssets)
                {
                    var pd = new PropertyData();
                    pd.id = string.IsNullOrEmpty(a.id) ? a.displayName.ToLower().Replace(" ", "_") : a.id;
                    pd.propertyType = a.propertyType;
                    pd.displayName = a.displayName;
                    pd.sceneName = a.sceneName;
                    pd.price = a.price;
                    pd.monthlyRent = a.monthlyRent;
                    pd.forRent = a.forRent;
                    pd.owned = false;
                    availableProperties.Add(pd);
                }
            }
            else
            {
                // fallback seed data when no assets are present
                if (availableProperties.Count == 0)
                {
                    availableProperties.Add(new PropertyData {
                        id = "apt_01",
                        propertyType = PropertyType.Apartment,
                        displayName = "Small Apartment",
                        sceneName = "ApartmentScene",
                        price = 5000,
                        monthlyRent = 500,
                        energyRecoveryRate = 0.8f,
                        skillGainBonus = 0f,
                        hasHomeOffice = false,
                        hasMasterBedroom = false,
                        luxuryLevel = 1,
                        propertyValue = 5000f,
                        appreciationRate = 0.0005f
                    });
                    availableProperties.Add(new PropertyData {
                        id = "condo_01",
                        propertyType = PropertyType.Condo,
                        displayName = "Modern Condo",
                        sceneName = "CondoScene",
                        price = 15000,
                        monthlyRent = 1500,
                        energyRecoveryRate = 1f,
                        skillGainBonus = 0.05f,
                        hasHomeOffice = false,
                        hasMasterBedroom = false,
                        luxuryLevel = 2,
                        propertyValue = 15000f,
                        appreciationRate = 0.0008f
                    });
                    availableProperties.Add(new PropertyData {
                        id = "villa_01",
                        propertyType = PropertyType.Villa,
                        displayName = "Luxury Villa",
                        sceneName = "VillaScene",
                        price = 50000,
                        monthlyRent = 4500,
                        energyRecoveryRate = 1.2f,
                        skillGainBonus = 0.1f,
                        hasHomeOffice = true,
                        hasMasterBedroom = false,
                        luxuryLevel = 3,
                        propertyValue = 50000f,
                        appreciationRate = 0.001f
                    });
                    availableProperties.Add(new PropertyData {
                        id = "man_01",
                        propertyType = PropertyType.Mansion,
                        displayName = "Mansion",
                        sceneName = "MansionScene",
                        price = 200000,
                        monthlyRent = 12000,
                        energyRecoveryRate = 1.5f,
                        skillGainBonus = 0.2f,
                        hasHomeOffice = true,
                        hasMasterBedroom = true,
                        luxuryLevel = 4,
                        propertyValue = 200000f,
                        appreciationRate = 0.0015f
                    });
                }
            }
        }

        public bool BuyProperty(string propertyId)
        {
            var prop = availableProperties.Find(p => p.id == propertyId);
            if (prop == null) return false;

            if (prop.owned) return false;

            if (Economy.EconomyManager.Instance == null) return false;

            if (Economy.EconomyManager.Instance.TrySpend(prop.price))
            {
                prop.owned = true;
                prop.propertyValue = prop.price; // Set initial value
                OnPropertyPurchased?.Invoke(prop);

                // If this is a home property, set as current home
                if (prop.propertyType == PropertyType.Apartment ||
                    prop.propertyType == PropertyType.Condo ||
                    prop.propertyType == PropertyType.Villa ||
                    prop.propertyType == PropertyType.Mansion)
                {
                    SetCurrentHome(prop);
                }

                return true;
            }
            return false;
        }

        public bool RentProperty(string propertyId)
        {
            var prop = availableProperties.Find(p => p.id == propertyId);
            if (prop == null) return false;
            if (prop.forRent == false) return false;

            // immediate rent cost
            if (Economy.EconomyManager.Instance.TrySpend(prop.monthlyRent))
            {
                // In a full system, track rent timers
                return true;
            }
            return false;
        }

        public void EnterProperty(string propertyId)
        {
            var prop = availableProperties.Find(p => p.id == propertyId);
            if (prop == null) return;
            if (string.IsNullOrEmpty(prop.sceneName))
            {
                Debug.LogWarning("Property has no scene assigned: " + prop.displayName);
                return;
            }

            // If entering home, apply home bonuses
            if (prop == currentHome)
            {
                ApplyHomeBonuses(prop);
            }

            SceneManager.LoadScene(prop.sceneName);
        }

        public void SetCurrentHome(PropertyData property)
        {
            currentHome = property;
        }

        public PropertyData GetCurrentHome()
        {
            return currentHome;
        }

        private void ApplyHomeBonuses(PropertyData home)
        {
            // Apply energy recovery and skill bonuses when at home
            var playerNeeds = FindObjectOfType<MarketHustle.Player.PlayerNeeds>();
            var playerSkills = FindObjectOfType<MarketHustle.Player.PlayerSkills>();

            if (playerNeeds != null)
            {
                // Boost energy recovery while at home
                // This would be handled by the needs system when detecting player is home
            }

            if (playerSkills != null)
            {
                // Skill gain bonus is applied in PlayerSkills when leveling up
            }
        }

        public void ProcessDailyPropertyUpdates()
        {
            foreach (var prop in availableProperties)
            {
                if (prop.owned)
                {
                    // Property appreciation
                    float oldValue = prop.propertyValue;
                    prop.propertyValue *= (1f + prop.appreciationRate);
                    if (prop.propertyValue != oldValue)
                    {
                        OnPropertyValueChanged?.Invoke(prop);
                    }
                }
            }
        }

        public List<PropertyData> GetOwnedProperties()
        {
            return availableProperties.FindAll(p => p.owned);
        }

        public float GetTotalPropertyValue()
        {
            float total = 0f;
            foreach (var prop in availableProperties)
            {
                if (prop.owned)
                {
                    total += prop.propertyValue;
                }
            }
            return total;
        }
    }
}
