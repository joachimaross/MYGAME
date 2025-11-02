using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace MarketHustle.Gameplay
{
    public enum HubType
    {
        BusinessClub,
        LocalBar,
        Gym,
        CoffeeShop,
        NetworkingEvent
    }

    [System.Serializable]
    public class SocialContact
    {
        public string name;
        public string occupation;
        public int relationshipLevel = 1; // 1-10
        public float networkingValue = 1f; // How useful their connections are
        public bool canOfferPartnership = false;
        public HubType preferredHub;
    }

    /// <summary>
    /// Manages social hubs where players can network, meet contacts, and form partnerships.
    /// </summary>
    public class SocialHub : MonoBehaviour
    {
        [Header("Hub Configuration")]
        public HubType hubType;
        public string hubName;
        public int maxOccupants = 20;
        public float entryFee = 50f;

        [Header("Networking")]
        public float contactSpawnRate = 0.1f; // Chance per visit to meet new contact
        public List<SocialContact> availableContacts;

        public UnityEvent<SocialContact> OnNewContact;
        public UnityEvent OnPartnershipOffered;

        private List<SocialContact> currentVisitors = new List<SocialContact>();

        void Start()
        {
            InitializeContacts();
        }

        void InitializeContacts()
        {
            availableContacts = new List<SocialContact>
            {
                new SocialContact
                {
                    name = "Marcus Chen",
                    occupation = "Real Estate Developer",
                    relationshipLevel = 1,
                    networkingValue = 2f,
                    canOfferPartnership = true,
                    preferredHub = HubType.BusinessClub
                },
                new SocialContact
                {
                    name = "Sarah Johnson",
                    occupation = "Marketing Consultant",
                    relationshipLevel = 1,
                    networkingValue = 1.5f,
                    canOfferPartnership = false,
                    preferredHub = HubType.CoffeeShop
                },
                new SocialContact
                {
                    name = "Roberto Garcia",
                    occupation = "Supplier Manager",
                    relationshipLevel = 1,
                    networkingValue = 1.8f,
                    canOfferPartnership = true,
                    preferredHub = HubType.NetworkingEvent
                },
                new SocialContact
                {
                    name = "Emma Wilson",
                    occupation = "Business Coach",
                    relationshipLevel = 1,
                    networkingValue = 1.3f,
                    canOfferPartnership = false,
                    preferredHub = HubType.Gym
                }
            };
        }

        public bool EnterHub()
        {
            var economy = MarketHustle.Economy.EconomyManager.Instance;
            if (economy == null || !economy.TrySpend((long)entryFee))
            {
                return false; // Can't afford entry
            }

            // Spawn some contacts
            PopulateHub();

            // Apply social need fulfillment
            var playerNeeds = FindObjectOfType<MarketHustle.Player.PlayerNeeds>();
            if (playerNeeds != null)
            {
                playerNeeds.Socialize(20f); // Social activities restore social need
            }

            // Chance to meet new contact
            if (Random.value < contactSpawnRate)
            {
                TryMeetNewContact();
            }

            return true;
        }

        void PopulateHub()
        {
            currentVisitors.Clear();
            int numContacts = Random.Range(3, maxOccupants);

            for (int i = 0; i < numContacts; i++)
            {
                var contact = availableContacts[Random.Range(0, availableContacts.Count)];
                if (!currentVisitors.Contains(contact))
                {
                    currentVisitors.Add(contact);
                }
            }
        }

        void TryMeetNewContact()
        {
            var eligibleContacts = availableContacts.FindAll(c =>
                c.preferredHub == hubType &&
                !currentVisitors.Contains(c)
            );

            if (eligibleContacts.Count > 0)
            {
                var newContact = eligibleContacts[Random.Range(0, eligibleContacts.Count)];
                currentVisitors.Add(newContact);
                OnNewContact?.Invoke(newContact);

                // Gain charisma experience
                var playerSkills = FindObjectOfType<MarketHustle.Player.PlayerSkills>();
                if (playerSkills != null)
                {
                    playerSkills.UseCharisma();
                }
            }
        }

        public void NetworkWithContact(SocialContact contact)
        {
            // Improve relationship
            contact.relationshipLevel = Mathf.Min(10, contact.relationshipLevel + 1);

            // Chance for partnership offer
            if (contact.canOfferPartnership && contact.relationshipLevel >= 7 &&
                Random.value < 0.3f)
            {
                OnPartnershipOffered?.Invoke();
            }

            // Gain negotiation experience
            var playerSkills = FindObjectOfType<MarketHustle.Player.PlayerSkills>();
            if (playerSkills != null)
            {
                playerSkills.UseNegotiation();
            }
        }

        public List<SocialContact> GetCurrentVisitors()
        {
            return currentVisitors;
        }

        public float GetNetworkingBonus()
        {
            float totalBonus = 0f;
            foreach (var contact in currentVisitors)
            {
                totalBonus += contact.networkingValue * (contact.relationshipLevel / 10f);
            }
            return totalBonus;
        }
    }
}