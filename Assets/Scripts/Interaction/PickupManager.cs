using System.Collections.Generic;
using UnityEngine;

namespace MarketHustle.Interaction
{
    public class PickupManager : MonoBehaviour
    {
        public static PickupManager Instance { get; private set; }

        List<StoreItemPickup> nearby = new List<StoreItemPickup>();

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void RegisterNearby(StoreItemPickup item)
        {
            if (!nearby.Contains(item)) nearby.Add(item);
        }

        public void UnregisterNearby(StoreItemPickup item)
        {
            if (nearby.Contains(item)) nearby.Remove(item);
        }

        public void InteractNearest()
        {
            if (nearby.Count == 0) return;

            // find nearest to player
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            StoreItemPickup best = null;
            float bestDist = float.MaxValue;
            foreach (var it in nearby)
            {
                if (it == null) continue;
                float d = Vector3.Distance(player.transform.position, it.transform.position);
                if (d < bestDist)
                {
                    bestDist = d; best = it;
                }
            }

            best?.Interact();
        }
    }
}
