using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gankx
{
    [CreateAssetMenu(fileName = "SceneBuildSetting", menuName = "settings/SceneBuildSetting", order = 1)]
    public class SceneBuildSetting : SettingBase<SceneBuildSetting>
    {
        protected static string MyAssetPath { get; set; }
        public List<string> sceneNames = new List<string>();
        public List<string> scenePaths = new List<string>();

        [NonSerialized]
        private Dictionary<string, string> myScenePathDictionary = new Dictionary<string, string>();

        static SceneBuildSetting()
        {
            MyAssetPath = "settings/SceneBuildSetting";
        }

        private static SceneBuildSetting MyInstance;
        public static SceneBuildSetting instance
        {
            get
            {
                if (MyInstance == null)
                {
                    MyInstance = SettingService.instance.GetSetting<SceneBuildSetting>(MyAssetPath);
                    MyInstance.OnInit();
                }

                return MyInstance;
            }
        }

        protected override void OnInit()
        {
            for (var i = 0; i < scenePaths.Count; ++i)
            {
                myScenePathDictionary[sceneNames[i]] = scenePaths[i];
            }
        }

        public string GetScenePath(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                return string.Empty;
            }

            if (myScenePathDictionary.ContainsKey(sceneName))
            {
                return myScenePathDictionary[sceneName];
            }

            return string.Empty;
        }

        public string GetScenePathByIndex(int index)
        {
            if (index <= 0 || index >= scenePaths.Count)
            {
                return string.Empty;
            }

            return scenePaths[index];
        }
    }
}