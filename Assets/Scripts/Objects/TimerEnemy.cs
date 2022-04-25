using System.Collections;
using UnityEngine;

public class TimerEnemy : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Задержка")] private bool isEnabledOnStart;
    
    [SerializeField]
    [Tooltip("Задержка перед началом")] private float delayBeforeStart;
    
    [SerializeField]
    [Tooltip("Задержка")] private float delayBeforeDangerous;

    [SerializeField]
    [Tooltip("Время в нанесении урона")] private float dangerousTime;

    [SerializeField] private GameObject collision;
    [SerializeField] private Animator animator;

    private readonly bool _enabled = true;
    private Coroutine _coroutine;
    private static readonly int EnemyActive = Animator.StringToHash("active");

    void Start()
    {
        collision.SetActive(false);
        animator.SetBool(EnemyActive, false);
        if (isEnabledOnStart)
            Activate();
    }

    private void Activate()
    {
        if(_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(DangerZoneState());
    }
    
    private void Deactivate()
    {
        if(_coroutine != null)
            StopCoroutine(_coroutine);
    }

    private IEnumerator DangerZoneState()
    {
        yield return new WaitForSeconds(delayBeforeStart);
        while(_enabled)
        {
            yield return new WaitForSeconds(delayBeforeDangerous);
            animator.SetBool(EnemyActive, true);
            yield return new WaitForSeconds(0.8f);
            
            collision.SetActive(true);
            yield return new WaitForSeconds(dangerousTime);
            collision.SetActive(false);
            animator.SetBool(EnemyActive, false);
        }
    }

    private void OnDestroy()
    {
        if(_coroutine != null)
            StopCoroutine(_coroutine);
    }
}
