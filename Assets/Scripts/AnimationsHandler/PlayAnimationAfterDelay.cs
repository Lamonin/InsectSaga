using UnityEngine;

public class PlayAnimationAfterDelay : MonoBehaviour
{
    public float delayFrom;
    public float delayTo;
    public bool randomFromRange;
    public string animationName;

    private float _counter;
    private Animator _animator;
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        if (!randomFromRange) delayFrom = delayTo;
        _counter = Random.Range(delayFrom, delayTo);
    }

    // Update is called once per frame
    void Update()
    {
        if (_counter>0)
        {
            _counter -= Time.deltaTime;
        }
        else if (_counter!=0)
        {
            _counter = 0;
            _animator.Play(animationName);
        }
    }
}
