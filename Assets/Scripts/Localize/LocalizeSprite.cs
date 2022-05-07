using UnityEngine;

namespace Localize
{
    public class LocalizeSprite : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer localizeSprite;
        [SerializeField] private Sprite engSprite;
        [SerializeField] private Sprite ruSprite;
        
        private void Start()
        {
            LocalizeManager.Manager.onLanguageChanged += UpdateLanguageSprite;
            UpdateLanguageSprite();
        }

        private void OnDestroy()
        {
            LocalizeManager.Manager.onLanguageChanged -= UpdateLanguageSprite;
        }

        private void UpdateLanguageSprite()
        {
            if (LocalizeManager.Manager.language == LanguageEnum.ENG)
                localizeSprite.sprite = engSprite;
            else //RU Language
                localizeSprite.sprite = ruSprite;
        }
    }
}
