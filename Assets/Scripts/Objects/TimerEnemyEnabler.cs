using UnityEngine;

public class TimerEnemyEnabler : MonoBehaviour {
    private TimerEnemy[] tEnemys;

    private void Awake() {
        tEnemys = transform.GetComponentsInChildren<TimerEnemy>();
    }

    public void ActivateAllTimerEnemy()
    {
        foreach (var tEnemy in tEnemys)
        {
            tEnemy.Activate();
        }
    }
}