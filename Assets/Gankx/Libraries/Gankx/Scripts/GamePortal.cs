using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Gankx;
using Gankx.IO;
using Gankx.PAL;
using Gankx.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using StreamReader = System.IO.StreamReader;
#if UNITY_IOS
using NotificationType = UnityEngine.iOS.NotificationType;
#endif

namespace Core
{
    public sealed class GamePortal : Gankx.Singleton<GamePortal>
    {
        public enum LuaScope
        {
            Login = 1,
            Hall = 2,
            Battle = 3,
            Tutorial = 4,
        }
        public int TargetFPS = -1;
        public LuaScope LuaInitScope = LuaScope.Login;

		private float fTime = 0;
		private int iFrame = 0;
		private int lastFps = 0;

        enum GameState {
            NONE,
            NORMAL,
            AFK,
        }

        private GameState m_CurrentGameState = GameState.NONE;
        private const int MAX_NO_INPUT_TIME = 5*60; 
        private float m_LastInputTime;

        public const int QUALITY_MAX = 3;
        private static int s_CurrentQualityLevel = QUALITY_MAX - 1;
        public static int QualityLevel {
            set
            {
                s_CurrentQualityLevel = value;
                InitQuality();
                PlayerPrefs.SetInt("Gankx_QualityLevel", s_CurrentQualityLevel);
            }
            get
            {
                return s_CurrentQualityLevel;
            }
        }

        private void CheckQuality()
        {
            if (PlayerPrefs.HasKey("Gankx_QualityLevel"))
            {
                QualityLevel = PlayerPrefs.GetInt("Gankx_QualityLevel");
                return;
            }

#if UNITY_ANDROID
            if (MemorySize > 4500) {
                QualityLevel = QUALITY_MAX - 1;
            }
            else if (MemorySize > 3300) {
                QualityLevel = 1;
            }
            else {
                QualityLevel = 0;
            } 
#elif UNITY_IOS
            if (UnityEngine.iOS.Device.generation >= UnityEngine.iOS.DeviceGeneration.iPhone7)
                QualityLevel = QUALITY_MAX - 1;
            else if (UnityEngine.iOS.Device.generation >= UnityEngine.iOS.DeviceGeneration.iPadAir2)
                QualityLevel = 1;
            else
                QualityLevel = 0;
#endif
        }


        private int m_MemorySize = -1;
        public int MemorySize {
            get {
                if (m_MemorySize > 0) return m_MemorySize;
#if UNITY_ANDROID && !UNITY_EDITOR
                try {
                    using (AndroidJavaObject fileReader = new AndroidJavaObject("java.io.FileReader", "/proc/meminfo")) {
                        using (AndroidJavaObject br = new AndroidJavaObject("java.io.BufferedReader", fileReader, 2048)) {
                            string mline = br.Call<string>("readLine");
                            br.Call("close");

                            mline = mline.Substring(mline.IndexOf("MemTotal:"));
                            mline = Regex.Match(mline, "(\\d+)").Groups[1].Value;

                            m_MemorySize = int.Parse(mline) / 1024;
                        }
                    }
                }
                catch (Exception e) 
#endif
                {
                    m_MemorySize = SystemInfo.systemMemorySize;
                }

                return m_MemorySize;
            }
        }

        public bool IsEmulator {
            get {
#if UNITY_ANDROID && !UNITY_EDITOR
                try {
                    using (AndroidJavaObject tm = new AndroidJavaObject("android.telephony.TelephonyManager")) {
                        string newImei = tm.Call<string>("getDeviceId");
                        if (!string.IsNullOrEmpty(newImei)) {
                            if (newImei.StartsWith("66666")) return false;
                        }
                    }   
                    using (AndroidJavaClass version = new AndroidJavaClass("android.os.Build")) {
                        string FINGERPRINT = version.GetStatic<string>("FINGERPRINT");
                        string MODEL = version.GetStatic<string>("MODEL");
                        string SERIAL = version.GetStatic<string>("SERIAL");
                        string MANUFACTURER = version.GetStatic<string>("MANUFACTURER");
                        string DEVICE = version.GetStatic<string>("DEVICE");
                        string PRODUCT = version.GetStatic<string>("PRODUCT");
                        string BRAND = version.GetStatic<string>("BRAND");

                        bool res = FINGERPRINT.StartsWith("generic") 
                            || FINGERPRINT.ToLower().Contains("vbox")
                            || FINGERPRINT.ToLower().Contains("test-keys") 

                            || MODEL.Contains("google_sdk") 
                            || MODEL.Contains("Emulator") 
                            || MODEL.Contains("Android SDK built for x86") 
                            
                            || SERIAL.Equals("android", StringComparison.CurrentCultureIgnoreCase)
                            
                            || MANUFACTURER.Contains("Genymotion")

                            || (BRAND.StartsWith("generic") && DEVICE.StartsWith("generic")) 
                            || PRODUCT.Equals("google_sdk")
                            ;
                        return res;
                    }
                }
                catch {
                    // ignored
                }
#endif
                return false;
            }
        }

