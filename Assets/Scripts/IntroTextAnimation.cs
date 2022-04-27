using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroTextAnimation : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 endPosition;
    
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private TextMeshProUGUI skipLabel;
    [SerializeField] private AudioSource audioSource;

    private bool _isEnded = true;

    private void Awake()
    {
        transform.position = startPosition;
        QualitySettings.vSyncCount = 2;
    }

    void Start()
    {
        transform.DOMove(endPosition, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            SceneManager.LoadScene("Prologue");
        });
        textMesh.DOFade(0, duration*0.1f).SetDelay(duration * 0.9f);
        audioSource.DOFade(0, duration*0.1f).SetDelay(duration * 0.9f);

        skipLabel.gameObject.SetActive(false);

        Invoke(nameof(DelayBeforeSkip), 5f);
    }

    private void Update()
    {
        if (!_isEnded && Input.anyKeyDown)
        {
            _isEnded = true;
            SkipIntroScene();
        }
    }

    private void DelayBeforeSkip() 
    {
        _isEnded = false;
        skipLabel.gameObject.SetActive(true);
        skipLabel.DOFade(1, 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }

    private void SkipIntroScene()
    {
        transform.DOKill();

        transform.DOMove(endPosition, 2).SetEase(Ease.InSine).OnComplete(() =>
        {
            SceneManager.LoadScene("Prologue");
        });
        textMesh.DOFade(0, 2);

        audioSource.DOKill();
        audioSource.DOFade(0, 1);
    }
}
