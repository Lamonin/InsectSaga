using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class DORotateTransform : MonoBehaviour
{
    [SerializeField] private float degree;
    [SerializeField] private float duration;
    [SerializeField] private Ease easeType = Ease.OutSine;
    [SerializeField] private UnityEvent onEnd;
    private bool _activated;


    public void Rotate()
    {
        if (_activated) return;
        _activated = true;
        transform.DOKill();
        transform.DORotate(new Vector3(0,0,degree), duration, RotateMode.WorldAxisAdd).SetEase(easeType).OnComplete(()=>
        {
            onEnd?.Invoke();
        });
    }
}
