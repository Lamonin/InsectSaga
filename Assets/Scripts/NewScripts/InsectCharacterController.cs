using System;
using UnityEngine;

namespace InsectCharacter
{
    public enum CharacterStateEnum {Normal, Crawl}
    
    public class InsectCharacterController : MonoBehaviour
    {
        [SerializeField] private InsectPlayerMediator mediator;

        [Space] [Header("Options")] 
        [SerializeField] private float walkSpeed = 2;
        [SerializeField] private float runSpeed = 4;
        [SerializeField] private float crawlSpeed = 5;
        [SerializeField] private float hangTime = 0.05f;
        [SerializeField] private float jumpBufferTime = 0.1f;

        [SerializeField] private float jumpPower = 10;

        [Header("Components")] [SerializeField]
        private Rigidbody2D _rb2d;

        public float moveDir { get; set; }

        public CharacterStateEnum chState { get; private set; } = CharacterStateEnum.Normal;
        public bool tryGoToCrawling { get; set; }
        public Rigidbody2D body => _rb2d;
        
        private float _hangCounter;
        private float _jumpCounter;

        private void Start()
        {

        }

        #region NORMAL_STATE
        
        public void ToNormalState()
        {
            tryGoToCrawling = false;
            
            if (chState == CharacterStateEnum.Normal) return;
            Debug.Log("IN NORMAL STATE");
            chState = CharacterStateEnum.Normal;
        }
        
        private void NormalJump()
        {
            _jumpCounter = jumpBufferTime;
            
            if (!mediator.detector.isOnGround && _hangCounter <= 0)
                return;
            
            _hangCounter = _jumpCounter = 0;
            
            _rb2d.velocity = new Vector2(_rb2d.velocity.x, jumpPower);
        }

        private void NormalMove()
        {
            moveDir = mediator.inputHandler.moveVector.x;
            if (moveDir != 0) 
                moveDir = Mathf.Sign(moveDir); //Set moveDir to -1 or 1
            
            if (moveDir != 0)
            {
                var speed = mediator.inputHandler.moveState == MoveStateEnum.Run ? runSpeed : walkSpeed;
                _rb2d.velocity = new Vector2(speed * moveDir, _rb2d.velocity.y);
            }
            else if (mediator.detector.isOnGround) //IMMEDIATELY STOP ON GROUND
            {
                _rb2d.velocity = new Vector2(0, _rb2d.velocity.y);
            }
            else //SLOW DOWN SPEED IN AIR
            {
                var velocity = _rb2d.velocity;
                velocity = new Vector2(velocity.x * 0.98f, velocity.y);
                _rb2d.velocity = velocity;
            }
        }

        private void NormalUpdate()
        {
            if (mediator.detector.isOnGround)
            {
                _hangCounter = hangTime;
                if (_jumpCounter > 0) NormalJump();
            }
            else
            {
                _hangCounter -= Time.deltaTime;
                if (_hangCounter < 0) _hangCounter = 0;
                
                _jumpCounter -= Time.deltaTime;
                if (_jumpCounter < 0) _jumpCounter = 0;
            }
        }
        
        #endregion

        
        
        #region CRAWL_STATE

        public void ToCrawlState()
        {
            tryGoToCrawling = true;
            
            if (chState == CharacterStateEnum.Crawl) return;
            Debug.Log("IN CRAWL STATE");
            chState = CharacterStateEnum.Crawl;
        }

        private void CrawlJump()
        {
            
        }

        private void CrawlMove()
        {
            
        }

        private void CrawlUpdate()
        {
            
        }

        #endregion

        public void Jump()
        {
            if (chState == CharacterStateEnum.Normal)
            {
                NormalJump();
            }
            else //CRAWL_STATE
            {
                CrawlJump();
            }
        }

        private void Update()
        {
            if (chState == CharacterStateEnum.Normal)
            {
                NormalUpdate();
            }
            else //CRAWL_STATE
            {
                
            }
        }

        private void FixedUpdate()
        {
            if (chState == CharacterStateEnum.Normal)
            {
                NormalMove();
            }
            else //CRAWL_STATE
            {
                CrawlMove();
            }
        }
    }
}
