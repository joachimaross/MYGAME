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
    }

    public class RealEstateManager : MonoBehaviour
    {
        public static RealEstateManager Instance { get; private set; }

        // Optional: author properties as ScriptableObjects for designer workflow
        public RealEstatePropertySO[] propertyAssets;

        public List<PropertyData> availableProperties = new List<PropertyData>();

        public event Action<PropertyData> OnPropertyPurchased;

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
                    availableProperties.Add(new PropertyData { id = "apt_01", propertyType = PropertyType.Apartment, displayName = "Small Apartment", sceneName = "ApartmentScene", price = 5000, monthlyRent = 500 });
                    availableProperties.Add(new PropertyData { id = "condo_01", propertyType = PropertyType.Condo, displayName = "Modern Condo", sceneName = "CondoScene", price = 15000, monthlyRent = 1500 });
                    availableProperties.Add(new PropertyData { id = "villa_01", propertyType = PropertyType.Villa, displayName = "Luxury Villa", sceneName = "VillaScene", price = 50000, monthlyRent = 4500 });
                    availableProperties.Add(new PropertyData { id = "man_01", propertyType = PropertyType.Mansion, displayName = "Mansion", sceneName = "MansionScene", price = 200000, monthlyRent = 12000 });
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
                OnPropertyPurchased?.Invoke(prop);
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

            SceneManager.LoadScene(prop.sceneName);
        }
    }
}
