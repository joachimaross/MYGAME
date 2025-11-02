using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToCity : MonoBehaviour
{
    public string sceneToReturn = "Jacamenoville";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneToReturn);
        }
    }
}
