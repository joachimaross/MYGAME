using UnityEngine;
using MarketHustle.RealEstate;

namespace MarketHustle.Scenes
{
    /// <summary>
    /// Generates property interiors based on property type and style.
    /// Attach to property scene GameObjects to create dynamic layouts.
    /// </summary>
    public class PropertyInteriorGenerator : MonoBehaviour
    {
        public PropertyType propertyType;
        public PropertyStyle propertyStyle;
        public Material wallMaterial;
        public Material floorMaterial;

        void Start()
        {
            GenerateInterior();
        }

        void GenerateInterior()
        {
            switch (propertyType)
            {
                case PropertyType.Apartment:
                    GenerateApartment();
                    break;
                case PropertyType.Condo:
                    GenerateCondo();
                    break;
                case PropertyType.Villa:
                    GenerateVilla();
                    break;
                case PropertyType.Mansion:
                    GenerateMansion();
                    break;
            }

            // Apply style-specific details
            ApplyStyleDetails();
        }

        void GenerateApartment()
        {
            // Small apartment: living room + bedroom + bathroom
            CreateRoom(new Vector3(0f, 0f, 0f), new Vector3(8f, 3f, 8f), "LivingRoom");

            // Add basic furniture placeholders
            CreateFurniturePlaceholder("Couch", new Vector3(-2f, 0.5f, -2f));
            CreateFurniturePlaceholder("TV", new Vector3(0f, 1f, -3.5f));
            CreateFurniturePlaceholder("Bed", new Vector3(2f, 0.5f, 2f));
        }

        void GenerateCondo()
        {
            // Medium condo: larger living space + bedroom + kitchen
            CreateRoom(new Vector3(0f, 0f, 0f), new Vector3(12f, 3f, 12f), "LivingRoom");

            // Add more furniture
            CreateFurniturePlaceholder("Couch", new Vector3(-3f, 0.5f, -3f));
            CreateFurniturePlaceholder("DiningTable", new Vector3(3f, 0.5f, 0f));
            CreateFurniturePlaceholder("Bed", new Vector3(3f, 0.5f, 3f));
            CreateFurniturePlaceholder("KitchenCounter", new Vector3(-4f, 0.5f, 2f));
        }

        void GenerateVilla()
        {
            // Large villa: multiple rooms
            CreateRoom(new Vector3(0f, 0f, 0f), new Vector3(16f, 3f, 16f), "MainLiving");

            // Add luxury furniture
            CreateFurniturePlaceholder("LargeCouch", new Vector3(-4f, 0.5f, -4f));
            CreateFurniturePlaceholder("DiningTable", new Vector3(4f, 0.5f, 0f));
            CreateFurniturePlaceholder("MasterBed", new Vector3(4f, 0.5f, 4f));
            CreateFurniturePlaceholder("Piano", new Vector3(-6f, 0.5f, 2f));
        }

        void GenerateMansion()
        {
            // Huge mansion: grand spaces
            CreateRoom(new Vector3(0f, 0f, 0f), new Vector3(20f, 4f, 20f), "GrandHall");

            // Add premium furniture
            CreateFurniturePlaceholder("GrandPiano", new Vector3(-6f, 0.5f, -6f));
            CreateFurniturePlaceholder("Chandelier", new Vector3(0f, 3.5f, 0f));
            CreateFurniturePlaceholder("MasterBed", new Vector3(6f, 0.5f, 6f));
            CreateFurniturePlaceholder("DiningTable", new Vector3(6f, 0.5f, 0f));
        }

        void CreateRoom(Vector3 center, Vector3 size, string roomName)
        {
            // Create floor
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = $"{roomName}_Floor";
            floor.transform.position = new Vector3(center.x, -0.5f, center.z);
            floor.transform.localScale = new Vector3(size.x, 1f, size.z);
            if (floorMaterial != null) floor.GetComponent<Renderer>().material = floorMaterial;

            // Create walls
            CreateWalls(center, size, roomName);
        }

