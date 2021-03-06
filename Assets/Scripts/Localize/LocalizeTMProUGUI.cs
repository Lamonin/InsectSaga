using TMPro;
using UnityEngine;

namespace Localize
{
    public class LocalizeTMProUGUI : MonoBehaviour
    {
        [SerializeField] private string textId;
        [SerializeField] private TextMeshProUGUI _textComponent;

        private void Start()
        {
            if (LocalizeManager.Manager.isLocalizeDataLoaded)
                GetLocalizedText();
            else //WAIT, WHEN DATA BEING LOADED
                LocalizeManager.Manager.onLanguageDataLoaded += GetLocalizedText;

            LocalizeManager.Manager.onLanguageChanged += GetLocalizedText;
        }

        private void OnDestroy()
        {
            LocalizeManager.Manager.onLanguageChanged -= GetLocalizedText;
        }

        private void GetLocalizedText()
        {
            _textComponent.text = LocalizeManager.Manager.GetLocalizedText(textId);
            LocalizeManager.Manager.onLanguageDataLoaded -= GetLocalizedText;
        }
    }
}