using UnityEditor;
using UnityEngine;
using MarketHustle.RealEstate;

public class CreateSampleProperties
{
    [MenuItem("MarketHustle/Create Sample Properties")]
    public static void Create()
    {
        string path = "Assets/RealEstateSamples";
        if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder("Assets", "RealEstateSamples");

        // Create 3 apartments with different styles
        CreateProperty(path + "/APT_Modern.asset", "apt_modern", PropertyType.Apartment, "Modern Apartment", "ApartmentScene", 5000, 500, true, RealEstatePropertySO.PropertyStyle.Modern, RealEstatePropertySO.Neighborhood.Downtown);
        CreateProperty(path + "/APT_Contemporary.asset", "apt_contemporary", PropertyType.Apartment, "Contemporary Apartment", "ApartmentScene", 7000, 650, true, RealEstatePropertySO.PropertyStyle.Contemporary, RealEstatePropertySO.Neighborhood.City);
        CreateProperty(path + "/APT_Hood.asset", "apt_hood", PropertyType.Apartment, "Starter Hood Apartment", "ApartmentScene", 2500, 250, true, RealEstatePropertySO.PropertyStyle.Hood, RealEstatePropertySO.Neighborhood.Hood);

        // Create 2 store locations
        CreateProperty(path + "/Store_Downtown.asset", "store_dt", PropertyType.Condo, "Downtown Storefront", "SupermarketInterior", 8000, 700, false, RealEstatePropertySO.PropertyStyle.Modern, RealEstatePropertySO.Neighborhood.Downtown);
        CreateProperty(path + "/Store_Southside.asset", "store_ss", PropertyType.Condo, "Southside Market", "SupermarketInterior", 4500, 400, false, RealEstatePropertySO.PropertyStyle.Hood, RealEstatePropertySO.Neighborhood.Hood);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Sample RealEstatePropertySO assets created in Assets/RealEstateSamples/");
    }

    static void CreateProperty(string assetPath, string id, PropertyType type, string displayName, string sceneName, long price, long rent, bool forRent, RealEstatePropertySO.PropertyStyle style, RealEstatePropertySO.Neighborhood neighborhood)
    {
        var so = ScriptableObject.CreateInstance<RealEstatePropertySO>();
        so.id = id;
        so.propertyType = type;
        so.displayName = displayName;
        so.sceneName = sceneName;
        so.price = price;
        so.monthlyRent = rent;
        so.forRent = forRent;
        so.style = style;
        so.neighborhood = neighborhood;
        AssetDatabase.CreateAsset(so, assetPath);
    }
}
