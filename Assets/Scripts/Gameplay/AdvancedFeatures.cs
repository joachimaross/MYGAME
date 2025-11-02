using UnityEngine;
using System.Collections.Generic;
using MarketHustle.Economy;
using MarketHustle.RealEstate;

namespace MarketHustle.Gameplay
{
    /// <summary>
    /// Advanced gameplay features: achievements, daily challenges, market trends, and expansion mechanics.
    /// </summary>
    public class AdvancedFeatures : MonoBehaviour
    {
        public static AdvancedFeatures Instance { get; private set; }

        [Header("Achievements")]
        public List<Achievement> achievements = new List<Achievement>();

        [Header("Daily Challenges")]
        public List<DailyChallenge> dailyChallenges = new List<DailyChallenge>();

        [Header("Market Trends")]
        public float marketVolatility = 0.1f;
        public float trendUpdateInterval = 300f; // 5 minutes

        [Header("Business Expansion")]
        public int maxStoreLocations = 5;
        public List<string> unlockedLocations = new List<string>();

        private float lastTrendUpdate;
        private Dictionary<string, float> marketTrends = new Dictionary<string, float>();

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeAchievements();
            InitializeDailyChallenges();
            InitializeMarketTrends();
        }

        void Start()
        {
            GenerateDailyChallenges();
        }

        void Update()
        {
            UpdateMarketTrends();
            CheckAchievements();
        }

        void InitializeAchievements()
        {
            achievements.Add(new Achievement("First Sale", "Make your first money", 1, () => EconomyManager.Instance.money > 0));
            achievements.Add(new Achievement("Property Owner", "Buy your first property", 1, () => GetOwnedPropertiesCount() > 0));
            achievements.Add(new Achievement("Store Manager", "Hire your first employee", 1, () => GetEmployeeCount() > 1));
            achievements.Add(new Achievement("Millionaire", "Earn $1,000,000", 1000000, () => EconomyManager.Instance.money >= 1000000));
            achievements.Add(new Achievement("Empire Builder", "Own 5 properties", 5, () => GetOwnedPropertiesCount() >= 5));
        }

        void InitializeDailyChallenges()
        {
            dailyChallenges.Add(new DailyChallenge("Sales Champion", "Earn $500 today", 500, DailyChallenge.ChallengeType.MoneyEarned));
            dailyChallenges.Add(new DailyChallenge("Busy Store", "Serve 50 customers", 50, DailyChallenge.ChallengeType.CustomersServed));
            dailyChallenges.Add(new DailyChallenge("Property Investor", "Buy a new property", 1, DailyChallenge.ChallengeType.PropertiesBought));
        }

        void InitializeMarketTrends()
        {
            marketTrends["Housing"] = 1.0f;
            marketTrends["Retail"] = 1.0f;
            marketTrends["Commercial"] = 1.0f;
            lastTrendUpdate = Time.time;
        }

        void UpdateMarketTrends()
        {
            if (Time.time - lastTrendUpdate > trendUpdateInterval)
            {
                foreach (var trend in marketTrends.Keys.ToArray())
                {
                    // Random walk with mean reversion
                    float change = Random.Range(-marketVolatility, marketVolatility);
                    marketTrends[trend] = Mathf.Clamp(marketTrends[trend] + change, 0.5f, 2.0f);
                }
                lastTrendUpdate = Time.time;

                // Update property prices based on trends
                UpdatePropertyPrices();
            }
        }

        void UpdatePropertyPrices()
        {
            var manager = RealEstateManager.Instance;
            if (manager == null) return;

            foreach (var prop in manager.availableProperties)
            {
                float trendMultiplier = 1.0f;
                if (prop.propertyType == PropertyType.Apartment || prop.propertyType == PropertyType.Condo)
                    trendMultiplier = marketTrends["Housing"];
                else if (prop.propertyType == PropertyType.Villa || prop.propertyType == PropertyType.Mansion)
                    trendMultiplier = marketTrends["Commercial"];

                // Store base price and apply trend
                if (!propPrices.ContainsKey(prop.id))
                    propPrices[prop.id] = prop.price;

                prop.price = (long)(propPrices[prop.id] * trendMultiplier);
            }
        }

