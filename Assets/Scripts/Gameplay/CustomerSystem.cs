using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace MarketHustle.Gameplay
{
    public enum CustomerType
    {
        BargainHunter,
        ImpulseBuyer,
        WealthyShopper,
        ImpatientCustomer,
        LoyalRegular,
        Tourist
    }

    [System.Serializable]
    public class CustomerPersonality
    {
        public CustomerType type;
        public string name;
        public float patience = 1f; // How long they wait in line
        public float priceSensitivity = 1f; // How much they care about prices
        public float impulseBuyChance = 0.3f; // Chance to buy unplanned items
        public float reviewLikelihood = 0.5f; // Chance to leave review
        public float averageSpend = 50f; // Average money spent
        public float reputationImpact = 1f; // How much their experience affects store reputation
    }

    /// <summary>
    /// Manages customer generation, behavior, and store reputation.
    /// Customers have personalities that affect shopping behavior.
    /// </summary>
    public class CustomerSystem : MonoBehaviour
    {
        [Header("Customer Generation")]
        public float customerSpawnRate = 0.5f; // Customers per minute
        public int maxCustomersInStore = 10;
        public List<CustomerPersonality> customerTypes;

        [Header("Store Attractiveness")]
        public float cleanliness = 50f; // 0-100, affects customer spawn
        public float stockLevel = 50f; // 0-100, affects customer satisfaction
        public float priceLevel = 1f; // Multiplier, affects price sensitivity

        [Header("Reputation")]
        public float reputation = 50f; // 0-100, affects customer types attracted
        public float reputationDecayRate = 0.1f; // Per day

        public UnityEvent<CustomerPersonality> OnCustomerSpawned;
        public UnityEvent<float> OnReputationChanged;

        private List<CustomerPersonality> activeCustomers = new List<CustomerPersonality>();
        private float spawnTimer = 0f;

        void Start()
        {
            InitializeCustomerTypes();
        }

        void Update()
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= 60f / customerSpawnRate)
            {
                TrySpawnCustomer();
                spawnTimer = 0f;
            }

            // Reputation decay
            reputation = Mathf.Max(0, reputation - reputationDecayRate * Time.deltaTime / 86400f); // Per day
        }

        void InitializeCustomerTypes()
        {
            customerTypes = new List<CustomerPersonality>
            {
                new CustomerPersonality
                {
                    type = CustomerType.BargainHunter,
                    name = "Bargain Hunter",
                    patience = 0.8f,
                    priceSensitivity = 1.5f,
                    impulseBuyChance = 0.1f,
                    reviewLikelihood = 0.3f,
                    averageSpend = 30f,
                    reputationImpact = 0.5f
                },
                new CustomerPersonality
                {
                    type = CustomerType.ImpulseBuyer,
                    name = "Impulse Buyer",
                    patience = 1.2f,
                    priceSensitivity = 0.8f,
                    impulseBuyChance = 0.8f,
                    reviewLikelihood = 0.4f,
                    averageSpend = 60f,
                    reputationImpact = 0.7f
                },
                new CustomerPersonality
                {
                    type = CustomerType.WealthyShopper,
                    name = "Wealthy Shopper",
                    patience = 1.5f,
                    priceSensitivity = 0.5f,
                    impulseBuyChance = 0.6f,
                    reviewLikelihood = 0.8f,
                    averageSpend = 150f,
                    reputationImpact = 1.5f
                },
                new CustomerPersonality
                {
                    type = CustomerType.ImpatientCustomer,
                    name = "Impatient Customer",
                    patience = 0.3f,
                    priceSensitivity = 1f,
                    impulseBuyChance = 0.2f,
                    reviewLikelihood = 0.9f,
                    averageSpend = 40f,
                    reputationImpact = 2f
                }
            };
        }

        void TrySpawnCustomer()
        {
            if (activeCustomers.Count >= maxCustomersInStore) return;

            // Calculate spawn chance based on store attractiveness
            float attractiveness = (cleanliness + stockLevel) / 200f; // 0-1
            attractiveness *= Mathf.Lerp(0.5f, 1.5f, reputation / 100f); // Reputation modifier

            if (Random.value < attractiveness)
            {
                CustomerPersonality customer = GenerateCustomer();
                activeCustomers.Add(customer);
                OnCustomerSpawned?.Invoke(customer);
            }
        }

        CustomerPersonality GenerateCustomer()
        {
            // Higher reputation attracts better customers
            float random = Random.value;
            float reputationFactor = reputation / 100f;

            if (random < 0.2f - reputationFactor * 0.1f)
                return customerTypes.Find(c => c.type == CustomerType.BargainHunter);
            else if (random < 0.4f - reputationFactor * 0.05f)
                return customerTypes.Find(c => c.type == CustomerType.ImpatientCustomer);
            else if (random < 0.6f + reputationFactor * 0.1f)
                return customerTypes.Find(c => c.type == CustomerType.ImpulseBuyer);
            else
                return customerTypes.Find(c => c.type == CustomerType.WealthyShopper);
        }

        public void CustomerLeaves(CustomerPersonality customer, bool satisfied)
        {
            activeCustomers.Remove(customer);

            if (satisfied)
            {
                reputation = Mathf.Min(100f, reputation + customer.reputationImpact);
            }
            else
            {
                reputation = Mathf.Max(0f, reputation - customer.reputationImpact * 2f);
            }

            OnReputationChanged?.Invoke(reputation);
        }

        public void UpdateStoreConditions(float newCleanliness, float newStockLevel, float newPriceLevel)
        {
            cleanliness = Mathf.Clamp(newCleanliness, 0, 100);
            stockLevel = Mathf.Clamp(newStockLevel, 0, 100);
            priceLevel = newPriceLevel;
        }

        public float GetReputation() => reputation;
        public int GetActiveCustomerCount() => activeCustomers.Count;
    }
}