using UnityEngine;

namespace MarketHustle.RealEstate
{
    public enum PropertyStyle { Modern, Contemporary, Hood }
    public enum Neighborhood { Downtown, Coast, Country, Hood, City }

    [CreateAssetMenu(fileName = "Property", menuName = "MarketHustle/Property", order = 100)]
    public class RealEstatePropertySO : ScriptableObject
    {
        public string id;
        public PropertyType propertyType;
        public string displayName;
        public string sceneName;
        public long price;
        public long monthlyRent;
        public bool forRent = false;

        // Additional metadata for starter selection
        public PropertyStyle style = PropertyStyle.Modern;
        public Neighborhood neighborhood = Neighborhood.City;
    }
}
