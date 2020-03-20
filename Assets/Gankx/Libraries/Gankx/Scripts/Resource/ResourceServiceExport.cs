using Gankx;
using Gankx.UI;
using UnityEngine;

public class ResourceServiceExport
{
    public static void UnloadUnusedAssets()
    {
        ResourceService.UnloadUnusedAssets();
    }

    public static void GC()
    {
        ResourceService.instance.CollectAll();
    }

    public static void Collect()
    {
        ResourceService.Collect();
    }

    public static void TryReleaseUnuseBundle()
    {
        ResourceService.TryReleaseUnuseBundle();
    }

    public static bool LoadGameObject(string path)
    {
        var prefab = ResourceService.Load<GameObject>(path);
        if (null == prefab)
        {
            return false;
        }

        return Object.Instantiate(prefab) != null;
    }

    public static void ClearGameObjectUnderWindow(uint windowId)
    {
        var window = PanelService.instance.GetWindow(windowId);
        if (window == null)
        {
            return;
        }

        for (var i = window.transform.childCount - 1; i >= 0; i--)
        {
            var child = window.transform.GetChild(i).gameObject;
            Object.DestroyImmediate(child);
        }
    }

    public static string LoadTextAsset(string filepath)
    {
        var textAsset = ResourceService.LoadFromResources<TextAsset>(filepath);
        if (textAsset != null)
        {
            return textAsset.text;
        }

        return string.Empty;
    }
}