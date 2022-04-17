using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    public string triggerTag;
    public UnityEvent triggerEvent;

    private bool _isTriggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(triggerTag) && !_isTriggered)
        {
            _isTriggered = true;
            triggerEvent?.Invoke();
        }
    }
}
