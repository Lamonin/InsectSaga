using UnityEngine;

namespace Localize
{
    public class LocalizeDialog : MonoBehaviour
    {
        [SerializeField] private string dialogId;
        [SerializeField] private Objects.DialogObject localizeDialog;

        private void Start()
        {
            if (LocalizeManager.Manager.isLocalizeDataLoaded)
                GetMessages();
            else //WAIT, WHEN DATA BEING LOADED
                LocalizeManager.Manager.onLanguageDataLoaded += GetMessages;
        }

        private void GetMessages()
        {
            localizeDialog.dialogMessages = LocalizeManager.Manager.GetLocalizedDialogMessages(dialogId);
            LocalizeManager.Manager.onLanguageDataLoaded -= GetMessages;
        }
    }
}