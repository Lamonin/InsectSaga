using UnityEngine;

public class SceneOptions : MonoBehaviour
{
    public static SceneOptions Handler;

    [Header("При загрузке")]
    public float timeToStartFade;
    public float startDelayTime;

    [Header("При смерти")]
    public float timeToFade;
    public float delayTime;
    
    private void Awake() {
        Handler ??= this;
    }

    private void OnDestroy() {
        Handler = null;
    }
}