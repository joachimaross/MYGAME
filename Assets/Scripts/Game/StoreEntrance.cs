using UnityEngine;
using UnityEngine.SceneManagement;

public class StoreEntrance : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("SupermarketInterior");
        }
    }
}
