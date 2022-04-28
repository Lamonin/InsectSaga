using System.Collections.Generic;
using System.Text.RegularExpressions;
using MEC;
using UnityEngine;
using UnityEngine.Events;

namespace Objects
{
    public class DialogObject : UsableObject
    {
        [Space]
        public bool canSkipDialogs;
        public bool stopPlayerInDialog;
        public bool isDialogOnTop;
        [Tooltip("Возможность использовать теги")] public bool richText;
        public float delayToTypeSymbol = 0.02f;
        public float delayBeforeStartDialogAgain = 0.5f;

        [Space]
        [TextArea] public string[] dialogMessages;

        public UnityEvent onDialogueEnd;
        public UnityEvent onDialogueBreak;

        //VARIABLES
        private int _currentMessageNumber;
        private string _message;
        private CoroutineHandle _messageTypeRoutine;
        private CoroutineHandle _delayBeforeNextMessage;
        
        private readonly Regex _delayTagExp = new Regex(@"<delay=\""(.*?)\"">");
        private readonly Regex _delayTagValueExp = new Regex(@"\""(.*)\""");
        
        protected override void Start()
        {
            base.Start();
        }

        protected override void OnDestroy()
        {
            Timing.KillCoroutines(_messageTypeRoutine);
            Timing.KillCoroutines(_delayBeforeNextMessage);
            
            base.OnDestroy();
        }

        public override void Interact()
        {
            if (_isTyping && !canSkipDialogs || _isDelayed) return;
            
            if (_currentMessageNumber == 0) //START DIALOG
            {
                EventBus.OnDialogueStart?.Invoke(stopPlayerInDialog, isDialogOnTop);
            }

            if (_isTyping) //SKIP MESSAGE
            {
                Timing.KillCoroutines(_messageTypeRoutine);
                _isTyping = false;
                _message = _delayTagExp.Replace(_message, "");

                GameUI.Handler.dialogText.text = _message;
            }
            else if (_currentMessageNumber < dialogMessages.Length) //NEXT MESSAGE
            {
                GameUI.Handler.dialogText.text = string.Empty;
                _message = dialogMessages[_currentMessageNumber];
                _messageTypeRoutine = Timing.RunCoroutine(MessageSymbolTyping());
                _currentMessageNumber++;
            }
            else //END DIALOG
            {
                EventBus.OnDialogueEnd?.Invoke();
                onDialogueEnd?.Invoke();
                _currentMessageNumber = 0;
                _delayBeforeNextMessage = Timing.RunCoroutine(DelayBeforeNextDialog());
            }
        }

        private bool _isTyping;
        private string _tag;
        private IEnumerator<float> MessageSymbolTyping()
        {
            if (string.IsNullOrEmpty(_message)) yield break;
            
            _isTyping = true;
            _tag = string.Empty;
            foreach (var ch in _message)
            {
                if (richText)
                {
                    if (ch == '>')
                    {
                        if (_tag.Contains("delay"))
                        {
                            var delay = int.Parse(_delayTagValueExp.Match(_tag).Groups[1].Value);
                            _tag = string.Empty;
                            yield return Timing.WaitForSeconds(delay/1000f);
                            continue;
                        }
                        
                        GameUI.Handler.dialogText.text += _tag + ch;
                        _tag = string.Empty;
                        continue;
                    }
                    
                    if (_tag.Length>0 || ch == '<')
                    {
                        _tag += ch;
                        continue;
                    }
                }

                GameUI.Handler.dialogText.text += ch;
                yield return Timing.WaitForSeconds(delayToTypeSymbol);
            }
            _isTyping = false;
        }

        private bool _isDelayed;
        private IEnumerator<float> DelayBeforeNextDialog()
        {
            _isDelayed = true;
            yield return Timing.WaitForSeconds(delayBeforeStartDialogAgain);
            _isDelayed = false;
        }

        public override void Deactivate()
        {
            Timing.KillCoroutines(_messageTypeRoutine);
            Timing.KillCoroutines(_delayBeforeNextMessage);
            GameUI.Handler.dialogText.gameObject.SetActive(false);
            _currentMessageNumber = 0;

            onDialogueBreak?.Invoke();

            base.Deactivate();
        }
    }
}