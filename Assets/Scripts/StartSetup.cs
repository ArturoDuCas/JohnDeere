using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSetup : MonoBehaviour
{
    public string DANIELA; // Name of the 3D game scene.

    public void StartGame()
    {
        Debug.Log("StartGame() called with scene name: " + DANIELA);
        SceneManager.LoadScene(DANIELA);
    }

}
