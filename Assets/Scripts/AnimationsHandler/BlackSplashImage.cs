using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BlackSplashImage : MonoBehaviour
{
    public float timeToFade = 0.5f;
    public float startDelay = 0.5f;
    private Image _image;
    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        FadeOut(startDelay);
    }

    public void FadeIn()
    {
        gameObject.SetActive(true);
        _image.DOFade(1, timeToFade);
    }

    public void FadeOut(float delay = 0)
    {
        _image.DOFade(0, timeToFade).SetDelay(delay).OnComplete(()=>{ gameObject.SetActive(false); });
    }
}
