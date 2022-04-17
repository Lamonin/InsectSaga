using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class GamepadHandler:MonoBehaviour
{
    private static float _counter;
    private float _power;
    private float _duration;
    private Coroutine _repeatVibrationRoutine;
    private bool isStillVibration;

    public void Vibrate()
    {
        if (Gamepad.current == null) return;
        Gamepad.current.SetMotorSpeeds(_power, _power);
        isStillVibration = true;
        _repeatVibrationRoutine = StartCoroutine(RepeatingVibration());
    }

    public void StopVibrate()
    {
        if (Gamepad.current == null) return;
        Gamepad.current.SetMotorSpeeds(0, 0);
        isStillVibration = false;
        if (_repeatVibrationRoutine != null)
            StopCoroutine(RepeatingVibration());
    }

    private IEnumerator RepeatingVibration()
    {
        while (isStillVibration)
        {
            if (Gamepad.current == null) yield break;
            Gamepad.current.SetMotorSpeeds(0, 0);
            Gamepad.current.SetMotorSpeeds(_power, _power);
            yield return new WaitForSeconds(2f);
        }
    }

    public void SetDuration(float duration)
    {
        _duration = duration;
    }

    public void SetPower(float power)
    {
        _power = power;
    }

    // private void Update()
    // {
    //     if (_counter < 0)
    //     {
    //         _counter = 0;
    //         Gamepad.current.SetMotorSpeeds(0, 0);
    //     }
    //     else if (_counter != 0)
    //     {
    //         _counter -= Time.deltaTime;
    //     }
    // }
}