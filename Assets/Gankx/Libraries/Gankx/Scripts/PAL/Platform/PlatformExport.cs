using Gankx;
using UnityEngine;

public class PlatformExport
{
    public static int GetRuntimePlatform()
    {
        return (int) Application.platform;
    }

    public static bool IsInStandaloneOrEditor()
    {
        if (Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.OSXEditor ||
            Application.platform == RuntimePlatform.WindowsEditor
            || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            return true;
        }

        return false;
    }

    public static void ApplicationQuit()
    {
        Application.Quit();
    }

    public static string GetAppVersion()
    {
        LiveUpdate.CheckAndLoadConfig();
        return LiveUpdate.appVersion;
    }

    public static string GetResVersion()
    {
        LiveUpdate.CheckAndLoadConfig();
        return LiveUpdate.resVersion;
    }

    public static string GetBuildVersion()
    {
        LiveUpdate.CheckAndLoadConfig();
        return LiveUpdate.buildVersion;
    }

    public static int GetResIntVersion()
    {
        LiveUpdate.CheckAndLoadConfig();

        if (!string.IsNullOrEmpty(LiveUpdate.resVersion))
        {
            var verNumStr = LiveUpdate.resVersion.Split('.');
            var verNum = new int[verNumStr.Length];
            for (var i = 0; i < verNum.Length; i++)
            {
                verNum[i] = int.Parse(verNumStr[i]);
            }

            var resInt = (verNum[0] << 24) + (verNum[1] << 16) + (verNum[2] << 8) + verNum[3];
            return resInt;
        }

        return 0;
    }

    public static int GetBundleVersionCode()
    {
        LiveUpdate.CheckAndLoadConfig();
        return int.Parse(LiveUpdate.bundleVersionCode);
    }

    public static bool IsPhone()
    {
        return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
    }

    public static bool IsAndroid()
    {
        return Application.platform == RuntimePlatform.Android;
    }

    public static bool IsIOS()
    {
        return Application.platform == RuntimePlatform.IPhonePlayer;
    }
}