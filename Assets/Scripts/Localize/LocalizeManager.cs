using System.Collections;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace Localize
{
    public enum LanguageEnum { ENG, RU }
    public class LocalizeManager : MonoBehaviour
    {
        public static LocalizeManager Manager;
        [field: SerializeField] public LanguageEnum language { get; private set; } = LanguageEnum.ENG;
        public bool isLocalizeDataLoaded { get; private set; }

        public delegate void ChangeLanguageDelegate();
        public event ChangeLanguageDelegate onLanguageChanged;
        public event ChangeLanguageDelegate onLanguageDataLoaded;

        private const string LOCALIZE_FILE_NAME = "localize_data.json";

        private void Awake()
        {
            if (Manager is null)
            {
                LoadLocalizeFile();
            }
            
            if (Manager != null && Manager != this)
            {
                _localizeData = Manager._localizeData;
                Destroy(Manager.gameObject);
                
                Manager = null;
            }

            if (Manager is null)
            {
                Manager ??= this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private JObject _localizeData;
        private void LoadLocalizeFile()
        {
            var path = Path.Combine(Application.streamingAssetsPath, LOCALIZE_FILE_NAME);
            StartCoroutine(LoadLocalizeRoutine(path));
        }

        private IEnumerator LoadLocalizeRoutine(string path)
        {
            isLocalizeDataLoaded = false;
            
            var readerNet = UnityWebRequest.Get(path);
            yield return readerNet.SendWebRequest();
        
            if (readerNet.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Failed to load: "+path+" error: "+readerNet.error);
            }
            
            _localizeData = JObject.Parse(readerNet.downloadHandler.text);
            isLocalizeDataLoaded = true;
            onLanguageDataLoaded?.Invoke();
        }

        public void ChangeLanguage(LanguageEnum newLanguage)
        {
            language = newLanguage;
            onLanguageChanged?.Invoke();
        }

        public string[] GetDialogMessages(string id)
        {
            var languageId = language == LanguageEnum.ENG ? "eng" : "ru";
            var token = _localizeData.SelectToken($"$.{languageId}.dialogs.{id}");

            if (token != null) return token.ToObject<string[]>();
            
            Debug.LogError($"Token by id: {id} in dialogs was not found!");
            return new string[] { };
        }
    }
}
