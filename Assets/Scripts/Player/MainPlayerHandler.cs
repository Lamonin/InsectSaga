using Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class MainPlayerHandler : MonoBehaviour
    {
        public delegate void Interaction();
        public Interaction interactAction = null;

        public InsectController chController;
    
        private InputScheme _input;

        private bool _ceilRay, _floorRay, _leftWallRay, _rightWallRay;

        private void Awake()
        {
            _input = new InputScheme();

            _input.Player.Jump.performed += context => { chController.Jump(); };
            _input.Player.RunModeOn.performed += context => { chController.tryToCrawl = true; };
            _input.Player.RunModeOff.performed += context => { chController.ToNormalState(); };
            _input.Player.Using.performed += context => { if (interactAction != null && chController.state != InsectController.CharacterStates.Crawl) interactAction(); };
        }
    
        private void OnEnable() { _input.Enable(); }
    
        private void OnDisable() { _input.Disable(); }

        private void Start()
        {
        }

        private bool CastRay(Vector2 dir, float distance)
        {
            return Physics2D.Raycast(transform.position, dir, distance, chController.groundLayer);
        }

        private void Update()
        {
            Vector2 inputDir = _input.Player.Movement.ReadValue<Vector2>();

            #region CorrectPlayerInputDirection

            float moveDir = chController.chSide switch
            {
                InsectController.GroundSide.Floor => inputDir.x,
                InsectController.GroundSide.Ceil => -inputDir.x,
                InsectController.GroundSide.LWall => -inputDir.y,
                InsectController.GroundSide.RWall => inputDir.y,
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
                case InsectController.GroundSide.Floor:
                    if (_rightWallRay && inputDir.x > 0 && inputDir.y < 0 || _leftWallRay && inputDir.x < 0 && inputDir.y < 0)
                        moveDir = 0;
                    break;
            
                case InsectController.GroundSide.Ceil:
                    if (_rightWallRay && inputDir.x > 0 && inputDir.y > 0 || _leftWallRay && inputDir.x < 0 && inputDir.y > 0)
                        moveDir = 0;
                    break;
            
                case InsectController.GroundSide.LWall:
                    if (_floorRay && inputDir.x < 0 && inputDir.y < 0 || _ceilRay && inputDir.x < 0 && inputDir.y > 0)
                        moveDir = 0;
                    break;
            
                case InsectController.GroundSide.RWall:
                    if (_floorRay && inputDir.x > 0 && inputDir.y < 0 || _ceilRay && inputDir.x > 0 && inputDir.y > 0)
                        moveDir = 0;
                    break;
            }


            #endregion
        
            chController.moveDir = moveDir;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag("Enemy")) return;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
