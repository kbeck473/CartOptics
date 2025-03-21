using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadNewScene()
    {
        SceneManager.LoadScene("Camera Frame Access Sample");
    }
}
