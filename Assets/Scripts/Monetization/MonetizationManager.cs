using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace MarketHustle.Monetization
{
    public enum CosmeticType
    {
        Furniture,
        Clothing,
        StoreDecor,
        AvatarAccessory,
        Pet
    }

    [System.Serializable]
    public class CosmeticItem
    {
        public string id;
        public string name;
        public string description;
        public CosmeticType type;
        public int premiumCurrencyCost = 0;
        public bool unlocked = false;
        public Sprite icon;
    }

    /// <summary>
    /// Manages monetization features that don't affect core gameplay.
    /// Focuses on cosmetics, convenience features, and premium currency.
    /// </summary>
    public class MonetizationManager : MonoBehaviour
    {
        [Header("Premium Currency")]
        public int premiumCurrency = 0;
        public int currencyPerAdWatched = 10;
        public int currencyPerPurchase = 100; // $4.99 = 100 currency

        [Header("Cosmetic Shop")]
        public List<CosmeticItem> availableCosmetics;

        [Header("Convenience Features")]
        public bool instantDeliveryUnlocked = false;
        public bool doubleExperienceUnlocked = false;
        public float temporaryDiscount = 0f; // From rewarded ads

        public UnityEvent<int> OnPremiumCurrencyChanged;
        public UnityEvent<CosmeticItem> OnCosmeticPurchased;

        void Start()
        {
            InitializeCosmetics();
        }

        void InitializeCosmetics()
        {
            availableCosmetics = new List<CosmeticItem>
            {
                new CosmeticItem
                {
                    id = "luxury_chair",
                    name = "Executive Office Chair",
                    description = "Premium ergonomic chair for your home office",
                    type = CosmeticType.Furniture,
                    premiumCurrencyCost = 50
                },
                new CosmeticItem
                {
                    id = "designer_clothes",
                    name = "Designer Business Suit",
                    description = "Professional attire for your avatar",
                    type = CosmeticType.Clothing,
                    premiumCurrencyCost = 75
                },
                new CosmeticItem
                {
                    id = "neon_sign",
                    name = "Neon Store Sign",
                    description = "Eye-catching exterior decoration",
                    type = CosmeticType.StoreDecor,
                    premiumCurrencyCost = 100
                },
                new CosmeticItem
                {
                    id = "golden_retriever",
                    name = "Golden Retriever Pet",
                    description = "Adorable companion for your properties",
                    type = CosmeticType.Pet,
                    premiumCurrencyCost = 150
                },
                new CosmeticItem
                {
                    id = "diamond_watch",
                    name = "Diamond Watch",
                    description = "Luxury accessory for your avatar",
                    type = CosmeticType.AvatarAccessory,
                    premiumCurrencyCost = 200
                }
            };
        }

        public void WatchRewardedAd()
        {
            // Simulate ad watching
            AddPremiumCurrency(currencyPerAdWatched);

            // Grant temporary convenience bonus
            GrantTemporaryDiscount(0.2f, 7200f); // 20% discount for 2 hours
        }

        public void PurchasePremiumCurrency(int amount)
        {
            // In real implementation, this would integrate with app store
            AddPremiumCurrency(amount);
        }

        public void AddPremiumCurrency(int amount)
        {
            premiumCurrency += amount;
            OnPremiumCurrencyChanged?.Invoke(premiumCurrency);
        }

        public bool PurchaseCosmetic(string cosmeticId)
        {
            var cosmetic = availableCosmetics.Find(c => c.id == cosmeticId);
            if (cosmetic == null || cosmetic.unlocked) return false;

            if (premiumCurrency >= cosmetic.premiumCurrencyCost)
            {
                premiumCurrency -= cosmetic.premiumCurrencyCost;
                cosmetic.unlocked = true;
                OnCosmeticPurchased?.Invoke(cosmetic);
                OnPremiumCurrencyChanged?.Invoke(premiumCurrency);
                return true;
            }
            return false;
        }

        public void GrantTemporaryDiscount(float discount, float durationSeconds)
        {
            temporaryDiscount = discount;
            Invoke("ClearTemporaryDiscount", durationSeconds);
        }

        void ClearTemporaryDiscount()
        {
            temporaryDiscount = 0f;
        }

        public void UnlockInstantDelivery()
        {
            // Premium feature to instantly complete supply chain deliveries
            instantDeliveryUnlocked = true;
        }

        public void UnlockDoubleExperience(float durationHours)
        {
            // Premium feature for 2x skill experience gain
            doubleExperienceUnlocked = true;
            Invoke("ClearDoubleExperience", durationHours * 3600f);
        }

        void ClearDoubleExperience()
        {
            doubleExperienceUnlocked = false;
        }

        public float GetCurrentDiscount()
        {
            return temporaryDiscount;
        }

        public bool HasInstantDelivery()
        {
            return instantDeliveryUnlocked;
        }

        public bool HasDoubleExperience()
        {
            return doubleExperienceUnlocked;
        }

        public List<CosmeticItem> GetAvailableCosmetics()
        {
            return availableCosmetics.FindAll(c => !c.unlocked);
        }

        public List<CosmeticItem> GetUnlockedCosmetics()
        {
            return availableCosmetics.FindAll(c => c.unlocked);
        }
    }
}