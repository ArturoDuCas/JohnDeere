using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ChangeToScene(string pruebaConArturo)
    {
        SceneManager.LoadScene(pruebaConArturo);
    }
}
