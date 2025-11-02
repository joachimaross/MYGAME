using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Programmatically creates a simple joystick UI (background + handle) and attaches a RuntimeJoystick component.
/// Useful for quick testing without creating prefabs in the editor.
/// </summary>
public class JoystickUIBuilder : MonoBehaviour
{
    public Canvas targetCanvas;
    public Vector2 size = new Vector2(200, 200);

    void Start()
    {
        if (targetCanvas == null)
        {
            var c = FindObjectOfType<Canvas>();
            if (c == null)
            {
                var go = new GameObject("Canvas_Joystick");
                c = go.AddComponent<Canvas>();
                c.renderMode = RenderMode.ScreenSpaceOverlay;
                go.AddComponent<CanvasScaler>();
                go.AddComponent<GraphicRaycaster>();
            }
            targetCanvas = c;
        }

        CreateJoystick(targetCanvas.transform);
    }

    void CreateJoystick(Transform parent)
    {
        GameObject bgGO = new GameObject("JoystickBG");
        bgGO.transform.SetParent(parent);
        var bgRect = bgGO.AddComponent<RectTransform>();
        bgRect.sizeDelta = size;
        bgRect.anchorMin = new Vector2(0f, 0f);
        bgRect.anchorMax = new Vector2(0f, 0f);
        bgRect.pivot = new Vector2(0f, 0f);
        bgRect.anchoredPosition = new Vector2(100f, 100f);
        var bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0.4f);

        GameObject handleGO = new GameObject("JoystickHandle");
        handleGO.transform.SetParent(bgGO.transform);
        var hRect = handleGO.AddComponent<RectTransform>();
        hRect.sizeDelta = size * 0.4f;
        hRect.anchorMin = new Vector2(0.5f, 0.5f);
        hRect.anchorMax = new Vector2(0.5f, 0.5f);
        hRect.pivot = new Vector2(0.5f, 0.5f);
        hRect.anchoredPosition = Vector2.zero;
        var hImage = handleGO.AddComponent<Image>();
        hImage.color = new Color(1f, 1f, 1f, 0.9f);

        var joystick = bgGO.AddComponent<RuntimeJoystick>();
        joystick.background = bgRect;
        joystick.handle = hRect;

        // Attach InputHandler if present
        var inputHandler = FindObjectOfType<InputHandler>();
        if (inputHandler != null)
        {
            inputHandler.moveJoystick = joystick;
        }
    }
}
