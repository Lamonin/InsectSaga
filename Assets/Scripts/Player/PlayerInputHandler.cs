using UnityEngine;

namespace Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        protected bool IsCharacterStopped;
        protected InputScheme _input;
        
        private void DialogueStart(bool stopped)
        {
            IsCharacterStopped = stopped;
        }

        private void DialogueEnd()
        {
            IsCharacterStopped = false;
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
        
        protected UsableObject _usableObject;

        protected virtual void InteractWithUsableObject()
        {
            if (_usableObject != null) 
                _usableObject.Interact();
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Usable"))
            {
                _usableObject = other.GetComponent<UsableObject>();
                GameUI.ShowUseIcon(_usableObject.useIconPosition);
            }
        }
        
        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Usable"))
            {
                _usableObject.Deactivate();
                _usableObject = null;
            }
        }
    }
}