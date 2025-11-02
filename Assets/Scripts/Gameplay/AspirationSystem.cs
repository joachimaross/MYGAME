using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace MarketHustle.Gameplay
{
    public enum AspirationType
    {
        Wealth,
        Legacy,
        Community,
        Innovation,
        Lifestyle
    }

    [System.Serializable]
    public class Aspiration
    {
        public string id;
        public string title;
        public string description;
        public AspirationType type;
        public bool completed = false;
        public bool active = false;

        // Requirements
        public long requiredNetWorth = 0;
        public int requiredProperties = 0;
        public int requiredEmployees = 0;
        public float requiredReputation = 0f;
        public int requiredSkillLevel = 0; // For specific skills

        // Rewards
        public long moneyReward = 0;
        public float reputationBonus = 0f;
        public string unlockFeature = ""; // Feature to unlock

        public UnityEvent OnCompleted;
    }

    /// <summary>
    /// Manages story-driven aspirations that give players long-term goals.
    /// Aspirations provide narrative context and meaningful progression.
    /// </summary>
    public class AspirationSystem : MonoBehaviour
    {
        [Header("Aspiration Sets")]
        public List<Aspiration> availableAspirations;

        public UnityEvent<Aspiration> OnAspirationCompleted;
        public UnityEvent<Aspiration> OnAspirationActivated;

        private List<Aspiration> activeAspirations = new List<Aspiration>();
        private List<Aspiration> completedAspirations = new List<Aspiration>();

        void Start()
        {
            InitializeAspirations();
        }

        void Update()
        {
            CheckAspirationProgress();
        }

        void InitializeAspirations()
        {
            availableAspirations = new List<Aspiration>
            {
                new Aspiration
                {
                    id = "southside_savior",
                    title = "The Southside Savior",
                    description = "Revitalize your home neighborhood by opening 3 stores in the area.",
                    type = AspirationType.Community,
                    requiredProperties = 3,
                    moneyReward = 10000,
                    reputationBonus = 20f
                },
                new Aspiration
                {
                    id = "property_mogul",
                    title = "The Property Mogul",
                    description = "Own one of each property type: Apartment, Condo, Villa, and Mansion.",
                    type = AspirationType.Wealth,
                    requiredNetWorth = 500000,
                    moneyReward = 50000,
                    unlockFeature = "Advanced Property Management"
                },
                new Aspiration
                {
                    id = "people_person",
                    title = "The People Person",
                    description = "Have 5 employees with maximum loyalty and skill.",
                    type = AspirationType.Legacy,
                    requiredEmployees = 5,
                    reputationBonus = 30f,
                    unlockFeature = "Employee Partnership Program"
                },
                new Aspiration
                {
                    id = "market_dominator",
                    title = "Market Dominator",
                    description = "Achieve 95% market share in your district.",
                    type = AspirationType.Wealth,
                    requiredNetWorth = 1000000,
                    moneyReward = 100000,
                    unlockFeature = "Franchise System"
                },
                new Aspiration
                {
                    id = "community_leader",
                    title = "Community Leader",
                    description = "Reach maximum reputation and host community events.",
                    type = AspirationType.Community,
                    requiredReputation = 100f,
                    reputationBonus = 10f,
                    unlockFeature = "Community Event System"
                },
                new Aspiration
                {
                    id = "innovation_pioneer",
                    title = "Innovation Pioneer",
                    description = "Max out all skills and implement cutting-edge business practices.",
                    type = AspirationType.Innovation,
                    requiredSkillLevel = 100, // Sum of all skills
                    moneyReward = 25000,
                    unlockFeature = "Advanced Analytics"
                },
                new Aspiration
                {
                    id = "lifestyle_emperor",
                    title = "Lifestyle Emperor",
                    description = "Own the most luxurious mansion and maintain perfect work-life balance.",
                    type = AspirationType.Lifestyle,
                    requiredNetWorth = 2000000,
                    reputationBonus = 25f,
                    unlockFeature = "Luxury Lifestyle Perks"
                }
            };
        }

        public void ActivateAspiration(string aspirationId)
        {
            var aspiration = availableAspirations.Find(a => a.id == aspirationId);
            if (aspiration != null && !aspiration.active && !aspiration.completed)
            {
                aspiration.active = true;
                activeAspirations.Add(aspiration);
                OnAspirationActivated?.Invoke(aspiration);
            }
        }

        void CheckAspirationProgress()
        {
            foreach (var aspiration in activeAspirations.ToArray())
            {
                if (CheckAspirationRequirements(aspiration))
                {
                    CompleteAspiration(aspiration);
                }
            }
        }

        bool CheckAspirationRequirements(Aspiration aspiration)
        {
            var economy = MarketHustle.Economy.EconomyManager.Instance;
            var realEstate = FindObjectOfType<MarketHustle.RealEstate.RealEstateManager>();
            var customerSystem = FindObjectOfType<CustomerSystem>();
            var employeeSystem = FindObjectOfType<EmployeeSystem>();
            var playerSkills = FindObjectOfType<MarketHustle.Player.PlayerSkills>();

            // Check net worth requirement
            if (aspiration.requiredNetWorth > 0)
            {
                long currentNetWorth = economy != null ? economy.money : 0;
                if (realEstate != null)
                    currentNetWorth += (long)realEstate.GetTotalPropertyValue();

                if (currentNetWorth < aspiration.requiredNetWorth) return false;
            }

            // Check properties requirement
            if (aspiration.requiredProperties > 0 && realEstate != null)
            {
                if (realEstate.GetOwnedProperties().Count < aspiration.requiredProperties) return false;
            }

            // Check reputation requirement
            if (aspiration.requiredReputation > 0 && customerSystem != null)
            {
                if (customerSystem.GetReputation() < aspiration.requiredReputation) return false;
            }

            // Check employees requirement
            if (aspiration.requiredEmployees > 0 && employeeSystem != null)
            {
                int qualifiedEmployees = employeeSystem.employees.FindAll(e =>
                    e.loyalty >= 90f && e.skillLevel >= 8).Count;
                if (qualifiedEmployees < aspiration.requiredEmployees) return false;
            }

            // Check skill level requirement
            if (aspiration.requiredSkillLevel > 0 && playerSkills != null)
            {
                float totalSkillLevel = playerSkills.GetSkillLevel("charisma") +
                                       playerSkills.GetSkillLevel("logistics") +
                                       playerSkills.GetSkillLevel("negotiation") +
                                       playerSkills.GetSkillLevel("handiness");
                if (totalSkillLevel < aspiration.requiredSkillLevel) return false;
            }

            return true;
        }

        void CompleteAspiration(Aspiration aspiration)
        {
            aspiration.completed = true;
            aspiration.active = false;
            activeAspirations.Remove(aspiration);
            completedAspirations.Add(aspiration);

            // Grant rewards
            var economy = MarketHustle.Economy.EconomyManager.Instance;
            var customerSystem = FindObjectOfType<CustomerSystem>();

            if (economy != null && aspiration.moneyReward > 0)
            {
                economy.AddMoney(aspiration.moneyReward);
            }

            if (customerSystem != null && aspiration.reputationBonus > 0)
            {
                // Apply reputation bonus
                float newRep = customerSystem.GetReputation() + aspiration.reputationBonus;
                customerSystem.UpdateStoreConditions(50f, 50f, 1f); // Trigger update
            }

            // Unlock features (would need feature unlock system)
            if (!string.IsNullOrEmpty(aspiration.unlockFeature))
            {
                Debug.Log($"Unlocked feature: {aspiration.unlockFeature}");
            }

            aspiration.OnCompleted?.Invoke();
            OnAspirationCompleted?.Invoke(aspiration);
        }

        public List<Aspiration> GetActiveAspirations()
        {
            return activeAspirations;
        }

        public List<Aspiration> GetCompletedAspirations()
        {
            return completedAspirations;
        }

        public Aspiration GetAspirationById(string id)
        {
            return availableAspirations.Find(a => a.id == id);
        }
    }
}