using System.Collections.Generic;
using UnityEngine;
using MEC;

public class TimerEnemy : MonoBehaviour
{
    [SerializeField] private bool isAlwaysActive;

    [SerializeField]
    [Tooltip("Задержка")] private bool isEnabledOnStart;
    
    [SerializeField]
    [Tooltip("Задержка перед началом")] private float delayBeforeStart;
    
    [SerializeField]
    [Tooltip("Задержка")] private float delayBeforeDangerous;

    [SerializeField]
    [Tooltip("Время в нанесении урона")] private float dangerousTime;

    [SerializeField]
    [Tooltip("Безопасное время")] private float safeTime = 0.8f;

    [SerializeField] private GameObject collision;
    [SerializeField] private Animator animator;

    private readonly bool _enabled = true;
    private CoroutineHandle _coroutine;
    private static readonly int EnemyActive = Animator.StringToHash("active");

    void Start()
    {
        collision.SetActive(false);
        animator.SetBool(EnemyActive, false);
        if (isEnabledOnStart)
            Activate();
    }

    public void Activate()
    {
        Timing.KillCoroutines(_coroutine);
        _coroutine = Timing.RunCoroutine(DangerZoneState());
    }
    
    public void Deactivate()
    {
        Timing.KillCoroutines(_coroutine);
        collision.SetActive(false);
        animator.SetBool(EnemyActive, false);
    }

    private IEnumerator<float> DangerZoneState()
    {
        yield return Timing.WaitForSeconds(delayBeforeStart);
        while(_enabled)
        {
            yield return Timing.WaitForSeconds(delayBeforeDangerous);
            animator.SetBool(EnemyActive, true);
            yield return Timing.WaitForSeconds(safeTime);
            
            collision.SetActive(true);
            if (isAlwaysActive) yield break;
            yield return Timing.WaitForSeconds(dangerousTime);
            collision.SetActive(false);
            animator.SetBool(EnemyActive, false);
        }
    }

    private void OnDestroy()
    {
        Timing.KillCoroutines(_coroutine);
    }
}
