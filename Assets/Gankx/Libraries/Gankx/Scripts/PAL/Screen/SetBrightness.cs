using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class Brightness
{
#if !UNITY_EDITOR && UNITY_IOS
    [DllImport("__Internal")]
    private static extern void setBrightness(float brightness);
    [DllImport("__Internal")]
    private static extern float getBrightness();
#endif

    private static float s_InitBrightness = -1;
    public static void Set(float brightness)
    {
#if UNITY_EDITOR
        return;
#elif UNITY_IOS
        if (s_InitBrightness < 0) {
            s_InitBrightness = getBrightness();
        }
        if (brightness < 0) brightness = s_InitBrightness;

        setBrightness(brightness);
#elif UNITY_ANDROID
        var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
            var window = activity.Call<AndroidJavaObject>("getWindow");
            var lp = window.Call<AndroidJavaObject>("getAttributes");
            lp.Set("screenBrightness", brightness);
            window.Call("setAttributes", lp);
        }));
#endif
    }

}