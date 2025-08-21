using System;
using System.IO;
using GoogleSheetsToUnity;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace AutoBattleCardGame.Editor
{
    public class GoogleClientSecretLoader : EditorWindow
    {
        private Setting setting;
        
        [MenuItem("ABC Utility/Google Client Setting Loader")]
        private static void Open()
        {
            GoogleClientSecretLoader window = GetWindow<GoogleClientSecretLoader>("Google Client Setting Loader");
            window.setting = Setting.LoadSetting();
        }

        private void OnGUI()
        {
            GUILayout.Label("Google Client Secret Json Path");
            setting.clientSecretJsonPath = GUILayout.TextField(setting.IsPathEmpty ? "Press 'Set Json Path'" : setting.clientSecretJsonPath);
            
            if (GUILayout.Button("Set Json Path"))
            {
                string downloadPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads"
                );
                
                string jsonPath = EditorUtility.OpenFilePanelWithFilters(
                    title: $"Please select Google Client Secret Json File",
                    directory: downloadPath,
                    new [] { "JSON File", "json" }
                );
                
                Debug.Log($"{nameof(GoogleClientSecretLoader)} : Saved Google Client Secret Json Path");

                setting.clientSecretJsonPath = jsonPath;
                Setting.SaveSetting(setting);
            }

            if (GUILayout.Button("Load Google Client Secret"))
            {
                if (!File.Exists(setting.clientSecretJsonPath))
                {
                    Debug.Log($"{nameof(GoogleClientSecretLoader)} : Google Client Secret Json Path not found, please set path again");
                    return;
                }

                using (StreamReader sr = new StreamReader(setting.clientSecretJsonPath))
                {
                    string serialized = sr.ReadToEnd();
                    
                    GoogleClientSecret clientSecret =  JsonConvert.DeserializeObject<GoogleClientSecret>(serialized);
                    GoogleSheetsToUnityConfig config = (GoogleSheetsToUnityConfig)Resources.Load("GSTU_Config");

                    config.CLIENT_ID = clientSecret.installed.clientId;
                    config.CLIENT_SECRET = clientSecret.installed.clientSecret;
                    config.PORT = 8080;
                    
                    EditorUtility.SetDirty(config);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    EditorApplication.ExecuteMenuItem("Window/GSTU/Open Config");
                }
            }
        }
        
        [Serializable]
        private class Setting
        {
            private const string PLAYERPREFS_KEY = "GOOGLE_CLIENT_SETTING_LOADER";
            
            public string clientSecretJsonPath;
            public bool IsPathEmpty => string.IsNullOrEmpty(clientSecretJsonPath);

            public static Setting LoadSetting()
            {
                string serialized = PlayerPrefs.GetString(PLAYERPREFS_KEY, string.Empty);
                Setting setting = JsonConvert.DeserializeObject<Setting>(serialized);
                
                return setting;
            }

            public static void SaveSetting(Setting setting)
            {
                string serialized = JsonConvert.SerializeObject(setting);
                PlayerPrefs.SetString(PLAYERPREFS_KEY, serialized);
            }
        }

        [Serializable]
        private class GoogleClientSecret
        {
            public Fields installed;
            
            [Serializable]
            public class Fields
            {
                [JsonProperty("client_id")]
                public string clientId;
                [JsonProperty("client_secret")]
                public string clientSecret;
            }
        }
    }
}