using UnityEngine;

namespace Controllers
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlatformerController : MonoBehaviour
    {
        [Header("Параметры")]
        [Tooltip("Скорость при ходьбе")] public float walkSpeed;
        [Tooltip("Скорость при беге")] public float runSpeed;
        [Tooltip("Насколько отклонить стик для перехода в режим бега")] 
        [Range(0.01f, 1)] public float stickOffsetBeforeRun = 0.1f;

        [Space]
        [Tooltip("Может ли прыгать")] public bool isCanJump = true;
        [Tooltip("Сила прыжка")] public float jumpPower;
        [Tooltip("Время для прыжка Койота")] public float hangTime;
        [Tooltip("Время для буфера прыжка")] public float jumpBufferTime;

        [Header("Дополнительно")]
        public string groundLayerName = "Ground";
        [HideInInspector] public LayerMask groundLayer;
        public Vector2 groundCheckPos;
        public float groundCheckRadius;
        
        [Header("Компоненты")]
        public SpriteRenderer sprite;
        
        //VARIABLES
        [HideInInspector] public float moveDir;
        private float _moveSpeed;
        private float _hangCounter;
        private float _jumpCounter;

        //COMPONENTS
        protected Rigidbody2D rb2d;
        public static bool IsStopped;
        
        //PROPERTIES
        public bool IsGround => Physics2D.OverlapCircle((Vector2) transform.position + groundCheckPos, groundCheckRadius, groundLayer);

        private void UpdateMoveSpeed()
        {
            _moveSpeed = 0;
            if (moveDir != 0)
                _moveSpeed = Mathf.Sign(moveDir) * (Mathf.Abs(moveDir) < stickOffsetBeforeRun ? walkSpeed : runSpeed);
        }

        public virtual void Move()
        {
            UpdateMoveSpeed();
            rb2d.position += Vector2.right * _moveSpeed*Time.fixedDeltaTime;
            
            //SLOW DOWN CHARACTER IMPULSE
            var v2 = rb2d.velocity;
            v2.x *= 0.9f;
            rb2d.velocity = v2;
        }

        public virtual void Jump()
        {
            if (!isCanJump) return;
            
            _jumpCounter = jumpBufferTime;
            if (!IsGround && _hangCounter <= 0) return;
            _hangCounter = -1;
            _jumpCounter = -1;
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpPower);
        }

        private void ManageHangAndJumpBufferTimer()
        {
            if (IsGround)
            {
                _hangCounter = hangTime;
                if (_jumpCounter > 0) Jump();
            }
            else
            {
                _hangCounter -= Time.deltaTime;
                _jumpCounter -= Time.deltaTime;
            }
        }
        
        protected virtual void Start()
        {
            rb2d = GetComponent<Rigidbody2D>();
            groundLayer = 1 << LayerMask.NameToLayer(groundLayerName);
        }
        
        protected virtual void Update()
        {
            if (moveDir != 0) sprite.flipX = moveDir < 0;
            ManageHangAndJumpBufferTimer();
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position+(Vector3) groundCheckPos, groundCheckRadius);
        }

        protected virtual void FixedUpdate()
        {
            Move();
        }
    }
}