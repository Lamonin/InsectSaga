using UnityEngine;
using Objects;
using Controllers;

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
            chController.OnStateChanged += UpdateUseIcon;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            chController.OnStateChanged -= UpdateUseIcon;
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
            if (isCharacterStopped)
            {
                chController.moveDir = 0;
                return;
            }
            
            chController.moveDir = GetPlayerInputDirection();
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

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                EventBus.OnPlayerDiedEvent?.Invoke();
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