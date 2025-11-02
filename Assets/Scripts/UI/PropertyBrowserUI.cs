using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MarketHustle.RealEstate;

namespace MarketHustle.UI
{
    /// <summary>
    /// Runtime property browser UI. Lists available properties from RealEstateManager.propertyAssets (or availableProperties)
    /// and allows purchase. Creates a basic UI if none is assigned.
    /// </summary>
    public class PropertyBrowserUI : MonoBehaviour
    {
        public Transform listParent;
        public GameObject itemPrefab; // optional

        void Start()
        {
            Refresh();
        }

        public void Refresh()
        {
            var manager = RealEstateManager.Instance;
            if (manager == null) return;

            var props = manager.availableProperties;
            if (props == null) return;

            if (listParent == null)
            {
                // create a simple scrollable list
                var canvas = FindObjectOfType<Canvas>();
                if (canvas == null)
                {
                    Debug.LogWarning("No Canvas found for PropertyBrowserUI");
                    return;
                }
                var panel = new GameObject("PropertyBrowserPanel");
                panel.transform.SetParent(canvas.transform, false);
                var rt = panel.AddComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.6f, 0.1f);
                rt.anchorMax = new Vector2(0.98f, 0.9f);
                rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
                panel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f);

                var content = new GameObject("Content");
                content.transform.SetParent(panel.transform, false);
                var contentRT = content.AddComponent<RectTransform>();
                contentRT.anchorMin = new Vector2(0f, 0f);
                contentRT.anchorMax = new Vector2(1f, 1f);
                contentRT.offsetMin = new Vector2(8f, 8f);
                contentRT.offsetMax = new Vector2(-8f, -8f);

                listParent = content.transform;
            }

            // clear
            foreach (Transform t in listParent) Destroy(t.gameObject);

            foreach (var p in props.OrderBy(x => x.price))
            {
                if (itemPrefab != null)
                {
                    var go = Instantiate(itemPrefab, listParent);
                    var txt = go.GetComponentInChildren<TextMeshProUGUI>();
                    if (txt != null) txt.text = p.displayName + " - $" + p.price;
                    var btn = go.GetComponentInChildren<Button>();
                    if (btn != null) { string id = p.id; btn.onClick.AddListener(() => OnBuy(id)); }
                }
                else
                {
                    var row = new GameObject("PropRow");
                    row.transform.SetParent(listParent, false);
                    var rt = row.AddComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(400, 36);

                    var labelGO = new GameObject("Label");
                    labelGO.transform.SetParent(row.transform, false);
                    var label = labelGO.AddComponent<TextMeshProUGUI>();
                    label.text = p.displayName + " - $" + p.price + " (" + p.style + ")";
                    label.fontSize = 20;

                    var btnGO = new GameObject("BuyBtn");
                    btnGO.transform.SetParent(row.transform, false);
                    var btn = btnGO.AddComponent<Button>();
                    var img = btnGO.AddComponent<Image>();
                    img.color = new Color(0.15f, 0.6f, 0.95f);
                    var btnTxtGO = new GameObject("Text"); btnTxtGO.transform.SetParent(btnGO.transform, false);
                    var btnTxt = btnTxtGO.AddComponent<TextMeshProUGUI>(); btnTxt.text = "Buy"; btnTxt.alignment = TextAlignmentOptions.Center;

                    string id = p.id;
                    btn.onClick.AddListener(() => OnBuy(id));
                }
            }
        }

        void OnBuy(string id)
        {
            bool ok = RealEstateManager.Instance.BuyProperty(id);
            if (ok) Refresh();
            else Debug.Log("Failed to buy: " + id);
        }
    }
}
