using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace MarketHustle.Monetization
{
    public enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter,
        Holiday
    }

    [System.Serializable]
    public class SeasonalEvent
    {
        public string name;
        public Season season;
        public string description;
        public int durationDays = 30;
        public bool isActive = false;

        // Battle Pass content
        public List<SeasonalReward> freeRewards;
        public List<SeasonalReward> premiumRewards;

        // Event bonuses
        public float demandMultiplier = 1f;
        public float experienceMultiplier = 1f;
        public int premiumCurrencyReward = 0;

        public UnityEvent OnEventStart;
        public UnityEvent OnEventEnd;
    }

    [System.Serializable]
    public class SeasonalReward
    {
        public string name;
        public string description;
        public int tier; // 1-10 for battle pass
        public bool isPremium = false;
        public CosmeticType cosmeticType;
        public bool claimed = false;
    }

    /// <summary>
    /// Manages seasonal events and battle pass system for player retention.
    /// Provides recurring engagement without affecting core gameplay.
    /// </summary>
    public class SeasonalEvents : MonoBehaviour
    {
        [Header("Current Season")]
        public Season currentSeason = Season.Spring;
        public SeasonalEvent currentEvent;

        [Header("Battle Pass")]
        public bool hasPremiumPass = false;
        public int currentTier = 1;
        public int maxTier = 10;
        public int experiencePerAction = 10;

        [Header("Progress")]
        public int currentExperience = 0;
        public int experienceToNextTier = 100;

        public UnityEvent<int> OnTierAdvanced;
        public UnityEvent<SeasonalReward> OnRewardClaimed;

        private float seasonTimer = 0f;
        private float seasonDuration = 30f * 86400f; // 30 days in seconds

        void Start()
        {
            InitializeSeasonalEvents();
            StartNewSeason();
        }

        void Update()
        {
            seasonTimer += Time.deltaTime;
            if (seasonTimer >= seasonDuration)
            {
                AdvanceSeason();
                seasonTimer = 0f;
            }
        }

        void InitializeSeasonalEvents()
        {
            // Initialize seasonal events for each season
            // This would be more elaborate in a full implementation
        }

        void StartNewSeason()
        {
            // Create seasonal event based on current season
            currentEvent = CreateSeasonalEvent(currentSeason);
            currentEvent.OnEventStart?.Invoke();

            // Reset battle pass progress
            currentTier = 1;
            currentExperience = 0;
            experienceToNextTier = 100;

            // Reset rewards
            foreach (var reward in currentEvent.freeRewards)
                reward.claimed = false;
            foreach (var reward in currentEvent.premiumRewards)
                reward.claimed = false;
        }

        SeasonalEvent CreateSeasonalEvent(Season season)
        {
            SeasonalEvent evt = new SeasonalEvent
            {
                season = season,
                durationDays = 30,
                freeRewards = new List<SeasonalReward>(),
                premiumRewards = new List<SeasonalReward>()
            };

            switch (season)
            {
                case Season.Spring:
                    evt.name = "Spring Renewal";
                    evt.description = "Fresh start with blooming opportunities!";
                    evt.demandMultiplier = 1.1f;
                    evt.experienceMultiplier = 1.05f;
                    break;
                case Season.Summer:
                    evt.name = "Summer Rush";
                    evt.description = "Beach season brings tourist crowds!";
                    evt.demandMultiplier = 1.3f;
                    evt.experienceMultiplier = 1.1f;
                    break;
                case Season.Fall:
                    evt.name = "Harvest Festival";
                    evt.description = "Celebrate the harvest with special deals!";
                    evt.demandMultiplier = 1.15f;
                    evt.experienceMultiplier = 1.08f;
                    break;
                case Season.Winter:
                    evt.name = "Winter Wonderland";
                    evt.description = "Cozy up with winter-themed cosmetics!";
                    evt.demandMultiplier = 0.9f; // People stay home more
                    evt.experienceMultiplier = 1.12f;
                    break;
                case Season.Holiday:
                    evt.name = "Holiday Hustle";
                    evt.description = "Festive season means peak shopping!";
                    evt.demandMultiplier = 1.5f;
                    evt.experienceMultiplier = 1.2f;
                    break;
            }

            // Generate rewards for this season
            GenerateSeasonalRewards(evt);
            return evt;
        }

        void GenerateSeasonalRewards(SeasonalEvent evt)
        {
            // Free track rewards
            for (int i = 1; i <= maxTier; i++)
            {
                evt.freeRewards.Add(new SeasonalReward
                {
                    name = $"Tier {i} Free Reward",
                    description = $"Seasonal bonus for reaching tier {i}",
                    tier = i,
                    cosmeticType = CosmeticType.Furniture
                });
            }

            // Premium track rewards (better rewards)
            for (int i = 1; i <= maxTier; i++)
            {
                evt.premiumRewards.Add(new SeasonalReward
                {
                    name = $"Tier {i} Premium Reward",
                    description = $"Exclusive seasonal cosmetic for tier {i}",
                    tier = i,
                    isPremium = true,
                    cosmeticType = CosmeticType.AvatarAccessory
                });
            }
        }

        public void GainBattlePassExperience(int amount)
        {
            if (currentEvent == null) return;

            currentExperience += amount * (hasPremiumPass ? 2 : 1); // Premium pass gives 2x XP

            while (currentExperience >= experienceToNextTier && currentTier < maxTier)
            {
                currentExperience -= experienceToNextTier;
                currentTier++;
                experienceToNextTier += 50; // Increasing XP requirements

                OnTierAdvanced?.Invoke(currentTier);

                // Auto-claim rewards
                ClaimAvailableRewards();
            }
        }

        void ClaimAvailableRewards()
        {
            // Claim free rewards
            var freeReward = currentEvent.freeRewards.Find(r => r.tier == currentTier && !r.claimed);
            if (freeReward != null)
            {
                freeReward.claimed = true;
                OnRewardClaimed?.Invoke(freeReward);
            }

            // Claim premium rewards if player has premium pass
            if (hasPremiumPass)
            {
                var premiumReward = currentEvent.premiumRewards.Find(r => r.tier == currentTier && !r.claimed);
                if (premiumReward != null)
                {
                    premiumReward.claimed = true;
                    OnRewardClaimed?.Invoke(premiumReward);
                }
            }
        }

        public void PurchasePremiumPass()
        {
            // In real implementation, this would cost real money
            hasPremiumPass = true;
            // Claim all previously missed premium rewards
            foreach (var reward in currentEvent.premiumRewards)
            {
                if (reward.tier <= currentTier && !reward.claimed)
                {
                    reward.claimed = true;
                    OnRewardClaimed?.Invoke(reward);
                }
            }
        }

        void AdvanceSeason()
        {
            // End current event
            currentEvent.OnEventEnd?.Invoke();

            // Move to next season
            currentSeason = (Season)(((int)currentSeason + 1) % 5);

            // Start new season
            StartNewSeason();
        }

        public SeasonalEvent GetCurrentEvent()
        {
            return currentEvent;
        }

        public int GetCurrentTier()
        {
            return currentTier;
        }

        public float GetTierProgress()
        {
            return (float)currentExperience / experienceToNextTier;
        }
    }
}