using System.Collections.Generic;
using Gankx;
using UnityEngine;

public class SimpleLiveUpdateIntegration : LiveUpdateIntegration
{
    public override string GetVersionLabel(string appVersion, string resVersion)
    {
        return "版本 v" + appVersion + " 资源 v" + resVersion;
    }

    public override string GetVersionResourcesPath()
    {
        return "version/appconfig";
    }

    public override string GetVersionDataPath()
    {
        return "localimage/ResVersion.json";
    }

    public override string GetVersionExtraPath()
    {
        return "ResVersion.json";
    }

    public override void GetBuildinFileList(List<string> buildinFileList)
    {
        buildinFileList.Clear();
        buildinFileList.Add("data/data1.bytes");
        buildinFileList.Add("data/data3.bytes");
        buildinFileList.Add("data/data4.bytes");
    }

    public override bool ShouldCleanupFile(string fileName)
    {
        return false;
    }

    public override void IntegrateBugreport()
    {
        Debug.LogError("You should integrate the bug report here!");
    }

    public override void UpdateRes(LiveUpdate liveUpdate)
    {
    }

    public override void UpdateApk(LiveUpdate liveUpdate)
    {
    }

    public override bool IsUpdating()
    {
        return false;
    }

    public override string GetForwardScene()
    {
        return "Login";
    }
}