using System;
using UnityEngine;

namespace Player
{
    public class CollisionDetector : MonoBehaviour
    {
        [SerializeField]
        private PlayerScript playerScript;
        
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Vector2 checkGroundPos;
        [SerializeField] private float checkGroundSize;
        
        

        private bool CheckGround()
        {
            return Physics2D.OverlapCircle(checkGroundPos, checkGroundSize, groundLayer);
        }
        
        private void Update()
        {
            playerScript.isGround = CheckGround();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(checkGroundPos, checkGroundSize);
        }
    }
}