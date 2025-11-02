using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simple scene bootstrapper that programmatically creates a MainCity placeholder environment.
/// Attach this script to an empty GameObject in a new scene (or run it from an empty scene) to auto-generate ground,
/// simple buildings, a player capsule with CharacterController + PlayerController, and a basic UI canvas with Money display.
/// This allows quick testing without pre-made Unity scenes or prefabs.
/// </summary>
public class SceneBootstrap : MonoBehaviour
{
    public Material buildingMaterial;

    void Start()
    {
        CreateEnvironment();
        CreatePlayer();
        CreateUI();
    }

    void CreateEnvironment()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(100f, 1f, 100f);
        ground.transform.position = Vector3.zero;

        // Create a few placeholder buildings
        for (int i = 0; i < 5; i++)
        {
            GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cube);
            b.name = "Building_" + i;
            b.transform.localScale = new Vector3(20f, 20f + i * 5f, 20f);
            b.transform.position = new Vector3(i * 30f - 60f, b.transform.localScale.y / 2f, 0f + (i % 2) * 40f);
            if (buildingMaterial != null)
            {
                var rend = b.GetComponent<Renderer>();
                rend.material = buildingMaterial;
            }
        }

        // Directional light
        GameObject lightObj = new GameObject("Sun");
        var light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1f;
        lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
    }

    void CreatePlayer()
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = new Vector3(0f, 1f, -10f);

        // remove collider and add CharacterController instead
        Destroy(player.GetComponent<Collider>());
        var cc = player.AddComponent<CharacterController>();
        cc.height = 2f;
        cc.center = new Vector3(0f, 1f, 0f);

        // Add PlayerController
        var pc = player.AddComponent<MarketHustle.Player.PlayerController>();

        // Add a camera and set as camera holder
        GameObject camObj = new GameObject("Main Camera");
        var cam = camObj.AddComponent<Camera>();
        camObj.tag = "MainCamera";
        camObj.transform.SetParent(player.transform);
        camObj.transform.localPosition = new Vector3(0f, 2f, -4f);
        camObj.transform.localEulerAngles = new Vector3(10f, 0f, 0f);

        pc.cameraHolder = camObj.transform;

        // Add simple InputHandler
        var inputMgr = new GameObject("InputManager");
        var inputHandler = inputMgr.AddComponent<InputHandler>();
    }

    void CreateUI()
    {
        // Create Canvas
        GameObject canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Create money text
        GameObject moneyTextGO = new GameObject("MoneyText");
        moneyTextGO.transform.SetParent(canvasGO.transform);

        var text = moneyTextGO.AddComponent<TextMeshProUGUI>();
        text.fontSize = 36;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.rectTransform.anchorMin = new Vector2(0f, 1f);
        text.rectTransform.anchorMax = new Vector2(0f, 1f);
        text.rectTransform.pivot = new Vector2(0f, 1f);
        text.rectTransform.anchoredPosition = new Vector2(10f, -10f);

        // link to MoneySystem (create if missing)
        var moneySysObj = GameObject.FindObjectOfType<MoneySystem>();
        if (moneySysObj == null)
        {
            var go = new GameObject("MoneySystem");
            moneySysObj = go.AddComponent<MoneySystem>();
        }
        moneySysObj.moneyText = text;
    }
}
