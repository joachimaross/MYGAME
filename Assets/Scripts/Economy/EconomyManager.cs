using System;
using UnityEngine;

namespace MarketHustle.Economy
{
    /// <summary>
    /// Tracks player money, income, and expenses. Exposes simple API for other systems.
    /// In a full game, connect this to store sales, rent timers, and UI updates.
    /// </summary>
    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance { get; private set; }

        [Header("Player Economy")]
        public long money = 1000;

        [Header("Daily/Recurring")]
        public int dailyProfit = 0; // computed by store systems

        public event Action<long> OnMoneyChanged;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void AddMoney(long amount)
        {
            money += amount;
            OnMoneyChanged?.Invoke(money);
        }

        public bool TrySpend(long amount)
        {
            if (amount <= 0) return true;
            if (money >= amount)
            {
                money -= amount;
                OnMoneyChanged?.Invoke(money);
                return true;
            }
            return false;
        }

        // Example: call at end of day
        public void ApplyDailyProfitAndCosts(long profit, long costs)
        {
            long net = profit - costs;
            AddMoney(net);
        }
    }
}
