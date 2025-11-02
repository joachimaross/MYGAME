using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MarketHustle.UI
{
    /// <summary>
    /// Manages mobile-specific UI optimizations and responsive design.
    /// </summary>
    public class MobileUIManager : MonoBehaviour
    {
        public Canvas canvas;
        public float referenceWidth = 1080f;
        public float referenceHeight = 1920f;

        [Header("UI Elements")]
        public TextMeshProUGUI moneyText;
        public Button interactButton;
        public RuntimeJoystick joystick;

        void Start()
        {
            OptimizeForMobile();
            SetupMobileUI();
        }

        void OptimizeForMobile()
        {
            if (canvas == null) return;

            // Set up canvas scaler for mobile
            var scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(referenceWidth, referenceHeight);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f; // Balance between width and height

            // Add safe area handling for notched devices
            ApplySafeArea();
        }

        void ApplySafeArea()
        {
            if (canvas == null) return;

            Rect safeArea = Screen.safeArea;
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            var rectTransform = canvas.GetComponent<RectTransform>();
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }

        void SetupMobileUI()
        {
            // Position money text in top-left safe area
            if (moneyText != null)
            {
                var moneyRT = moneyText.GetComponent<RectTransform>();
                moneyRT.anchorMin = new Vector2(0f, 1f);
                moneyRT.anchorMax = new Vector2(0f, 1f);
                moneyRT.pivot = new Vector2(0f, 1f);
                moneyRT.anchoredPosition = new Vector2(20f, -20f);
                moneyRT.sizeDelta = new Vector2(200f, 50f);
            }

            // Position interact button in bottom-right
            if (interactButton != null)
            {
                var buttonRT = interactButton.GetComponent<RectTransform>();
                buttonRT.anchorMin = new Vector2(1f, 0f);
                buttonRT.anchorMax = new Vector2(1f, 0f);
                buttonRT.pivot = new Vector2(1f, 0f);
                buttonRT.anchoredPosition = new Vector2(-20f, 20f);
                buttonRT.sizeDelta = new Vector2(120f, 80f);

                // Ensure button text is readable on mobile
                var buttonText = interactButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.fontSize = 24;
                    buttonText.enableAutoSizing = true;
                    buttonText.fontSizeMin = 18;
                    buttonText.fontSizeMax = 28;
                }
            }

            // Ensure joystick is properly positioned
            if (joystick != null)
            {
                var joystickRT = joystick.GetComponent<RectTransform>();
                joystickRT.anchorMin = new Vector2(0f, 0f);
                joystickRT.anchorMax = new Vector2(0f, 0f);
                joystickRT.pivot = new Vector2(0f, 0f);
                joystickRT.anchoredPosition = new Vector2(20f, 20f);
            }
        }

        // Public method to show/hide mobile UI elements
        public void SetMobileUIActive(bool active)
        {
            if (interactButton != null)
                interactButton.gameObject.SetActive(active);

            if (joystick != null)
                joystick.gameObject.SetActive(active);
        }

        // Handle orientation changes
        void OnRectTransformDimensionsChange()
        {
            ApplySafeArea();
        }
    }
}