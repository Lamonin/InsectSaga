using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
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

    public static void ShowUseIcon(Vector2 pos)
    {
        
        Handler._useIconPos = pos;
        Handler.useIcon.rectTransform.position = Handler.mainCamera.WorldToScreenPoint(Handler._useIconPos);
        Handler.useIcon.gameObject.SetActive(true);
        Handler.useIcon.DOKill();
        Handler.useIcon.DOFade(1, Handler.fadeUseIconTime);
    }
    
    public static void HideUseIcon(bool immediately = false)
    {
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

    private void Start()
    {
        Handler ??= this;
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
