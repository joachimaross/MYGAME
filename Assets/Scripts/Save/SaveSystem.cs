using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace MarketHustle.Save
{
    [Serializable]
    public class SaveData
    {
        public long money;
        public List<string> ownedPropertyIds = new List<string>();
        // furniture serialized via FurnitureManager separate file (or embed here)
    }

    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }

        string fileName = "market_hustle_save.json";

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void SaveGame()
        {
            try
            {
                var data = new SaveData();
                var econ = Economy.EconomyManager.Instance;
                if (econ != null) data.money = econ.money;

                if (RealEstate.RealEstateManager.Instance != null)
                {
                    foreach (var p in RealEstate.RealEstateManager.Instance.availableProperties)
                    {
                        if (p.owned) data.ownedPropertyIds.Add(p.id);
                    }
                }

                string json = JsonUtility.ToJson(data, true);
                string path = Path.Combine(Application.persistentDataPath, fileName);
                File.WriteAllText(path, json);
                Debug.Log("Saved game to " + path);

                // also save furniture placements
                Furniture.FurnitureManager.Instance?.SaveFurniture();
            }
            catch (Exception ex)
            {
                Debug.LogError("Save failed: " + ex.Message);
            }
        }

        public void LoadGame()
        {
            try
            {
                string path = Path.Combine(Application.persistentDataPath, fileName);
                if (!File.Exists(path)) { Debug.Log("No save file found: " + path); return; }

                string json = File.ReadAllText(path);
                var data = JsonUtility.FromJson<SaveData>(json);
                if (data == null) return;

                var econ = Economy.EconomyManager.Instance;
                if (econ != null) econ.money = data.money;

                if (RealEstate.RealEstateManager.Instance != null)
                {
                    foreach (var p in RealEstate.RealEstateManager.Instance.availableProperties)
                    {
                        p.owned = data.ownedPropertyIds.Contains(p.id);
                    }
                }

                // load furniture
                Furniture.FurnitureManager.Instance?.LoadFurniture();

                Debug.Log("Loaded game from " + path);
            }
            catch (Exception ex)
            {
                Debug.LogError("Load failed: " + ex.Message);
            }
        }
    }
}
