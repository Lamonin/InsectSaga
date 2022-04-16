using Player;
using UnityEngine;

namespace Objects
{
    public class UsableObject : MonoBehaviour
    {
        public Vector2 useIconPosition;
        [HideInInspector] public Vector2 useIconOffset;

        public bool interactable = true;
        
        public static PlayerInputHandler playerHandler;
        private static int _usableObjectsCount;
    
        protected virtual void Start()
        {
            _usableObjectsCount++;

            playerHandler ??= FindObjectOfType<PlayerInputHandler>(true);
        }

        protected virtual void OnDestroy()
        {
            _usableObjectsCount--;
            if (_usableObjectsCount == 0)
                playerHandler = null;
        }

        public virtual void Interact()
        {
            Debug.Log("Было взаимодействие!");
        }

        public virtual void Deactivate()
        {
            GameUI.HideUseIcon();
        }
        
        public void SetInteractable(bool b) => interactable = b;

        public void LateUpdate()
        {
            useIconOffset = (Vector2) transform.position + useIconPosition;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere((Vector2) transform.position + useIconPosition, 0.1f);
        }
    }
}
