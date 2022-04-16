using UnityEngine;

namespace Objects
{
    public class MultipleDialogs : UsableObject
    {
        private int _currentDialog;
        private DialogObject[] _dialogObjects;

        protected override void Start()
        {
            _dialogObjects = transform.GetComponentsInChildren<DialogObject>();
            base.Start();
        }

        public override void Interact()
        {
            _dialogObjects[_currentDialog].Interact();
        }

        public override void Deactivate()
        {
            _dialogObjects[_currentDialog].Deactivate();
            base.Deactivate();
        }

        public void SetCurrentDialog(int curDialog) => _currentDialog = curDialog;
    }
}