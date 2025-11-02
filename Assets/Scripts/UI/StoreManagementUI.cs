using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MarketHustle.Economy;

namespace MarketHustle.UI
{
    /// <summary>
    /// UI for managing store operations: hiring employees, setting prices, viewing profits.
    /// </summary>
    public class StoreManagementUI : MonoBehaviour
    {
        public GameObject panel;
        public TextMeshProUGUI profitText;
        public TextMeshProUGUI employeeCountText;
        public Button hireButton;
        public Button closeButton;

        [Header("Store Stats")]
        public int baseDailyProfit = 100;
        public int employeeCost = 50;
        public int maxEmployees = 10;

        private int employeeCount = 1;
        private int dailyProfit;

        void Start()
        {
            if (panel != null) panel.SetActive(false);
            UpdateUI();

            if (hireButton != null)
                hireButton.onClick.AddListener(HireEmployee);

            if (closeButton != null)
                closeButton.onClick.AddListener(() => panel.SetActive(false));
        }

        void Update()
        {
            // Update profit calculation
            dailyProfit = baseDailyProfit + (employeeCount * 20);
        }

        public void ShowManagementPanel()
        {
            if (panel != null)
            {
                panel.SetActive(true);
                UpdateUI();
            }
        }

        void HireEmployee()
        {
            if (employeeCount >= maxEmployees) return;

            var economy = EconomyManager.Instance;
            if (economy != null && economy.TrySpend(employeeCost))
            {
                employeeCount++;
                UpdateUI();
                Debug.Log("Hired new employee! Total: " + employeeCount);
            }
            else
            {
                Debug.Log("Not enough money to hire employee");
            }
        }

        void UpdateUI()
        {
            if (profitText != null)
                profitText.text = $"Daily Profit: ${dailyProfit}";

            if (employeeCountText != null)
                employeeCountText.text = $"Employees: {employeeCount}/{maxEmployees}";

            if (hireButton != null)
            {
                hireButton.interactable = employeeCount < maxEmployees;
                var hireText = hireButton.GetComponentInChildren<TextMeshProUGUI>();
                if (hireText != null)
                    hireText.text = $"Hire (${employeeCost})";
            }
        }

        // Called by game systems to apply daily profit
        public void ApplyDailyProfit()
        {
            var economy = EconomyManager.Instance;
            if (economy != null)
            {
                economy.AddMoney(dailyProfit);
                Debug.Log($"Applied daily profit: ${dailyProfit}");
            }
        }
    }
}