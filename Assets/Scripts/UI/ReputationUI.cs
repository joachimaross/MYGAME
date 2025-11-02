using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MarketHustle.Gameplay;

namespace MarketHustle.UI
{
    /// <summary>
    /// UI component to display store reputation and customer feedback.
    /// Shows reputation meter, recent reviews, and customer satisfaction.
    /// </summary>
    public class ReputationUI : MonoBehaviour
    {
        [Header("Reputation Display")]
        public Slider reputationBar;
        public TMP_Text reputationText;
        public Image reputationFill;

        [Header("Customer Feedback")]
        public TMP_Text customerCountText;
        public TMP_Text recentReviewText;
        public GameObject positiveReviewIcon;
        public GameObject negativeReviewIcon;

        [Header("Colors")]
        public Color lowReputationColor = Color.red;
        public Color mediumReputationColor = Color.yellow;
        public Color highReputationColor = Color.green;

        private CustomerSystem customerSystem;
        private string[] positiveReviews = {
            "Great store! Will come back!",
            "Love the selection here.",
            "Friendly staff and clean store.",
            "Found exactly what I needed."
        };

        private string[] negativeReviews = {
            "Too expensive for what you get.",
            "Store was messy and understocked.",
            "Long wait times, not coming back.",
            "Poor customer service experience."
        };

        void Start()
        {
            customerSystem = FindObjectOfType<CustomerSystem>();
            if (customerSystem != null)
            {
                customerSystem.OnReputationChanged.AddListener(UpdateReputationUI);
            }
            UpdateReputationUI(50f); // Default
        }

        void Update()
        {
            if (customerSystem != null)
            {
                customerCountText.text = $"Customers in store: {customerSystem.GetActiveCustomerCount()}";
            }
        }

        void UpdateReputationUI(float newReputation)
        {
            reputationBar.value = newReputation / 100f;
            reputationText.text = $"Reputation: {newReputation:F0}/100";

            // Update color based on reputation level
            if (newReputation < 30f)
                reputationFill.color = lowReputationColor;
            else if (newReputation < 70f)
                reputationFill.color = mediumReputationColor;
            else
                reputationFill.color = highReputationColor;
        }

        public void ShowReview(bool positive)
        {
            string review = positive ?
                positiveReviews[Random.Range(0, positiveReviews.Length)] :
                negativeReviews[Random.Range(0, negativeReviews.Length)];

            recentReviewText.text = $"Recent Review: \"{review}\"";
            positiveReviewIcon.SetActive(positive);
            negativeReviewIcon.SetActive(!positive);

            // Auto-hide after 5 seconds
            Invoke("ClearReview", 5f);
        }

        void ClearReview()
        {
            recentReviewText.text = "No recent reviews";
            positiveReviewIcon.SetActive(false);
            negativeReviewIcon.SetActive(false);
        }
    }
}