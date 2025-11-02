using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace MarketHustle.Gameplay
{
    [System.Serializable]
    public class PrestigeBonus
    {
        public string name;
        public string description;
        public bool unlocked = false;

        // Bonus effects
        public float moneyMultiplier = 1f;
        public float experienceMultiplier = 1f;
        public float reputationMultiplier = 1f;
        public int startingMoneyBonus = 0;
        public bool unlockFeature = false;
    }

    /// <summary>
    /// Manages prestige mode and permanent bonuses after "beating" the game.
    /// Provides meaningful replayability with carryover bonuses.
    /// </summary>
    public class PrestigeSystem : MonoBehaviour
    {
        [Header("Prestige Requirements")]
        public long prestigeThreshold = 1000000; // Net worth required
        public int completedAspirationsRequired = 5;

        [Header("Available Bonuses")]
        public List<PrestigeBonus> availableBonuses;

        [Header("Current Prestige")]
        public int currentPrestigeLevel = 0;
        public bool prestigeModeUnlocked = false;

        public UnityEvent OnPrestigeUnlocked;
        public UnityEvent<int> OnPrestigeLevelChanged;

        void Start()
        {
            InitializeBonuses();
            CheckPrestigeUnlock();
        }

        void InitializeBonuses()
        {
            availableBonuses = new List<PrestigeBonus>
            {
                new PrestigeBonus
                {
                    name = "Supplier Veteran",
                    description = "10% better supplier deals forever",
                    moneyMultiplier = 0.9f // 10% discount
                },
                new PrestigeBonus
                {
                    name = "Handiness Expert",
                    description = "Start with Handiness skill unlocked",
                    experienceMultiplier = 1.5f,
                    unlockFeature = true
                },
                new PrestigeBonus
                {
                    name = "Reputation Legacy",
                    description = "Start with 25 reputation bonus",
                    reputationMultiplier = 1.25f
                },
                new PrestigeBonus
                {
                    name = "Capitalist",
                    description = "Start with $50,000 bonus cash",
                    startingMoneyBonus = 50000
                },
                new PrestigeBonus
                {
                    name = "Networking Pro",
                    description = "15% faster skill experience gain",
                    experienceMultiplier = 1.15f
                },
                new PrestigeBonus
                {
                    name = "Property Magnate",
                    description = "Properties appreciate 25% faster",
                    moneyMultiplier = 1.25f // For property appreciation
                },
                new PrestigeBonus
                {
                    name = "Employee Loyalty",
                    description = "Employees start with higher loyalty",
                    reputationMultiplier = 1.2f // Affects employee relations
                },
                new PrestigeBonus
                {
                    name = "Market Master",
                    description = "Start with premium supplier unlocked",
                    unlockFeature = true
                }
            };
        }

        void CheckPrestigeUnlock()
        {
            var economy = MarketHustle.Economy.EconomyManager.Instance;
            var realEstate = FindObjectOfType<MarketHustle.RealEstate.RealEstateManager>();
            var aspirationSystem = FindObjectOfType<AspirationSystem>();

            if (economy == null || realEstate == null || aspirationSystem == null) return;

            long currentNetWorth = economy.money + (long)realEstate.GetTotalPropertyValue();
            int completedAspirations = aspirationSystem.GetCompletedAspirations().Count;

            if (currentNetWorth >= prestigeThreshold &&
                completedAspirations >= completedAspirationsRequired &&
                !prestigeModeUnlocked)
            {
                prestigeModeUnlocked = true;
                OnPrestigeUnlocked?.Invoke();
            }
        }

        public void EnterPrestigeMode()
        {
            if (!prestigeModeUnlocked) return;

            currentPrestigeLevel++;

            // Choose 2 random bonuses
            List<PrestigeBonus> chosenBonuses = ChooseRandomBonuses(2);

            // Apply bonuses permanently
            ApplyPrestigeBonuses(chosenBonuses);

            // Reset game state but keep bonuses
            ResetGameForPrestige();

            OnPrestigeLevelChanged?.Invoke(currentPrestigeLevel);
        }

        List<PrestigeBonus> ChooseRandomBonuses(int count)
        {
            List<PrestigeBonus> available = availableBonuses.FindAll(b => !b.unlocked);
            List<PrestigeBonus> chosen = new List<PrestigeBonus>();

            for (int i = 0; i < Mathf.Min(count, available.Count); i++)
            {
                int randomIndex = Random.Range(0, available.Count);
                PrestigeBonus bonus = available[randomIndex];
                bonus.unlocked = true;
                chosen.Add(bonus);
                available.RemoveAt(randomIndex);
            }

            return chosen;
        }

        void ApplyPrestigeBonuses(List<PrestigeBonus> bonuses)
        {
            foreach (var bonus in bonuses)
            {
                // Apply permanent bonuses to relevant systems
                var economy = MarketHustle.Economy.EconomyManager.Instance;
                var playerSkills = FindObjectOfType<MarketHustle.Player.PlayerSkills>();
                var customerSystem = FindObjectOfType<CustomerSystem>();

                if (economy != null)
                {
                    if (bonus.startingMoneyBonus > 0)
                    {
                        economy.AddMoney(bonus.startingMoneyBonus);
                    }
                }

                if (playerSkills != null && bonus.experienceMultiplier > 1f)
                {
                    // This would need to be integrated into the skill system
                    Debug.Log($"Applied experience multiplier: {bonus.experienceMultiplier}");
                }

                // Store bonuses for other systems to use
                PlayerPrefs.SetFloat($"PrestigeBonus_{bonus.name}_MoneyMult", bonus.moneyMultiplier);
                PlayerPrefs.SetFloat($"PrestigeBonus_{bonus.name}_ExpMult", bonus.experienceMultiplier);
                PlayerPrefs.SetFloat($"PrestigeBonus_{bonus.name}_RepMult", bonus.reputationMultiplier);
                PlayerPrefs.SetInt($"PrestigeBonus_{bonus.name}_StartMoney", bonus.startingMoneyBonus);
                PlayerPrefs.SetInt($"PrestigeBonus_{bonus.name}_Feature", bonus.unlockFeature ? 1 : 0);

                PlayerPrefs.Save();
            }
        }

        void ResetGameForPrestige()
        {
            // Reset economy
            var economy = MarketHustle.Economy.EconomyManager.Instance;
            if (economy != null)
            {
                economy.money = 1000; // Starting money
            }

            // Reset properties (keep one basic property)
            var realEstate = FindObjectOfType<MarketHustle.RealEstate.RealEstateManager>();
            if (realEstate != null)
            {
                var ownedProperties = realEstate.GetOwnedProperties();
                foreach (var prop in ownedProperties)
                {
                    if (prop.propertyType != MarketHustle.RealEstate.PropertyType.Apartment)
                    {
                        prop.owned = false;
                    }
                }
            }

            // Reset reputation
            var customerSystem = FindObjectOfType<CustomerSystem>();
            if (customerSystem != null)
            {
                // Reset to base level but keep prestige bonuses
            }

            // Reset skills but keep prestige bonuses
            var playerSkills = FindObjectOfType<MarketHustle.Player.PlayerSkills>();
            if (playerSkills != null)
            {
                // Reset skill levels but keep unlocked features
            }

            // Reset aspirations
            var aspirationSystem = FindObjectOfType<AspirationSystem>();
            if (aspirationSystem != null)
            {
                foreach (var asp in aspirationSystem.availableAspirations)
                {
                    asp.completed = false;
                    asp.active = false;
                }
                aspirationSystem.GetActiveAspirations().Clear();
                aspirationSystem.GetCompletedAspirations().Clear();
            }
        }

        public List<PrestigeBonus> GetUnlockedBonuses()
        {
            return availableBonuses.FindAll(b => b.unlocked);
        }

        public bool CanPrestige()
        {
            return prestigeModeUnlocked;
        }

        public int GetCurrentPrestigeLevel()
        {
            return currentPrestigeLevel;
        }
    }
}