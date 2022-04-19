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
            
            InputScheme.Player.Jump.performed += context =>
            {
                if (isCharacterStopped) return;
                chController.Jump();
            };
            
            InputScheme.Player.Using.performed += context => { InteractWithUsableObject(); };
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (isCharacterStopped)
            {
                chController.moveDir = 0;    
                return;
            }
            chController.moveDir = InputScheme.Player.Movement.ReadValue<Vector2>().x;
        }
    }
}