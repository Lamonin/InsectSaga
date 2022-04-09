using UnityEngine;

public class FrameBasedAnimator
{
    private Animator _animator;
    private string _currentState;

    public FrameBasedAnimator(Animator animator)
    {
        _animator = animator;
    }

    public void ChangeAnimation(string name)
    {
        if (_currentState == name && GetCurrentAnimatorState().loop) return;
        _animator.Play(name);
        _currentState = name;
    }

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
