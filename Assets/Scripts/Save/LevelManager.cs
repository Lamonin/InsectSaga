using SaveIsEasy;
using UnityEngine;

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
        //EventBus.OnPlayerDiedEvent += RestartLevelFromCheckpoint;
        EventBus.OnBlackScreenFadeInEvent += RestartLevelFromCheckpoint;
    }

    private void OnDisable()
    {
        //EventBus.OnPlayerDiedEvent -= RestartLevelFromCheckpoint;
        EventBus.OnBlackScreenFadeInEvent -= RestartLevelFromCheckpoint;
    }

    void Start()
    {
        if (loadSaveOnStart) LoadData();
    }

    [ContextMenu("Restart Level")]
    private void RestartLevelFromCheckpoint()
    {
        LoadData();
        Debug.Log("Level restarted!");
    }

    [ContextMenu("Save Level")]
    public void SaveData()
    {
        SaveIsEasyAPI.SaveAll();
    }

    public void LoadData()
    {
        var path = SaveIsEasyAPI.SaveFolderPath + SaveIsEasyAPI.SceneConfig.SceneFileName + ".game";
        if (SaveIsEasyAPI.FileExists(path))
        {
            Debug.Log("Загрузка сохранения!");
            SaveIsEasyAPI.LoadAll();
            BlackSplashImage.Handler.FadeOut();
            EventBus.OnPlayerRespawned?.Invoke();
        }
        else
        {
            Debug.Log("Не найдено файла сохранения!");
        }
    }
}
