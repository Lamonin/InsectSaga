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

        [Header("Другое")]
        public LayerMask groundLayer;
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
        
        public Rigidbody2D GetRigidBody2D => rb2d;

        //PROPERTIES
        [HideInInspector] public bool isGround;

        private void UpdateMoveSpeed()
        {
            if (moveDir != 0)
                _moveSpeed = Mathf.Sign(moveDir) * (Mathf.Abs(moveDir) <= stickOffsetBeforeRun ? walkSpeed : runSpeed);
            else
                _moveSpeed = 0;
        }

        protected virtual void Move()
        {
            UpdateMoveSpeed();
            
            if (isGround || _moveSpeed != 0)
            {
                rb2d.velocity = new Vector2(_moveSpeed, rb2d.velocity.y);
            }
            else
            {
                var v2 = rb2d.velocity;
                v2.x *= 0.95f;
                rb2d.velocity = v2;
            }
        }

        public virtual void Jump()
        {
            if (!isCanJump) return;
            
            _jumpCounter = jumpBufferTime;
            if (!isGround && _hangCounter <= 0) return;
            _hangCounter = -1;
            _jumpCounter = -1;
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpPower);
        }

        private void ManageHangAndJumpBufferTimer()
        {
            if (isGround)
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
        }
        
        protected virtual void Update()
        {
            isGround = Physics2D.OverlapCircle((Vector2) transform.position + groundCheckPos, groundCheckRadius, groundLayer);
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