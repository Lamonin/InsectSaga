using SaveIsEasy;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameVersionText;

    void Start()
    {
        gameVersionText.text = $"ver. {Application.version}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ContinueGame()
    {
        var path = SaveIsEasyAPI.SaveFolderPath + SaveIsEasyAPI.SceneConfig.SceneFileName + ".game";
        if (SaveIsEasyAPI.FileExists(path))
            SaveIsEasyAPI.LoadSceneAndGame();
        else
            Debug.Log("Game save in not find!");
    }

    public void NewGame()
    {
        SceneManager.LoadScene("IntroTextScene");
    }

    public void LoadGame()
    {
        var path = SaveIsEasyAPI.SaveFolderPath + SaveIsEasyAPI.SceneConfig.SceneFileName + ".game";
        if (SaveIsEasyAPI.FileExists(path))
            SaveIsEasyAPI.LoadSceneAndGame();
        else
            SceneManager.LoadScene("IntroTextScene");
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
