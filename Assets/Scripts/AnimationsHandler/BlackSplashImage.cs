using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class BlackSplashImage : MonoBehaviour
{
    public static BlackSplashImage Handler;

    public TextMeshProUGUI quoteLabel;
    public float timeToFade = 0.5f;
    public float startDelay = 0.5f;
    private Image _image;
    private void Awake()
    {
        Handler ??= this;
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        FadeOut(timeToFade, startDelay);
    }

    private void OnDestroy()
    {
        Handler = null;
    }

    public void ForceFadeIn()
    {
        _image.DOKill();
        gameObject.SetActive(true);
        _image.DOFade(1, timeToFade).SetEase(Ease.Linear);
    }

    public void FlashFadeIn()
    {
        _image.DOKill();
        gameObject.SetActive(true);
        _image.DOFade(1, 0.01f).SetEase(Ease.Linear).OnComplete(()=>EventBus.OnBlackScreenFadeInEvent?.Invoke());
    }
    
    public void ForceFadeIn(float timeToFade)
    {
        float temp = this.timeToFade;
        this.timeToFade = timeToFade;
        ForceFadeIn();
        this.timeToFade = temp;
    }

    public void FadeIn()
    {
        _image.DOKill();
        gameObject.SetActive(true);
        _image.DOFade(1, timeToFade).SetEase(Ease.Linear).OnComplete(()=>EventBus.OnBlackScreenFadeInEvent?.Invoke());
    }

    public void FadeIn(float timeToFade)
    {
        float temp = this.timeToFade;
        this.timeToFade = timeToFade;
        FadeIn();
        this.timeToFade = temp;
    }

    public void FadeOut(float duration = 1, float delay = 0, Action actionAfterDelay = null)
    {
        _image.DOKill();
        _image.DOFade(0, duration).SetDelay(delay).SetEase(Ease.Linear).OnStart(() =>
        {
            actionAfterDelay?.Invoke();
        }).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });

        if (quoteLabel != null)
            quoteLabel.DOFade(0, 1).SetDelay(Mathf.Clamp(delay-1, 0, Mathf.Infinity));
    }
}
