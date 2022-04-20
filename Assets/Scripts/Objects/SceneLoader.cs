using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName;
    
    public void LoadDefinedScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadDefinedSceneAfterDelay(float delay)
    {
        Invoke(nameof(LoadDefinedScene), delay);
    }
}
