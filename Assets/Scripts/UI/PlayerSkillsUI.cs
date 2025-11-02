using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MarketHustle.Player;

namespace MarketHustle.UI
{
    /// <summary>
    /// UI component to display player skills and their levels.
    /// Shows progress bars and level numbers for each skill.
    /// </summary>
    public class PlayerSkillsUI : MonoBehaviour
    {
        [Header("Skill Bars")]
        public Slider charismaBar;
        public Slider logisticsBar;
        public Slider negotiationBar;
        public Slider handinessBar;

        [Header("Skill Texts")]
        public TMP_Text charismaText;
        public TMP_Text logisticsText;
        public TMP_Text negotiationText;
        public TMP_Text handinessText;

        [Header("Level Up Effect")]
        public GameObject levelUpEffect;
        public AudioClip levelUpSound;

        private PlayerSkills playerSkills;
        private AudioSource audioSource;

        void Start()
        {
            playerSkills = FindObjectOfType<PlayerSkills>();
            audioSource = GetComponent<AudioSource>();
            if (playerSkills != null)
            {
                playerSkills.OnSkillLeveledUp.AddListener(PlayLevelUpEffect);
            }
            UpdateUI();
        }

        void Update()
        {
            if (playerSkills != null)
            {
                UpdateUI();
            }
        }

        void UpdateUI()
        {
            charismaBar.value = playerSkills.GetSkillLevel("charisma") / 100f;
            logisticsBar.value = playerSkills.GetSkillLevel("logistics") / 100f;
            negotiationBar.value = playerSkills.GetSkillLevel("negotiation") / 100f;
            handinessBar.value = playerSkills.GetSkillLevel("handiness") / 100f;

            charismaText.text = $"Charisma: {playerSkills.GetSkillLevel("charisma"):F0}";
            logisticsText.text = $"Logistics: {playerSkills.GetSkillLevel("logistics"):F0}";
            negotiationText.text = $"Negotiation: {playerSkills.GetSkillLevel("negotiation"):F0}";
            handinessText.text = $"Handiness: {playerSkills.GetSkillLevel("handiness"):F0}";
        }

        void PlayLevelUpEffect()
        {
            if (levelUpEffect != null)
            {
                levelUpEffect.SetActive(true);
                Invoke("HideLevelUpEffect", 2f);
            }
            if (audioSource != null && levelUpSound != null)
            {
                audioSource.PlayOneShot(levelUpSound);
            }
        }

        void HideLevelUpEffect()
        {
            if (levelUpEffect != null)
            {
                levelUpEffect.SetActive(false);
            }
        }
    }
}