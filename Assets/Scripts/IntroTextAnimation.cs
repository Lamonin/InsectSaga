using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroTextAnimation : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 endPosition;
    
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private TextMeshProUGUI skipLabel;
    [SerializeField] private Image fillSkipIcon;
    [SerializeField] private AudioSource audioSource;


    private GUIActions _input;
    private bool _isEnded = true;

    private void Awake()
    {
        _input = new GUIActions();
        _input.UI.Skip.started += _ => 
        {
            fillSkipIcon.DOKill();
            fillSkipIcon.DOFillAmount(1, 1).SetEase(Ease.InSine);
        };

        _input.UI.Skip.canceled += _ => 
        {
            fillSkipIcon.DOKill();
            fillSkipIcon.DOFillAmount(0, 0.2f).SetEase(Ease.InSine);
            fillSkipIcon.transform.DOShakePosition(0.2f, 2);
        };

        _input.UI.Skip.performed += _ => 
        {
            _input.Disable();
            fillSkipIcon.DOFillAmount(0, 0.2f).SetEase(Ease.InSine);
            SkipIntroScene();
        };

        _input.Disable();

        transform.position = startPosition;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 300;
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


    private void DelayBeforeSkip() 
    {
        _isEnded = false;
        skipLabel.gameObject.SetActive(true);
        skipLabel.DOFade(1, 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        _input.Enable();
    }

    private void SkipIntroScene()
    {
        if (_isEnded) return;
        _isEnded = true;
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
