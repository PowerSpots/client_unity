using System.Collections;
using System.Collections.Generic;
using Gankx;
using Core;
using UnityEngine;

public class GamePortalExport : MonoBehaviour
{
    public static int GetInitScope()
    {
        GamePortal portal = FindObjectOfType<GamePortal>();
        if (null == portal)
        {
            return 1;
        }

        return (int)portal.LuaInitScope;
    }

    public static void QuitApp() 
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try {
            if (LuaService.ContainsInstance()) {
                LuaService.Instance.FireEvent("OnApplicationQuit");
            }
            if (GamePortal.ContainsInstance())
                GamePortal.Instance.DelayKill();
        }
        catch (System.Exception e)
#endif
        {
            Application.Quit();
        }
    }

    public static void SetQuality(int iQuality)
    {
        GamePortal.QualityLevel = iQuality;
    }

    public static bool IsHighCharacterQuality()
    {
        return GetDetailedQuality("Character") > 1;
    }

    public static int GetQuality()
    {
        return GamePortal.QualityLevel + 1;
    }

    private static Dictionary<string, string> m_QualityMap = new Dictionary<string, string>();

    private static string GetQualityKey(string type) {
        if(m_QualityMap == null) m_QualityMap = new Dictionary<string, string>();
        if (m_QualityMap.ContainsKey(type)) return m_QualityMap[type];
        string key = "QualitySetting_" + type;
        m_QualityMap.Add(type, key);
        return key;
    }

    public static bool HasDetailedQuality(string type) {
        string key = GetQualityKey(type);
        return SimplePlayerPrefs.HasKey(key);
    }

    //这个接口有GC.Alloc update里不要用这个接口，用GetDetailedQualityByKey
    public static int GetDetailedQuality(string type) {
        string key = GetQualityKey(type);
        if (SimplePlayerPrefs.HasKey(key)) {
            int res = SimplePlayerPrefs.GetInt(key);
            return res;
        }

        return GetQuality();
    }

    public static int GetDetailedQualityByKey(string key)
    {
        if (SimplePlayerPrefs.HasKey(key))
        {   
            int res = SimplePlayerPrefs.GetInt(key);
            return res;
        }

        return GetQuality();
    }

    public static void InitDetailedQuality(string type) {
        if (!HasDetailedQuality(type)) return;
        SetDetailedQuality(type, GetDetailedQuality(type), true);
    }

    public static void SetDetailedQuality(string type, int level, bool force = false) {
        string key = GetQualityKey(type);
        if (SimplePlayerPrefs.HasKey(key) && SimplePlayerPrefs.GetInt(key) == level) {
            if (!force) {
                return;
            }
        }
        SimplePlayerPrefs.SetInt(key, level);
        //if (type == "Total") {
        //    SetQuality(level - 1);
        //}
        if (type == "Effect") {
            // TODO GameEffectService
//            GameEffectService.instance.SetEffectLODLevel(Mathf.Clamp(3 - level, 0, 2));
        }
        else if (type == "Scene") {
            // TODO SceneLODUtility
//            if (SceneLODUtility.Instance != null) {
//                SceneLODUtility.Instance.ApplyLodChange();
//            }
        }
        else if (type == "Water") {
            // TODO WaterLOD
//            if (WaterLod.Instance != null) {
//                WaterLod.Instance.enabled = true;
//                WaterLod.Instance.ApplyLodChange();
//            }
        }
        else if (type == "Resolution") {
            bool fullScreen = Application.platform != RuntimePlatform.WindowsPlayer || Screen.fullScreen;
            if (level == 1) {
                Screen.SetResolution((int) (ScreenRTMgr.instance.m_InitScreenWidth * 0.89f),
                    (int) (ScreenRTMgr.instance.m_InitScreenHeight * 0.89f), fullScreen);
            }
            else {
                Screen.SetResolution(ScreenRTMgr.instance.m_InitScreenWidth, ScreenRTMgr.instance.m_InitScreenHeight,
                    fullScreen);
            }
            ScreenRTMgr.instance.Release();
        }
        else if (type == "Shadow") {
            // TODO ShadowLOD
//            SceneShadowInit.ResetShadowRoot();
        }
        else if (type == "Shader") {
            if (level == 1) {
                Shader.globalMaximumLOD = 100;
            }
            else if (level == 2) {
                Shader.globalMaximumLOD = 200;
            }
            else {
                Shader.globalMaximumLOD = 500;
            }
        }
        else if(type == "Texture") {
            QualitySettings.SetQualityLevel(level - 1);
        }
    }

    public static void SetInBattleOutlineState(int level) {
        SimplePlayerPrefs.SetInt("InBattleActorShaderLODMaximunLOD", level);
    }

    public static void SetActorOutlineState(int level) {
        SimplePlayerPrefs.SetInt("ActorShaderLODMaximunLOD", level);
    }

    public static int GetOutlineState(bool isInBattle, bool clearIfMiss) {
        string key = isInBattle ? "InBattleActorShaderLODMaximunLOD" : "OutBattleActorShaderLODMaximunLOD";
        if (!SimplePlayerPrefs.HasKey(key)) {
            if (clearIfMiss) {
                SimplePlayerPrefs.DeleteKey("ActorShaderLODMaximunLOD");
                return -1;
            }
            return Shader.globalMaximumLOD;
        }
        return SimplePlayerPrefs.GetInt(key);
    }

    public static int MemorySize() {
        return GamePortal.instance.MemorySize;
    }

    public static bool IsEmulator() {
        return GamePortal.instance.IsEmulator;
    }

    public static bool IsModelAllowed()
    {
        return GamePortal.instance.IsModelAllowed();
    }

    public static string GetProcessorType()
    {
        return SystemInfo.processorType;
    }

    public static string GetDeviceModel()
    {
        return SystemInfo.deviceModel;
    }

    public static string GetGraphicsDeviceName() {
        return SystemInfo.graphicsDeviceName;
    }

    public static void CleanUpMessenger()
    {
        Messenger.Cleanup();
    }

    public static void QuickFixClient() {
        if (LuaService.ContainsInstance() && LuaService.instance.env != null) {
            LuaService.instance.env.GC();
        }
        Gankx.DontDestroyOnLoadManager.DestroyAll();

        LiveUpdate.isFixClient = true;
        SceneMgr.LoadSceneByName("Update");
    }

    public static void BackToUpdate() {
        if (LuaService.ContainsInstance() && LuaService.instance.env != null) {
            LuaService.instance.env.GC();
        }
        Gankx.DontDestroyOnLoadManager.DestroyAll();

        LiveUpdate.isFixClient = false;
        SceneMgr.LoadSceneByName("Update");
    }

    public static int GetiOSGeneration() {
#if UNITY_IOS
        return (int)UnityEngine.iOS.Device.generation;
#endif
        return 0;
    }

    public static string GetiOSGenerationString() {
#if UNITY_IOS
        return UnityEngine.iOS.Device.generation.ToString();
#endif
        return "";
    }
}
