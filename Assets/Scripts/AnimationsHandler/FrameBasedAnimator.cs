using System.Collections;
using UnityEngine;

public class FrameBasedAnimator
{
    private Animator _animator;
    private string _currentState;
    private bool _freeze;

    public FrameBasedAnimator(Animator animator)
    {
        _animator = animator;
    }

    public void ChangeAnimation(string name)
    {
        if (_currentState == name && GetCurrentAnimatorState().loop || _freeze) return;
        _animator.Play(name);
        _currentState = name;
    }

    public IEnumerator ChangeAnimationToEnd(string name)
    {
        if (_freeze) yield break;
        ChangeAnimation(name);
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

    public string CurrentState => _currentState;
}
