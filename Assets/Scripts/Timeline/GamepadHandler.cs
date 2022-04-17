using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class GamepadHandler:MonoBehaviour
{
    private static float _counter;
    private float _power;
    private float _duration;
    private Coroutine _repeatVibrationRoutine;

    public void Vibrate()
    {
        Gamepad.current.SetMotorSpeeds(_power, _power);
        _repeatVibrationRoutine = StartCoroutine(RepeatingVibration());
    }

    public void StopVibrate()
    {
        Gamepad.current.SetMotorSpeeds(_power, _power);
        if (_repeatVibrationRoutine != null)
            StopCoroutine(RepeatingVibration());
    }

    private IEnumerator RepeatingVibration()
    {
        while (true)
        {
            Gamepad.current.SetMotorSpeeds(_power, _power);
            yield return new WaitForSeconds(1);
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