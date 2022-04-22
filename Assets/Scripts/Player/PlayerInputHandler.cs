using Objects;
using SaveIsEasy;
using UnityEngine;

namespace Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public bool isCharacterStopped { get; set; }
        [field: SerializeField] public bool isCanRun { get; set; } = true;
        [field: SerializeField] public bool isCanJump { get; set; } =  true;
        [field: SerializeField] public bool isCanCrawl { get; set; } =  true;
        
        [AvoidSaving] protected InputScheme InputScheme;
        
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
            InputScheme = new InputScheme();
        }

        protected virtual void OnEnable()
        {
            EnableInput();

            EventBus.OnDialogueStart += DialogueStart;
            EventBus.OnDialogueEnd += DialogueEnd;
        }

        protected virtual void OnDisable()
        {
            DisableInput();
            
            EventBus.OnDialogueStart -= DialogueStart;
            EventBus.OnDialogueEnd -= DialogueEnd;
        }
        
        public void EnableInput()
        {
            InputScheme.Enable();
        }

        public void DisableInput()
        {
            InputScheme.Disable();
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