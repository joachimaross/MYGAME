using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Helper script to programmatically create and set up the main game scenes.
/// Run this from Unity Editor to create Jacamenoville, SupermarketInterior, and basic property scenes.
/// </summary>
public class SceneSetupHelper : MonoBehaviour
{
    [MenuItem("MarketHustle/Setup Scenes/Create All Scenes")]
    static void CreateAllScenes()
    {
        CreateJacamenovilleScene();
        CreateSupermarketInteriorScene();
        CreatePropertyScenes();
        Debug.Log("All scenes created! Remember to save them manually.");
    }

    [MenuItem("MarketHustle/Setup Scenes/Create Jacamenoville Scene")]
    static void CreateJacamenovilleScene()
    {
        // Create new scene
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "Jacamenoville";

        // Create main GameManager
        GameObject gameManager = new GameObject("GameManager");
        gameManager.AddComponent<MarketHustle.Economy.EconomyManager>();
        gameManager.AddComponent<MarketHustle.RealEstate.RealEstateManager>();
        gameManager.AddComponent<MarketHustle.Save.SaveSystem>();
        gameManager.AddComponent<MarketHustle.Furniture.FurnitureManager>();

        // Create layout generator
        GameObject layoutGen = new GameObject("JacamenovilleLayoutGenerator");
        var gen = layoutGen.AddComponent<JacamenovilleLayoutGenerator>();

        // Create player
        GameObject player = CreatePlayer();
        player.name = "Player";

        // Create UI systems
        CreateUI();

        // Create Real Estate Shop trigger (optional)
        GameObject shopTrigger = new GameObject("RealEstateShopTrigger");
        shopTrigger.transform.position = new Vector3(20f, 1f, 20f);
        var col = shopTrigger.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(4f, 2f, 4f);
        // Add trigger script if needed

        // Create sample properties
        CreateSampleProperties();

        Debug.Log("Jacamenoville scene created. Save it manually!");
    }

    [MenuItem("MarketHustle/Setup Scenes/Create Supermarket Interior Scene")]
    static void CreateSupermarketInteriorScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "SupermarketInterior";

        // Create floor
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.localScale = new Vector3(10f, 1f, 10f);

        // Create supermarket interior generator
        GameObject interiorGen = new GameObject("SupermarketInterior");
        interiorGen.AddComponent<SupermarketInterior>();

        // Create player spawn point
        GameObject playerSpawn = new GameObject("PlayerSpawn");
        playerSpawn.transform.position = new Vector3(0f, 1f, 8f);

        // Create UI
        CreateUI();

