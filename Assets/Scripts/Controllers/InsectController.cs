using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Controllers
{
    public class InsectController : PlatformerController
    {
        [Header("Ползанье параметры")] [Tooltip("Скорость ползанья")]
        public float crawlSpeed;

        [Tooltip("Скорость поворота на углах")]
        public float rotationSpeed;

        [Tooltip("Дистанция прицепления к стене")]
        public float grabWallDistance;

        [Tooltip("Дистанция прицепления к полу")]
        public float grabGroundDistance;

        [Tooltip("Сила прыжка от стены")] public float jumpCrawlFromWallPower;
        [Tooltip("Сила прыжка")] public float jumpCrawlPower;
        public float rayGroundLength;

        [Header("Компоненты")]
        public Animator animator;
        public Transform collTransform;

        //ENUMS
        public enum CharacterStates { Normal, Crawl, JumpCrawl }

        public enum GroundSide { Floor, Ceil, LWall, RWall }

        [Header("Вспомогательное")] public CharacterStates state;
        public GroundSide chSide;

        //VARIABLES
        [HideInInspector] public bool tryToCrawl;
        private bool _flip;
        private bool _isJumpInCrawl;
        private bool _isNowRotated;
        private FrameBasedAnimator _frameAnimator;
        private float _distanceToWall;
        private int _crawlDir;

        private void ToCrawlState()
        {
            if (state == CharacterStates.Crawl || _isJumpInCrawl) return;
            var isOnWall = CheckRunWall();
            var isOnWallSecond = CheckRunWallSecond();
            var isOnGround = CheckRunGround();
            if (!isOnGround && !isOnWall && !isOnWallSecond) return;

            /*if (isOnWall) Debug.Log("isOnWall");
            if (isOnWallSecond) Debug.Log("isOnWallSecond");
            if (isOnGround) Debug.Log("isOnGround");*/

            Quaternion angleRot = Quaternion.Euler(0, 0, 0);

            if (isOnWall)
                angleRot = Quaternion.FromToRotation(Vector3.up, isOnWall.normal);
            else if (isOnWallSecond)
                angleRot = Quaternion.FromToRotation(Vector3.up, isOnWallSecond.normal);

            collTransform.localRotation = Quaternion.Euler(0, 0, -90);
            angleRot = Quaternion.Euler(0, 0, angleRot.eulerAngles.z);

            
            if (state == CharacterStates.Normal)
            {
                transform.rotation = angleRot;
            }
            else
            {
                _isNowRotated = true;
                transform.DORotateQuaternion(angleRot, 0.1f).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed)
                .OnComplete(() => _isNowRotated = false);
            }

            state = CharacterStates.Crawl;
            rb2d.constraints = RigidbodyConstraints2D.None;
            rb2d.gravityScale = 0;
            rb2d.velocity = Vector2.zero; //Reset velocity, otherwise it accumulates
        }

        private void FlipAndRotateCharacter(float rotAngle)
        {
            transform.rotation = Quaternion.Euler(0, 0, rotAngle);
            _flip = _crawlDir == 1;
            _crawlDir = _flip ? -1 : 1;
            sprite.flipX = _flip;
        }

        private IEnumerator ToCrawlJumpState()
        {
            if (state == CharacterStates.JumpCrawl || _isJumpInCrawl) yield break;

            state = CharacterStates.JumpCrawl;
            _isJumpInCrawl = true;
            rb2d.gravityScale = 4;
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb2d.velocity = Vector2.zero;
            transform.DOKill();

            if (chSide == GroundSide.LWall || chSide == GroundSide.RWall || chSide == GroundSide.Ceil && moveDir == 0)
            {
                //By default values set for LWall
                Vector2 force = new Vector2(1, 0.5f);
                float rotAngle = 90;

                if (chSide == GroundSide.RWall)
                {
                    force = new Vector2(-1, 0.5f);
                    rotAngle = -90;
                }
                else if (chSide == GroundSide.Ceil)
                {
                    force = Vector2.down;
                    rotAngle = 0;
                }

                rb2d.AddForce(force * jumpCrawlFromWallPower, ForceMode2D.Impulse);
                FlipAndRotateCharacter(rotAngle);
            }
            else //JUMP FROM FLOOR or CEIL
            {
                var ceil = chSide == GroundSide.Ceil ? -1 : 1;
                rb2d.AddForce(new Vector2(_crawlDir * 1.2f, 1.2f) * ceil * jumpCrawlPower, ForceMode2D.Impulse);
                if (ceil == -1) //Flips character if he jumps from the ceiling
                    FlipAndRotateCharacter(0);
                else //RESET ROTATION ANGLE FOR BEST LOOK
                    transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            yield return new WaitForSeconds(0.1f);
            _isJumpInCrawl = false;
        }

        public void ToNormalState()
        {
            tryToCrawl = false;
            if (state == CharacterStates.Normal) return;

            transform.DOKill();
            StopCoroutine(ToCrawlJumpState());

            collTransform.localRotation = Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.Euler(0, 0, 0);

            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb2d.gravityScale = 4;

            _isNowRotated = false;
            _isJumpInCrawl = false;
            _frameAnimator.ChangeAnimation(PLAYER_IDLE);

            state = CharacterStates.Normal;
        }

        public override void Jump()
        {
            if (state == CharacterStates.Normal)
            {
                base.Jump();
            }
            else if (state != CharacterStates.JumpCrawl && !_isNowRotated)
            {
                StopCoroutine(ToCrawlJumpState());
                StartCoroutine(ToCrawlJumpState());
            }
        }

        #region RAYCASTS

        private RaycastHit2D CheckRunGround()
        {
            return Physics2D.Raycast(transform.position, Vector2.down, grabGroundDistance, groundLayer);
        }

        private RaycastHit2D CheckRunWall()
        {
            float mult = state != CharacterStates.Normal ? 1.5f : 1;
            return Physics2D.Raycast(transform.position, transform.right * _crawlDir, grabWallDistance * mult,
                groundLayer);
        }

        private RaycastHit2D CheckRunWallSecond()
        {
            return Physics2D.Raycast(transform.position, -transform.up, grabWallDistance, groundLayer);
        }

        #endregion

        private RaycastHit2D _rayGround, _rayWall;

        public override void Move()
        {
            if (state == CharacterStates.JumpCrawl) return;

            if (state == CharacterStates.Normal)
            {
                base.Move();
            }
            else if (state == CharacterStates.Crawl)
            {
                var tRight = transform.right * _crawlDir;
                var tPos = transform.position;
                var tUp = transform.up;

                if (moveDir != 0)
                {
                    _rayGround = Physics2D.Raycast(tPos, tRight, _distanceToWall, groundLayer);
                    _rayWall = Physics2D.Raycast(tPos + tRight * 0.5f, -tUp, rayGroundLength, groundLayer);

                    if (!_rayWall)
                    {
                        rb2d.MoveRotation(rb2d.rotation + (_flip ? rotationSpeed : -rotationSpeed));
                    }
                    else if (_rayGround)
                    {
                        rb2d.MoveRotation(rb2d.rotation + (_flip ? -rotationSpeed : rotationSpeed));
                    }
                }

                var pos = rb2d.position - (Vector2) tUp * Time.fixedDeltaTime * 2; //gravity to surface
                if (moveDir != 0)
                    pos += (Vector2) (tRight * crawlSpeed * Time.fixedDeltaTime);
                rb2d.MovePosition(pos);
            }
        }

        protected GroundSide GetPlayerGroundSide()
        {
            var tUp = transform.up;

            if (Mathf.Abs(tUp.x) > Mathf.Abs(tUp.y) && Mathf.Abs(tUp.y) < 0.1f)
                return tUp.x > 0 ? GroundSide.LWall : GroundSide.RWall;

            if (Mathf.Abs(tUp.x) < 0.1f)
                return tUp.y > 0 ? GroundSide.Floor : GroundSide.Ceil;

            return chSide;
        }

        #region ANIMATIONS

        private const string PLAYER_IDLE = "idle";
        private const string PLAYER_WALK = "walk";
        private const string PLAYER_RUN = "run";
        private const string PLAYER_CRAWL = "crawl";

        private void UpdateAnimations()
        {
            if (state != CharacterStates.Normal)
            {
                _frameAnimator.ChangeAnimation(PLAYER_CRAWL);
            }
            else if (IsGround && state == CharacterStates.Normal)
            {
                if (moveDir != 0)
                {
                    _frameAnimator.ChangeAnimation(Mathf.Abs(moveDir) < stickOffsetBeforeRun ? PLAYER_WALK : PLAYER_RUN);
                }
                else
                {
                    _frameAnimator.ChangeAnimation(PLAYER_IDLE);
                }
            }
        }

        #endregion

        private void UpdatePlayerDirection()
        {
            if (state == CharacterStates.JumpCrawl) return;
            if (moveDir < 0)
            {
                _flip = true;
                _crawlDir = -1;
            }
            else if (moveDir > 0)
            {
                _flip = false;
                _crawlDir = 1;
            }
        }

        protected override void Start()
        {
            base.Start();
            _frameAnimator = new FrameBasedAnimator(animator);

            _distanceToWall = 0.2f + 1 / Mathf.Sqrt(2);
            state = CharacterStates.Normal;
            chSide = GroundSide.Floor;
        }

        protected override void Update()
        {
            if (tryToCrawl) ToCrawlState();
            UpdatePlayerDirection();
            UpdateAnimations();
            chSide = GetPlayerGroundSide();
            base.Update();
        }

        protected override void FixedUpdate()
        {
            Move();
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
        }
    }
}