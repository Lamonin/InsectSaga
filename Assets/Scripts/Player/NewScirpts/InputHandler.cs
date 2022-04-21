using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField]
        private PlayerScript playerScript;
        private InputScheme _input;

        private void Awake()
        {
            _input = new InputScheme();

            _input.Player.Jump.performed += _ =>
            {
                if (!playerScript.СanJump) return;
                playerScript.OnJumpEvent?.Invoke();
            };
            
            _input.Player.Using.performed += _ =>
            {
                playerScript.OnUseEvent?.Invoke();
            };
        }

        private void OnEnable()
        {
            _input.Enable();
        }

        private void OnDisable()
        {
            _input.Disable();
        }

        private void Update()
        {
            playerScript.moveInput = _input.Player.Movement.ReadValue<Vector2>();
        }
    }
}