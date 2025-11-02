using UnityEngine;

namespace MarketHustle.Interaction
{
    /// <summary>
    /// Attach to shelf items in the supermarket. When player presses the interact key while within trigger,
    /// the player 'picks' the item and receives money (simulates a sale or action). For mobile you can wire
    /// this to a tap gesture or button calling Interact().
    /// </summary>
    public class StoreItemPickup : MonoBehaviour
    {
        public int value = 5; // money rewarded on pickup
        bool playerNearby = false;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerNearby = true;
                PickupManager.Instance?.RegisterNearby(this);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerNearby = false;
                PickupManager.Instance?.UnregisterNearby(this);
            }
        }

        void Update()
        {
            if (playerNearby && Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }

        public void Interact()
        {
            var econ = MarketHustle.Economy.EconomyManager.Instance;
            if (econ != null) econ.AddMoney(value);
            else
            {
                var ms = FindObjectOfType<MoneySystem>();
                if (ms != null) ms.AddMoney(value);
            }

            // simple feedback: destroy or disable
            Destroy(gameObject);
        }
    }
}
