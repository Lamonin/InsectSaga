using SaveIsEasy;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Handler;
    public bool loadSaveOnStart;

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
        if (loadSaveOnStart) LoadData();
    }

    [ContextMenu("Restart Level")]
    private void RestartLevelFromCheckpoint()
    {
        if (!LoadData())
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

    public bool LoadData()
    {
        var path = SaveIsEasyAPI.SaveFolderPath + SaveIsEasyAPI.SceneConfig.SceneFileName + ".game";
        if (SaveIsEasyAPI.FileExists(path))
        {
            Debug.Log("Загрузка сохранения!");
            SaveIsEasyAPI.LoadAll();
            BlackSplashImage.Handler.FadeOut(1,0.8f, ()=>{ EventBus.OnPlayerRespawned?.Invoke(); });
            return true;
        }
        Debug.Log("Не найдено файла сохранения!");
        return false;
    }
}
