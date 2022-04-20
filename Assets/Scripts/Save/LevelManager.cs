using SaveIsEasy;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Handler;
    public bool loadOnStart = true;

    private void Awake()
    {
        Handler ??= this;
    }

    private void OnEnable()
    {
        EventBus.OnPlayerDiedEvent += RestartLevelFromCheckpoint;
    }

    private void OnDisable()
    {
        EventBus.OnPlayerDiedEvent -= RestartLevelFromCheckpoint;
    }

    void Start()
    {
        LoadData();
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
            SaveIsEasyAPI.LoadAll();
    }
}
