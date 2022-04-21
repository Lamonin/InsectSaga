using System;
using UnityEngine;

namespace Player
{
    public enum MoveTypeEnum { Walk, Run }
    public enum StatesEnum { Normal, Crawl }
    
    public class PlayerScript : MonoBehaviour
    {
        [SerializeField] private bool canJump;
        [SerializeField] private bool canRun;
        
        [SerializeField] private CharacterController characterController;
        [SerializeField] private CollisionDetector collisionDetector;
        [SerializeField] private InputHandler inputHandler;

        [Space] [SerializeField] private Rigidbody2D rb2d;
        [Space] [SerializeField] private Animator animator;
        

        public bool СanJump
        {
            get => canJump;
            set => canJump = value;
        }
        
        public bool СanRun
        {
            get => canRun;
            set => canRun = value;
        }

        //MOVE_TYPE
        private MoveTypeEnum _moveType;
        public MoveTypeEnum MoveType => _moveType;
        
        //STATE
        private StatesEnum _state;
        public StatesEnum State => _state;
        
        [HideInInspector]
        public Vector2 moveInput;
        
        public Action OnJumpEvent;
        public Action OnUseEvent;

        [HideInInspector] public bool isGround;

        public Rigidbody2D GetRigidBody2D => rb2d;

        private void Awake()
        {
            
        }
        
    }
}