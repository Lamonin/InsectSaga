using System.Collections;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerCharacter : MonoBehaviour
{
    [Header("Обычный режим")] 
    [Tooltip("Скорость при ходьбе")]
    public float walkSpeed;
    
    [Tooltip("Скорость при беге")] 
    public float runSpeed;
    
    [Tooltip("Сила прыжка")] 
    public float jumpPower;
    
    [Tooltip("Время для прыжка Койота")] 
    public float hangTime;
    
    [Tooltip("Время для буфера прыжка")] 
    public float jumpBufferTime;
    
    [Tooltip("Насколько отклонить стик для перехода в режим бега")] 
    [Range(0.01f, 1)] public float stickOffsetBeforeRun;
    
    
    [Header("Режим ползанья")]
    [Tooltip("Скорость ползанья")]
    public float crawlSpeed;
    
    [Tooltip("Скорость поворота на углах")]
    public float rotationSpeed;
    
    [Tooltip("Дистанция прицепления к стене")]
    public float grabWallDistance;
    
    [Tooltip("Дистанция прицепления к полу")]
    public float grabGroundDistance;
    
    [Tooltip("Сила прыжка от стены")]
    public float jumpCrawlFromWallPower;
    
    [Tooltip("Сила прыжка")]
    public float jumpCrawlPower;

    [Header("Другие параметры")]
    public LayerMask groundLayer;
    public float groundCheckRadius;
    public float rayGroundLength;

    [Header("Компоненты")]
    public Animator animator;
    public Transform collTransform;
    public Transform groundChecker;
    public SpriteRenderer spriteRenderer;

    public enum CharacterStates { Normal, Crawl, JumpCrawl}

    [Header("Вспомогательное")] 
    public CharacterStates state;

    //VARIABLES
    protected bool TryToRun;
    protected float MoveDir;
    [SerializeField] protected GroundSide CurrentSide;
    
    private int _crawlDir = 1;
    private bool _flip;
    private bool _isGround;
    private bool _isJumpInCrawl;
    private float _moveSpeed;
    private float _hangCounter;
    private float _jumpCounter;
    private float _distanceToWall;

    private Rigidbody2D _rb2d;
    private FrameBasedAnimator _frameAnimator;

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

        Quaternion angleRot = Quaternion.Euler(0,0,0);
        
        if (isOnWall)
        {
            angleRot = Quaternion.FromToRotation(Vector3.up, isOnWall.normal);
        }
        else if (isOnWallSecond)
        {
            angleRot = Quaternion.FromToRotation(Vector3.up, isOnWallSecond.normal);
        }

        collTransform.localRotation = Quaternion.Euler(0, 0, -90);
        if (state == CharacterStates.Normal)
            transform.rotation = angleRot;
        else
        {
            transform.DORotateQuaternion(angleRot, 0.1f).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed);
        }

        _rb2d.constraints = RigidbodyConstraints2D.None;
        _rb2d.gravityScale = 0;
        
        state = CharacterStates.Crawl;
    }

    private IEnumerator ToCrawlJumpState()
    {
        if (state == CharacterStates.JumpCrawl) yield break;
        
        state = CharacterStates.JumpCrawl;
        _isJumpInCrawl = true;
        _rb2d.gravityScale = 4;
        _rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rb2d.velocity = Vector2.zero;

        if (CurrentSide == GroundSide.LWall || CurrentSide == GroundSide.RWall) //InsectOnWall
        {
            //By default values set for LWall
            Vector2 force = new Vector2(1, 0.5f);
            float rotAngle = 90;

            if (CurrentSide == GroundSide.RWall)
            {
                force = new Vector2(-1, 0.5f);
                rotAngle = -90;
            }

            _rb2d.AddForce(force* jumpCrawlFromWallPower, ForceMode2D.Impulse);
            transform.rotation = Quaternion.Euler(0,0,rotAngle);
            _flip = _crawlDir == 1;
            _crawlDir = _flip ? -1 : 1;
            spriteRenderer.flipX = _flip;
        }
        else //JUMP FROM FLOOR or CEIL
        {
            var ceil = CurrentSide == GroundSide.Ceil ? -1 : 1;
            _rb2d.AddForce(new Vector2(_crawlDir*1.2f , 1.2f) * ceil * jumpCrawlPower, ForceMode2D.Impulse);
        }
        
        yield return new WaitForSeconds(0.1f);
        _isJumpInCrawl = false;
    }
    
    protected void ToNormalState()
    {
        TryToRun = false;
        if (state == CharacterStates.Normal) return;
        transform.DOKill();
        StopCoroutine(ToCrawlJumpState());
        
        collTransform.localRotation = Quaternion.Euler(0, 0, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        
        _rb2d.velocity = Vector2.zero; //Reset velocity, otherwise it accumulates
        _rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rb2d.gravityScale = 4;
        
        _isJumpInCrawl = false;
        _frameAnimator.ChangeAnimation(PLAYER_IDLE);
        
        state = CharacterStates.Normal;
    }
    
    protected void TryJump()
    {
        if (state == CharacterStates.Normal)
        {
            _jumpCounter = jumpBufferTime;
            if (!_isGround && _hangCounter <= 0) return;
            _hangCounter = -1;
            _rb2d.velocity = new Vector2(_rb2d.velocity.x, jumpPower);
        }
        else if (state != CharacterStates.JumpCrawl)
        {
            StartCoroutine(ToCrawlJumpState());
        }
    }

    #region RAYCASTS
    private bool CheckGround()
    {
        return Physics2D.OverlapCircle(groundChecker.position, groundCheckRadius, groundLayer);
    }
    
    private RaycastHit2D CheckRunGround()
    {
        // float raySize = Mathf.Clamp(Mathf.Abs(Mathf.Abs(180-transform.rotation.eulerAngles.z)-180)/90.0f, 0.4f, 1);
        // if (state == CharacterStates.Normal) raySize = 1;
        return Physics2D.Raycast(transform.position, Vector2.down, grabGroundDistance, groundLayer);
    }

    private RaycastHit2D CheckRunWall()
    {
        float mult = state != CharacterStates.Normal ? 1.5f : 1;
        return Physics2D.Raycast(transform.position, transform.right*_crawlDir, grabWallDistance*mult, groundLayer);
    }
    
    private RaycastHit2D CheckRunWallSecond()
    {
        return Physics2D.Raycast(transform.position, -transform.up, grabWallDistance, groundLayer);
    }
    #endregion

    private void UpdateMoveSpeed()
    {
        if (MoveDir != 0)
            _moveSpeed = Mathf.Sign(MoveDir) * (Mathf.Abs(MoveDir) < stickOffsetBeforeRun ? walkSpeed : runSpeed);
        else
            _moveSpeed = 0;
    }
    
    private RaycastHit2D rayGround, rayWall;
    private void Move()
    {
        if (state == CharacterStates.JumpCrawl) { return; }
        
        if (state == CharacterStates.Normal)
        {
            UpdateMoveSpeed();
            _rb2d.velocity = new Vector2(_moveSpeed, _rb2d.velocity.y);
        }
        else if (state == CharacterStates.Crawl)
        {
            var tRight = transform.right * _crawlDir;
            var tPos = transform.position;
            var tUp = transform.up;
            
            if (MoveDir != 0)
            {
                rayGround = Physics2D.Raycast(tPos, tRight, _distanceToWall, groundLayer);
                rayWall = Physics2D.Raycast(tPos + tRight * 0.5f, -tUp, rayGroundLength, groundLayer);

                if (!rayWall)
                {
                    _rb2d.MoveRotation(_rb2d.rotation + (_flip ? rotationSpeed : -rotationSpeed));
                }
                else if (rayGround)
                {
                    _rb2d.MoveRotation(_rb2d.rotation + (_flip ? -rotationSpeed : rotationSpeed));
                }
            }

            Vector2 pos = _rb2d.position - (Vector2) tUp * Time.fixedDeltaTime*2; //gravity to surface
            if (MoveDir != 0) 
                pos += (Vector2) (tRight * crawlSpeed * Time.fixedDeltaTime);
            _rb2d.MovePosition(pos);
        }
    }

    protected enum GroundSide { Floor, Ceil, LWall, RWall }
    protected GroundSide GetPlayerGroundSide()
    {
        var tUp = transform.up;

        if (Mathf.Abs(tUp.x) > Mathf.Abs(tUp.y) && Mathf.Abs(tUp.y) < 0.1f)
        {
            return tUp.x > 0 ? GroundSide.LWall : GroundSide.RWall;
        }

        return tUp.y > 0 ? GroundSide.Floor : GroundSide.Ceil;
    }

    private void UpdatePlayerDirection()
    {
        if (state == CharacterStates.JumpCrawl) return;
        if (MoveDir < 0)
        {
            _flip = true;
            _crawlDir = -1;
        }
        else if (MoveDir > 0)
        {
            _flip = false;
            _crawlDir = 1;
        }
        spriteRenderer.flipX = _flip;
    }

    private void DrawDebugRays()
    {
        Debug.DrawRay(transform.position, transform.right * _crawlDir * _distanceToWall, Color.red);
        Debug.DrawRay(transform.position + transform.right * _crawlDir * 0.5f, -transform.up * rayGroundLength, Color.red);
        
        float raySize = Mathf.Clamp(Mathf.Abs(Mathf.Abs(180-transform.rotation.eulerAngles.z)-180)/90.0f, 0.4f, 1);
        if (state == CharacterStates.Normal) raySize = 1;
        Debug.DrawRay(transform.position, Vector2.down * grabGroundDistance*raySize, Color.blue);
        float mult = state != CharacterStates.Normal ? 1.5f : 1;
        Debug.DrawRay(transform.position, transform.right * _crawlDir * grabWallDistance * mult, Color.yellow);
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
        else if (_isGround && state == CharacterStates.Normal)
        {
            if (MoveDir != 0)
            {
                _frameAnimator.ChangeAnimation(Mathf.Abs(MoveDir) < stickOffsetBeforeRun ? PLAYER_WALK : PLAYER_RUN);
            }
            else
            {
                _frameAnimator.ChangeAnimation(PLAYER_IDLE);
            }
        }
    }

    #endregion

    #region UNITY_BASE
    protected virtual void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _frameAnimator = new FrameBasedAnimator(animator);
        
        _distanceToWall  = 0.2f + 1 / Mathf.Sqrt(2);
        state = CharacterStates.Normal;
    }
    
    protected virtual void Update()
    {
        CurrentSide = GetPlayerGroundSide();
        _isGround = CheckGround();

        //MANAGE JUMP BUFFER TIMER
        if (_isGround && _jumpCounter > 0)
        {
            TryJump();
        }
        else
        {
            _jumpCounter -= Time.deltaTime;
        }
        
        //MANAGE HANG TIMER
        if (_isGround)
            _hangCounter = hangTime;
        else
            _hangCounter -= Time.deltaTime;

        if (TryToRun) ToCrawlState();
        UpdatePlayerDirection();
        UpdateAnimations();
        DrawDebugRays();
    }

    private void FixedUpdate()
    {
        Move();
    }

    #endregion
}
