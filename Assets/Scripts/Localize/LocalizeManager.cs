using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using MEC;

namespace Localize
{
    public enum LanguageEnum { ENG, RU }
    public class LocalizeManager
    {
        public static LocalizeManager Manager
        {
            get
            {
                _manager ??= new LocalizeManager();
                return _manager;
            }
        }

        private static LocalizeManager _manager;
        public LanguageEnum language { get; private set; } = LanguageEnum.ENG;
        public bool isLocalizeDataLoaded { get; private set; }

        public delegate void ChangeLanguageDelegate();
        public event ChangeLanguageDelegate onLanguageChanged;
        public event ChangeLanguageDelegate onLanguageDataLoaded;

        private const string LOCALIZE_FILE_NAME = "localize_data.json";

        private LocalizeManager()
        {
            Debug.Log("LOAD LOCALIZE FILE!");
            LoadLocalizeFile();
        }

        private JObject _localizeData;
        private void LoadLocalizeFile()
        {
            var path = Path.Combine(Application.streamingAssetsPath, LOCALIZE_FILE_NAME);
            Timing.RunCoroutine(LoadLocalizeRoutine(path));
        }

        private IEnumerator<float> LoadLocalizeRoutine(string path)
        {
            isLocalizeDataLoaded = false;
            
            var readerNet = UnityWebRequest.Get(path);
            yield return Timing.WaitUntilDone(readerNet.SendWebRequest());
        
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
