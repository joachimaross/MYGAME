using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MarketHustle.Gameplay
{
    /// <summary>
    /// Handles visual feedback effects like screen shakes, particle effects, and floating text.
    /// Provides satisfying feedback for player actions.
    /// </summary>
    public class VisualFeedback : MonoBehaviour
    {
        public static VisualFeedback Instance { get; private set; }

        [Header("Screen Shake")]
        public float shakeDuration = 0.2f;
        public float shakeMagnitude = 0.1f;

        [Header("Floating Text")]
        public GameObject floatingTextPrefab;
        public Canvas uiCanvas;

        [Header("Particle Effects")]
        public ParticleSystem moneyParticles;
        public ParticleSystem levelUpParticles;
        public ParticleSystem reputationParticles;

        [Header("UI Animations")]
        public Animator moneyTextAnimator;
        public Animator reputationBarAnimator;

        private Vector3 originalCameraPosition;
        private Camera mainCamera;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            mainCamera = Camera.main;
            if (mainCamera != null)
            {
                originalCameraPosition = mainCamera.transform.localPosition;
            }
        }

        // Screen shake effect
        public void ShakeScreen(float duration = 0.2f, float magnitude = 0.1f)
        {
            if (mainCamera == null) return;

            shakeDuration = duration;
            shakeMagnitude = magnitude;
            InvokeRepeating("DoShake", 0f, 0.01f);
            Invoke("StopShake", duration);
        }

        void DoShake()
        {
            if (mainCamera == null) return;

            Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            shakeOffset.z = 0; // Don't shake depth
            mainCamera.transform.localPosition = originalCameraPosition + shakeOffset;
        }

        void StopShake()
        {
            CancelInvoke("DoShake");
            if (mainCamera != null)
            {
                mainCamera.transform.localPosition = originalCameraPosition;
            }
        }

        // Floating text effects
        public void ShowFloatingText(string text, Vector3 worldPosition, Color color, float duration = 2f)
        {
            if (floatingTextPrefab == null || uiCanvas == null) return;

            // Convert world position to screen position
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

            // Create floating text
            GameObject textObj = Instantiate(floatingTextPrefab, uiCanvas.transform);
            textObj.transform.position = screenPos;

            TMP_Text tmpText = textObj.GetComponent<TMP_Text>();
            if (tmpText != null)
            {
                tmpText.text = text;
                tmpText.color = color;
            }

            // Animate upward and fade out
            StartCoroutine(AnimateFloatingText(textObj, duration));
        }

        System.Collections.IEnumerator AnimateFloatingText(GameObject textObj, float duration)
        {
            float elapsed = 0f;
            Vector3 startPos = textObj.transform.position;
            TMP_Text tmpText = textObj.GetComponent<TMP_Text>();
            Color startColor = tmpText != null ? tmpText.color : Color.white;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;

                // Move upward
                textObj.transform.position = startPos + Vector3.up * (progress * 100f);

                // Fade out
                if (tmpText != null)
                {
                    Color newColor = startColor;
                    newColor.a = 1f - progress;
                    tmpText.color = newColor;
                }

                yield return null;
            }

            Destroy(textObj);
        }

        // Money earned feedback
        public void ShowMoneyEarned(long amount, Vector3 position)
        {
            string text = $"+${amount:N0}";
            ShowFloatingText(text, position, Color.green);

            // Play cha-ching sound
            var audioManager = MarketHustle.Audio.AudioManager.Instance;
            if (audioManager != null)
            {
                audioManager.PlayChaChing();
            }

            // Screen shake for big earnings
            if (amount >= 1000)
            {
                ShakeScreen(0.3f, 0.15f);
            }

            // Money particles
            if (moneyParticles != null)
            {
                moneyParticles.transform.position = position;
                moneyParticles.Play();
            }

            // Animate money UI
            if (moneyTextAnimator != null)
            {
                moneyTextAnimator.SetTrigger("MoneyEarned");
            }
        }

        // Property value increase feedback
        public void ShowPropertyValueIncrease(float increasePercent, Vector3 position)
        {
            string text = $"+{increasePercent:F1}% Value";
            ShowFloatingText(text, position, Color.cyan);

            // Subtle screen shake
            ShakeScreen(0.1f, 0.05f);
        }

        // Level up feedback
        public void ShowLevelUp(string skillName, int newLevel, Vector3 position)
        {
            string text = $"{skillName} Level {newLevel}!";
            ShowFloatingText(text, position, Color.yellow, 3f);

            // Level up particles
            if (levelUpParticles != null)
            {
                levelUpParticles.transform.position = position;
                levelUpParticles.Play();
            }

            // Level up sound
            var audioManager = MarketHustle.Audio.AudioManager.Instance;
            if (audioManager != null)
            {
                audioManager.PlayLevelUp();
            }

            // Stronger screen shake
            ShakeScreen(0.4f, 0.2f);
        }

        // Reputation change feedback
        public void ShowReputationChange(float change, Vector3 position)
        {
            string text = change > 0 ? $"+{change:F1} Rep" : $"{change:F1} Rep";
            Color color = change > 0 ? Color.green : Color.red;
            ShowFloatingText(text, position, color);

            // Reputation particles
            if (reputationParticles != null)
            {
                reputationParticles.transform.position = position;
                reputationParticles.Play();
            }

            // Animate reputation bar
            if (reputationBarAnimator != null)
            {
                reputationBarAnimator.SetTrigger(change > 0 ? "ReputationUp" : "ReputationDown");
            }

            // Reputation sound
            var audioManager = MarketHustle.Audio.AudioManager.Instance;
            if (audioManager != null)
            {
                if (change > 0)
                    audioManager.PlayReputationIncrease();
                else
                    audioManager.PlayCustomerDissatisfied();
            }
        }

        // Perfect sale feedback (when prices are set optimally)
        public void ShowPerfectSale(Vector3 position)
        {
            ShowFloatingText("PERFECT SALE!", position, Color.magenta, 3f);
            ShakeScreen(0.5f, 0.25f);

            // Extra money particles
            if (moneyParticles != null)
            {
                moneyParticles.transform.position = position;
                moneyParticles.Play();
                Invoke("PlayMoneyParticlesAgain", 0.2f);
            }
        }

        void PlayMoneyParticlesAgain()
        {
            if (moneyParticles != null)
            {
                moneyParticles.Play();
            }
        }
    }
}