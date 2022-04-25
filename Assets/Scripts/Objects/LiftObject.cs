using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class LiftObject : MonoBehaviour
{
    [SerializeField] [Tooltip("Высота лифта")] private float height = 7f;
    [field: SerializeField] [Tooltip("Позиция, где лифт остановится")] public float PlatformEndHeight { get; set; }

    [SerializeField] private float duration = 10f;

    [SerializeField] private UnityEvent endEvent;

    [Space] [SerializeField]
    private Ease easeType = Ease.Linear;
    [SerializeField] private float blockOffset = 0.5f;
    
    [Space]
    [SerializeField] private Transform platform;
    [SerializeField] private Transform blockLeft;
    [SerializeField] private Transform blockRight;
    [SerializeField] private SpriteRenderer backPillars;
    [SerializeField] private SpriteRenderer frontPillars;
    [SerializeField] private SpriteRenderer cables;

    private bool _isActivated;
    
    void Start()
    {
        ResizeLift();
    }
    
    public void Activate()
    {
        if (_isActivated) return;
        _isActivated = true;
        
        platform.DOMove(transform.position + new Vector3(0, PlatformEndHeight-1, 0), duration)
            .SetEase(easeType)
            .OnComplete(
            () =>
            {
                _isActivated = false;
                endEvent?.Invoke();
            });
        
        var blockPos = Mathf.Clamp(PlatformEndHeight + blockOffset, blockOffset, height-blockOffset);

        blockLeft.DOLocalMove(new Vector3(blockLeft.localPosition.x, blockPos, 0.1f), duration).SetEase(easeType);
        blockRight.DOLocalMove(new Vector3(blockRight.localPosition.x, height-blockPos, 0.1f), duration).SetEase(easeType);
    }

    [ContextMenu("Resize")]
    private void ResizeLift()
    {
        backPillars.size = new Vector2(backPillars.size.x, height);
        frontPillars.size = new Vector2(frontPillars.size.x, height);
        cables.size = new Vector2(cables.size.x, height);
        
        var blockPos = Mathf.Clamp(platform.localPosition.y + blockOffset, blockOffset, height-blockOffset);
        blockLeft.localPosition = new Vector3(blockLeft.localPosition.x, blockPos, 0.1f);
        blockRight.localPosition = new Vector3(blockRight.localPosition.x, height-blockPos, 0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position+new Vector3(0,height/2, 0), new Vector3(3, height, 0));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position+new Vector3(0,PlatformEndHeight-0.5f, 0), new Vector3(2, 1f, 0));
    }
}
