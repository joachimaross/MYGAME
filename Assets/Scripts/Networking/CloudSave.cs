using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

namespace MarketHustle.Networking
{
    /// <summary>
    /// Cloud save functionality for cross-device synchronization.
    /// </summary>
    public class CloudSave : MonoBehaviour
    {
        public static CloudSave Instance { get; private set; }

        [Header("Cloud Settings")]
        public string cloudURL = "https://your-cloud-service.com/api/save";
        public string apiKey = "your-api-key";
        public float autoSaveInterval = 300f; // 5 minutes

        private float lastAutoSave;
        private bool isOnline = true;

        [System.Serializable]
        public class CloudSaveData
        {
            public string playerId;
            public long timestamp;
            public string gameVersion;
            public string saveData;
            public string checksum;
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            CheckConnectivity();
            LoadFromCloud();
        }

        void Update()
        {
            // Auto-save to cloud
            if (Time.time - lastAutoSave > autoSaveInterval && isOnline)
            {
                SaveToCloud();
                lastAutoSave = Time.time;
            }
        }

        public void SaveToCloud()
        {
            if (!isOnline) return;

            var localSave = GetLocalSaveData();
            if (string.IsNullOrEmpty(localSave)) return;

            var cloudData = new CloudSaveData
            {
                playerId = GetPlayerId(),
                timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                gameVersion = Application.version,
                saveData = localSave,
                checksum = GenerateChecksum(localSave)
            };

            string json = JsonUtility.ToJson(cloudData);
            StartCoroutine(UploadSaveData(json));
        }

        public void LoadFromCloud()
        {
            if (!isOnline) return;

            StartCoroutine(DownloadSaveData());
        }

        System.Collections.IEnumerator UploadSaveData(string json)
        {
            using (UnityWebRequest www = new UnityWebRequest(cloudURL, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("Authorization", $"Bearer {apiKey}");

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Cloud save successful");
                }
                else
                {
                    Debug.LogError($"Cloud save failed: {www.error}");
                    // Fallback to local save
                    SaveLocally(json);
                }
            }
        }

        System.Collections.IEnumerator DownloadSaveData()
        {
            string url = $"{cloudURL}?playerId={GetPlayerId()}";

            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                www.SetRequestHeader("Authorization", $"Bearer {apiKey}");

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string json = www.downloadHandler.text;
                    var cloudData = JsonUtility.FromJson<CloudSaveData>(json);

                    if (ValidateChecksum(cloudData.saveData, cloudData.checksum))
                    {
                        LoadSaveData(cloudData.saveData);
                        Debug.Log("Cloud load successful");
                    }
                    else
                    {
                        Debug.LogError("Cloud data checksum validation failed");
                    }
                }
                else
                {
                    Debug.LogError($"Cloud load failed: {www.error}");
                    // Load local save as fallback
                    LoadLocalSave();
                }
            }
        }

        void CheckConnectivity()
        {
            // Simple connectivity check
            isOnline = Application.internetReachability != NetworkReachability.NotReachable;
        }

        string GetPlayerId()
        {
            // In a real implementation, use device ID or user account ID
            return SystemInfo.deviceUniqueIdentifier;
        }

        string GetLocalSaveData()
        {
            // Collect all save data from managers
            var saveData = new Dictionary<string, string>();

            // Economy data
            var economy = MarketHustle.Economy.EconomyManager.Instance;
            if (economy != null)
                saveData["economy"] = JsonUtility.ToJson(new { money = economy.money });

            // Real estate data
            var realEstate = MarketHustle.RealEstate.RealEstateManager.Instance;
            if (realEstate != null)
                saveData["realEstate"] = JsonUtility.ToJson(realEstate.availableProperties);

            // Furniture data
            var furniture = MarketHustle.Furniture.FurnitureManager.Instance;
            if (furniture != null)
                saveData["furniture"] = JsonUtility.ToJson(furniture.placedFurniture);

            return JsonUtility.ToJson(saveData);
        }

        void LoadSaveData(string json)
        {
            var saveData = JsonUtility.FromJson<Dictionary<string, string>>(json);

            // Load economy
            if (saveData.ContainsKey("economy"))
            {
                var economyData = JsonUtility.FromJson<Dictionary<string, object>>(saveData["economy"]);
                var economy = MarketHustle.Economy.EconomyManager.Instance;
                if (economy != null && economyData.ContainsKey("money"))
                    economy.money = System.Convert.ToInt64(economyData["money"]);
            }

            // Load real estate
            if (saveData.ContainsKey("realEstate"))
            {
                var realEstate = MarketHustle.RealEstate.RealEstateManager.Instance;
                if (realEstate != null)
                    realEstate.availableProperties = JsonUtility.FromJson<List<MarketHustle.RealEstate.PropertyData>>(saveData["realEstate"]);
            }

            // Load furniture
            if (saveData.ContainsKey("furniture"))
            {
                var furniture = MarketHustle.Furniture.FurnitureManager.Instance;
                if (furniture != null)
                    furniture.placedFurniture = JsonUtility.FromJson<List<MarketHustle.Furniture.FurnitureData>>(saveData["furniture"]);
            }
        }

        void SaveLocally(string data)
        {
            string path = Path.Combine(Application.persistentDataPath, "cloud_backup.json");
            File.WriteAllText(path, data);
        }

        void LoadLocalSave()
        {
            string path = Path.Combine(Application.persistentDataPath, "cloud_backup.json");
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                var cloudData = JsonUtility.FromJson<CloudSaveData>(json);
                LoadSaveData(cloudData.saveData);
            }
        }

        string GenerateChecksum(string data)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
                return System.BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        bool ValidateChecksum(string data, string checksum)
        {
            return GenerateChecksum(data) == checksum;
        }

        // Conflict resolution
        public void ResolveSaveConflict(CloudSaveData local, CloudSaveData remote)
        {
            // Simple strategy: use the newer save
            if (local.timestamp > remote.timestamp)
            {
                SaveToCloud(); // Upload local
            }
            else
            {
                LoadSaveData(remote.saveData); // Use remote
            }
        }
    }
}