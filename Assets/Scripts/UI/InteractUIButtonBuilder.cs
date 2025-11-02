using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Runtime builder for a mobile-interact button. Wires the button to PickupManager.InteractNearest.
/// </summary>
public class InteractUIButtonBuilder : MonoBehaviour
{
    public Vector2 anchoredPosition = new Vector2(-100f, 100f);
    public Vector2 size = new Vector2(120f, 80f);

    void Start()
    {
        var canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject cgo = new GameObject("Canvas_Interact");
            canvas = cgo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            cgo.AddComponent<CanvasScaler>();
            cgo.AddComponent<GraphicRaycaster>();
        }

        GameObject btnGO = new GameObject("InteractButton");
        btnGO.transform.SetParent(canvas.transform, false);
        var rt = btnGO.AddComponent<RectTransform>();
        rt.sizeDelta = size;
        rt.anchorMin = new Vector2(1f, 0f);
        rt.anchorMax = new Vector2(1f, 0f);
        rt.anchoredPosition = anchoredPosition;

        var img = btnGO.AddComponent<Image>(); img.color = new Color(0.1f, 0.6f, 0.2f, 0.9f);
        var button = btnGO.AddComponent<Button>();

        GameObject txtGO = new GameObject("Text"); txtGO.transform.SetParent(btnGO.transform, false);
        var txt = txtGO.AddComponent<UnityEngine.UI.Text>(); txt.text = "Interact"; txt.alignment = TextAnchor.MiddleCenter; txt.color = Color.white;

        button.onClick.AddListener(() => { MarketHustle.Interaction.PickupManager.Instance?.InteractNearest(); });
    }
}
