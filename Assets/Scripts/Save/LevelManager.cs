using SaveIsEasy;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Handler;
    public bool loadSaveOnStart;
    public bool forceRestart;

    private void Awake()
    {
        Handler ??= this;
    }

    private void OnEnable()
    {
        EventBus.OnBlackScreenFadeInEvent += RestartLevelFromCheckpoint;
    }

    private void OnDisable()
    {
        EventBus.OnBlackScreenFadeInEvent -= RestartLevelFromCheckpoint;
    }

    void Start()
    {
        //if (loadSaveOnStart) LoadData();
    }

    private void OnDestroy()
    {
        Handler = null;
    }

    [ContextMenu("Restart Level")]
    private void RestartLevelFromCheckpoint()
    {
        if (IsSaveExist())
        {
            if (forceRestart)
                SaveIsEasyAPI.LoadSceneAndGame();
            else
                ReloadData();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        Debug.Log("Level restarted!");
    }

    [ContextMenu("Save Level")]
    public void SaveData()
    {
        SaveIsEasyAPI.SaveAll();
    }

    private bool IsSaveExist()
    {
        var path = SaveIsEasyAPI.SaveFolderPath + SaveIsEasyAPI.SceneConfig.SceneFileName + ".game";
        
        if (SaveIsEasyAPI.FileExists(path))
        {
            var i = SceneManager.GetActiveScene().buildIndex;
            if (SaveIsEasyAPI.GetSceneFile(path).SceneBuildIndex == i)
                return true;
        }
        return false;
    }

    public void ReloadData(bool isReload = false)
    {
        SaveIsEasyAPI.LoadAll();

        BlackSplashImage.Handler.FadeOut(1, 0.8f, ()=>
        {
            EventBus.OnPlayerRespawned?.Invoke();
        });
    }
}
