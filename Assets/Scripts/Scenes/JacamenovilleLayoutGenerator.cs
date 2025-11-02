using UnityEngine;

/// <summary>
/// Procedurally generates a Chicago-inspired Jacamenoville layout with downtown skyscrapers,
/// southside low-rises, roads, a river, and landmarks for a downtown/southside feel.
/// </summary>
public class JacamenovilleLayoutGenerator : MonoBehaviour
{
    public Material downtownMat;
    public Material southsideMat;
    public Material supermarketMat;
    public Material roadMat;
    public Material riverMat;

    void Start()
    {
        GenerateGround();
        GenerateDowntown();
        GenerateSouthside();
        CreateSupermarket();
        CreateApartments();
        GenerateRoads();
        CreateRiver();
        AddLandmarks();
    }

    void GenerateGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "Ground";
        ground.transform.position = new Vector3(0f, -0.5f, 0f);
        ground.transform.localScale = new Vector3(400f, 1f, 400f);
        ground.GetComponent<Renderer>().material = new Material(Shader.Find("Standard")) { color = new Color(0.2f, 0.5f, 0.1f) }; // green ground
    }

    void GenerateDowntown()
    {
        Vector3 origin = new Vector3(0f, 0f, 50f);
        for (int x = -3; x <= 3; x++)
        {
            for (int z = -3; z <= 3; z++)
            {
                if (Mathf.Abs(x) <= 1 && Mathf.Abs(z) <= 1) // denser center
                {
                    float height = Random.Range(80f, 150f);
                    GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    b.name = "Downtown_Skyscraper_" + x + "_" + z;
                    b.transform.position = origin + new Vector3(x * 15f, height / 2f, z * 15f);
                    b.transform.localScale = new Vector3(12f, height, 12f);
                    if (downtownMat != null) b.GetComponent<Renderer>().material = downtownMat;
                }
                else
                {
                    float height = Random.Range(40f, 100f);
                    GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    b.name = "Downtown_Building_" + x + "_" + z;
                    b.transform.position = origin + new Vector3(x * 15f, height / 2f, z * 15f);
                    b.transform.localScale = new Vector3(10f, height, 10f);
                    if (downtownMat != null) b.GetComponent<Renderer>().material = downtownMat;
                }
            }
        }
    }

    void GenerateSouthside()
    {
        Vector3 origin = new Vector3(80f, 0f, -20f);
        for (int i = 0; i < 20; i++)
        {
            float w = Random.Range(8f, 15f);
            float h = Random.Range(8f, 25f);
            GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cube);
            b.name = "South_Building_" + i;
            b.transform.position = origin + new Vector3((i % 5) * 18f, h / 2f, (i / 5) * 18f);
            b.transform.localScale = new Vector3(w, h, w);
            if (southsideMat != null) b.GetComponent<Renderer>().material = southsideMat;
        }
    }

    void CreateSupermarket()
    {
        GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cube);
        s.name = "Supermarket";
        s.transform.position = new Vector3(-40f, 3f, 0f);
        s.transform.localScale = new Vector3(25f, 8f, 35f);
        if (supermarketMat != null) s.GetComponent<Renderer>().material = supermarketMat;

        // add entrance trigger
        GameObject entrance = new GameObject("SupermarketEntrance");
        var c = entrance.AddComponent<BoxCollider>(); c.isTrigger = true;
        entrance.transform.position = s.transform.position + new Vector3(0f, 1f, -18f);
        var se = entrance.AddComponent<MarketHustle.Game.StoreEntrance>();
    }

    void CreateApartments()
    {
        Vector3 origin = new Vector3(-60f, 0f, 60f);
        for (int i = 0; i < 9; i++)
        {
            GameObject a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            a.name = "ApartmentBlock_" + i;
            a.transform.position = origin + new Vector3((i % 3) * 20f, 10f, (i / 3) * 20f);
            a.transform.localScale = new Vector3(15f, 20f, 15f);
        }
    }

    void GenerateRoads()
    {
        // Horizontal roads
        for (int z = -100; z <= 100; z += 20)
        {
            GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
            road.name = "Road_H_" + z;
            road.transform.position = new Vector3(0f, 0.1f, z);
            road.transform.localScale = new Vector3(400f, 0.2f, 4f);
            if (roadMat != null) road.GetComponent<Renderer>().material = roadMat;
            else road.GetComponent<Renderer>().material.color = Color.gray;
        }

        // Vertical roads
        for (int x = -100; x <= 100; x += 20)
        {
            GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
            road.name = "Road_V_" + x;
            road.transform.position = new Vector3(x, 0.1f, 0f);
            road.transform.localScale = new Vector3(4f, 0.2f, 400f);
            if (roadMat != null) road.GetComponent<Renderer>().material = roadMat;
            else road.GetComponent<Renderer>().material.color = Color.gray;
        }
    }

    void CreateRiver()
    {
        GameObject river = GameObject.CreatePrimitive(PrimitiveType.Cube);
        river.name = "ChicagoRiver";
        river.transform.position = new Vector3(0f, -0.2f, 0f);
        river.transform.localScale = new Vector3(10f, 0.1f, 400f);
        if (riverMat != null) river.GetComponent<Renderer>().material = riverMat;
        else river.GetComponent<Renderer>().material.color = new Color(0.2f, 0.4f, 0.8f); // blue river
    }

    void AddLandmarks()
    {
        // Willis Tower-like landmark in downtown
        GameObject landmark = GameObject.CreatePrimitive(PrimitiveType.Cube);
        landmark.name = "WillisTower";
        landmark.transform.position = new Vector3(0f, 75f, 50f);
        landmark.transform.localScale = new Vector3(8f, 150f, 8f);
        if (downtownMat != null) landmark.GetComponent<Renderer>().material = downtownMat;

        // Cloud Gate (Bean) in downtown
        GameObject bean = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bean.name = "CloudGate";
        bean.transform.position = new Vector3(10f, 2f, 40f);
        bean.transform.localScale = new Vector3(6f, 3f, 6f);
        bean.GetComponent<Renderer>().material = new Material(Shader.Find("Standard")) { color = Color.white };
    }
}
