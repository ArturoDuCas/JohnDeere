using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    // Method to change the scene based on the scene name received in a WebSocket message
    public void ChangeToScene(string DANIELA)
    {
        // Load the specified scene
        SceneManager.LoadScene(DANIELA);
    }
}
