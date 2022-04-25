using UnityEngine;
using DG.Tweening;

public class SoundController : MonoBehaviour
{
    private float _duration = 1f;
    private float _targetVolume = 1f;
    private float _targetPlayingTime;

    public void SetVolumeChangeDuration(float duration) => _duration = duration;
    public void SetTargetVolume(float volume) => _targetVolume = volume;

    public void ChangeVolume(AudioSource source)
    {
        source.DOFade(_targetVolume, _duration);
    }

    public void SetTargetPlayingTime(float time) => _targetPlayingTime = time;

    public void SetPlayingTime(AudioSource source)
    {
        source.time = _targetPlayingTime;
    }
}
