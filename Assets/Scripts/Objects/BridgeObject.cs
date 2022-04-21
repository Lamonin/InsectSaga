
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class BridgeObject : MonoBehaviour
{
    [SerializeField] private float timeToUp = 1f;
    [SerializeField] private float angleToRotate;
    [SerializeField] private bool onlyOnce;
    
    [SerializeField] private UnityEvent beginEvent;
    [SerializeField] private UnityEvent endEvent;

    [Space] [SerializeField]
    private Ease easeType = Ease.Linear;

    [SerializeField] private Transform pivot;
    private bool _isUpped;
    private bool _isProcessing;
    private float _angleStart;

    void Start()
    {
        _angleStart = pivot.rotation.eulerAngles.z;
    }

    public void Activate()
    {
        if (_isProcessing) return;
        _isProcessing = true;
        beginEvent?.Invoke();
        pivot.DORotate(new Vector3(0, 0, _isUpped ? _angleStart : angleToRotate), timeToUp).SetEase(easeType)
            .OnComplete(
            () =>
            {
                endEvent?.Invoke();
                if (!onlyOnce) _isProcessing = false;
            });
        _isUpped = !_isUpped;
    }
}
