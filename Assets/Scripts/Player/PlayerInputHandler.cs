using Objects;
using UnityEngine;

namespace Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public bool isCharacterStopped;
        protected InputScheme _input;
        
        private void DialogueStart(bool stopped)
        {
            isCharacterStopped = stopped;
        }

        private void DialogueEnd()
        {
            isCharacterStopped = false;
        }

        protected virtual void Awake()
        {
            _input = new InputScheme();
        }

        protected virtual void OnEnable()
        {
            _input.Enable();
            EventBus.OnDialogueStart += DialogueStart;
            EventBus.OnDialogueEnd += DialogueEnd;
        }

        protected virtual void OnDisable()
        {
            _input.Disable();
            EventBus.OnDialogueStart -= DialogueStart;
            EventBus.OnDialogueEnd -= DialogueEnd;
        }
        
        protected UsableObject UsableObject;

        protected virtual void InteractWithUsableObject()
        {
            if (UsableObject != null) 
                UsableObject.Interact();
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Usable"))
            {
                UsableObject = other.GetComponent<UsableObject>();
                GameUI.ShowUseIcon(UsableObject);
            }
        }
        
        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Usable"))
            {
                if (UsableObject is null) return;
                UsableObject.Deactivate();
                UsableObject = null;
            }
        }
    }
}