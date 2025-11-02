using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace MarketHustle.Gameplay
{
    public enum EventType
    {
        Opportunity,
        Crisis,
        Seasonal,
        Random
    }

    [System.Serializable]
    public class GameEvent
    {
        public string id;
        public string title;
        public string description;
        public EventType type;
        public float durationDays = 1f; // How long event lasts
        public bool isActive = false;
        public float timeRemaining = 0f;

        // Effects
        public float demandMultiplier = 1f;
        public float priceMultiplier = 1f;
        public float reputationChange = 0f;
        public long moneyReward = 0;
        public long moneyCost = 0;

        // Requirements/Conditions
        public int minNetWorth = 0;
        public float minReputation = 0f;

        public UnityEvent OnEventStart;
        public UnityEvent OnEventEnd;
    }

    /// <summary>
    /// Manages procedural events that create dynamic gameplay.
    /// Events can be opportunities, crises, seasonal, or random.
    /// </summary>
    public class EventSystem : MonoBehaviour
    {
        [Header("Event Pool")]
        public List<GameEvent> availableEvents;

        [Header("Event Settings")]
        public float eventSpawnChance = 0.02f; // Chance per day
        public int maxConcurrentEvents = 2;

        public UnityEvent<GameEvent> OnEventTriggered;
        public UnityEvent<GameEvent> OnEventExpired;

        private List<GameEvent> activeEvents = new List<GameEvent>();

        void Start()
        {
            InitializeEvents();
        }

        void Update()
        {
            // Update active events
            for (int i = activeEvents.Count - 1; i >= 0; i--)
            {
                var evt = activeEvents[i];
                evt.timeRemaining -= Time.deltaTime / 86400f; // Convert to days

                if (evt.timeRemaining <= 0f)
                {
                    EndEvent(evt);
                }
            }

            // Try to spawn new events
            if (Random.value < eventSpawnChance * Time.deltaTime / 86400f &&
                activeEvents.Count < maxConcurrentEvents)
            {
                TryTriggerRandomEvent();
            }
        }

        void InitializeEvents()
        {
            availableEvents = new List<GameEvent>
            {
                new GameEvent
                {
                    id = "brewery_closing",
                    title = "Brewery Liquidation",
                    description = "A local brewery is going out of business and selling stock at 80% off!",
                    type = EventType.Opportunity,
                    durationDays = 3f,
                    demandMultiplier = 1.5f,
                    priceMultiplier = 0.8f,
                    minNetWorth = 10000
                },
                new GameEvent
                {
                    id = "freezer_breakdown",
                    title = "Freezer Malfunction",
                    description = "Your freezer unit broke down, spoiling 50% of frozen goods!",
                    type = EventType.Crisis,
                    durationDays = 1f,
                    moneyCost = 2000,
                    reputationChange = -10f
                },
                new GameEvent
                {
                    id = "city_festival",
                    title = "City Festival",
                    description = "A city-wide festival is happening - demand for snacks is up 300%!",
                    type = EventType.Seasonal,
                    durationDays = 7f,
                    demandMultiplier = 3f,
                    priceMultiplier = 1.2f
                },
                new GameEvent
                {
                    id = "supplier_delay",
                    title = "Supplier Delay",
                    description = "Your regular supplier is experiencing delays - stock will arrive late.",
                    type = EventType.Crisis,
                    durationDays = 2f,
                    demandMultiplier = 0.7f
                },
                new GameEvent
                {
                    id = "viral_social_media",
                    title = "Viral Moment",
                    description = "Your store got featured on social media! Reputation boost incoming.",
                    type = EventType.Opportunity,
                    durationDays = 1f,
                    reputationChange = 15f,
                    demandMultiplier = 1.8f
                },
                new GameEvent
                {
                    id = "competitor_bankruptcy",
                    title = "Competitor Bankruptcy",
                    description = "A competing store went bankrupt - you can buy their location cheap!",
                    type = EventType.Opportunity,
                    durationDays = 14f,
                    moneyReward = 50000,
                    minNetWorth = 50000
                }
            };
        }

        void TryTriggerRandomEvent()
        {
            var economy = MarketHustle.Economy.EconomyManager.Instance;
            var customerSystem = FindObjectOfType<CustomerSystem>();

            if (economy == null || customerSystem == null) return;

            // Filter events based on player progress
            var eligibleEvents = availableEvents.FindAll(evt =>
                !evt.isActive &&
                economy.money >= evt.minNetWorth &&
                customerSystem.GetReputation() >= evt.minReputation
            );

            if (eligibleEvents.Count == 0) return;

            var selectedEvent = eligibleEvents[Random.Range(0, eligibleEvents.Count)];
            TriggerEvent(selectedEvent);
        }

        public void TriggerEvent(GameEvent gameEvent)
        {
            if (gameEvent.isActive) return;

            gameEvent.isActive = true;
            gameEvent.timeRemaining = gameEvent.durationDays;
            activeEvents.Add(gameEvent);

            // Apply immediate effects
            ApplyEventEffects(gameEvent, true);

            OnEventTriggered?.Invoke(gameEvent);
            gameEvent.OnEventStart?.Invoke();
        }

        void EndEvent(GameEvent gameEvent)
        {
            gameEvent.isActive = false;
            activeEvents.Remove(gameEvent);

            // Remove effects
            ApplyEventEffects(gameEvent, false);

            OnEventExpired?.Invoke(gameEvent);
            gameEvent.OnEventEnd?.Invoke();
        }

        void ApplyEventEffects(GameEvent gameEvent, bool apply)
        {
            float multiplier = apply ? 1f : -1f;

            var economy = MarketHustle.Economy.EconomyManager.Instance;
            var customerSystem = FindObjectOfType<CustomerSystem>();
            var supplyChain = FindObjectOfType<MarketHustle.Economy.SupplyChainManager>();

            if (economy != null)
            {
                if (gameEvent.moneyReward != 0)
                    economy.AddMoney(gameEvent.moneyReward * (apply ? 1 : -1));
                if (gameEvent.moneyCost != 0)
                    economy.TrySpend(gameEvent.moneyCost);
            }

            if (customerSystem != null && gameEvent.reputationChange != 0)
            {
                float newRep = customerSystem.GetReputation() + (gameEvent.reputationChange * multiplier);
                customerSystem.UpdateStoreConditions(50f, 50f, 1f); // Trigger reputation update
            }

            if (supplyChain != null)
            {
                // Modify market demand through supply chain
                // This would need integration with the supply chain system
            }
        }

        public List<GameEvent> GetActiveEvents()
        {
            return activeEvents;
        }

        public bool IsEventActive(string eventId)
        {
            return activeEvents.Exists(e => e.id == eventId);
        }
    }
}