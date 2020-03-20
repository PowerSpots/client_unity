using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gankx.IO;
using Gankx.PAL;
using MiniJSON;
using UnityEngine;
using UnityEngine.UI;

namespace Gankx
{
    public class LiveUpdate : MonoBehaviour
    {
        private const string FirstExtractVersion = "UPDATE_FIRST_EXTRACT_VERSION";

        private static string MyAppVersion = string.Empty;
        private static string MyResVersion = string.Empty;
        private static string MyBuildVersion = string.Empty;
        private static string MyBundleVersionCode = string.Empty;
        private static bool MyForceUpdate = true;
        private static bool MyFixClient = false;

        public Text versionText;
        public Text updateInfoText;

        private LiveUpdateIntegration myIntegration;
        private List<string> myBuildinFileList = new List<string>();

        public static string appVersion
        {
            get
            {
                if (string.IsNullOrEmpty(MyAppVersion))
                {
                    return "0.0.0.0";
                }

                return MyAppVersion;
            }
        }

        public static string resVersion
        {
            get
            {
                if (string.IsNullOrEmpty(MyResVersion))
                {
                    if (PlayerPrefs.HasKey("MyResVersion"))
                    {
                        MyResVersion = PlayerPrefs.GetString("MyResVersion");
                        return MyResVersion;
                    }

                    return "0.0.0.0";
                }

                return MyResVersion;
            }
        }

        public static string buildVersion
        {
            get
            {
                if (string.IsNullOrEmpty(MyBuildVersion))
                {
                    return "1";
                }

                return MyBuildVersion;
            }
        }

        public static string bundleVersionCode
        {
            get
            {
                if (!string.IsNullOrEmpty(MyBundleVersionCode))
                {
                    return MyBundleVersionCode;
                }

                var verNumStr = appVersion.Split('.');
                var verNum = new int[verNumStr.Length];
                for (var i = 0; i < verNum.Length; i++)
                {
                    verNum[i] = int.Parse(verNumStr[i]);
                }

                var bundleVersionCodeValue = (verNum[0] << 24) + (verNum[1] << 16) + (verNum[2] << 8) + verNum[3];
                MyBundleVersionCode = bundleVersionCodeValue.ToString();
                return MyBundleVersionCode;
            }
        }

        // ReSharper disable once ConvertToAutoProperty
        public static bool isFixClient
        {
            get { return MyFixClient;}
            set { MyFixClient = value; }
        }

        public static bool forceUpdate
        {
            get { return MyForceUpdate; }
            set { MyForceUpdate = value; }
        }

        public static void CheckAndLoadConfig()
        {
            if (string.IsNullOrEmpty(MyAppVersion))
            {
                LoadVersionConfig();
            }
        }

