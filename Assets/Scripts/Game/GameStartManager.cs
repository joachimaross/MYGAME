using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MarketHustle.RealEstate;

namespace MarketHustle.Game
{
    /// <summary>
    /// Presents a simple startup UI where the player picks an initial apartment (3 choices: Modern, Contemporary, Hood)
    /// and a store location (2 choices). After selections, it marks the chosen property as owned in RealEstateManager.
    /// </summary>
    public class GameStartManager : MonoBehaviour
    {
        public Canvas canvasPrefab; // optional

        void Start()
        {
            ShowStartChoices();
        }

        void ShowStartChoices()
        {
            var canvas = FindObjectOfType<Canvas>();
            if (canvas == null && canvasPrefab != null)
            {
                canvas = Instantiate(canvasPrefab);
            }
            if (canvas == null)
            {
                Debug.LogWarning("No Canvas found for GameStartManager");
                return;
            }

            // Panel
            var panelGO = new GameObject("StartPanel");
            panelGO.transform.SetParent(canvas.transform, false);
            var panelRT = panelGO.AddComponent<RectTransform>();
            panelRT.anchorMin = new Vector2(0.1f, 0.1f);
            panelRT.anchorMax = new Vector2(0.9f, 0.9f);
            panelGO.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.6f);

            // Title
            var titleGO = new GameObject("Title"); titleGO.transform.SetParent(panelGO.transform, false);
            var title = titleGO.AddComponent<TextMeshProUGUI>(); title.text = "Choose your starter apartment"; title.fontSize = 30;

            // Apartment choices: find three Apartment properties with different styles
            var manager = RealEstateManager.Instance;
            if (manager == null) return;

            var apartments = manager.availableProperties.Where(p => p.propertyType == PropertyType.Apartment).ToList();
            // if fewer than 3, just use what's available
            int count = Mathf.Min(3, apartments.Count);
            for (int i = 0; i < count; i++)
            {
                var p = apartments[i];
                CreateChoiceButton(panelGO.transform, p.displayName + " (" + p.style + ") - $" + p.price, () => OnApartmentChosen(p.id));
            }

            // Store location choices: pick first two properties that are not apartments (or just first two overall)
            CreateChoiceButton(panelGO.transform, "Choose Store Location:", null);
            var stores = manager.availableProperties.Where(p => p.propertyType != PropertyType.Apartment).ToList();
            for (int i = 0; i < Mathf.Min(2, stores.Count); i++)
            {
                var s = stores[i];
                CreateChoiceButton(panelGO.transform, s.displayName + " - " + s.neighborhood, () => OnStoreChosen(s.id));
            }
        }

        void CreateChoiceButton(Transform parent, string label, System.Action onClick)
        {
            var go = new GameObject("Btn"); go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>(); rt.sizeDelta = new Vector2(400, 40);
            var btn = go.AddComponent<Button>(); var img = go.AddComponent<Image>(); img.color = new Color(0.2f, 0.6f, 0.9f);
            var txtGO = new GameObject("Txt"); txtGO.transform.SetParent(go.transform, false);
            var txt = txtGO.AddComponent<TextMeshProUGUI>(); txt.text = label; txt.alignment = TextAlignmentOptions.Center;
            if (onClick != null) btn.onClick.AddListener(() => onClick());
        }

        void OnApartmentChosen(string id)
        {
            var mgr = RealEstateManager.Instance;
            var p = mgr.availableProperties.Find(x => x.id == id);
            if (p != null) { p.owned = true; Debug.Log("Apartment chosen: " + p.displayName); }
        }

        void OnStoreChosen(string id)
        {
            var mgr = RealEstateManager.Instance;
            var p = mgr.availableProperties.Find(x => x.id == id);
            if (p != null) { p.owned = true; Debug.Log("Store chosen: " + p.displayName); }
        }
    }
}
