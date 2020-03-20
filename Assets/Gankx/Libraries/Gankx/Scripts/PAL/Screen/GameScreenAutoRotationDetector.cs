using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreenAutoRotationDetector : MonoBehaviour {

    private static bool AutoRotationOn;

#if UNITY_ANDROID && !UNITY_EDITOR
    void OnApplicationFocus(bool haveFocus)
    {
        if (haveFocus) GameScreenAutoRotationDetector.ToggleAutoRotation();
    }

    static void ToggleAutoRotation()
    {
        AutoRotationOn = DeviceAutoRotationIsOn();
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = AutoRotationOn;
        Screen.autorotateToLandscapeRight = AutoRotationOn;
        Screen.orientation = ScreenOrientation.AutoRotation;
    }
#endif

    static bool DeviceAutoRotationIsOn()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
            var context = actClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass systemGlobal = new AndroidJavaClass("android.provider.Settings$System");
            int rotationOn = 0;
            try {
                rotationOn = systemGlobal.CallStatic<int>("getInt",
                    context.Call<AndroidJavaObject>("getContentResolver"), "accelerometer_rotation");
            }
            catch (Exception ex) {
                return false;
            }
 
            return rotationOn == 1;
        }
#endif
        return true;
    }
}
