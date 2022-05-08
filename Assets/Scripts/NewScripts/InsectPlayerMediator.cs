using UnityEngine;

namespace InsectCharacter
{
    public class InsectPlayerMediator : MonoBehaviour
    {
        public static InsectPlayerMediator Mediator;

        [SerializeField] private InsectCharacterController _controller;
        [SerializeField] private InsectInputHandler _inputHandler;
        [SerializeField] private InsectCollisionDetector _detector;
        [SerializeField] private InsectSpriteController _spriteController;

        public InsectInputHandler inputHandler => _inputHandler;
        public InsectCharacterController controller => _controller;
        public InsectCollisionDetector detector => _detector;
        public InsectSpriteController spriteController => _spriteController;
        public Vector2 pos => transform.position;

        void Awake()
        {

        }

        void Start()
        {

        }

        void Update()
        {

        }

        private void OnDestroy()
        {
            Mediator = null;
        }
    }
}