        private static void GetConfigString(Dictionary<string, object> dict, string key, ref string value)
        {
            if (dict == null || string.IsNullOrEmpty(key) || !dict.ContainsKey(key))
            {
                return;
            }

            try
            {
                value = dict[key] as string;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private static void LoadConfigFromDict(Dictionary<string, object> dictionary)
        {
            GetConfigString(dictionary, "app_version", ref MyAppVersion);
            GetConfigString(dictionary, "build_version", ref MyBuildVersion);
            GetConfigString(dictionary, "bundle_version_code", ref MyBundleVersionCode);
        }

        private static void LoadVersionConfig()
        {
            var integration = LiveUpdateIntegration.instance;

            var appversion = Resources.Load<TextAsset>(integration.GetVersionResourcesPath());
            if (appversion != null)
            {
                var dictionary = (Dictionary<string, object>) Json.Deserialize(appversion.text);
                LoadConfigFromDict(dictionary);
            }

            var empty = string.Empty;
            if (FileLoaderHelper.LoadFromRootPath(integration.GetVersionDataPath(), ref empty))
            {
                var dictionary = (Dictionary<string, object>) Json.Deserialize(empty);
                GetConfigString(dictionary, "res_version", ref MyResVersion);
            }
            else
            {
                var bytes = FileLoaderHelper.LoadFromStreamAssets("data/" + integration.GetVersionDataPath());
                if (bytes != null)
                {
                    var dictionary = (Dictionary<string, object>) Json.Deserialize(Encoding.UTF8.GetString(bytes));
                    GetConfigString(dictionary, "res_version", ref MyResVersion);
                }
            }

            if (FileLoaderHelper.LoadFromExtraPath(integration.GetVersionExtraPath(), ref empty))
            {
                var dictionary = (Dictionary<string, object>) Json.Deserialize(empty);
                LoadConfigFromDict(dictionary);
            }
        }

        private void Awake()
        {
            myIntegration = LiveUpdateIntegration.instance;

            forceUpdate = true;

            myIntegration.IntegrateBugreport();

            myIntegration.GetBuildinFileList(myBuildinFileList);

            UIScreenAdaptor.SetCanvasScaler(gameObject);

            LoadVersionConfig();

            UpdateVersionInfo();

            if (updateInfoText != null)
            {
                updateInfoText.text = string.Empty;
            }
        }

        private IEnumerator Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            if (isFixClient && PlatformExport.IsPhone())
            {
                isFixClient = false;
                {
                    InitializeData();

                    LoadVersionConfig();
                }
            }

            PalScreen.InitScreenSize(Screen.width, Screen.height);

            var current = Screen.currentResolution;
            if (current.height > 1080)
            {
                var width = (int) ((float) current.width / current.height * 1080);
                Screen.SetResolution(width, 1080, true);
            }


#if !UNITY_EDITOR
            Debug.LogError("检查版本更新");

            {
                myIntegration.UpdateApk(this);

                while (myIntegration.IsUpdating())
                {
                    yield return null;
                }
            }
#endif


            {
                var lastVersion = PlayerPrefs.GetString(FirstExtractVersion);
                var currentVersion = appVersion + " Build " + buildVersion;

                if (currentVersion != lastVersion || !CheckDataIntegrity())
                {
                    InitializeData();

                    PlayerPrefs.SetString(FirstExtractVersion, currentVersion);

                    LoadVersionConfig();

                    UpdateVersionInfo();
                }
            }

            {
                var lastResVersion = resVersion;
                myIntegration.UpdateRes(this);

                while (myIntegration.IsUpdating())
                {
                    yield return null;
                }

                LoadVersionConfig();

                UpdateVersionInfo();

                if (resVersion != lastResVersion)
                {
                    DeleteDirectory(FileService.dataPath + "Extend");
                }
            }

            Screen.sleepTimeout = SleepTimeout.SystemSetting;

            SceneMgr.LoadSceneByName(myIntegration.GetForwardScene());

            Destroy(this);
        }

        private void InitializeData()
        {
            if (!Application.isMobilePlatform)
            {
                return;
            }

            try
            {
                CleanupData();

                if (!Directory.Exists(FileService.dataPath))
                {
                    Directory.CreateDirectory(FileService.dataPath);
                }

                for (var index = 0; index < myBuildinFileList.Count; index++)
                {
                    ExtractBuildinFile(myBuildinFileList[index]);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void ExtractBuildinFile(string fileName)
        {
            var bytes = FileLoaderHelper.LoadFromStreamAssets(fileName);
            var dstPath = Application.persistentDataPath + "/" + fileName;

            Debug.Log(dstPath);
            if (File.Exists(dstPath))
            {
                File.Delete(dstPath);
            }

            File.WriteAllBytes(dstPath, bytes);
        }

        private void UpdateVersionInfo()
        {
            if (versionText != null)
            {
                versionText.gameObject.SetActive(!string.IsNullOrEmpty(MyAppVersion));
                versionText.text = myIntegration.GetVersionLabel(appVersion, resVersion);
            }
        }

        [ContextMenu("FirstExtractVersion")]
        private void ClearFirstExtractVersion()
        {
            PlayerPrefs.DeleteKey(FirstExtractVersion);
        }

        private bool CheckDataIntegrity()
        {
            for (var i = 0; i < myBuildinFileList.Count; i++)
            {
                if (!File.Exists(Application.persistentDataPath + "/" + myBuildinFileList[i]))
                {
                    Debug.LogError("can not find the file: " + myBuildinFileList[i]);
                    return false;
                }
            }

            return true;
        }

        private void CleanupData()
        {
            if (string.IsNullOrEmpty(Application.persistentDataPath) || !PlatformExport.IsPhone())
            {
                return;
            }

            var directoryInfo = new DirectoryInfo(Application.persistentDataPath);
            if (directoryInfo.Exists)
            {
                var files = directoryInfo.GetFiles();
                for (var i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".apk"))
                    {
                        File.SetAttributes(files[i].FullName, FileAttributes.Normal);
                        File.Delete(files[i].FullName);
                    }
                    else if (myIntegration.ShouldCleanupFile(files[i].Name))
                    {
                        File.SetAttributes(files[i].FullName, FileAttributes.Normal);
                        File.Delete(files[i].FullName);
                    }
                }

                ClearAttributes(FileService.dataPath);

                if (Directory.Exists(FileService.dataPath))
                {
                    DeleteDirectory(FileService.dataPath);
                }

                if (!Directory.Exists(FileService.dataPath))
                {
                    Directory.CreateDirectory(FileService.dataPath);
                }
            }
        }

        public static void ClearAttributes(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            var subDirs = Directory.GetDirectories(path);
            for (var i = 0; i < subDirs.Length; i++)
            {
                ClearAttributes(subDirs[i]);
            }

            var files = Directory.GetFiles(path);
            for (var i = 0; i < files.Length; i++)
            {
                File.SetAttributes(files[i], FileAttributes.Normal);
            }
        }

        public static void DeleteDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            var files = Directory.GetFiles(path);
            var dirs = Directory.GetDirectories(path);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(path, false);
        }

        public static void MoveFile(string sourceFileName, string destFileName)
        {
            if (!File.Exists(sourceFileName))
            {
                return;
            }

            var destDir = Path.GetDirectoryName(destFileName);
            if (destDir != null && !Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            try
            {
                File.Move(sourceFileName, destFileName);
            }
            catch (Exception e)
            {
                Debug.Log("Move Fail From path:" + sourceFileName + " to path:" + destFileName + e);
            }
        }

        public static void MoveDir(string sourceDirName, string destDirName)
        {
            if (!Directory.Exists(sourceDirName))
            {
                return;
            }

            var parentInfo = Directory.GetParent(destDirName);
            if (parentInfo == null)
            {
                return;
            }

            if (!parentInfo.Exists)
            {
                Directory.CreateDirectory(parentInfo.FullName);
            }
            else
            {
                DeleteDirectory(destDirName);
            }

            ClearAttributes(sourceDirName);
            ClearAttributes(destDirName);
            Directory.Move(sourceDirName, destDirName);
        }
    }
}