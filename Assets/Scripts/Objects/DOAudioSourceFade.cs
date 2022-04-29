using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class DOAudioSourceFade : MonoBehaviour {
    [SerializeField] private float timeToFade;  

    void Awake()
    {
        var audio = GetComponent<AudioSource>();
        audio.volume = 0;
        audio.DOFade(1, timeToFade);
    }  
}