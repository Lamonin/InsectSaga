using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using MEC;

public class EventTrigger : MonoBehaviour
{
    public bool interactable { get; set; } = true;
    public string triggerTag;
    public float delay = 0;
    public UnityEvent triggerEvent;

    private bool _isTriggered;

    public void ResetTrigger()
    {
        _isTriggered = false;
    }

    private void Awake()
    {
        gameObject.SetActive(false);
        Timing.RunCoroutine(TriggerDelay());
    }

    private IEnumerator<float> TriggerDelay()
    {
        yield return Timing.WaitForSeconds(delay);
        gameObject.SetActive(true);
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
