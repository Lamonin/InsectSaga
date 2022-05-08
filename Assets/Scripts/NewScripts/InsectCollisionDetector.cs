using UnityEngine;

namespace InsectCharacter
{
    public class InsectCollisionDetector : MonoBehaviour
    {
        [SerializeField] private InsectPlayerMediator mediator;
        [Space] [SerializeField] private float _groundCheckRadius;
        [SerializeField] private Vector2 _groundCheckPos;
        [SerializeField] private LayerMask _groundLayer;
        public LayerMask groundLayer => _groundLayer;

        public bool isOnGround { get; private set; }

        private void Awake()
        {
            isOnGround = false;
        }

        private void Start()
        {

        }

        private void FixedUpdate()
        {
            isOnGround = Physics2D.OverlapCircle(mediator.pos + _groundCheckPos, _groundCheckRadius, _groundLayer);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + (Vector3) _groundCheckPos, _groundCheckRadius);
        }
    }
}
