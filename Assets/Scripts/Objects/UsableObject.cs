using UnityEngine;

namespace Objects
{
    public class UsableObject : MonoBehaviour
    {
        public Vector2 useIconPosition;
        [HideInInspector] public Vector2 useIconOffset;

        public bool interactable { get; set; } = true;

        protected virtual void Start()
        {

        }

        protected virtual void OnDestroy()
        {

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

        public void Update()
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
