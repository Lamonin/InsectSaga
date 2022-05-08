using System;
using UnityEngine;

namespace InsectCharacter
{
    public class InsectSpriteController : MonoBehaviour
    {
        [SerializeField] private InsectPlayerMediator mediator;
        [Space] [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private Animator _animator;
        public SpriteRenderer sprite => _sprite;

        private FrameBasedAnimator _frameAnimator;

        #region ANIMATIONS_DESCRIPTION

        private readonly int IDLE = Animator.StringToHash("idle");
        private readonly int WALK = Animator.StringToHash("walk");
        private readonly int RUN = Animator.StringToHash("run");
        private readonly int CRAWL = Animator.StringToHash("crawl");
        private readonly int IDLE_CRAWL = Animator.StringToHash("idle_crawl");
        private readonly int JUMP = Animator.StringToHash("jump");
        private readonly int FALL = Animator.StringToHash("fall");
        private readonly int GROUND = Animator.StringToHash("ground");

        #endregion

        private void Awake()
        {
            _frameAnimator = new FrameBasedAnimator(_animator);
        }

        private void UpdateAnimation()
        {
            var velocity = mediator.controller.body.velocity;
            if (!mediator.detector.isOnGround)
            {
                var velY = velocity.y;
                if (velY < -1f)
                {
                    _frameAnimator.ChangeAnimation(FALL);
                }
                else if (velY > 1f)
                {
                    _frameAnimator.ChangeAnimation(JUMP);
                }

                return;
            }
            
            if (mediator.controller.moveDir != 0 && Mathf.Abs(velocity.x) > 0.1f)
            {
                switch (mediator.inputHandler.moveState)
                {
                    case MoveStateEnum.Walk:
                        _frameAnimator.ChangeAnimation(WALK);
                        break;
                    case MoveStateEnum.Run:
                        _frameAnimator.ChangeAnimation(RUN);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                _frameAnimator.ChangeAnimation(IDLE);
            }
        }

        private void Update()
        {
            UpdateAnimation();
            if (mediator.controller.moveDir < 0)
            {
                _sprite.flipX = true;
            }
            else if (mediator.controller.moveDir > 0)
            {
                _sprite.flipX = false;
            }
        }
    }
}