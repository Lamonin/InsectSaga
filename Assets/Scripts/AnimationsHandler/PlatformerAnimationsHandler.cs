using Controllers;
using UnityEngine;

namespace AnimationsHandler
{
    public class PlatformerAnimationsHandler : MonoBehaviour
    {
        public PlatformerController chController;
        public Animator animator;
        private FrameBasedAnimator _frameAnimator;

        private readonly int IDLE = Animator.StringToHash("idle");
        private readonly int WALK = Animator.StringToHash("walk");
        private readonly int RUN = Animator.StringToHash("run");
        private readonly int JUMP = Animator.StringToHash("jump");

        void Start()
        {
            _frameAnimator = new FrameBasedAnimator(animator);
        }

        private void UpdateAnimations()
        {
            if (chController.IsGround)
            {
                if (chController.moveDir != 0 && Mathf.Abs(chController.GetRigidBody2D.velocity.x) > 0.02f)
                {
                    _frameAnimator.ChangeAnimation(Mathf.Abs(chController.moveDir) <= chController.stickOffsetBeforeRun ? WALK : RUN);
                }
                else
                {
                    _frameAnimator.ChangeAnimation(IDLE);
                }
            }
            else
            {
                _frameAnimator.ChangeAnimation(JUMP);
            }
        }

        void Update()
        {
            UpdateAnimations();
        }
    }
}
