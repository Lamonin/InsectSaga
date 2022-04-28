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
        SceneManager.sceneLoaded += ForceReloadScene;
    }

    private void OnDisable()
    {
        EventBus.OnBlackScreenFadeInEvent -= RestartLevelFromCheckpoint;
        SceneManager.sceneLoaded -= ForceReloadScene;
    }

    void ForceReloadScene(Scene scene, LoadSceneMode mode)
    {
        var path = SaveIsEasyAPI.SaveFolderPath + SaveIsEasyAPI.SceneConfig.SceneFileName + ".game";
        if (!SaveIsEasyAPI.FileExists(path)) return;

        var i = SceneManager.GetActiveScene().buildIndex;


        SceneManager.SetActiveScene(scene);
        SceneManager.UnloadSceneAsync(i);

        if (SaveIsEasyAPI.GetSceneFile(path).SceneBuildIndex != i)
        {
            LoadData();
        }
    }

    void Start()
    {
        if (loadSaveOnStart) LoadData();
    }

    private void OnDestroy()
    {
        Handler = null;
    }

    [ContextMenu("Restart Level")]
    private void RestartLevelFromCheckpoint()
    {
        Debug.Log("Try restart!");
        if (forceRestart || !LoadData(true) )
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
        Debug.Log("Level restarted!");
    }

    [ContextMenu("Save Level")]
    public void SaveData()
    {
        SaveIsEasyAPI.SaveAll();
    }

    public bool LoadData(bool isReload = false)
    {
        var path = SaveIsEasyAPI.SaveFolderPath + SaveIsEasyAPI.SceneConfig.SceneFileName + ".game";
        
        if (SaveIsEasyAPI.FileExists(path))
        {
            Debug.Log("Загрузка сохранения!");

            var i = SceneManager.GetActiveScene().buildIndex;
            if (SaveIsEasyAPI.GetSceneFile(path).SceneBuildIndex == i)
                SaveIsEasyAPI.LoadAll();

            EventBus.ClearEvents();
            if (isReload) BlackSplashImage.Handler.FadeOut(1, 0.8f, ()=>{ EventBus.OnPlayerRespawned?.Invoke(); });
            return true;
        }
        Debug.Log("Не найдено файла сохранения!");
        return false;
    }
}