        public GameObject m_InitBG;
        public TweenSlider m_Tween;

        protected override void OnInit()
        {
            CheckQuality();
            UnityConfig();
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnLoaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;

            // 拉起RT
            ScreenRTMgr.instance.GetDepthRT();
        }

        protected override void OnRelease()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnLoaded;
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        private void UnityConfig()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            Application.targetFrameRate = TargetFPS;
#else
           Application.targetFrameRate = 30;
#endif
            Application.backgroundLoadingPriority = ThreadPriority.Normal;

            m_CurrentGameState = GameState.NORMAL;
            m_LastInputTime = Time.realtimeSinceStartup;
        }

        private List<string> _sceneStack = new List<string>();

        public Scene GetSceneAt(int index)
        {
            return SceneManager.GetSceneByName(_sceneStack[index]);
        }

        private void OnSceneLoaded(Scene scene,LoadSceneMode mode)
        {
            _sceneStack.Add(scene.name);
            SceneMgr.SetActiveScene(scene);
            LuaService.instance.FireEvent("OnSceneLoaded",scene.name);
            //StartCoroutine(GC());
        }

        void OnSceneUnLoaded(Scene scene) {
            _sceneStack.Remove(scene.name);

            for (int i = _sceneStack.Count - 1; i >= 0; i--)
            {
                string sceneName = _sceneStack[i];
                
                Scene toActiveScene = SceneManager.GetSceneByName(sceneName);
                if (toActiveScene.IsValid() && toActiveScene.isLoaded && toActiveScene.name != "Login")
                {
                    SceneMgr.SetActiveScene(toActiveScene);
                    break;
                }
            }                    

            LuaService.instance.FireEvent("OnSceneUnloaded", scene.name);
            
            //StartCoroutine(GC());
        }

        void OnActiveSceneChanged(Scene current, Scene next)
        {
            Messenger.Broadcast("Active_Scene_Changed", next.name);
            int index = _sceneStack.IndexOf(next.name);
            // 没找到或者在队尾，无视
            if (index < 0 || index == _sceneStack.Count - 1)
            {
                return;
            }

            _sceneStack.RemoveAt(index);
            _sceneStack.Add(next.name);
        }

        /*IEnumerator GC()
        {
            yield return Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }*/

        /*****
        apple push服务在注册的时候会从APNS得到一个token发给客户端，客户端需要把这个token转发到飞鹰系统，随后飞鹰可以正常发送消息给APNS,APNS根据这个token发送到对应app上
        为了完成这个操作，客户端需要
        1.一个push证书，
        2.entitlement中包含aps-environment，
        3.mobileprovision中包含对应aps-environment(rdm上自动会生成)，provision具体内容可以用$security cms -D -i ***.mobileprovision查看
        4.默认unity打包的时候会 #define UNITY_USES_REMOTE_NOTIFICATIONS 0，这会导致客户端不会转发token给飞鹰，解决方案是至少enable一下就可以
        *****/
#if UNITY_IOS
        private void enableAPNsPushToken()
        {
            UnityEngine.iOS.NotificationServices.RegisterForNotifications(
                NotificationType.Alert |
                NotificationType.Badge |
                NotificationType.Sound);
        }
#endif


        IEnumerator Start() 
        {
            if (m_Tween != null) {
                m_Tween.@from = 0;
                m_Tween.to = 1;
                m_Tween.duration = 0.1f;
                m_Tween.ResetToBeginning();
            }

            yield return null;

#if UNITY_IOS
            enableAPNsPushToken();
#endif

            yield return StartCoroutine(GameStart());

            StartGameUpdateService();

            Application.lowMemory += OnLowMemory;
        }

        private void StartGameUpdateService()
        {
            // TODO CameraProtect
//            var dummy = CameraProtect.Instance;
        }

        static void InitQuality()
        {
            GamePortalExport.SetDetailedQuality("Texture", GamePortalExport.GetDetailedQuality("Texture"), true);
            GamePortalExport.SetDetailedQuality("Water", GamePortalExport.GetDetailedQuality("Water"), true);
            GamePortalExport.SetDetailedQuality("Shader", GamePortalExport.GetDetailedQuality("Shader"), true);
            GamePortalExport.SetDetailedQuality("Resolution", GamePortalExport.GetDetailedQuality("Resolution"), true);
            GamePortalExport.SetDetailedQuality("Effect", GamePortalExport.GetDetailedQuality("Effect"), true);

            if (!GamePortalExport.HasDetailedQuality("Character")) {
                if (s_CurrentQualityLevel == 2) {
                    GamePortalExport.SetDetailedQuality("Character", 2);
                }
                else {
                    GamePortalExport.SetDetailedQuality("Character", 1);
                }
            }
        }

