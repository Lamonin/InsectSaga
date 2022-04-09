using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerCharacter : MonoBehaviour
{
    [Header("Обычный режим")] 
    [Tooltip("Скорость при ходьбе")] public float walkSpeed;
    [Tooltip("Скорость при беге")] public float runSpeed;
    [Tooltip("Сила прыжка")] public float jumpPower;
    public float hangTime;
    [Tooltip("Насколько отклонить стик для перехода в режим бега")] 
    [Range(0.01f, 1)] public float stickOffsetBeforeRun;
    
    [Header("Режим ползанья")]
    [Tooltip("Скорость ползанья")] public float crawlSpeed;
    [Tooltip("Скорость поворота на углах")] public float rotationSpeed;
    [Tooltip("Дистанция прицепления к стене")] public float grabWallDistance;
    [Tooltip("Дистанция прицепления к полу")] public float grabGroundDistance;

    [Header("Другие параметры")]
    public LayerMask groundLayer;
    public float groundCheckRadius;
    public float rayGroundLength;

    [Header("Компоненты")]
    public Animator animator;
    public Transform collTransform;
    public Transform groundChecker;
    public SpriteRenderer spriteRenderer;

    [Header("Вспомогательное")]
    [SerializeField] private bool _isCrawl;

    //VARIABLES
    protected bool TryToRun;
    protected float MoveDir;
    private int _crawlDir = 1;
    private bool _flip;
    private bool _isGround;
    private float _hangCounter;
    private float _moveSpeed;
    private float _distanceToWall;

    private Rigidbody2D _rb2d;
    private FrameBasedAnimator _frameAnimator;

    protected void ToCrawlState()
    {
        if (_isCrawl) return;
        if (!CheckRunGround() && !CheckRunWall()) return;

        var angleRot = CheckRunWall() ? Quaternion.Euler(0, 0, 90 * _crawlDir) : Quaternion.Euler(0, 0, 0);

        collTransform.localRotation = Quaternion.Euler(0, 0, -90);
        transform.rotation = angleRot;
        
        _rb2d.constraints = RigidbodyConstraints2D.None;
        _rb2d.gravityScale = 0;
        _isCrawl = true;
    }
    
    protected void ToNormalState()
    {
        TryToRun = false;
        if (!_isCrawl) return;
        collTransform.localRotation = Quaternion.Euler(0, 0, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        _rb2d.velocity = Vector2.zero; //Reset velocity, otherwise it accumulates
        _rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rb2d.gravityScale = 4;
        _isCrawl = false;
        _frameAnimator.ChangeAnimation(PLAYER_IDLE);
    }
    
    protected void Jump()
    {
        if ((!_isGround || _isCrawl) && _hangCounter <= 0) return;
        _rb2d.velocity = new Vector2(_rb2d.velocity.x, jumpPower);
    }

    #region RAYCASTS
    private bool CheckGround()
    {
        return Physics2D.OverlapCircle(groundChecker.position, groundCheckRadius, groundLayer);
    }
    
    private RaycastHit2D CheckRunGround()
    {
        return Physics2D.Raycast(transform.position, -transform.up, grabGroundDistance, groundLayer);
    }

    private RaycastHit2D CheckRunWall()
    {
        return Physics2D.Raycast(transform.position, transform.right*_crawlDir, grabWallDistance, groundLayer);
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
        if (!_isCrawl)
        {
            UpdateMoveSpeed();
            _rb2d.velocity = new Vector2(_moveSpeed, _rb2d.velocity.y);
        }
        else
        {
            var transformUp = transform.up;
            var transformRight = transform.right * _crawlDir;
            
            rayGround = Physics2D.Raycast(transform.position, transformRight, _distanceToWall, groundLayer);
            rayWall = Physics2D.Raycast(transform.position + transformRight*0.5f, -transformUp, rayGroundLength, groundLayer);

            if (rayWall.collider is null)
            {
                _rb2d.MoveRotation(_rb2d.rotation + (_flip ? rotationSpeed : -rotationSpeed));
            }
            else if (rayGround.collider != null)
            {
                _rb2d.MoveRotation(_rb2d.rotation + (_flip ? -rotationSpeed : rotationSpeed));
            }

            Vector2 pos = _rb2d.position + (Vector2) (transformRight * crawlSpeed * Time.fixedDeltaTime - transformUp * Time.fixedDeltaTime*2);
            _rb2d.MovePosition(pos);
        }
    }

    private void UpdatePlayerDirection()
    {
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
        if (_isCrawl)
        {
            Debug.DrawRay(transform.position, transform.right * _crawlDir * _distanceToWall, Color.red);
            Debug.DrawRay(transform.position + transform.right * _crawlDir * 0.5f, -transform.up * rayGroundLength, Color.red);
        }
        Debug.DrawRay(transform.position, transform.right * _crawlDir * grabWallDistance, Color.blue);
        Debug.DrawRay(transform.position, -transform.up * grabGroundDistance, Color.blue);
    }

    #region ANIMATIONS

    private const string PLAYER_IDLE = "idle";
    private const string PLAYER_WALK = "walk";
    private const string PLAYER_RUN = "run";
    private const string PLAYER_CRAWL = "crawl";

    private void UpdateAnimations()
    {
        if (_isGround && !_isCrawl)
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
        else if (_isCrawl)
        {
            _frameAnimator.ChangeAnimation(PLAYER_CRAWL);
        }
    }

    #endregion

    #region UNITY_BASE
    protected virtual void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _distanceToWall  = 0.5f+1 / Mathf.Sqrt(2);
        _frameAnimator = new FrameBasedAnimator(animator);
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
