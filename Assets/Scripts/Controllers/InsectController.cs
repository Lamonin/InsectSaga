using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Controllers
{
    //ENUMS
    public enum ChState { Normal, Crawl, CrawlJump, Lift }
    public enum GroundSide { Floor, Ceil, LWall, RWall }
    
    public class InsectController : PlatformerController
    {
        public float crawlSpeed;
        public float rotationSpeed;
        public float grabWallDistance;
        public float grabGroundDistance;
        public float jumpCrawlPower;
        public float jumpCrawlFromWallPower;
        public float rayGroundLength;
        public LayerMask groundLayer;

        public Animator animator;
        public Transform collTransform;
        public GameObject platformerColl;

        //VARIABLES
        public bool tryToCrawl;
        private int _crawlDir = 1;
        private bool _flip;
        private bool _isNowRotated;
        private bool _isJumpInCrawl;
        private float _distanceToWall;
        private float _savedGravityScale;
        private Coroutine _groundAnimRoutine;
        private Coroutine _jumpInCrawlRoutine;
        private FrameBasedAnimator _frameAnimator;
        public event Action<ChState> OnStateChanged;
        
        public GroundSide chSide = GroundSide.Floor;
        //Properties
        private ChState _state = ChState.Normal;
        public ChState State
        {
            get => _state;
            set
            {
                if (_groundAnimRoutine != null)
                {
                    StopCoroutine(_groundAnimRoutine);
                    _frameAnimator.Unfreeze();
                }
                _state = value;
                OnStateChanged?.Invoke(_state);
            }
        }

        private void ToCrawlState()
        {
            if (_state == ChState.Crawl || _isJumpInCrawl) return;
            
            var isOnWall = CheckRunWall();
            var isOnWallSecond = CheckRunWallSecond();
            var isOnGround = CheckRunGround();
            
            
            if (!isOnGround && !isOnWall && !(isOnWallSecond&&CheckRunWallSecond(true))) return;

            /*if (isOnWall) Debug.Log("isOnWall");
            if (isOnWallSecond) Debug.Log("isOnWallSecond");
            if (isOnGround) Debug.Log("isOnGround");*/
            
            platformerColl.SetActive(false);
            collTransform.gameObject.SetActive(true);

            rb2d.freezeRotation = false;
            rb2d.angularVelocity = 0;
            rb2d.gravityScale = 0;
            rb2d.velocity = Vector2.zero; //Reset velocity, otherwise it accumulates
            
            Quaternion angleRot = Quaternion.Euler(0, 0, 0);

            if (isOnWall)
                angleRot = Quaternion.FromToRotation(Vector3.up, isOnWall.normal);
            else if (isOnWallSecond)
                angleRot = Quaternion.FromToRotation(Vector3.up, isOnWallSecond.normal);

            collTransform.localRotation = Quaternion.Euler(0, 0, -90);
            angleRot = Quaternion.Euler(0, 0, angleRot.eulerAngles.z);
            transform.DOKill();

            
            if (_state == ChState.Normal)
            {
                transform.rotation = angleRot;
            }
            else
            {
                _isNowRotated = true;
                transform.DORotateQuaternion(angleRot, 0.1f).SetEase(Ease.OutSine).SetUpdate(UpdateType.Fixed)
                .OnComplete(() => _isNowRotated = false).OnKill(() => _isNowRotated = false);
            }

            State = ChState.Crawl;
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
            if (_state == ChState.CrawlJump) yield break;

            State = ChState.CrawlJump;
            rb2d.gravityScale = _savedGravityScale;
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
                rb2d.AddForce(new Vector2(_crawlDir * 1.2f, 1.2f) * (ceil * jumpCrawlPower), ForceMode2D.Impulse);
                if (ceil == -1) //Flips character if he jumps from the ceiling
                    FlipAndRotateCharacter(0);
                else //RESET ROTATION ANGLE FOR BEST LOOK
                    transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            _isJumpInCrawl = true;
            yield return new WaitForSeconds(0.1f);
            _isJumpInCrawl = false;
        }

        public void ToNormalState()
        {
            tryToCrawl = false;
            if (State == ChState.Normal) return;
            if (_jumpInCrawlRoutine != null)
                StopCoroutine(_jumpInCrawlRoutine);
            
            transform.DOKill();
            platformerColl.SetActive(true);

            collTransform.localRotation = Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            
            rb2d.freezeRotation = true;
            rb2d.gravityScale = _savedGravityScale;

            collTransform.gameObject.SetActive(false);
            _isNowRotated = false;
            _isJumpInCrawl = false;
            _frameAnimator.ChangeAnimation(IDLE);

            State = ChState.Normal;
        }

        public override void Jump()
        {
            if (State == ChState.Normal)
            {
                base.Jump();
            }
            else if (State != ChState.CrawlJump && !_isNowRotated)
            {
                _jumpInCrawlRoutine = StartCoroutine(ToCrawlJumpState());
            }
        }

        #region RAYCASTS

        private RaycastHit2D CheckRunGround()
        {
            var dist = grabGroundDistance;
            
            //Need to cling to the ground later
            //if (_state == ChState.CrawlJump && chSide == GroundSide.Floor) dist /= 1.5f;
            if (_state == ChState.CrawlJump && Mathf.Abs(WrapAngle(transform.localEulerAngles.z)) < 6) dist /= 1.5f;
            return Physics2D.Raycast(transform.position, Vector2.down, dist, groundLayer);
        }

        private RaycastHit2D CheckRunWall()
        {
            float mult = _state != ChState.Normal ? 1.6f : 1;
            return Physics2D.Raycast(transform.position, Vector2.right * _crawlDir, grabWallDistance * mult,
                groundLayer);
        }

        private RaycastHit2D CheckRunWallSecond(bool secondRay = false)
        {
            if (secondRay)
                return Physics2D.Raycast(transform.position+transform.right * (-_crawlDir * 0.2f), -transform.up, grabWallDistance/1.2f, groundLayer);
            return Physics2D.Raycast(transform.position, -transform.up, grabWallDistance, groundLayer);
        }

        #endregion

        private RaycastHit2D _rayGround, _rayWall;

        protected override void Move()
        {
            if (_state == ChState.CrawlJump) return;

            if (_state == ChState.Normal)
            {
                base.Move();
            }
            else if (_state == ChState.Crawl)
            {
                var tRight = transform.right * _crawlDir;
                var tPos = transform.position;
                var tUp = transform.up;

                if (moveDir != 0)
                {
                    _rayGround = Physics2D.Raycast(tPos + tRight * 0.5f, -tUp, rayGroundLength, groundLayer);
                    _rayWall = Physics2D.Raycast(tPos, tRight, _distanceToWall, groundLayer);

                    if (!_rayGround)
                    {
                        rb2d.MoveRotation(rb2d.rotation + (_flip ? rotationSpeed : -rotationSpeed));
                    }
                    else if (_rayWall)
                    {
                        rb2d.MoveRotation(rb2d.rotation + (_flip ? -rotationSpeed : rotationSpeed));
                    }
                }
                
                var pos = rb2d.position - (Vector2) tUp * (Time.fixedDeltaTime * 2); //gravity to surface
                if (moveDir != 0)
                    pos += (Vector2) (tRight * (crawlSpeed * Time.fixedDeltaTime));
                rb2d.MovePosition(pos);
            }
        }
        
        private float WrapAngle(float angle)
        {
            angle %= 360;
            if(angle > 180)
                return angle - 360;
 
            return angle;
        }
        
        private float WrapAngleAbs(float angle)
        {
            return Mathf.Abs(WrapAngle(angle));
        }

        protected GroundSide GetPlayerGroundSide()
        {
            var rot = WrapAngle(transform.localEulerAngles.z);
            
            if (MathF.Abs(rot) > 120)
                return GroundSide.Ceil;
            if (MathF.Abs(rot) < 60)
                return GroundSide.Floor;

            return rot < 0 ? GroundSide.LWall : GroundSide.RWall;
        }

        public void ResetState()
        {
            rb2d.gravityScale = _savedGravityScale;
            rb2d.velocity = Vector2.zero;
            ToNormalState();
        }

        #region ANIMATIONS

        private readonly int IDLE = Animator.StringToHash("idle");
        private readonly int WALK = Animator.StringToHash("walk");
        private readonly int RUN = Animator.StringToHash("run");
        private readonly int CRAWL = Animator.StringToHash("crawl");
        private readonly int IDLE_CRAWL = Animator.StringToHash("idle_crawl");
        private readonly int JUMP = Animator.StringToHash("jump");
        private readonly int FALL = Animator.StringToHash("fall");
        private readonly int GROUND = Animator.StringToHash("ground");

        private void UpdateAnimations()
        {
            switch (State)
            {
                case ChState.Normal:
                    if (isGround)
                    {
                        if (moveDir != 0 && Mathf.Abs(rb2d.velocity.x) > 0.02f)
                            _frameAnimator.ChangeAnimation(Mathf.Abs(moveDir) <= stickOffsetBeforeRun ? WALK : RUN);
                        else
                        {
                            _frameAnimator.ChangeAnimation(IDLE);
                        }
                    }
                    else
                    {
                        float velY = rb2d.velocity.y;
                        if (velY > 2f) _frameAnimator.ChangeAnimation(JUMP);
                        else if (velY < -2f) _frameAnimator.ChangeAnimation(FALL);
                    }
                    
                    break;
                
                case ChState.Crawl:
                    _frameAnimator.ChangeAnimation(moveDir != 0 ? CRAWL : IDLE_CRAWL);
                    break;
                
                case ChState.CrawlJump:
                    _frameAnimator.ChangeAnimation(IDLE_CRAWL);
                    break;
                
                case ChState.Lift:
                    _frameAnimator.ChangeAnimation(IDLE);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GroundedAnim()
        {
            if (_state == ChState.Normal && _frameAnimator.CurrentState != GROUND && rb2d.velocity.y < -9)
            {
                //PLAY_GROUNDED_ANIM
                _groundAnimRoutine = StartCoroutine(_frameAnimator.ChangeAnimationToEnd(GROUND));
            }
        }

        #endregion

        private void UpdatePlayerDirection()
        {
            if (_state == ChState.CrawlJump) return;
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
        private void PrecalculateRotation()
        {
            var dir = rb2d.velocity.normalized;
            if (dir.y > 0)
            {
                dir.y = 0;
                dir = dir.normalized;
            }

            var ray = Physics2D.Raycast(transform.position, dir, 3, groundLayer);
            var rayFloor = Physics2D.Raycast(transform.position, Vector2.down, 3, groundLayer);

            if (ray || rayFloor)
            {
                Quaternion rot;
                if (Vector2.Distance(transform.position, ray.point) < Vector2.Distance(transform.position, rayFloor.point))
                {
                    rot = Quaternion.LookRotation(transform.forward, ray.normal);
                }
                else
                {
                    rot = Quaternion.LookRotation(transform.forward, rayFloor.normal);
                }

                transform.rotation = Quaternion.Slerp(transform.rotation, rot, 5*Time.fixedDeltaTime);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _savedGravityScale = rb2d.gravityScale;
            _frameAnimator = new FrameBasedAnimator(animator);
            _distanceToWall = 0.2f + 1 / Mathf.Sqrt(2);
        }

        protected override void Start()
        {
            base.Start();
            OnGroundedEvent += GroundedAnim;
        }

        private void OnDestroy() {
            OnGroundedEvent -= GroundedAnim;
        }

        protected override void Update()
        {
            if (tryToCrawl) ToCrawlState();
            chSide = GetPlayerGroundSide();
            UpdateAnimations();
            if (_state != ChState.CrawlJump)
            {
                UpdatePlayerDirection();
                base.Update();
            }
        }


        protected override void FixedUpdate()
        {
            if (rb2d.velocity.y<-18) rb2d.velocity = new Vector2(rb2d.velocity.x, -18);
            if (_state == ChState.CrawlJump) PrecalculateRotation();
            ManageGround();
            Move();
        }

        protected override void OnDrawGizmosSelected()
        {
            var dist = grabGroundDistance;
            if (_state == ChState.CrawlJump && Mathf.Abs(transform.rotation.eulerAngles.z) < 6) dist /= 1.5f;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, Vector2.down*dist);

            Gizmos.color = Color.magenta;
            float mult = _state != ChState.Normal ? 1.6f : 1;
            Gizmos.DrawRay(transform.position, transform.right * _crawlDir*grabWallDistance*mult);
            
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, -transform.up*grabWallDistance);
            
            Gizmos.color = Color.red;
            //Physics2D.Raycast(tPos + tRight * 0.5f, -tUp, moveDir != 0 ? rayGroundLength:rayGroundLength*2, groundLayer);
            Gizmos.DrawRay(transform.position+transform.right*_crawlDir*0.3f, -transform.up*rayGroundLength);

            if (_state == ChState.CrawlJump)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transform.position, rb2d.velocity.normalized*3);
            }

            base.OnDrawGizmosSelected();
        }
    }
}