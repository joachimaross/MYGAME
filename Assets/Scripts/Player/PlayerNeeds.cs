using UnityEngine;
using UnityEngine.Events;

namespace MarketHustle.Player
{
    /// <summary>
    /// Manages player needs similar to The Sims: Energy, Hunger, Social, Hygiene.
    /// Needs decay over time and affect gameplay mechanics.
    /// </summary>
    public class PlayerNeeds : MonoBehaviour
    {
        [Header("Current Needs (0-100)")]
        [Range(0, 100)] public float energy = 100f;
        [Range(0, 100)] public float hunger = 100f;
        [Range(0, 100)] public float social = 100f;
        [Range(0, 100)] public float hygiene = 100f;

        [Header("Decay Rates (per minute)")]
        public float energyDecay = 5f;
        public float hungerDecay = 3f;
        public float socialDecay = 2f;
        public float hygieneDecay = 4f;

        [Header("Critical Thresholds")]
        public float criticalThreshold = 20f;

        public UnityEvent OnNeedsCritical;
        public UnityEvent OnNeedsRestored;

        private float decayTimer = 0f;
        private bool needsCritical = false;

        void Update()
        {
            decayTimer += Time.deltaTime;
            if (decayTimer >= 60f) // Decay every minute
            {
                DecayNeeds();
                decayTimer = 0f;
            }
        }

        void DecayNeeds()
        {
            energy = Mathf.Max(0, energy - energyDecay);
            hunger = Mathf.Max(0, hunger - hungerDecay);
            social = Mathf.Max(0, social - socialDecay);
            hygiene = Mathf.Max(0, hygiene - hygieneDecay);

            ApplyNeedEffects();

            bool currentlyCritical = IsAnyNeedCritical();
            if (currentlyCritical && !needsCritical)
            {
                needsCritical = true;
                OnNeedsCritical?.Invoke();
            }
            else if (!currentlyCritical && needsCritical)
            {
                needsCritical = false;
                OnNeedsRestored?.Invoke();
            }
        }

        void ApplyNeedEffects()
        {
            var controller = GetComponent<PlayerController>();
            if (controller != null)
            {
                // Low energy reduces movement speed
                float energyMultiplier = Mathf.Lerp(0.5f, 1f, energy / 100f);
                controller.moveSpeed = 5f * energyMultiplier;

                // Low hunger reduces work efficiency (could affect store management)
                // Low social affects customer interactions
                // Low hygiene affects reputation
            }
        }

        bool IsAnyNeedCritical()
        {
            return energy <= criticalThreshold ||
                   hunger <= criticalThreshold ||
                   social <= criticalThreshold ||
                   hygiene <= criticalThreshold;
        }

        // Public methods for fulfilling needs
        public void Eat(float amount)
        {
            hunger = Mathf.Min(100, hunger + amount);
        }

        public void Sleep(float amount)
        {
            energy = Mathf.Min(100, energy + amount);
        }

        public void Socialize(float amount)
        {
            social = Mathf.Min(100, social + amount);
        }

        public void Clean(float amount)
        {
            hygiene = Mathf.Min(100, hygiene + amount);
        }

        // Get need levels for UI
        public float GetEnergy() => energy;
        public float GetHunger() => hunger;
        public float GetSocial() => social;
        public float GetHygiene() => hygiene;
    }
}