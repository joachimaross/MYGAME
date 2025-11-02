using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MarketHustle.Gameplay
{
    /// <summary>
    /// Analytics and statistics tracking for gameplay insights and monetization.
    /// </summary>
    public class GameAnalytics : MonoBehaviour
    {
        public static GameAnalytics Instance { get; private set; }

        [System.Serializable]
        public class SessionData
        {
            public float sessionStartTime;
            public float sessionDuration;
            public int moneyEarned;
            public int moneySpent;
            public int propertiesBought;
            public int customersServed;
            public int achievementsUnlocked;
            public Dictionary<string, int> featureUsage = new Dictionary<string, int>();
        }

        [System.Serializable]
        public class PlayerStats
        {
            public long totalMoneyEarned;
            public long totalMoneySpent;
            public int totalPropertiesOwned;
            public int totalCustomersServed;
            public int totalAchievementsUnlocked;
            public float totalPlayTime;
            public int sessionsPlayed;
            public float averageSessionLength;
            public Dictionary<string, int> favoriteFeatures = new Dictionary<string, int>();
        }

        private SessionData currentSession;
        private PlayerStats playerStats;
        private float sessionStartTime;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadPlayerStats();
            StartNewSession();
        }

        void StartNewSession()
        {
            sessionStartTime = Time.time;
            currentSession = new SessionData
            {
                sessionStartTime = sessionStartTime,
                featureUsage = new Dictionary<string, int>()
            };
        }

        void OnApplicationQuit()
        {
            EndSession();
            SavePlayerStats();
        }

        void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                EndSession();
                SavePlayerStats();
            }
            else
            {
                StartNewSession();
            }
        }

        void EndSession()
        {
            if (currentSession != null)
            {
                currentSession.sessionDuration = Time.time - sessionStartTime;
                UpdatePlayerStats(currentSession);
                SendAnalyticsEvent("session_end", currentSession);
            }
        }

        void UpdatePlayerStats(SessionData session)
        {
            playerStats.totalMoneyEarned += session.moneyEarned;
            playerStats.totalMoneySpent += session.moneySpent;
            playerStats.totalPropertiesOwned = Mathf.Max(playerStats.totalPropertiesOwned, session.propertiesBought);
            playerStats.totalCustomersServed += session.customersServed;
            playerStats.totalAchievementsUnlocked += session.achievementsUnlocked;
            playerStats.totalPlayTime += session.sessionDuration;
            playerStats.sessionsPlayed++;

            if (playerStats.sessionsPlayed > 0)
            {
                playerStats.averageSessionLength = playerStats.totalPlayTime / playerStats.sessionsPlayed;
            }

            // Update favorite features
            foreach (var feature in session.featureUsage)
            {
                if (!playerStats.favoriteFeatures.ContainsKey(feature.Key))
                    playerStats.favoriteFeatures[feature.Key] = 0;
                playerStats.favoriteFeatures[feature.Key] += feature.Value;
            }
        }

        // Public API for tracking events
        public void TrackMoneyEarned(int amount)
        {
            if (currentSession != null)
                currentSession.moneyEarned += amount;
        }

        public void TrackMoneySpent(int amount)
        {
            if (currentSession != null)
                currentSession.moneySpent += amount;
        }

        public void TrackPropertyBought()
        {
            if (currentSession != null)
                currentSession.propertiesBought++;
        }

        public void TrackCustomerServed()
        {
            if (currentSession != null)
                currentSession.customersServed++;
        }

        public void TrackAchievementUnlocked()
        {
            if (currentSession != null)
                currentSession.achievementsUnlocked++;
        }

        public void TrackFeatureUsage(string featureName)
        {
            if (currentSession != null)
            {
                if (!currentSession.featureUsage.ContainsKey(featureName))
                    currentSession.featureUsage[featureName] = 0;
                currentSession.featureUsage[featureName]++;
            }
        }

        public void SendAnalyticsEvent(string eventName, object data = null)
        {
            // In a real implementation, send to analytics service (Firebase, GameAnalytics, etc.)
            string jsonData = data != null ? JsonUtility.ToJson(data) : "{}";
            Debug.Log($"Analytics Event: {eventName} - {jsonData}");

            // Example: Send to Firebase Analytics
            // FirebaseAnalytics.LogEvent(eventName, new Parameter("data", jsonData));
        }

        // Getters for UI/stats display
        public PlayerStats GetPlayerStats()
        {
            return playerStats;
        }

        public SessionData GetCurrentSession()
        {
            return currentSession;
        }

        public string GetTopFeature()
        {
            if (playerStats.favoriteFeatures.Count == 0) return "None";
            return playerStats.favoriteFeatures.OrderByDescending(x => x.Value).First().Key;
        }

        public float GetEngagementScore()
        {
            // Simple engagement score based on various metrics
            float score = 0f;
            score += playerStats.totalPropertiesOwned * 10f;
            score += playerStats.totalAchievementsUnlocked * 5f;
            score += playerStats.sessionsPlayed * 2f;
            score += playerStats.averageSessionLength / 60f; // Minutes
            return score;
        }

        // Save/Load
        void SavePlayerStats()
        {
            string json = JsonUtility.ToJson(playerStats);
            PlayerPrefs.SetString("PlayerStats", json);
            PlayerPrefs.Save();
        }

        void LoadPlayerStats()
        {
            string json = PlayerPrefs.GetString("PlayerStats", "{}");
            playerStats = JsonUtility.FromJson<PlayerStats>(json);
            if (playerStats == null)
            {
                playerStats = new PlayerStats
                {
                    favoriteFeatures = new Dictionary<string, int>()
                };
            }
        }

        // Debug methods
        [ContextMenu("Print Player Stats")]
        void PrintPlayerStats()
        {
            Debug.Log($"Total Money Earned: ${playerStats.totalMoneyEarned}");
            Debug.Log($"Total Properties Owned: {playerStats.totalPropertiesOwned}");
            Debug.Log($"Total Play Time: {playerStats.totalPlayTime} seconds");
            Debug.Log($"Sessions Played: {playerStats.sessionsPlayed}");
            Debug.Log($"Average Session: {playerStats.averageSessionLength} seconds");
            Debug.Log($"Top Feature: {GetTopFeature()}");
            Debug.Log($"Engagement Score: {GetEngagementScore()}");
        }

        [ContextMenu("Reset Player Stats")]
        void ResetPlayerStats()
        {
            playerStats = new PlayerStats { favoriteFeatures = new Dictionary<string, int>() };
            SavePlayerStats();
            Debug.Log("Player stats reset");
        }
    }
}