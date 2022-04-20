using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    public bool interactable = true;
    public string triggerTag;
    public UnityEvent triggerEvent;

    private bool _isTriggered;

    public void ResetTrigger()
    {
        _isTriggered = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!interactable) return;
        if (other.CompareTag(triggerTag) && !_isTriggered)
        {
            _isTriggered = true;
            triggerEvent?.Invoke();
        }
    }
}
