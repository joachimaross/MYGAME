using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Small helper to create a basic Canvas and a Money TMP text at runtime (if none present).
/// Useful for quick playtesting in a bootstrap scene.
/// </summary>
public class UIBuilder : MonoBehaviour
{
    void Start()
    {
        // if a MoneySystem already has a reference, don't overwrite
        var ms = GameObject.FindObjectOfType<MoneySystem>();
        if (ms != null && ms.moneyText != null) return;

        GameObject canvasGO = new GameObject("Canvas_Runtime");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject moneyTextGO = new GameObject("MoneyText_Runtime");
        moneyTextGO.transform.SetParent(canvasGO.transform);
        var text = moneyTextGO.AddComponent<TextMeshProUGUI>();
        text.fontSize = 36;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.rectTransform.anchorMin = new Vector2(0f, 1f);
        text.rectTransform.anchorMax = new Vector2(0f, 1f);
        text.rectTransform.pivot = new Vector2(0f, 1f);
        text.rectTransform.anchoredPosition = new Vector2(10f, -10f);

        if (ms == null)
        {
            var go = new GameObject("MoneySystem");
            ms = go.AddComponent<MoneySystem>();
        }

        ms.moneyText = text;
    }
}
