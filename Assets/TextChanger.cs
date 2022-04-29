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
        _input.UI.Enable();
    }

    private void Start()
    {
        _input.UI.Escape.performed += _ =>
        {
            if( !_isCycled) GoToMainMenu();
        };

        for (var i = 0; i < messagesTMPro.Length; i++)
        {
            if (i != 0) messagesTMPro[i].alpha = 0;
        }

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
