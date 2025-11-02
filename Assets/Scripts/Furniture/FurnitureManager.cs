using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MarketHustle.Furniture
{
    [Serializable]
    public class FurnitureData
    {
        public string prefabName;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale = Vector3.one;
        public string parentPropertyId; // which home/property this furniture belongs to
    }

    public class FurnitureManager : MonoBehaviour
    {
        public static FurnitureManager Instance { get; private set; }

        // A registry of furniture prefabs by name (populate in inspector)
        public GameObject[] furniturePrefabs;

        // runtime placed furniture
        public List<FurnitureData> placedFurniture = new List<FurnitureData>();

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public GameObject GetPrefabByName(string prefabName)
        {
            foreach (var p in furniturePrefabs)
            {
                if (p.name == prefabName) return p;
            }
            return null;
        }

        // Place a furniture prefab in the scene and record it
        public GameObject PlaceFurniture(string prefabName, Vector3 position, Quaternion rotation, Vector3 scale, string propertyId)
        {
            var prefab = GetPrefabByName(prefabName);
            if (prefab == null) { Debug.LogWarning("Prefab not found: " + prefabName); return null; }

            GameObject go = Instantiate(prefab, position, rotation);
            go.transform.localScale = scale;

            var data = new FurnitureData { prefabName = prefabName, position = position, rotation = rotation, scale = scale, parentPropertyId = propertyId };
            placedFurniture.Add(data);
            return go;
        }

        // Remove entry from record (does not destroy GameObject)
        public void RemovePlacedFurniture(FurnitureData data)
        {
            placedFurniture.Remove(data);
        }

        // Simple JSON save/load for furniture placements
        public void SaveFurniture(string filename = "furniture.json")
        {
            try
            {
                string path = Path.Combine(Application.persistentDataPath, filename);
                string json = JsonUtility.ToJson(new Wrapper { items = placedFurniture.ToArray() }, true);
                File.WriteAllText(path, json);
                Debug.Log("Saved furniture to " + path);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error saving furniture: " + ex.Message);
            }
        }

        public void LoadFurniture(string filename = "furniture.json")
        {
            try
            {
                string path = Path.Combine(Application.persistentDataPath, filename);
                if (!File.Exists(path)) { Debug.Log("No furniture save found: " + path); return; }
                string json = File.ReadAllText(path);
                var wrapper = JsonUtility.FromJson<Wrapper>(json);
                placedFurniture = new List<FurnitureData>(wrapper.items ?? new FurnitureData[0]);
                Debug.Log("Loaded furniture from " + path);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error loading furniture: " + ex.Message);
            }
        }

        [Serializable]
        private class Wrapper { public FurnitureData[] items; }
    }
}
