using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MarketHustle.RealEstate;
using MarketHustle.Economy;
using MarketHustle.Gameplay;

namespace MarketHustle.UI
{
    /// <summary>
    /// Empire Management UI showing overview of all businesses and properties.
    /// Displays financial health, property portfolio, and business metrics.
    /// </summary>
    public class EmpireViewUI : MonoBehaviour
    {
        [Header("Financial Overview")]
        public TMP_Text totalNetWorthText;
        public TMP_Text monthlyIncomeText;
        public TMP_Text monthlyExpensesText;
        public TMP_Text cashOnHandText;

        [Header("Property Portfolio")]
        public Transform propertyListContainer;
        public GameObject propertyListItemPrefab;

        [Header("Business Metrics")]
        public TMP_Text totalStoresText;
        public TMP_Text totalEmployeesText;
        public TMP_Text averageReputationText;
        public Slider reputationBar;

        [Header("Charts/Graphs")]
        public RectTransform netWorthGraph;
        public RectTransform incomeGraph;

        private RealEstateManager realEstateManager;
        private EconomyManager economyManager;
        private CustomerSystem customerSystem;
        private EmployeeSystem employeeSystem;

        void Start()
        {
            realEstateManager = FindObjectOfType<RealEstateManager>();
            economyManager = EconomyManager.Instance;
            customerSystem = FindObjectOfType<CustomerSystem>();
            employeeSystem = FindObjectOfType<EmployeeSystem>();

            UpdateEmpireOverview();
        }

        void Update()
        {
            // Update real-time values
            if (cashOnHandText != null && economyManager != null)
            {
                cashOnHandText.text = $"Cash: ${economyManager.money:N0}";
            }
        }

        public void UpdateEmpireOverview()
        {
            if (realEstateManager == null || economyManager == null) return;

            // Financial Overview
            var ownedProperties = realEstateManager.GetOwnedProperties();
            long totalPropertyValue = (long)realEstateManager.GetTotalPropertyValue();
            long netWorth = economyManager.money + totalPropertyValue;

            totalNetWorthText.text = $"Net Worth: ${netWorth:N0}";
            cashOnHandText.text = $"Cash: ${economyManager.money:N0}";

            // Calculate monthly income/expenses (simplified)
            long monthlyIncome = CalculateMonthlyIncome();
            long monthlyExpenses = CalculateMonthlyExpenses();

            monthlyIncomeText.text = $"Income: ${monthlyIncome:N0}/mo";
            monthlyExpensesText.text = $"Expenses: ${monthlyExpenses:N0}/mo";

            // Property Portfolio
            UpdatePropertyList(ownedProperties);

            // Business Metrics
            int storeCount = ownedProperties.FindAll(p =>
                p.propertyType == RealEstate.PropertyType.Condo || // Assuming stores are condos for now
                p.propertyType == RealEstate.PropertyType.Villa).Count;

            totalStoresText.text = $"Stores: {storeCount}";

            if (employeeSystem != null)
            {
                totalEmployeesText.text = $"Employees: {employeeSystem.employees.Count}";
            }

            if (customerSystem != null)
            {
                averageReputationText.text = $"Avg Reputation: {customerSystem.GetReputation():F1}";
                reputationBar.value = customerSystem.GetReputation() / 100f;
            }
        }

        void UpdatePropertyList(System.Collections.Generic.List<RealEstate.PropertyData> properties)
        {
            // Clear existing items
            foreach (Transform child in propertyListContainer)
            {
                Destroy(child.gameObject);
            }

            // Create new items
            foreach (var property in properties)
            {
                var item = Instantiate(propertyListItemPrefab, propertyListContainer);
                var itemScript = item.GetComponent<PropertyListItem>();

                if (itemScript != null)
                {
                    itemScript.Setup(property);
                }
            }
        }

        long CalculateMonthlyIncome()
        {
            long income = 0;

            // Store profits (simplified - would need actual store sales data)
            income += 5000; // Placeholder

            // Property appreciation isn't monthly income, but could include rental income
            var ownedProperties = realEstateManager.GetOwnedProperties();
            foreach (var prop in ownedProperties)
            {
                if (prop.forRent)
                {
                    income += prop.monthlyRent;
                }
            }

            return income;
        }

        long CalculateMonthlyExpenses()
        {
            long expenses = 0;

            // Employee salaries
            if (employeeSystem != null)
            {
                foreach (var employee in employeeSystem.employees)
                {
                    expenses += (long)employee.salary;
                }
            }

            // Property maintenance (simplified)
            var ownedProperties = realEstateManager.GetOwnedProperties();
            foreach (var prop in ownedProperties)
            {
                expenses += prop.monthlyRent / 10; // 10% of rent as maintenance
            }

            return expenses;
        }

        public void ShowDetailedPropertyView(RealEstate.PropertyData property)
        {
            // Open detailed property management window
            // This would show property-specific metrics, upgrades, etc.
        }

        public void ShowBusinessAnalytics()
        {
            // Show detailed business performance charts
            // Revenue over time, customer trends, etc.
        }
    }
}