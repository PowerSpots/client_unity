//#define PROFILING 0
using UnityEngine;
[ExecuteInEditMode]
public class EngineMgr : MonoBehaviour
{
    //types
    public enum EngineConfig : int
    {
        EC_Low,
        EC_Medium,
        EC_High,
    }

    //const
#if UNITY_ANDROID
    const EngineConfig DefaultSpecLevel = EngineConfig.EC_Medium;
#else
    const EngineConfig DefaultSpecLevel = EngineConfig.EC_High;
#endif
    const int lowWidth = 1280;

    public static EngineMgr Instance;

    int defaultWidth;
    int defaultHeight;
    float defaultRatio;

    static public EngineConfig SpecLevel = DefaultSpecLevel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //setup resolution
        if (defaultWidth == 0 && defaultHeight == 0)
        {
            defaultWidth = Screen.width;
            defaultHeight = Screen.height;
            defaultRatio = (float) defaultHeight/defaultWidth;
        }

#if UNITY_ANDROID
        ConfigResolution();
#endif
    }

    void ConfigResolution()
    {
        //downgrade preset low resolution
        if (SpecLevel == EngineConfig.EC_Low)
        {
            if (lowWidth < defaultWidth)
            {
                int height = Mathf.FloorToInt(defaultRatio * lowWidth);
                if (height % 2 != 0)
                {
                    ++height;
                }
                Screen.SetResolution(lowWidth, height, true);
            }
        }
        else
        {
            Screen.SetResolution(defaultWidth, defaultHeight, true);
        }
    }

    public void SetSpecLevel(EngineConfig v)
    {
        if (v == SpecLevel)
        {
            return;
        }

        SpecLevel = v;

#if UNITY_ANDROID
        ConfigResolution();
#endif
    }

    public bool LowFxVersion()
    {
        return false;
    }
}
