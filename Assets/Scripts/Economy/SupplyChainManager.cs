using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace MarketHustle.Economy
{
    public enum SupplierType
    {
        Budget,
        Standard,
        Premium,
        Local
    }

    [System.Serializable]
    public class Supplier
    {
        public string name;
        public SupplierType type;
        public float priceMultiplier = 1f; // Cost to buy from this supplier
        public float qualityMultiplier = 1f; // How long items last/sell for
        public int deliveryTimeDays = 1; // Days to receive order
        public float reliability = 0.9f; // Chance order arrives on time
        public bool available = true;
    }

    [System.Serializable]
    public class Competitor
    {
        public string name;
        public float marketShare = 0.3f; // Percentage of market they control
        public float priceCompetitiveness = 1f; // How aggressively they price
        public bool isHavingSale = false;
        public float saleDiscount = 0.2f;
    }

    /// <summary>
    /// Manages suppliers, delivery times, and competitor actions.
    /// Adds strategic depth to inventory management.
    /// </summary>
    public class SupplyChainManager : MonoBehaviour
    {
        [Header("Suppliers")]
        public List<Supplier> suppliers;

        [Header("Competitors")]
        public List<Competitor> competitors;

        [Header("Market Dynamics")]
        public float marketDemand = 1f; // Overall market demand multiplier
        public float seasonalDemand = 1f; // Seasonal demand fluctuations

        public UnityEvent<Supplier> OnSupplierSelected;
        public UnityEvent OnCompetitorSale;
        public UnityEvent OnDeliveryArrived;

        private Supplier currentSupplier;
        private Dictionary<string, int> pendingDeliveries = new Dictionary<string, int>(); // item -> days remaining

        void Start()
        {
            InitializeSuppliers();
            InitializeCompetitors();
        }

        void Update()
        {
            // Simulate competitor actions
            if (Random.value < 0.001f) // 0.1% chance per frame
            {
                TriggerCompetitorSale();
            }
        }

        void InitializeSuppliers()
        {
            suppliers = new List<Supplier>
            {
                new Supplier
                {
                    name = "Budget Bulk Co",
                    type = SupplierType.Budget,
                    priceMultiplier = 0.8f,
                    qualityMultiplier = 0.7f,
                    deliveryTimeDays = 3,
                    reliability = 0.7f
                },
                new Supplier
                {
                    name = "Standard Supply Inc",
                    type = SupplierType.Standard,
                    priceMultiplier = 1f,
                    qualityMultiplier = 1f,
                    deliveryTimeDays = 2,
                    reliability = 0.85f
                },
                new Supplier
                {
                    name = "Premium Partners",
                    type = SupplierType.Premium,
                    priceMultiplier = 1.3f,
                    qualityMultiplier = 1.4f,
                    deliveryTimeDays = 1,
                    reliability = 0.95f
                },
                new Supplier
                {
                    name = "Local Market Hub",
                    type = SupplierType.Local,
                    priceMultiplier = 1.1f,
                    qualityMultiplier = 1.1f,
                    deliveryTimeDays = 1,
                    reliability = 0.9f
                }
            };
        }

        void InitializeCompetitors()
        {
            competitors = new List<Competitor>
            {
                new Competitor { name = "QuickMart", marketShare = 0.25f, priceCompetitiveness = 0.95f },
                new Competitor { name = "ValuePlus", marketShare = 0.2f, priceCompetitiveness = 0.9f }
            };
        }

        public void SelectSupplier(Supplier supplier)
        {
            currentSupplier = supplier;
            OnSupplierSelected?.Invoke(supplier);
        }

        public void PlaceOrder(string itemName, int quantity, long cost)
        {
            // Check if order succeeds based on reliability
            if (Random.value > currentSupplier.reliability)
            {
                Debug.Log($"Order failed! {currentSupplier.name} delivery unreliable.");
                return;
            }

            // Add to pending deliveries
            if (pendingDeliveries.ContainsKey(itemName))
                pendingDeliveries[itemName] += currentSupplier.deliveryTimeDays;
            else
                pendingDeliveries[itemName] = currentSupplier.deliveryTimeDays;

            // Deduct money
            var economy = EconomyManager.Instance;
            if (economy != null)
            {
                economy.TrySpend(cost);
            }
        }

        public void ProcessDayEnd()
        {
            // Process deliveries
            List<string> arrivedItems = new List<string>();
            foreach (var delivery in pendingDeliveries)
            {
                pendingDeliveries[delivery.Key]--;
                if (pendingDeliveries[delivery.Key] <= 0)
                {
                    arrivedItems.Add(delivery.Key);
                }
            }

            foreach (string item in arrivedItems)
            {
                pendingDeliveries.Remove(item);
                OnDeliveryArrived?.Invoke();
            }

            // Update market conditions
            UpdateMarketDemand();
        }

        void UpdateMarketDemand()
        {
            // Simulate market fluctuations
            marketDemand = Mathf.Lerp(0.8f, 1.3f, Mathf.PerlinNoise(Time.time * 0.1f, 0f));
            seasonalDemand = 1f + 0.2f * Mathf.Sin(Time.time * 0.01f); // Seasonal variation
        }

        void TriggerCompetitorSale()
        {
            Competitor competitor = competitors[Random.Range(0, competitors.Count)];
            competitor.isHavingSale = true;
            competitor.saleDiscount = Random.Range(0.1f, 0.3f);

            // This will reduce your sales temporarily
            marketDemand *= 0.8f; // 20% drop in demand

            OnCompetitorSale?.Invoke();

            // Sale lasts for 3-7 days
            Invoke("EndCompetitorSale", Random.Range(3f, 7f) * 86400f);
        }

        void EndCompetitorSale()
        {
            foreach (Competitor comp in competitors)
            {
                comp.isHavingSale = false;
            }
            marketDemand /= 0.8f; // Restore demand
        }

        public float GetEffectiveDemand() => marketDemand * seasonalDemand;
        public Supplier GetCurrentSupplier() => currentSupplier;
        public bool HasPendingDelivery(string itemName) => pendingDeliveries.ContainsKey(itemName);
        public int GetDeliveryDaysRemaining(string itemName) => pendingDeliveries.ContainsKey(itemName) ? pendingDeliveries[itemName] : 0;
    }
}