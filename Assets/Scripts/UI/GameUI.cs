using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Objects;
using TMPro;

public class GameUI : MonoBehaviour
{
    public static GameUI Handler;
    
    public float fadeUseIconTime = 0.2f;

    [Header("Компоненты")]
    public Image useIcon;
    public TextMeshProUGUI dialogText;
    public GameObject pauseMenu;
    private Camera mainCamera;
    
    //VARIABLES
    private Vector3 _useIconPos = Vector3.zero;
    private GUIActions _input;

    public static void ShowUseIcon(UsableObject useObject)
    {
        if (Handler is null) return;
        if (useObject is null || !useObject.interactable) return;
        Handler._useIconPos = useObject.useIconOffset;
        Handler.useIcon.rectTransform.position = Handler.mainCamera.WorldToScreenPoint(Handler._useIconPos);
        Handler.useIcon.gameObject.SetActive(true);
        Handler.useIcon.DOKill();
        Handler.useIcon.DOFade(1, Handler.fadeUseIconTime);
    }
    
    public static void HideUseIcon(bool immediately = false)
    {
        if (Handler is null) return;
        if (!Handler.useIcon.gameObject.activeSelf) return;
        Handler.useIcon.DOKill();
        
        if (immediately)
        {
            Handler.useIcon.DOFade(0, 0);
            Handler.useIcon.gameObject.SetActive(false);
        }
        else
        {
            Handler.useIcon.DOFade(0, Handler.fadeUseIconTime).OnComplete(() => Handler.useIcon.gameObject.SetActive(false));
        }
    }

    public void ShowPauseMenu()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        EventBus.OnPauseMenuOpenEvent?.Invoke();
    }

    public void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        EventBus.OnPauseMenuCloseEvent?.Invoke();
    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1;
    }

    private void UpdateUseIconPosition()
    {
        if (useIcon.gameObject.activeSelf)
        {
            useIcon.rectTransform.position = mainCamera.WorldToScreenPoint(_useIconPos);
        }
    }

    private void DialogueStart(bool b, bool isOnTop = false)
    {
        dialogText.gameObject.SetActive(true);
        var rt = dialogText.GetComponent<RectTransform>();
        
        if (isOnTop)
        {
            rt.anchorMin = new Vector2(0.5f, 1);
            rt.anchorMax = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, -270);
        }
        else
        {
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.anchoredPosition = new Vector2(0, 0);
        }
    }

    private void DialogueEnd()
    {
        dialogText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _input.Enable();
        EventBus.OnDialogueStart += DialogueStart;
        EventBus.OnDialogueEnd += DialogueEnd;
    }

    private void OnDisable()
    {
        _input.Disable();
        EventBus.OnDialogueStart -= DialogueStart;
        EventBus.OnDialogueEnd -= DialogueEnd;
    }

    private void Awake()
    {
        _input = new GUIActions();
        _input.UI.Escape.performed += _ => 
        {
            if (pauseMenu == null) return;
            if (pauseMenu.activeSelf)
                ClosePauseMenu();
            else
                ShowPauseMenu();
        };
        
        Handler ??= this;
        mainCamera = Camera.main;
    }

    private void Start()
    {
        useIcon.gameObject.SetActive(false);
        dialogText.gameObject.SetActive(false);
    }


    private void Update()
    {
        UpdateUseIconPosition();
    }

    private void OnDestroy()
    {
        Handler = null;
    }
}
