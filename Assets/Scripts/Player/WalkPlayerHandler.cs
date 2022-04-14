using Controllers;
using UnityEngine;

namespace Player
{
    public class WalkPlayerHandler : MonoBehaviour
    {
        public delegate void Interaction();
        public Interaction interactAction = null;

        public PlatformerController chController;
    
        private InputScheme _input;

        private void Use()
        {
            Debug.Log("Try to use smth!");
        }
        

        private void Awake()
        {
            _input = new InputScheme();

            _input.Player.Jump.performed += context => { chController.Jump(); };
            _input.Player.Using.performed += context => { Use(); };
        }
    
        private void OnEnable() { _input.Enable(); }
    
        private void OnDisable() { _input.Disable(); }

        private void Start()
        {
        }
        
        private void Update()
        {
            Vector2 inputDir = _input.Player.Movement.ReadValue<Vector2>();
            chController.moveDir = inputDir.x;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {

        }
    }
}