        void CreateWalls(Vector3 center, Vector3 size, string roomName)
        {
            Vector3[] wallPositions = {
                new Vector3(center.x, size.y / 2f, center.z + size.z / 2f), // North
                new Vector3(center.x, size.y / 2f, center.z - size.z / 2f), // South
                new Vector3(center.x + size.x / 2f, size.y / 2f, center.z), // East
                new Vector3(center.x - size.x / 2f, size.y / 2f, center.z)  // West
            };

            Vector3[] wallScales = {
                new Vector3(size.x, size.y, 0.2f),
                new Vector3(size.x, size.y, 0.2f),
                new Vector3(0.2f, size.y, size.z),
                new Vector3(0.2f, size.y, size.z)
            };

            string[] wallNames = { "North", "South", "East", "West" };

            for (int i = 0; i < wallPositions.Length; i++)
            {
                GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.name = $"{roomName}_Wall_{wallNames[i]}";
                wall.transform.position = wallPositions[i];
                wall.transform.localScale = wallScales[i];
                if (wallMaterial != null) wall.GetComponent<Renderer>().material = wallMaterial;
            }
        }

        void CreateFurniturePlaceholder(string furnitureType, Vector3 position)
        {
            GameObject placeholder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            placeholder.name = $"{furnitureType}_Placeholder";
            placeholder.transform.position = position;

            // Size based on furniture type
            Vector3 scale = GetFurnitureScale(furnitureType);
            placeholder.transform.localScale = scale;

            // Color code by type
            Color color = GetFurnitureColor(furnitureType);
            placeholder.GetComponent<Renderer>().material.color = color;

            // Add collider for interaction
            var collider = placeholder.AddComponent<BoxCollider>();
            collider.isTrigger = true;
        }

        Vector3 GetFurnitureScale(string furnitureType)
        {
            switch (furnitureType)
            {
                case "Couch": return new Vector3(2f, 1f, 1f);
                case "LargeCouch": return new Vector3(3f, 1f, 1.5f);
                case "Bed": return new Vector3(2f, 0.5f, 1.5f);
                case "MasterBed": return new Vector3(2.5f, 0.5f, 2f);
                case "TV": return new Vector3(0.5f, 1f, 0.2f);
                case "DiningTable": return new Vector3(2f, 1f, 1f);
                case "KitchenCounter": return new Vector3(1f, 1f, 0.5f);
                case "Piano": return new Vector3(1.5f, 1.5f, 0.8f);
                case "GrandPiano": return new Vector3(2f, 1.5f, 1f);
                case "Chandelier": return new Vector3(1f, 0.5f, 1f);
                default: return new Vector3(1f, 1f, 1f);
            }
        }

        Color GetFurnitureColor(string furnitureType)
        {
            switch (furnitureType)
            {
                case "Couch": return Color.blue;
                case "Bed": return Color.red;
                case "TV": return Color.black;
                case "DiningTable": return Color.yellow;
                case "KitchenCounter": return Color.gray;
                case "Piano": return Color.white;
                default: return Color.green;
            }
        }

        void ApplyStyleDetails()
        {
            // Apply style-specific modifications
            switch (propertyStyle)
            {
                case PropertyStyle.Modern:
                    // Clean, minimalist - already basic cubes
                    break;
                case PropertyStyle.Contemporary:
                    // Add some angular elements
                    AddContemporaryDetails();
                    break;
                case PropertyStyle.Hood:
                    // Add urban/street elements
                    AddHoodDetails();
                    break;
            }
        }

        void AddContemporaryDetails()
        {
            // Add some geometric shapes
            GameObject geometric = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            geometric.name = "Contemporary_Detail";
            geometric.transform.position = new Vector3(2f, 2f, 2f);
            geometric.transform.localScale = new Vector3(0.5f, 2f, 0.5f);
            geometric.GetComponent<Renderer>().material.color = Color.magenta;
        }

        void AddHoodDetails()
        {
            // Add street art style elements
            GameObject graffiti = GameObject.CreatePrimitive(PrimitiveType.Quad);
            graffiti.name = "Hood_Graffiti";
            graffiti.transform.position = new Vector3(-3f, 2f, 0f);
            graffiti.transform.localScale = new Vector3(2f, 2f, 1f);
            graffiti.GetComponent<Renderer>().material.color = Color.cyan;
        }
    }
}