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
    public Camera mainCamera;
    
    //VARIABLES
    private Vector3 _useIconPos = Vector3.zero;

    public static void ShowUseIcon(UsableObject useObject)
    {
        if (Handler is null) return;
        if (!useObject.interactable) return;
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
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.anchoredPosition = new Vector2(0, -75);
        }
        else
        {
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.anchoredPosition = new Vector2(0, 75);
        }
    }

    private void DialogueEnd()
    {
        dialogText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventBus.OnDialogueStart += DialogueStart;
        EventBus.OnDialogueEnd += DialogueEnd;
    }

    private void OnDisable()
    {
        EventBus.OnDialogueStart -= DialogueStart;
        EventBus.OnDialogueEnd -= DialogueEnd;
    }

    private void Awake()
    {
        Handler ??= this;
        QualitySettings.vSyncCount = 2;
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
