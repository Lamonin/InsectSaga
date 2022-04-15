using Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class MainPlayerHandler : PlayerInputHandler
    {
        public InsectController chController;

        private bool _ceilRay, _floorRay, _leftWallRay, _rightWallRay;

        protected override void Awake()
        {
            base.Awake();

            _input.Player.Jump.performed += context =>
            {
                if (isCharacterStopped) return;
                chController.Jump();
            };
            
            _input.Player.RunModeOn.performed += context =>
            {
                if (isCharacterStopped) return;
                chController.tryToCrawl = true;
            };
            
            _input.Player.RunModeOff.performed += context =>
            {
                if (isCharacterStopped) return;
                chController.ToNormalState();
            };
            
            _input.Player.Using.performed += context => { InteractWithUsableObject(); };
        }

        private void Start()
        {
            chController.OnStateChanged += UpdateUseIcon;
        }

        private bool CastRay(Vector2 dir, float distance)
        {
            return Physics2D.Raycast(transform.position, dir, distance, chController.groundLayer);
        }

        private float GetPlayerInputDirection()
        {
            Vector2 inputDir = _input.Player.Movement.ReadValue<Vector2>();

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

            _rightWallRay = CastRay(Vector2.right, 0.5f);
            _leftWallRay = CastRay(Vector2.left, 0.5f);
            _ceilRay = CastRay(Vector2.up, 0.5f);
            _floorRay = CastRay(Vector2.down, 0.5f);

            switch (chController.chSide)
            {
                case GroundSide.Floor:
                    if (_rightWallRay && inputDir.x > 0 && inputDir.y < 0 ||
                        _leftWallRay && inputDir.x < 0 && inputDir.y < 0)
                        moveDir = 0;
                    break;

                case GroundSide.Ceil:
                    if (_rightWallRay && inputDir.x > 0 && inputDir.y > 0 ||
                        _leftWallRay && inputDir.x < 0 && inputDir.y > 0)
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

            return moveDir;
        }

        private void Update()
        {
            if (isCharacterStopped) return;
            chController.moveDir = GetPlayerInputDirection();
        }

        protected override void InteractWithUsableObject()
        {
            if (chController.State != CharacterStates.Normal) return;
            base.InteractWithUsableObject();
        }

        private void UpdateUseIcon()
        {
            if (_usableObject is null) return;
            if (chController.State == CharacterStates.Normal)
            {
                GameUI.ShowUseIcon(_usableObject.useIconPosition);
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
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else if (other.CompareTag("Usable"))
            {
                _usableObject = other.GetComponent<UsableObject>();
                if (chController.State == CharacterStates.Normal)
                {
                    GameUI.ShowUseIcon(_usableObject.useIconPosition);
                }
            }
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Usable"))
            {
                _usableObject.Deactivate();
                _usableObject = null;
            }
        }
    }
}