using UnityEngine;

[RequireComponent(typeof(ES3AutoSaveMgr))]
public class LevelManager : MonoBehaviour
{
    public static LevelManager Handler;
    
    private void Awake()
    {
        Handler ??= this;
        LoadData();
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
        ES3AutoSaveMgr.Current.Save();
    }

    public void LoadData()
    {
        ES3AutoSaveMgr.Current.Load();
    }
}