        Debug.Log("SupermarketInterior scene created. Save it manually!");
    }

    [MenuItem("MarketHustle/Setup Scenes/Create Property Scenes")]
    static void CreatePropertyScenes()
    {
        string[] propertyTypes = { "ApartmentScene", "CondoScene", "VillaScene", "MansionScene" };

        foreach (string propType in propertyTypes)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = propType;

            // Create basic interior
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.localScale = new Vector3(5f, 1f, 5f);

            // Create walls
            CreateWalls();

            // Create player spawn
            GameObject playerSpawn = new GameObject("PlayerSpawn");
            playerSpawn.transform.position = new Vector3(0f, 1f, 0f);

            // Create exit trigger
            GameObject exit = new GameObject("ExitToCity");
            exit.transform.position = new Vector3(0f, 1f, -4f);
            var col = exit.AddComponent<BoxCollider>();
            col.isTrigger = true;
            exit.AddComponent<ExitToCity>();
            var exitScript = exit.GetComponent<ExitToCity>();
            exitScript.sceneToReturn = "Jacamenoville";

            // Create UI
            CreateUI();

            // Create furniture manager for this property
            GameObject furnitureMgr = new GameObject("FurnitureManager");
            furnitureMgr.AddComponent<MarketHustle.Furniture.FurnitureManager>();

            Debug.Log($"{propType} scene created. Save it manually!");
        }
    }

    static GameObject CreatePlayer()
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.tag = "Player";
        player.transform.position = new Vector3(0f, 1f, -10f);

        // Remove collider and add CharacterController
        DestroyImmediate(player.GetComponent<Collider>());
        var cc = player.AddComponent<CharacterController>();
        cc.height = 2f;
        cc.center = new Vector3(0f, 1f, 0f);

        // Add PlayerController
        player.AddComponent<MarketHustle.Player.PlayerController>();

        // Add camera
        GameObject camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        var cam = camObj.AddComponent<Camera>();
        camObj.transform.SetParent(player.transform);
        camObj.transform.localPosition = new Vector3(0f, 1.6f, -3f);
        camObj.transform.localEulerAngles = new Vector3(15f, 0f, 0f);

        // Add input systems
        GameObject inputMgr = new GameObject("InputManager");
        inputMgr.AddComponent<InputHandler>();

        // Add mobile joystick
        GameObject joystick = new GameObject("Joystick");
        joystick.AddComponent<RuntimeJoystick>();

        return player;
    }

    static void CreateUI()
    {
        // Create Canvas
        GameObject canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // Create money display
        GameObject moneyTextGO = new GameObject("MoneyText");
        moneyTextGO.transform.SetParent(canvasGO.transform);
        var moneyText = moneyTextGO.AddComponent<TMPro.TextMeshProUGUI>();
        moneyText.text = "$1000";
        moneyText.fontSize = 24;
        moneyText.alignment = TMPro.TextAlignmentOptions.TopLeft;
        var moneyRT = moneyTextGO.GetComponent<RectTransform>();
        moneyRT.anchorMin = new Vector2(0f, 1f);
        moneyRT.anchorMax = new Vector2(0f, 1f);
        moneyRT.pivot = new Vector2(0f, 1f);
        moneyRT.anchoredPosition = new Vector2(10f, -10f);

        // Link to MoneySystem
        var moneySys = FindObjectOfType<MarketHustle.Economy.EconomyManager>();
        if (moneySys != null)
        {
            // Assuming MoneySystem has moneyText field - adjust as needed
            var moneySysComponent = moneySys.GetComponent<MoneySystem>();
            if (moneySysComponent != null)
            {
                moneySysComponent.moneyText = moneyText;
            }
        }

        // Add mobile interact button
        GameObject interactBtn = new GameObject("InteractUIButtonBuilder");
        interactBtn.AddComponent<InteractUIButtonBuilder>();
    }

    static void CreateWalls()
    {
        // Simple room walls
        string[] wallNames = { "Wall_North", "Wall_South", "Wall_East", "Wall_West" };
        Vector3[] positions = {
            new Vector3(0f, 2.5f, 5f),
            new Vector3(0f, 2.5f, -5f),
            new Vector3(5f, 2.5f, 0f),
            new Vector3(-5f, 2.5f, 0f)
        };
        Vector3[] scales = {
            new Vector3(10f, 5f, 0.2f),
            new Vector3(10f, 5f, 0.2f),
            new Vector3(0.2f, 5f, 10f),
            new Vector3(0.2f, 5f, 10f)
        };

        for (int i = 0; i < wallNames.Length; i++)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = wallNames[i];
            wall.transform.position = positions[i];
            wall.transform.localScale = scales[i];
        }
    }

    static void CreateSampleProperties()
    {
        // This would normally be done via the editor tool, but we can create basic ones
        // For now, just ensure RealEstateManager has some properties
        var manager = FindObjectOfType<MarketHustle.RealEstate.RealEstateManager>();
        if (manager != null && manager.availableProperties.Count == 0)
        {
            // Add fallback properties if none exist
            manager.availableProperties.Add(new MarketHustle.RealEstate.PropertyData {
                id = "apt_modern", propertyType = MarketHustle.RealEstate.PropertyType.Apartment,
                displayName = "Modern Apartment", sceneName = "ApartmentScene", price = 50000, monthlyRent = 800
            });
            manager.availableProperties.Add(new MarketHustle.RealEstate.PropertyData {
                id = "store_downtown", propertyType = MarketHustle.RealEstate.PropertyType.Condo,
                displayName = "Downtown Store", sceneName = "CondoScene", price = 100000, monthlyRent = 2000
            });
        }
    }
}