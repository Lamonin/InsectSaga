using System.Collections;
using UnityEngine;

public class FrameBasedAnimator
{
    private Animator _animator;
    private int _currentState;
    private bool _freeze;

    public FrameBasedAnimator(Animator animator)
    {
        _animator = animator;
    }
    
    public void ChangeAnimation(int id)
    {
        if (_currentState == id && GetCurrentAnimatorState().loop || _freeze) return;
        _animator.Play(id);
        _currentState = id;
    }

    public void ChangeAnimation(string name)
    {
        ChangeAnimation(Animator.StringToHash(name));
    }

    public IEnumerator ChangeAnimationToEnd(int id)
    {
        if (_freeze) yield break;
        ChangeAnimation(id);
        _freeze = true;
        yield return new WaitForSeconds(GetCurrentAnimatorState().length);
        _freeze = false;
    }

    public void Unfreeze() => _freeze = false;

    public AnimatorStateInfo GetCurrentAnimatorState()
    {
        return _animator.GetCurrentAnimatorStateInfo(0);
    }

    /// <summary>
    /// Returns the duration of the current animation
    /// </summary>
    public float GetLength()
    {
        return GetCurrentAnimatorState().length;
    }

    public int CurrentState => _currentState;
}
