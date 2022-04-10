using System.Collections;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerCharacter : MonoBehaviour
{
    [Header("Обычный режим")] 
    [Tooltip("Скорость при ходьбе")] public float walkSpeed;
    [Tooltip("Скорость при беге")] public float runSpeed;
    [Tooltip("Сила прыжка")] public float jumpPower;
    [Tooltip("Время для прыжка Койота")] public float hangTime;
    [Tooltip("Насколько отклонить стик для перехода в режим бега")] 
    [Range(0.01f, 1)] public float stickOffsetBeforeRun;
    
    [Header("Режим ползанья")]
    [Tooltip("Скорость ползанья")] public float crawlSpeed;
    [Tooltip("Скорость поворота на углах")] public float rotationSpeed;
    [Tooltip("Дистанция прицепления к стене")] public float grabWallDistance;
    [Tooltip("Дистанция прицепления к полу")] public float grabGroundDistance;
    [Tooltip("Сила прыжка от стены")] public float jumpCrawlFromWallPower;
    [Tooltip("Сила прыжка")] public float jumpCrawlPower;

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
    private int _crawlDir = 1;
    private bool _flip;
    private bool _isGround;
    private bool _isJumpInCrawl;
    private float _hangCounter;
    private float _moveSpeed;
    private float _distanceToWall;

    private Rigidbody2D _rb2d;
    private FrameBasedAnimator _frameAnimator;

    protected void ToCrawlState()
    {
        if (state == CharacterStates.Crawl || _isJumpInCrawl) return;
        var isOnWall = CheckRunWall();
        bool isOnWallSecond = CheckRunWallSecond();
        if (!CheckRunGround() && !isOnWall && !isOnWallSecond) return;
        
        if (isOnWall) Debug.Log("isOnWall");
        if (isOnWallSecond) Debug.Log("isOnWallSecond");
        if (CheckRunGround()) Debug.Log("isOnGround");

        Quaternion angleRot = Quaternion.Euler(0,0,0);
        
        if (isOnWall.collider != null)
        {
            if (isOnWall.normal == Vector2.right)
            {
                angleRot = Quaternion.Euler(0,0,-90);
            }
            else if (isOnWall.normal == Vector2.left)
            {
                angleRot = Quaternion.Euler(0,0,90);
            }
        }

        //angleRot = isOnWall || isOnWallSecond ? Quaternion.Euler(0, 0, 90 * _crawlDir) : Quaternion.Euler(0, 0, 0);

        if (state != CharacterStates.JumpCrawl || !isOnWallSecond)
        {
            collTransform.localRotation = Quaternion.Euler(0, 0, -90);
            transform.rotation = angleRot;
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
        if (Mathf.Abs(Mathf.Abs(180-transform.rotation.eulerAngles.z)-90)<45) //OnWall
        {
            _rb2d.AddForce((transform.up * 1.2f+transform.right*_crawlDir*0.5f) * jumpCrawlFromWallPower, ForceMode2D.Impulse);
            yield return new WaitForFixedUpdate();
            var rotAngle = transform.rotation.eulerAngles.z > 180 ? 90 : -90;
            transform.rotation = Quaternion.Euler(0,0,0);
            transform.Rotate(0,0, rotAngle);
            //_rb2d.rotation = rotAngle;
            _flip = _crawlDir == 1;
            spriteRenderer.flipX = _flip;
            _crawlDir = _crawlDir == 1 ? -1 : 1;
        }
        else
        {
            _rb2d.AddForce((transform.up * 1.2f + transform.right * _crawlDir) * jumpCrawlPower, ForceMode2D.Impulse);
        }
        yield return new WaitForSeconds(0.1f);
        _isJumpInCrawl = false;
    }
    
    protected void ToNormalState()
    {
        TryToRun = false;
        if (state == CharacterStates.Normal) return;
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
    
    protected void Jump()
    {
        if (state == CharacterStates.Normal)
        {
            if (!_isGround && _hangCounter <= 0) return;
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
        else _moveSpeed = 0;
    }
    
    private RaycastHit2D rayGround, rayWall;
    private void Move()
    {
        if (state == CharacterStates.JumpCrawl)
        {
            return;
        }
        if (state == CharacterStates.Normal)
        {
            UpdateMoveSpeed();
            _rb2d.velocity = new Vector2(_moveSpeed, _rb2d.velocity.y);
        }
        else if (state == CharacterStates.Crawl)
        {
            var transformUp = transform.up;
            var transformRight = transform.right * _crawlDir;
            
            if (MoveDir != 0)
            {
                rayGround = Physics2D.Raycast(transform.position, transformRight, _distanceToWall, groundLayer);
                rayWall = Physics2D.Raycast(transform.position + transformRight * 0.5f, -transformUp, rayGroundLength,
                    groundLayer);

                if (rayWall.collider is null)
                {
                    _rb2d.MoveRotation(_rb2d.rotation + (_flip ? rotationSpeed : -rotationSpeed));
                }
                else if (rayGround.collider != null)
                {
                    _rb2d.MoveRotation(_rb2d.rotation + (_flip ? -rotationSpeed : rotationSpeed));
                }
            }
            
            
            Vector2 pos = _rb2d.position - (Vector2) transformUp * Time.fixedDeltaTime*2;
            if (MoveDir != 0) 
                pos += (Vector2) (transformRight * crawlSpeed * Time.fixedDeltaTime);
            _rb2d.MovePosition(pos);
        }
        // else if (state == CharacterStates.JumpCrawl)
        // {
        //     var transformRight = transform.right * _crawlDir;
        //     rayWall = Physics2D.Raycast(transform.position + transformRight * 0.5f, -transform.up, rayGroundLength, groundLayer);
        //
        //     if (rayWall.collider is null)
        //     {
        //         _rb2d.MoveRotation(_rb2d.rotation + (_flip ? rotationSpeed : -rotationSpeed));
        //     }
        // }
    }

    private void UpdatePlayerDirection()
    {
        spriteRenderer.flipX = _flip;
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
    }

    private void DrawDebugRays()
    {
        if (state == CharacterStates.Crawl)
        {
            
        }
        Debug.DrawRay(transform.position, transform.right * _crawlDir * _distanceToWall, Color.red);
        Debug.DrawRay(transform.position + transform.right * _crawlDir * 0.5f, -transform.up * rayGroundLength, Color.red);
        Debug.DrawRay(transform.position, Vector2.down * grabGroundDistance, Color.blue);
        float mult = state != CharacterStates.Normal ? 1.5f : 1;
        Debug.DrawRay(transform.position, transform.right * _crawlDir * grabWallDistance * mult, Color.yellow);
        Debug.DrawRay(transform.position, -transform.up * grabWallDistance, Color.magenta);
    }

    #region ANIMATIONS

    private const string PLAYER_IDLE = "idle";
    private const string PLAYER_WALK = "walk";
    private const string PLAYER_RUN = "run";
    private const string PLAYER_CRAWL = "crawl";

    private void UpdateAnimations()
    {
        if (state == CharacterStates.Crawl || state == CharacterStates.JumpCrawl)
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
        _distanceToWall  = 0.5f+1 / Mathf.Sqrt(2);
        _frameAnimator = new FrameBasedAnimator(animator);
        state = CharacterStates.Normal;
    }
    
    protected virtual void Update()
    {
        _isGround = CheckGround();
        if (_isGround) { _hangCounter = hangTime; }
        else { _hangCounter -= Time.deltaTime; }

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
