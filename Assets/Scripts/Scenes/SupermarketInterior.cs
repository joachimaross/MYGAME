using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Placeholder script for the SupermarketInterior scene. Creates simple shelves and an exit trigger to return to Jacamenoville.
/// </summary>
public class SupermarketInterior : MonoBehaviour
{
    void Start()
    {
        CreateShelves();
        CreateExitTrigger();
    }

    void CreateShelves()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject shelf = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shelf.name = "Shelf_" + i;
            shelf.transform.localScale = new Vector3(6f, 3f, 1f);
            shelf.transform.position = new Vector3(-8f + i * 6f, 1.5f, 0f);
        }
    }

    void CreateExitTrigger()
    {
        GameObject exit = new GameObject("ExitTrigger");
        var col = exit.AddComponent<BoxCollider>();
        col.isTrigger = true;
        exit.transform.position = new Vector3(0f, 1f, -8f);
        exit.transform.localScale = new Vector3(4f, 2f, 1f);
        var et = exit.AddComponent<ExitToCity>();
        et.sceneToReturn = "Jacamenoville"; // scene name for city
    }
}
