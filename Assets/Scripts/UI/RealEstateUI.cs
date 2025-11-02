using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MarketHustle.RealEstate;

namespace MarketHustle.UI
{
    /// <summary>
    /// Minimal UI bridge for the RealEstateManager. Hook up buttons and text elements from the scene.
    /// </summary>
    public class RealEstateUI : MonoBehaviour
    {
        public Transform propertyListParent;
    public GameObject propertyListItemPrefab; // small UI prefab with Text + Buy button (optional)

        void Start()
        {
            RefreshList();
        }

        public void RefreshList()
        {
            if (propertyListParent == null) return;

            // clear existing children
            foreach (Transform t in propertyListParent) Destroy(t.gameObject);

            foreach (var p in RealEstateManager.Instance.availableProperties)
            {
                if (propertyListItemPrefab != null)
                {
                    var go = Instantiate(propertyListItemPrefab, propertyListParent);
                    var text = go.GetComponentInChildren<UnityEngine.UI.Text>();
                    if (text) text.text = p.displayName + " - $" + p.price;

                    var btn = go.GetComponentInChildren<Button>();
                    if (btn)
                    {
                        string id = p.id;
                        btn.onClick.AddListener(() => OnBuyClicked(id));
                    }
                }
                else
                {
                    // create a simple UI row programmatically (Text + Button)
                    GameObject row = new GameObject("PropertyRow");
                    row.transform.SetParent(propertyListParent, false);
                    var rt = row.AddComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(400, 40);

                    var textGO = new GameObject("Label");
                    textGO.transform.SetParent(row.transform, false);
                    var txt = textGO.AddComponent<TextMeshProUGUI>();
                    txt.text = p.displayName + " - $" + p.price;
                    txt.fontSize = 24;
                    var txtRt = textGO.GetComponent<RectTransform>();
                    txtRt.anchorMin = new Vector2(0f, 0f);
                    txtRt.anchorMax = new Vector2(0.75f, 1f);
                    txtRt.offsetMin = new Vector2(8f, 4f);

                    var btnGO = new GameObject("BuyButton");
                    btnGO.transform.SetParent(row.transform, false);
                    var btn = btnGO.AddComponent<Button>();
                    var img = btnGO.AddComponent<Image>();
                    img.color = new Color(0.2f, 0.6f, 1f, 1f);
                    var btnRt = btnGO.GetComponent<RectTransform>();
                    btnRt.anchorMin = new Vector2(0.75f, 0f);
                    btnRt.anchorMax = new Vector2(1f, 1f);
                    btnRt.offsetMin = new Vector2(-8f, 4f);

                    var btnTextGO = new GameObject("Text");
                    btnTextGO.transform.SetParent(btnGO.transform, false);
                    var btnTxt = btnTextGO.AddComponent<TextMeshProUGUI>();
                    btnTxt.text = "Buy";
                    btnTxt.alignment = TextAlignmentOptions.Center;
                    btnTxt.fontSize = 20;

                    string id = p.id;
                    btn.onClick.AddListener(() => OnBuyClicked(id));
                }
            }
        }

        public void OnBuyClicked(string propertyId)
        {
            bool ok = RealEstateManager.Instance.BuyProperty(propertyId);
            if (ok)
            {
                RefreshList();
            }
            else
            {
                Debug.Log("Could not buy property " + propertyId);
            }
        }
    }
}
