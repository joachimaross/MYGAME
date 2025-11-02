using UnityEngine;
using UnityEditor;
using TMPro;

public class CreatePropertyListItemPrefab
{
    [MenuItem("MarketHustle/Create PropertyListItem Prefab")]
    public static void Create()
    {
        string folder = "Assets/Prefabs";
        if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder("Assets", "Prefabs");

        // Create a temporary GameObject hierarchy for the prefab
        GameObject root = new GameObject("PropertyListItem");
        var rt = root.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(400, 36);

        GameObject labelGO = new GameObject("Label");
        labelGO.transform.SetParent(root.transform, false);
        var label = labelGO.AddComponent<TextMeshProUGUI>();
        label.text = "Property Name - $Price";
        label.fontSize = 20;
        var labelRT = label.GetComponent<RectTransform>();
        labelRT.anchorMin = new Vector2(0f, 0f);
        labelRT.anchorMax = new Vector2(0.75f, 1f);
        labelRT.offsetMin = new Vector2(8f, 4f);

        GameObject btnGO = new GameObject("BuyButton");
        btnGO.transform.SetParent(root.transform, false);
        var btnRT = btnGO.AddComponent<RectTransform>();
        btnRT.anchorMin = new Vector2(0.75f, 0f);
        btnRT.anchorMax = new Vector2(1f, 1f);
        btnRT.offsetMin = new Vector2(-8f, 4f);
        var img = btnGO.AddComponent<UnityEngine.UI.Image>();
        img.color = new Color(0.15f, 0.6f, 0.95f);
        var btn = btnGO.AddComponent<UnityEngine.UI.Button>();

        GameObject btnTxtGO = new GameObject("Text"); btnTxtGO.transform.SetParent(btnGO.transform, false);
        var btnTxt = btnTxtGO.AddComponent<TextMeshProUGUI>(); btnTxt.text = "Buy"; btnTxt.alignment = TMPro.TextAlignmentOptions.Center;

        // Save as prefab
        string prefabPath = folder + "/PropertyListItem.prefab";
        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        GameObject.DestroyImmediate(root);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("PropertyListItem prefab created at " + prefabPath);
    }
}