        public IEnumerator GameStart()
        {
            try
            {
                Platform.instance.Init();
                ResourceService.instance.Initialize();
                LuaService.instance.Init();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Exception in GameStart." + ex.ToString());
            }

            float progress = 0.0f;
            do {
                progress = LuaService.instance.BootProgress();
                SetTweenBar(progress);
                yield return null;
            } while (progress < 1);

            if (m_Tween != null)
            {
                m_Tween.value = 1;
            }
            yield return null;

            if (m_InitBG != null) {
                Destroy(m_InitBG);
            }
        }

        void SetTweenBar(float amout) {
            if (m_Tween == null) return;
            m_Tween.to = amout;
            m_Tween.SetStartToCurrentValue();
            m_Tween.ResetToBeginning();
            m_Tween.Play(true);
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (LuaService.ContainsInstance())
            {
                LuaService.instance.FireEvent("OnApplicationPause", pauseStatus);
            }
        }

        void OnApplicationFocus(bool focus)
        {
            m_LastInputTime = Time.realtimeSinceStartup;

            if (m_CurrentGameState == GameState.AFK)
            {
                m_CurrentGameState = GameState.NORMAL;
                if (LuaService.ContainsInstance())
                {
                    Brightness.Set(-1f);
                    LuaService.instance.FireEvent("OnGameAFKStateChanged", false);
                }
            }

            if (LuaService.ContainsInstance())
            {
                LuaService.instance.FireEvent("OnApplicationFocus", focus);
            }
        }

        void OnApplicationQuit()
        {
            if (LuaService.ContainsInstance())
            {
                LuaService.instance.FireEvent("OnApplicationQuit");
            }
        }


        void LateUpdate() {
            if (Input.GetMouseButton(0)) {
                m_LastInputTime = Time.realtimeSinceStartup;
            }
            if (Time.realtimeSinceStartup - m_LastInputTime > MAX_NO_INPUT_TIME) {
                if (m_CurrentGameState == GameState.NORMAL) {
                    m_CurrentGameState = GameState.AFK;
                    if (LuaService.ContainsInstance()) {
                        Brightness.Set(0.01f);
                        LuaService.instance.FireEvent("OnGameAFKStateChanged", true);
                    }
                }
            }
            else {
                if (m_CurrentGameState == GameState.AFK) {
                    m_CurrentGameState = GameState.NORMAL;
                    if (LuaService.ContainsInstance()) {
                        Brightness.Set(-1f);
                        LuaService.instance.FireEvent("OnGameAFKStateChanged", false);
                    }
                }
            }

            if(Input.GetKeyUp(KeyCode.Escape))
            {
                LuaService.instance.FireEvent("OnQuitApp");
            }


#if UNITY_STANDALONE_WIN
            if(Debug.isDebugBuild && Debug.developerConsoleVisible)
                Debug.developerConsoleVisible = false;
#endif

#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.Pause)) {
                UnityEditor.EditorApplication.isPaused = !UnityEditor.EditorApplication.isPaused;
            }
#endif
        }

        public bool IsModelAllowed()
        {
            try
            {
                var path = FileService.dataPath + "localimage/BlackListConfig.txt";
                var model = SystemInfo.processorType;
                Debug.LogError(model);
                using (var sr = new StreamReader(path, Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (model == line)
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return true;
        }

        public bool IsAFK()
        {
            return m_CurrentGameState == GameState.AFK;
        }

        public void OnSaveJPGFinished(string path)
        {
            if(!string.IsNullOrEmpty(path))
                LuaService.instance.FireEvent("OnSaveToLocalFinished",true);
            else
                LuaService.instance.FireEvent("OnSaveToLocalFinished",false);
        }

        public void DelayKill() {
            StartCoroutine(OnDelayKill());
        }

        IEnumerator OnDelayKill() {
            yield return new WaitForSeconds(0.2f);
#if UNITY_ANDROID && !UNITY_EDITOR
            using (AndroidJavaClass process = new AndroidJavaClass("android.os.Process")) {
                int pid = process.CallStatic<int>("myPid");
                process.CallStatic("killProcess", pid);
            }
#else
            Application.Quit();
#endif
        }

        void OnLowMemory()
        {
            if (LuaService.ContainsInstance())
            {
                LuaService.instance.FireEvent("OnLowMemory"); 
            }
            
            if(ResourceService.ContainsInstance())
            {
                ResourceService.instance.CollectAll();
            }
        }

    }
}