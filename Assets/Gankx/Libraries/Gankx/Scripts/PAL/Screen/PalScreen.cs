using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gankx.PAL
{
    public sealed class PalScreen {
        public static int s_InitWidth = -1, s_InitHeight = -1;
        private static float s_InitRatio = -1;

        public static void InitScreenSize(int width, int height) {
            if (s_InitWidth >= 0) return;
            s_InitWidth = width;
            s_InitHeight = height;
            s_InitRatio = s_InitWidth * 1.0f / s_InitHeight;
        }

        public static int InitWidth {
            get {
                if (s_InitWidth > 0) return s_InitWidth;
                return Screen.width;
            }
        }

        public static int InitHeight {
            get {
                if (s_InitHeight > 0) return s_InitHeight;
                return Screen.height;
            }
        }

        public static float InitRatio {
            get {
                if (s_InitRatio > 0) return s_InitRatio;
                s_InitRatio = InitWidth * 1.0f / InitHeight;
                return s_InitRatio;
            }
        }

        public enum AndroidPhoneType {
            NONE,
            // 华为
            HUAWEI,
            // 小米
            XIAOMI,
            //oppo
            OPPO,
            //vivo
            VIVO,
            // 三星
            SAMSUNG,
            MAX,
        }

        public static AndroidPhoneType CurrentAndroidPhoneType = AndroidPhoneType.MAX;
        public static AndroidPhoneType GetAndroidPhoneType() {
            if (CurrentAndroidPhoneType != AndroidPhoneType.MAX) return CurrentAndroidPhoneType;
            string phoneUpperModel = SystemInfo.deviceModel.ToUpper();

            for (int i = (int)AndroidPhoneType.NONE + 1; i < (int)AndroidPhoneType.MAX; i++) {
                AndroidPhoneType current = (AndroidPhoneType)i;
                if (!phoneUpperModel.Contains(current.ToString())) continue;
                CurrentAndroidPhoneType = current;
                break;
            }

            return CurrentAndroidPhoneType;
        }

        public static bool s_IsNotchScreen = false;
        public static float s_NotchEdge = -1;

        public static float NotchEdge {
            get {
                if (s_NotchEdge < 0) {
                    s_IsNotchScreen = CheckIsNotchScreen(out s_NotchEdge);
                }

                return s_NotchEdge;
            }
        }

        public static bool IsNotchScreen {
            get {
                if (s_NotchEdge < 0) {
                    s_IsNotchScreen = CheckIsNotchScreen(out s_NotchEdge);
                }

                return s_IsNotchScreen;
            }
        }

        public static bool CheckIsNotchScreen(out float edge) {
            if (s_NotchEdge >= 0) {
                edge = s_NotchEdge;
                return s_IsNotchScreen;
            }

            edge = 0;
            s_NotchEdge = edge;

            // 比例小于 2 的认为不是全面屏
            if (InitWidth * 1.0f / InitHeight < 2) {
                s_IsNotchScreen = false;
                return s_IsNotchScreen;
            }

            if (GetSDKInt() >= 28) {
                s_IsNotchScreen = hasNotchInScreen_AndroidP(out edge);
                s_NotchEdge = edge;
                return s_IsNotchScreen;
            }

            AndroidPhoneType phoneType = GetAndroidPhoneType();
            if (phoneType == AndroidPhoneType.NONE) {
                s_IsNotchScreen = false;
                return s_IsNotchScreen;
            }

            switch (phoneType) {
                case AndroidPhoneType.XIAOMI:
                    s_IsNotchScreen = hasNotchInScreen_Xiaomi(out edge);
                    break;
                case AndroidPhoneType.HUAWEI:
                    s_IsNotchScreen = hasNotchInScreen_Huawei(out edge);
                    break;
                case AndroidPhoneType.VIVO:
                    s_IsNotchScreen = hasNotchInScreen_Vivo(out edge);
                    break;
                case AndroidPhoneType.OPPO:
                    s_IsNotchScreen = hasNotchInScreen_Oppo(out edge);
                    break;
                case AndroidPhoneType.SAMSUNG:
                    s_IsNotchScreen = hasNotchInScreen_Samsung(out edge);
                    break;
            }

            s_NotchEdge = edge;
            return true;
        }

        private static int s_AndroidSDK = -1;
        static int GetSDKInt() {
            if (s_AndroidSDK >= 0) return s_AndroidSDK;

            using (AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION")) {
                s_AndroidSDK = version.GetStatic<int>("SDK_INT");
            }

            return s_AndroidSDK;
        }

        public static bool hasNotchInScreen_AndroidP(out float edge) {
            edge = 0;

            try {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                    using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
                        using (AndroidJavaObject window = activity.Call<AndroidJavaObject>("getWindow")) {
                            using (AndroidJavaObject decorView = window.Call<AndroidJavaObject>("getDecorView")) {
                                using (AndroidJavaObject windowInsets =
                                    decorView.Call<AndroidJavaObject>("getRootWindowInsets")) {
                                    AndroidJavaObject displayCutout = windowInsets.Call<AndroidJavaObject>("getDisplayCutout");
                                    if (displayCutout != null) {
                                        int notchHeight = displayCutout.Call<int>("getSafeInsetLeft");
                                        edge = notchHeight * 1.0f / InitWidth;
                                        displayCutout.Dispose();
                                        return notchHeight > 0;
                                    }
                                    else {
                                        // 三星的非全面屏需要检查是否是圆角屏
                                        if (GetAndroidPhoneType() == AndroidPhoneType.SAMSUNG) {
                                            return hasNotchInScreen_Samsung(out edge);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception e) {
                Debug.LogError("Android P hasNotch 报错" + e);
            }

            return false;
        }

        public static bool hasNotchInScreen_Xiaomi(out float edge) {
            edge = 0;

            try {
                using (AndroidJavaClass jo = new AndroidJavaClass("android/os/SystemProperties")) {
                    string hasNotch = jo.CallStatic<string>("get", "ro.miui.notch");

                    if (hasNotch != "1") return hasNotch == "1";
                    float notchHeight = 89;
                    using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                        using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
                            using (AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext")) {
                                using (AndroidJavaObject resources = context.Call<AndroidJavaObject>("getResources")) {
                                    // MIUI 10 支持动态获取Notch高度
                                    int resourceId = resources.Call<int>("getIdentifier", "notch_height", "dimen", "android");
                                    if (resourceId > 0) {
                                        notchHeight = resources.Call<int>("getDimensionPixelSize", resourceId);
                                    }
                                    else {
                                        resourceId = resources.Call<int>("getIdentifier", "status_bar_height", "dimen", "android");
                                        if (resourceId > 0) {
                                            notchHeight = resources.Call<int>("getDimensionPixelSize", resourceId) - 20;
                                        }
                                    }
                                }
                            }
                        }

                        edge = notchHeight * 1.0f / InitWidth;
                    }

                    return hasNotch == "1";
                }
            }
            catch (System.Exception e) {
                Debug.LogError("小米手机 hasNotch 报错" + e);
            }

            return false;
        }

        public static bool hasNotchInScreen_Huawei(out float edge) {
            edge = 0;
            try {
                using (AndroidJavaClass jo = new AndroidJavaClass("com.huawei.android.util.HwNotchSizeUtil")) {
                    bool hasNotchInScreen = jo.CallStatic<bool>("hasNotchInScreen");
                    int[] notchSize = jo.CallStatic<int[]>("getNotchSize");

                    edge = notchSize[1] * 1.0f / InitWidth;
                    return hasNotchInScreen;
                }
            }
            catch (System.Exception e) {
                Debug.LogError("华为手机 报错" + e);
            }

            return false;
        }

        public const int NOTCH_IN_SCREEN_VOIO_MARK = 0x00000020;//是否有凹槽
        public const int ROUNDED_IN_SCREEN_VOIO_MARK = 0x00000008;//是否有圆角
        public static bool hasNotchInScreen_Vivo(out float edge) {
            edge = 0;
            try {
                using (AndroidJavaClass jo = new AndroidJavaClass("android.util.FtFeature")) {
                    bool hasNotchInScreen = jo.CallStatic<bool>("isFeatureSupport", NOTCH_IN_SCREEN_VOIO_MARK);

                    bool hasRoundInScreen = jo.CallStatic<bool>("isFeatureSupport", ROUNDED_IN_SCREEN_VOIO_MARK);

                    if (hasNotchInScreen) {
                        edge = 80 * 1.0f / InitWidth;
                    }
                    else if (hasRoundInScreen) {
                        edge = 40 * 1.0f / InitWidth;
                    }

                    return hasNotchInScreen;
                }
            }
            catch (System.Exception e) {
                Debug.LogError("Vivo手机 报错" + e);
            }

            return false;
        }

        public static bool hasNotchInScreen_Oppo(out float edge) {
            edge = 0;

            try {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                    using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
                        using (AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext")) {
                            using (AndroidJavaObject manager = activity.Call<AndroidJavaObject>("getPackageManager")) {
                                bool hasNotchInScreen = manager.Call<bool>("hasSystemFeature", "com.oppo.feature.screen.heteromorphism");

                                int notchHeight = 80;
                                using (AndroidJavaObject resources = context.Call<AndroidJavaObject>("getResources")) {
                                    int resourceId = resources.Call<int>("getIdentifier", "status_bar_height", "dimen", "android");
                                    if (resourceId > 0) {
                                        notchHeight = resources.Call<int>("getDimensionPixelSize", resourceId);
                                    }
                                }

                                if (!hasNotchInScreen) {
                                    if (InitRatio > 2.1f) {
                                        hasNotchInScreen = true;
                                    }
                                }

                                if (hasNotchInScreen) {
                                    edge = notchHeight * 1.0f / InitWidth;
                                }

                                return hasNotchInScreen;
                            }
                        }
                    }
                }
            }
            catch (System.Exception e) {
                Debug.LogError("Oppo手机 报错" + e);
            }
            return false;
        }

        private const int SAMSUNG_COCKTAIL_PANEL = 7;
        public static bool hasNotchInScreen_Samsung(out float edge) {
            edge = 0;

            try {
                using (AndroidJavaClass jo = new AndroidJavaClass("com.samsung.android.sdk.look.SlookImpl")) {
                    bool hasNotchInScreen = jo.CallStatic<bool>("isFeatureEnabled", SAMSUNG_COCKTAIL_PANEL);
                    if (hasNotchInScreen) edge = 0.03612f; // 88.0f / 2436
                    return hasNotchInScreen;
                }
            }
            catch (System.Exception e) {
                Debug.LogError("三星手机 报错" + e);
            }
            return false;
        }
    }
}