using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using MEC;


public class Timer : MonoBehaviour
{
    [SerializeField] private bool startFromAwake;
    [SerializeField] private float delay;
    [SerializeField] private UnityEvent onEnd;

    private CoroutineHandle _routine;

    private void Awake()
    {
        if (startFromAwake) StartTimer();
    }

    public void StartTimer()
    {
        _routine = Timing.RunCoroutine(EnableTimer());
    }

    private IEnumerator<float> EnableTimer()
    {
        yield return Timing.WaitForSeconds(delay);
        onEnd?.Invoke();
    }

    private void OnDestroy()
    {
        Timing.KillCoroutines(_routine);
    }
}
