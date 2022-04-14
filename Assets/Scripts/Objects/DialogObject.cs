using System;
using System.Collections;
using UnityEngine;

namespace Objects
{
    public class DialogObject : UsableObject
    {
        public bool canSkipDialogs;
        public bool stopPlayerInDialog;
        public float delayToTypeSymbol = 0.05f;
        public float delayBeforeStartDialogAgain = 0.1f;
        [TextArea] public string[] dialogMessages;

        //VARIABLES
        private int _currentMessageNumber;
        private string _message;
        private Coroutine _messageTypeRoutine;
        private Coroutine _delayBeforeNextMessage;

        protected override void OnDestroy()
        {
            if (_messageTypeRoutine != null)
                StopCoroutine(_messageTypeRoutine);
            if (_delayBeforeNextMessage != null)
                StopCoroutine(_delayBeforeNextMessage);
            base.OnDestroy();
        }

        public override void Interact()
        {
            if (_isTyping && !canSkipDialogs || _isDelayed) return;
            
            if (_currentMessageNumber == 0)
            {
                GameUI.Handler.dialogText.gameObject.SetActive(true);
            }

            if (_isTyping)
            {
                StopCoroutine(_messageTypeRoutine);
                _isTyping = false;
                GameUI.Handler.dialogText.text = _message;
            }
            else if (_currentMessageNumber < dialogMessages.Length)
            {
                _message = dialogMessages[_currentMessageNumber];
                GameUI.Handler.dialogText.text = String.Empty;
                _messageTypeRoutine = StartCoroutine(StartMessageSymbolTyping());
                _currentMessageNumber++;
            }
            else //END DIALOG
            {
                GameUI.Handler.dialogText.gameObject.SetActive(false);
                _currentMessageNumber = 0;
                _delayBeforeNextMessage = StartCoroutine(DelayBeforeNextDialog());
            }
        }

        private bool _isTyping;
        private IEnumerator StartMessageSymbolTyping()
        {
            if (String.IsNullOrEmpty(_message)) yield break;
            
            _isTyping = true;
            foreach (var ch in _message)
            {
                GameUI.Handler.dialogText.text += ch;
                yield return new WaitForSeconds(delayToTypeSymbol);
            }

            _isTyping = false;
        }

        private bool _isDelayed;
        private IEnumerator DelayBeforeNextDialog()
        {
            _isDelayed = true;
            yield return new WaitForSeconds(delayBeforeStartDialogAgain);
            _isDelayed = false;
        }

        public override void Deactivate()
        {
            GameUI.Handler.dialogText.gameObject.SetActive(false);
            _currentMessageNumber = 0;
            base.Deactivate();
        }
    }
}