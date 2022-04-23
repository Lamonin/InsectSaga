using SaveIsEasy;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGame()
    {
        var path = SaveIsEasyAPI.SaveFolderPath + SaveIsEasyAPI.SceneConfig.SceneFileName + ".game";
        if (SaveIsEasyAPI.FileExists(path))
            SaveIsEasyAPI.LoadSceneAndGame();
        else
        {
            SceneManager.LoadScene("IntroTextScene");
        }
    }

    public void LoadTestScene()
    {
        SceneManager.LoadScene("TestRoom");
    }


    public void CloseGame()
    {
        Debug.Log("EXIT FROM GAME");
        Application.Quit();
    }
}
