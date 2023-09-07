using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public string ARTURO;
    // Method to change the scene based on the scene name received in a WebSocket message
    public void ChangeToScene()
    {
        // Load the specified scene
        SceneManager.LoadScene(ARTURO);
    }
}
