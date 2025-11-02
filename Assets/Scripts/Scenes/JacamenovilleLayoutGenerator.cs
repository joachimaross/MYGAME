using UnityEngine;

/// <summary>
/// Procedurally generates a simple Jacamenoville layout with a downtown skyscraper cluster
/// and a southside (hood) low-rise cluster plus a supermarket and apartment blocks.
/// </summary>
public class JacamenovilleLayoutGenerator : MonoBehaviour
{
    public Material downtownMat;
    public Material southsideMat;
    public Material supermarketMat;

    void Start()
    {
        GenerateDowntown();
        GenerateSouthside();
        CreateSupermarket();
        CreateApartments();
    }

    void GenerateDowntown()
    {
        Vector3 origin = new Vector3(0f, 0f, 50f);
        for (int x = -2; x <= 2; x++)
        {
            for (int z = -2; z <= 2; z++)
            {
                float height = Random.Range(40f, 120f);
                GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cube);
                b.name = "Downtown_Building_" + x + "_" + z;
                b.transform.position = origin + new Vector3(x * 12f, height / 2f, z * 12f);
                b.transform.localScale = new Vector3(10f, height, 10f);
                if (downtownMat != null) b.GetComponent<Renderer>().material = downtownMat;
            }
        }
    }

    void GenerateSouthside()
    {
        Vector3 origin = new Vector3(80f, 0f, -20f);
        for (int i = 0; i < 12; i++)
        {
            float w = Random.Range(6f, 12f);
            float h = Random.Range(6f, 18f);
            GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cube);
            b.name = "South_Building_" + i;
            b.transform.position = origin + new Vector3((i % 4) * 14f, h / 2f, (i / 4) * 14f);
            b.transform.localScale = new Vector3(w, h, w);
            if (southsideMat != null) b.GetComponent<Renderer>().material = southsideMat;
        }
    }

    void CreateSupermarket()
    {
        GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cube);
        s.name = "Supermarket";
        s.transform.position = new Vector3(-40f, 3f, 0f);
        s.transform.localScale = new Vector3(20f, 6f, 30f);
        if (supermarketMat != null) s.GetComponent<Renderer>().material = supermarketMat;

        // add entrance trigger
        GameObject entrance = new GameObject("SupermarketEntrance");
        var c = entrance.AddComponent<BoxCollider>(); c.isTrigger = true;
        entrance.transform.position = s.transform.position + new Vector3(0f, 1f, -16f);
        var se = entrance.AddComponent<MarketHustle.Game.StoreEntrance>();
        // ensure player tag exists and is set in the player prefab/actor
    }

    void CreateApartments()
    {
        Vector3 origin = new Vector3(-60f, 0f, 60f);
        for (int i = 0; i < 6; i++)
        {
            GameObject a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            a.name = "ApartmentBlock_" + i;
            a.transform.position = origin + new Vector3((i % 3) * 18f, 8f, (i / 3) * 18f);
            a.transform.localScale = new Vector3(12f, 16f, 12f);
        }
    }
}
