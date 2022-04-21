using System;
using UnityEngine;

namespace Player
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField]
        private PlayerScript playerScript;

        [SerializeField] private float walkSpeed = 2;
        [SerializeField] private float runSpeed = 4;
        [Range(0.01f, 1)]
        [SerializeField] private float stickOffsetBeforeRun = 0.5f;

        [SerializeField] private float crawlSpeed = 5;

        private Rigidbody2D _rb2d;
        private float _moveSpeed;
        private float _moveDir;
        
        #region MOVEMENT

        private void Jump()
        {
            
        }
        
        private bool _ceilRay, _floorRay, _lWallRay, _rWallRay;
        // private bool CastRay(Vector2 dir, float distance)
        // {
        //     return Physics2D.Raycast(transform.position, dir, distance, chController.groundLayer);
        // }

        private float RecalculateMoveInput(float moveDir)
        {
            // float moveDir = chController.chSide switch
            // {
            //     GroundSide.Floor => inputDir.x,
            //     GroundSide.Ceil => -inputDir.x,
            //     GroundSide.LWall => -inputDir.y,
            //     GroundSide.RWall => inputDir.y,
            //     _ => chController.moveDir
            // };
            //
            // float rot = transform.rotation.eulerAngles.z;
            //
            // if (rot > 5 && rot < 85 && inputDir.x < 0 && inputDir.y > 0)
            //     moveDir = 0;
            // else if (rot > 95 && rot < 175 && inputDir.x < 0 && inputDir.y < 0)
            //     moveDir = 0;
            // else if (rot > 185 && rot < 265 && inputDir.x > 0 && inputDir.y < 0)
            //     moveDir = 0;
            // else if (rot > 275 && rot < 355 && inputDir.x > 0 && inputDir.y > 0)
            //     moveDir = 0;
            //
            // _rWallRay = CastRay(Vector2.right, 0.5f);
            // _lWallRay = CastRay(Vector2.left, 0.5f);
            // _ceilRay = CastRay(Vector2.up, 0.5f);
            // _floorRay = CastRay(Vector2.down, 0.5f);
            //
            // switch (chController.chSide)
            // {
            //     case GroundSide.Floor:
            //         if (_rWallRay && inputDir.x > 0 && inputDir.y < 0 || _lWallRay && inputDir.x < 0 && inputDir.y < 0)
            //             moveDir = 0;
            //         break;
            //
            //     case GroundSide.Ceil:
            //         if (_rWallRay && inputDir.x > 0 && inputDir.y > 0 || _lWallRay && inputDir.x < 0 && inputDir.y > 0)
            //             moveDir = 0;
            //         break;
            //
            //     case GroundSide.LWall:
            //         if (_floorRay && inputDir.x < 0 && inputDir.y < 0 || _ceilRay && inputDir.x < 0 && inputDir.y > 0)
            //             moveDir = 0;
            //         break;
            //
            //     case GroundSide.RWall:
            //         if (_floorRay && inputDir.x > 0 && inputDir.y < 0 || _ceilRay && inputDir.x > 0 && inputDir.y > 0)
            //             moveDir = 0;
            //         break;
            // }
            
            return moveDir;
        }

        private float CalculateMoveSpeed()
        {
            if (playerScript.State == StatesEnum.Normal) return crawlSpeed;
            return _moveDir <= stickOffsetBeforeRun ? walkSpeed : runSpeed;
        }

        private void Move()
        {
            if (playerScript.State == StatesEnum.Normal)
            {
                _rb2d.velocity = new Vector2(_moveSpeed * _moveDir, _rb2d.velocity.y);
            }
            else if (playerScript.State == StatesEnum.Crawl)
            {
                
            }
        }
        
        #endregion

        #region UNITY_BASE
        private void OnEnable()
        {
            playerScript.OnJumpEvent += Jump;
        }

        private void OnDisable()
        {
            playerScript.OnJumpEvent -= Jump;
        }

        private void Start()
        {
            _rb2d = playerScript.GetRigidBody2D;
        }

        private void Update()
        {
            _moveDir = playerScript.moveInput.x;
        }

        private void FixedUpdate()
        {
            
            _moveSpeed = CalculateMoveSpeed();
            
        }

        #endregion
    }
}