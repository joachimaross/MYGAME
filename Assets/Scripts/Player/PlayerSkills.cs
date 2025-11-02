using UnityEngine;
using UnityEngine.Events;

namespace MarketHustle.Player
{
    /// <summary>
    /// Manages developable skills: Charisma, Logistics, Negotiation, Handiness.
    /// Skills level up through use and provide gameplay bonuses.
    /// </summary>
    public class PlayerSkills : MonoBehaviour
    {
        [System.Serializable]
        public class Skill
        {
            public string name;
            [Range(0, 100)] public float level = 0f;
            public float experience = 0f;
            public float experienceToNextLevel = 100f;
            public UnityEvent<float> OnLevelChanged;
        }

        [Header("Skills")]
        public Skill charisma = new Skill { name = "Charisma" };
        public Skill logistics = new Skill { name = "Logistics" };
        public Skill negotiation = new Skill { name = "Negotiation" };
        public Skill handiness = new Skill { name = "Handiness" };

        [Header("Experience Multipliers")]
        public float baseExpMultiplier = 1f;
        public float skillGainBonus = 0.1f; // Bonus from property upgrades

        public UnityEvent OnSkillLeveledUp;

        // Gain experience in a skill
        public void GainExperience(string skillName, float amount)
        {
            Skill skill = GetSkillByName(skillName);
            if (skill == null) return;

            float actualAmount = amount * (baseExpMultiplier + skillGainBonus);
            skill.experience += actualAmount;

            while (skill.experience >= skill.experienceToNextLevel && skill.level < 100f)
            {
                skill.experience -= skill.experienceToNextLevel;
                skill.level = Mathf.Min(100f, skill.level + 1f);
                skill.experienceToNextLevel *= 1.2f; // Increasing XP requirements
                skill.OnLevelChanged?.Invoke(skill.level);
                OnSkillLeveledUp?.Invoke();
            }
        }

        // Get skill level (0-100)
        public float GetSkillLevel(string skillName)
        {
            Skill skill = GetSkillByName(skillName);
            return skill != null ? skill.level : 0f;
        }

        // Apply skill bonuses to gameplay mechanics
        public float GetCharismaBonus() => charisma.level * 0.01f; // 1% per level for customer satisfaction
        public float GetLogisticsBonus() => logistics.level * 0.005f; // 0.5% per level for efficiency
        public float GetNegotiationBonus() => negotiation.level * 0.01f; // 1% per level for better deals
        public float GetHandinessBonus() => handiness.level * 0.02f; // 2% per level for repair speed

        private Skill GetSkillByName(string name)
        {
            switch (name.ToLower())
            {
                case "charisma": return charisma;
                case "logistics": return logistics;
                case "negotiation": return negotiation;
                case "handiness": return handiness;
                default: return null;
            }
        }

        // Public methods for specific skill usage
        public void UseCharisma() => GainExperience("charisma", 10f);
        public void UseLogistics() => GainExperience("logistics", 15f);
        public void UseNegotiation() => GainExperience("negotiation", 20f);
        public void UseHandiness() => GainExperience("handiness", 25f);
    }
}