        private Dictionary<string, long> propPrices = new Dictionary<string, long>();

        void GenerateDailyChallenges()
        {
            // Reset progress
            foreach (var challenge in dailyChallenges)
            {
                challenge.currentProgress = 0;
                challenge.isCompleted = false;
            }
        }

        void CheckAchievements()
        {
            foreach (var achievement in achievements)
            {
                if (!achievement.isUnlocked && achievement.condition())
                {
                    achievement.isUnlocked = true;
                    OnAchievementUnlocked(achievement);
                }
            }
        }

        void OnAchievementUnlocked(Achievement achievement)
        {
            Debug.Log($"Achievement Unlocked: {achievement.name} - {achievement.description}");
            // TODO: Show achievement popup, play sound, grant rewards
            EconomyManager.Instance.AddMoney(achievement.reward);
        }

        // Public API
        public void ReportMoneyEarned(long amount)
        {
            foreach (var challenge in dailyChallenges.Where(c => c.type == DailyChallenge.ChallengeType.MoneyEarned))
            {
                challenge.currentProgress += amount;
                CheckChallengeCompletion(challenge);
            }
        }

        public void ReportCustomerServed()
        {
            foreach (var challenge in dailyChallenges.Where(c => c.type == DailyChallenge.ChallengeType.CustomersServed))
            {
                challenge.currentProgress++;
                CheckChallengeCompletion(challenge);
            }
        }

        public void ReportPropertyBought()
        {
            foreach (var challenge in dailyChallenges.Where(c => c.type == DailyChallenge.ChallengeType.PropertiesBought))
            {
                challenge.currentProgress++;
                CheckChallengeCompletion(challenge);
            }
        }

        void CheckChallengeCompletion(DailyChallenge challenge)
        {
            if (!challenge.isCompleted && challenge.currentProgress >= challenge.targetValue)
            {
                challenge.isCompleted = true;
                OnChallengeCompleted(challenge);
            }
        }

        void OnChallengeCompleted(DailyChallenge challenge)
        {
            Debug.Log($"Daily Challenge Completed: {challenge.name}");
            // Grant rewards
            EconomyManager.Instance.AddMoney(challenge.reward);
        }

        // Helper methods
        int GetOwnedPropertiesCount()
        {
            var manager = RealEstateManager.Instance;
            return manager != null ? manager.availableProperties.Count(p => p.owned) : 0;
        }

        int GetEmployeeCount()
        {
            // This would need to be implemented in StoreManagementUI
            return 1; // Placeholder
        }

        public float GetMarketTrend(string category)
        {
            return marketTrends.ContainsKey(category) ? marketTrends[category] : 1.0f;
        }
    }

    [System.Serializable]
    public class Achievement
    {
        public string name;
        public string description;
        public long reward;
        public System.Func<bool> condition;
        public bool isUnlocked;

        public Achievement(string name, string desc, long reward, System.Func<bool> condition)
        {
            this.name = name;
            this.description = desc;
            this.reward = reward;
            this.condition = condition;
            this.isUnlocked = false;
        }
    }

    [System.Serializable]
    public class DailyChallenge
    {
        public string name;
        public string description;
        public float targetValue;
        public ChallengeType type;
        public long reward;
        public float currentProgress;
        public bool isCompleted;

        public enum ChallengeType { MoneyEarned, CustomersServed, PropertiesBought }

        public DailyChallenge(string name, string desc, float target, ChallengeType type, long reward = 100)
        {
            this.name = name;
            this.description = desc;
            this.targetValue = target;
            this.type = type;
            this.reward = reward;
            this.currentProgress = 0;
            this.isCompleted = false;
        }
    }
}