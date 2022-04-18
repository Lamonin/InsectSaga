using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

namespace Objects
{
    public class DialogObject : UsableObject
    {
        [Space]
        public bool canSkipDialogs;
        public bool stopPlayerInDialog;
        [Tooltip("Возможность использовать теги")] public bool richText;
        public float delayToTypeSymbol = 0.02f;
        public float delayBeforeStartDialogAgain = 0.5f;

        [Space]
        [TextArea] public string[] dialogMessages;

        public UnityEvent onDialogueEnd;

        //VARIABLES
        private int _currentMessageNumber;
        private string _message;
        private Coroutine _messageTypeRoutine;
        private Coroutine _delayBeforeNextMessage;
        
        protected override void Start()
        {
            base.Start();
        }

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
            
            if (_currentMessageNumber == 0) //START DIALOG
            {
                //GameUI.Handler.dialogText.gameObject.SetActive(true);
                EventBus.OnDialogueStart?.Invoke(stopPlayerInDialog);
            }

            if (_isTyping) //SKIP MESSAGE
            {
                StopCoroutine(_messageTypeRoutine);
                _isTyping = false;
                GameUI.Handler.dialogText.text = _message;
            }
            else if (_currentMessageNumber < dialogMessages.Length) //NEXT MESSAGE
            {
                GameUI.Handler.dialogText.text = String.Empty;
                _message = dialogMessages[_currentMessageNumber];
                _messageTypeRoutine = StartCoroutine(MessageSymbolTyping());
                _currentMessageNumber++;
            }
            else //END DIALOG
            {
                //GameUI.Handler.dialogText.gameObject.SetActive(false);
                EventBus.OnDialogueEnd?.Invoke();
                onDialogueEnd?.Invoke();
                _currentMessageNumber = 0;
                _delayBeforeNextMessage = StartCoroutine(DelayBeforeNextDialog());
            }
        }

        private bool _isTyping;
        private string _tag;
        readonly Regex _regExp = new Regex(@"\""(.*)\""");
        private IEnumerator MessageSymbolTyping()
        {
            if (String.IsNullOrEmpty(_message)) yield break;
            
            _isTyping = true;
            _tag = String.Empty;
            foreach (var ch in _message)
            {
                if (richText)
                {
                    if (ch == '>')
                    {
                        if (_tag.Contains("delay"))
                        {
                            var delay = int.Parse(_regExp.Match(_tag).Groups[1].Value);
                            _tag = String.Empty;
                            yield return new WaitForSeconds(delay/1000f);
                            continue;
                        }
                        GameUI.Handler.dialogText.text += _tag;
                        _tag = String.Empty;
                    }
                    
                    if (_tag.Length>0 || ch == '<')
                    {
                        _tag += ch;
                        continue;
                    }
                }
                
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