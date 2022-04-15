using Controllers;
using UnityEngine;

namespace AnimationsHandler
{
    public class PlatformerAnimationsHandler : MonoBehaviour
    {
        public PlatformerController chController;
        public Animator animator;
        private FrameBasedAnimator _frameAnimator;

        private const string IDLE = "idle";
        private const string WALK = "walk";
    
        void Start()
        {
            _frameAnimator = new FrameBasedAnimator(animator);
        }

        private void UpdateAnimations()
        {
            if (chController.moveDir != 0)
            {
                _frameAnimator.ChangeAnimation(WALK);
            }
            else
            {
                _frameAnimator.ChangeAnimation(IDLE);
            }
        }

        void Update()
        {
            UpdateAnimations();
        }
    }
}
