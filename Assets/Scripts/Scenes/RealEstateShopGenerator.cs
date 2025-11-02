using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MarketHustle.UI;

/// <summary>
/// Runtime generator for a Real Estate Shop scene/panel. Creates a panel and attaches PropertyBrowserUI.
/// </summary>
public class RealEstateShopGenerator : MonoBehaviour
{
    void Start()
    {
        CreateShopPanel();
    }

    void CreateShopPanel()
    {
        var canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject cgo = new GameObject("Canvas");
            canvas = cgo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            cgo.AddComponent<CanvasScaler>();
            cgo.AddComponent<GraphicRaycaster>();
        }

        GameObject panel = new GameObject("RealEstatePanel");
        panel.transform.SetParent(canvas.transform, false);
        var rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.55f, 0.05f);
        rt.anchorMax = new Vector2(0.98f, 0.95f);
        panel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.6f);

        var browser = panel.AddComponent<PropertyBrowserUI>();
        // Let PropertyBrowserUI build its own listParent
        browser.listParent = null;
    }
}
