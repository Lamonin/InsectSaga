using UnityEngine;

namespace Objects
{
    public class MultipleDialogs : UsableObject
    {
        public int currentDialog;
        private DialogObject[] _dialogObjects;

        protected override void Start()
        {
            _dialogObjects = transform.GetComponentsInChildren<DialogObject>();
            base.Start();
        }

        public override void Interact()
        {
            _dialogObjects[currentDialog].Interact();
        }

        public override void Deactivate()
        {
            _dialogObjects[currentDialog].Deactivate();
            base.Deactivate();
        }

        public void SetCurrentDialog(int curDialog) => currentDialog = curDialog;
    }
}