using System.Collections.Generic;
using UnityEngine;
using MEC;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TextChanger : MonoBehaviour
{
    [SerializeField] private float delayBetweenMessages;
    [SerializeField] private float lastDelay = 10;
    [SerializeField] private float timeToFadeIn;
    [SerializeField] private float timeToFadeOut;
    [SerializeField] private CanvasGroup[] messagesTMPro;

    private CoroutineHandle _routine;
    private GUIActions _input;

    private void Awake()
    {
        _input = new GUIActions();
        
    }

    private void OnEnable()
    {
        _input.UI.Enable();
    }

    private void OnDisable()
    {
        _input.UI.Disable();
    }

    private void Start()
    {
        _input.UI.Escape.performed += _ =>
        {
            if( !_isCycled) GoToMainMenu();
        };

        foreach (var t in messagesTMPro)
        {
            t.alpha = 0;
        }

        messagesTMPro[0].DOFade(1, timeToFadeIn);

        _routine = Timing.RunCoroutine(ChangeMessage());
    }

    private void OnDestroy()
    {
        Timing.KillCoroutines(_routine);
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private int _currentMessage;
    private bool _isCycled = true;
    private IEnumerator<float> ChangeMessage()
    {
        while(_isCycled)
        {
            _currentMessage++;
            if (_currentMessage >= messagesTMPro.Length)
            {
                _isCycled = false;
                break;
            }
            
            yield return Timing.WaitForSeconds(delayBetweenMessages);
            messagesTMPro[_currentMessage - 1].DOFade(0, timeToFadeOut);
            yield return Timing.WaitForSeconds(timeToFadeOut);

            messagesTMPro[_currentMessage].DOFade(1, timeToFadeIn);
            yield return Timing.WaitForSeconds(timeToFadeIn);
        }

        yield return Timing.WaitForSeconds(lastDelay);
        GoToMainMenu();
    }
}
