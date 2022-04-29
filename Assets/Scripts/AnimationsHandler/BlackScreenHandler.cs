using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BlackScreenHandler : MonoBehaviour
{
    public static BlackScreenHandler Handler;

    public Image image;
    private float timeToFade;
    private float delayTime;

    private void Awake() {
        Handler ??= this;
    }

    private void Start() {
        image.DOFade(0, SceneOptions.Handler.timeToStartFade).SetDelay(SceneOptions.Handler.startDelayTime);
    }

    private void OnDestroy() {
        Handler = null;
    }

    public void FadeIn(float duration)
    {
        image.DOFade(1, duration);
    }
}