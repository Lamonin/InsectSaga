using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace InsectCharacter
{
    public enum MoveStateEnum { Walk, Run }
    
    public class InsectInputHandler : MonoBehaviour
    {
        [SerializeField] private InsectPlayerMediator mediator;
        [Space] [SerializeField] private float _stickOffsetBeforeRun = 0.5f;

        private InputScheme _input;
        private bool _isInputEnabled;
        private bool _prevInputEnableState;

        public Vector2 moveVector { get; private set; }
        public MoveStateEnum moveState { get; private set; }
        public bool walkModeEnabled { get; private set; }

        private void Awake()
        {
            InitializeInput();
        }

        private void InitializeInput()
        {
            _input = new InputScheme();
            
            _input.Player.Jump.performed += _ => { mediator.controller.Jump(); };
            _input.Player.WalkMode.performed += _ => { walkModeEnabled = !walkModeEnabled; };
            _input.Player.RunModeOn.performed += _ => { mediator.controller.ToCrawlState(); };
            _input.Player.RunModeOff.performed += _ => { mediator.controller.ToNormalState(); };
            _input.Player.RunModeOff.canceled += _ => { mediator.controller.ToNormalState(); };

             var pl = FindObjectOfType<PlayerInput>();
             
            // pl.onControlsChanged += input =>
            // {
            //     if (input.currentControlScheme == _input.MouseandKeyboardScheme.name)
            //     {
            //         Debug.Log("KEYBOARD");
            //     }
            //     else if (input.currentControlScheme == _input.GamepadScheme.name)
            //     {
            //         Debug.Log("GAMEPAD");
            //     }
            // };
        }

        private void OnEnable()
        {
            _input.Enable();
            _isInputEnabled = true;
        }

        private void OnDisable()
        {
            _input.Disable();
            _isInputEnabled = false;
        }

        public void EnableInput(bool force = false)
        {
            if (force)
                _prevInputEnableState = true;

            if (_prevInputEnableState)
            {
                _input.Enable();
                _isInputEnabled = true;
            }
        }

        public void DisableInput()
        {
            _prevInputEnableState = _isInputEnabled;
            _isInputEnabled = false;
            _input.Disable();
        }

        private void Start()
        {

        }

        private void Update()
        {
            moveVector = _input.Player.Movement.ReadValue<Vector2>();
            moveState = Mathf.Abs(moveVector.x) < _stickOffsetBeforeRun ? MoveStateEnum.Walk : MoveStateEnum.Run;
            if (walkModeEnabled) moveState = MoveStateEnum.Walk;
        }
    }
}
