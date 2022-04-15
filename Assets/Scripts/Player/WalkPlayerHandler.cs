using Controllers;
using UnityEngine;

namespace Player
{
    public class WalkPlayerHandler : PlayerInputHandler
    {
        public PlatformerController chController;

        protected override void Awake()
        {
            base.Awake();
            
            _input.Player.Jump.performed += context =>
            {
                if (isCharacterStopped) return;
                chController.Jump();
            };
            
            _input.Player.Using.performed += context => { InteractWithUsableObject(); };
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (isCharacterStopped) return;
            chController.moveDir = _input.Player.Movement.ReadValue<Vector2>().x;
        }
    }
}