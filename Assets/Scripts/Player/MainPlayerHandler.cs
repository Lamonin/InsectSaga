using UnityEngine;
using Objects;
using Controllers;
using UnityEngine.SceneManagement;

namespace Player
{
    public class MainPlayerHandler : PlayerInputHandler
    {
        public InsectController chController;

        private bool _ceilRay, _floorRay, _lWallRay, _rWallRay;
        
        protected override void Awake()
        {
            base.Awake();
            
            InputScheme.Player.Jump.performed += _ =>
            {
                if (isCharacterStopped ||!isCanJump) return;
                chController.Jump();
            };
            
            InputScheme.Player.RunModeOn.performed += _ =>
            {
                if (isCharacterStopped || !isCanCrawl) return;
                chController.tryToCrawl = true;
            };
            
            InputScheme.Player.RunModeOff.performed += _ =>
            {
                if (isCharacterStopped) return;
                chController.ToNormalState();
            };
            
            InputScheme.Player.Using.performed += _ => { InteractWithUsableObject(); };
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EventBus.OnPlayerRespawned += RespawnPlayer;
            chController.OnStateChanged += UpdateUseIcon;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EventBus.OnPlayerRespawned -= RespawnPlayer;
            chController.OnStateChanged -= UpdateUseIcon;
        }

        private void RespawnPlayer()
        {
            InputScheme.Enable();
            chController.ResetState();
        }
        
        private void Start()
        {
        }

        private bool CastRay(Vector2 dir, float distance)
        {
            return Physics2D.Raycast(transform.position, dir, distance, chController.groundLayer);
        }

        private float GetPlayerInputDirection()
        {
            var inputDir = InputScheme.Player.Movement.ReadValue<Vector2>();

            #region CorrectPlayerInputDirection
            
            float moveDir = chController.chSide switch
            {
                GroundSide.Floor => inputDir.x,
                GroundSide.Ceil => -inputDir.x,
                GroundSide.LWall => -inputDir.y,
                GroundSide.RWall => inputDir.y,
                _ => chController.moveDir
            };

            float rot = transform.rotation.eulerAngles.z;

            if (rot > 5 && rot < 85 && inputDir.x < 0 && inputDir.y > 0)
                moveDir = 0;
            else if (rot > 95 && rot < 175 && inputDir.x < 0 && inputDir.y < 0)
                moveDir = 0;
            else if (rot > 185 && rot < 265 && inputDir.x > 0 && inputDir.y < 0)
                moveDir = 0;
            else if (rot > 275 && rot < 355 && inputDir.x > 0 && inputDir.y > 0)
                moveDir = 0;

            _rWallRay = CastRay(Vector2.right, 0.5f);
            _lWallRay = CastRay(Vector2.left, 0.5f);
            _ceilRay = CastRay(Vector2.up, 0.5f);
            _floorRay = CastRay(Vector2.down, 0.5f);

            switch (chController.chSide)
            {
                case GroundSide.Floor:
                    if (_rWallRay && inputDir.x > 0 && inputDir.y < 0 || _lWallRay && inputDir.x < 0 && inputDir.y < 0)
                        moveDir = 0;
                    break;

                case GroundSide.Ceil:
                    if (_rWallRay && inputDir.x > 0 && inputDir.y > 0 || _lWallRay && inputDir.x < 0 && inputDir.y > 0)
                        moveDir = 0;
                    break;

                case GroundSide.LWall:
                    if (_floorRay && inputDir.x < 0 && inputDir.y < 0 || _ceilRay && inputDir.x < 0 && inputDir.y > 0)
                        moveDir = 0;
                    break;

                case GroundSide.RWall:
                    if (_floorRay && inputDir.x > 0 && inputDir.y < 0 || _ceilRay && inputDir.x > 0 && inputDir.y > 0)
                        moveDir = 0;
                    break;
            }

            #endregion
            
            if (!isCanRun)
            {
                moveDir = Mathf.Clamp(moveDir, -chController.stickOffsetBeforeRun, chController.stickOffsetBeforeRun);
            }

            return moveDir;
        }

        private void Update()
        {
            chController.moveDir = isCharacterStopped ? 0 : GetPlayerInputDirection();
        }

        protected override void InteractWithUsableObject()
        {
            if (chController.State != ChState.Normal) return;
            base.InteractWithUsableObject();
        }

        private void UpdateUseIcon(ChState newState)
        {
            if (UsableObject is null) return;
            if (newState == ChState.Normal)
            {
                GameUI.ShowUseIcon(UsableObject);
            }
            else
            {
                GameUI.HideUseIcon();
            }
        }

        public void ActivateLift(Transform lift)
        {
            InputScheme.Disable();
            chController.State = ChState.Lift;
            transform.SetParent(lift);
            transform.localPosition = new Vector3(0, 0.525f, 0);
            chController.GetRigidBody2D.gravityScale = 0;
            chController.GetRigidBody2D.velocity = Vector2.zero;
        }

        public void FreeFromEvevator()
        {
            InputScheme.Enable();
            transform.SetParent(null);
            chController.ToNormalState();
        }

        private void ResetPlayerStateInvoke()
        {
            chController.ToNormalState();
            chController.tryToCrawl = false;
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                if (BlackSplashImage.Handler != null)
                {
                    InputScheme.Disable();
                    
                    Invoke(nameof(ResetPlayerStateInvoke), 0.1f);
                    
                    // BlackSplashImage.Handler.FadeIn(0.2f);
                    BlackSplashImage.Handler.FlashFadeIn();
                    if (LevelManager.Handler == null)
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                    EventBus.OnPlayerDiedEvent?.Invoke();
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
            else if (other.CompareTag("Usable"))
            {
                UsableObject = other.GetComponent<UsableObject>();
                if (chController.State == ChState.Normal)
                {
                    GameUI.ShowUseIcon(UsableObject);
                }
            }
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Usable"))
            {
                if (UsableObject is null) return;
                UsableObject.Deactivate();
                UsableObject = null;
            }
        }
    }
}