using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace MarketHustle.UI
{
    /// <summary>
    /// Advanced UI features: tooltips, notifications, mini-map, and dynamic UI elements.
    /// </summary>
    public class AdvancedUI : MonoBehaviour
    {
        public static AdvancedUI Instance { get; private set; }

        [Header("Tooltip System")]
        public GameObject tooltipPanel;
        public TextMeshProUGUI tooltipText;
        public float tooltipDelay = 0.5f;

        [Header("Notification System")]
        public GameObject notificationPrefab;
        public Transform notificationContainer;
        public int maxNotifications = 5;

        [Header("Mini-map")]
        public RawImage miniMap;
        public Camera miniMapCamera;
        public float miniMapSize = 200f;

        [Header("Dynamic UI")]
        public GameObject dynamicPanelPrefab;
        public Transform dynamicContainer;

        private List<GameObject> activeNotifications = new List<GameObject>();
        private float tooltipTimer;
        private bool tooltipShowing;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void Start()
        {
            InitializeMiniMap();
        }

        void Update()
        {
            UpdateTooltip();
        }

        // Tooltip System
        public void ShowTooltip(string text, Vector3 position)
        {
            if (tooltipPanel == null || tooltipText == null) return;

            tooltipText.text = text;
            tooltipPanel.transform.position = position + new Vector3(0f, 50f, 0f);
            tooltipPanel.SetActive(true);
            tooltipShowing = true;
            tooltipTimer = 0f;
        }

        public void HideTooltip()
        {
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(false);
                tooltipShowing = false;
            }
        }

        void UpdateTooltip()
        {
            if (tooltipShowing)
            {
                tooltipTimer += Time.deltaTime;
                if (tooltipTimer >= tooltipDelay)
                {
                    // Auto-hide after delay
                    HideTooltip();
                }
            }
        }

        // Notification System
        public void ShowNotification(string message, NotificationType type = NotificationType.Info, float duration = 3f)
        {
            if (notificationPrefab == null || notificationContainer == null) return;

            // Limit notifications
            if (activeNotifications.Count >= maxNotifications)
            {
                Destroy(activeNotifications[0]);
                activeNotifications.RemoveAt(0);
            }

            GameObject notification = Instantiate(notificationPrefab, notificationContainer);
            var notificationScript = notification.AddComponent<NotificationItem>();
            notificationScript.Initialize(message, type, duration);

            activeNotifications.Add(notification);

            // Animate in
            StartCoroutine(AnimateNotificationIn(notification));
        }

        System.Collections.IEnumerator AnimateNotificationIn(GameObject notification)
        {
            var rect = notification.GetComponent<RectTransform>();
            var startPos = rect.anchoredPosition;
            var endPos = new Vector2(0f, startPos.y);

            float duration = 0.3f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                yield return null;
            }
        }

        public void RemoveNotification(GameObject notification)
        {
            if (activeNotifications.Contains(notification))
            {
                activeNotifications.Remove(notification);
                Destroy(notification);
            }
        }

        // Mini-map System
        void InitializeMiniMap()
        {
            if (miniMapCamera == null || miniMap == null) return;

            // Set up mini-map camera
            miniMapCamera.orthographic = true;
            miniMapCamera.orthographicSize = miniMapSize / 2f;
            miniMapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            // Position above player
            UpdateMiniMapPosition();
        }

        void UpdateMiniMapPosition()
        {
            if (miniMapCamera == null) return;

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                miniMapCamera.transform.position = player.transform.position + new Vector3(0f, 50f, 0f);
            }
        }

        public void ToggleMiniMap(bool show)
        {
            if (miniMap != null)
                miniMap.gameObject.SetActive(show);
        }

        // Dynamic UI Panels
        public GameObject CreateDynamicPanel(string title, Vector2 size, Vector2 position)
        {
            if (dynamicPanelPrefab == null || dynamicContainer == null) return null;

            GameObject panel = Instantiate(dynamicPanelPrefab, dynamicContainer);
            var rect = panel.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = position;

            // Set title
            var titleText = panel.GetComponentInChildren<TextMeshProUGUI>();
            if (titleText != null)
                titleText.text = title;

            return panel;
        }

        public void ShowPropertyDetails(MarketHustle.RealEstate.PropertyData property)
        {
            var panel = CreateDynamicPanel("Property Details", new Vector2(300f, 200f), new Vector2(200f, 0f));

            if (panel != null)
            {
                var content = panel.transform.Find("Content");
                if (content != null)
                {
                    var text = content.GetComponentInChildren<TextMeshProUGUI>();
                    if (text != null)
                    {
                        text.text = $"{property.displayName}\n" +
                                   $"Price: ${property.price}\n" +
                                   $"Rent: ${property.monthlyRent}\n" +
                                   $"Type: {property.propertyType}\n" +
                                   $"Style: {property.style}";
                    }
                }
            }
        }

        // Achievement Popup
        public void ShowAchievementPopup(string achievementName, string description)
        {
            ShowNotification($"🏆 {achievementName}\n{description}", NotificationType.Success, 5f);
        }

        // Market Trend Indicator
        public void UpdateMarketIndicator(float housingTrend, float retailTrend)
        {
            string trendText = $"Housing: {(housingTrend > 1 ? "📈" : housingTrend < 1 ? "📉" : "➡️")} {housingTrend:F1}x\n" +
                              $"Retail: {(retailTrend > 1 ? "📈" : retailTrend < 1 ? "📉" : "➡️")} {retailTrend:F1}x";

            ShowNotification(trendText, NotificationType.Info, 10f);
        }
    }

    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error
    }

    public class NotificationItem : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public Image background;
        private float duration;
        private float timer;

        public void Initialize(string message, NotificationType type, float duration)
        {
            this.duration = duration;
            timer = 0f;

            if (text != null)
                text.text = message;

            if (background != null)
            {
                switch (type)
                {
                    case NotificationType.Success:
                        background.color = new Color(0.2f, 0.8f, 0.2f, 0.9f);
                        break;
                    case NotificationType.Warning:
                        background.color = new Color(0.8f, 0.8f, 0.2f, 0.9f);
                        break;
                    case NotificationType.Error:
                        background.color = new Color(0.8f, 0.2f, 0.2f, 0.9f);
                        break;
                    default:
                        background.color = new Color(0.2f, 0.6f, 0.8f, 0.9f);
                        break;
                }
            }
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= duration)
            {
                AdvancedUI.Instance.RemoveNotification(gameObject);
            }
        }
    }
}