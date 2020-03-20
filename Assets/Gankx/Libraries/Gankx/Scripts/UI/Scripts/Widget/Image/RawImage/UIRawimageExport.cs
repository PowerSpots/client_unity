using System.Collections.Generic;
using Gankx;
using Gankx.UI;
using UnityEngine;
using UnityEngine.UI;

public static class UIRawimageExport
{
    private static readonly Dictionary<string, Texture2D> DefaultDictionary = new Dictionary<string, Texture2D>();

    public static void SetUrl(uint windowId, string url, string defaultAssetPath = "")
    {
        var urlLoader = PanelService.GetWindowComponent<RawImageUrlLoader>(windowId);
        if (null == urlLoader)
        {
            var obj = PanelService.GetWindowObject(windowId);
            Debug.LogError("RawImageUrlLoader component missing，WindowId:" + windowId + ", Path:" +
                           UITools.GetHierarchy(obj));
            return;
        }

        if (!string.IsNullOrEmpty(defaultAssetPath))
        {
            Texture2D tex;
            if (DefaultDictionary.ContainsKey(defaultAssetPath))
            {
                tex = DefaultDictionary[defaultAssetPath];
            }
            else
            {
                tex = ResourceService.Load<Texture2D>(defaultAssetPath);
                DefaultDictionary.Add(defaultAssetPath, tex);
            }

            urlLoader.supportDefault = true;
            urlLoader.defaultTexture = tex;
        }

        urlLoader.url = url;
    }

    public static void UnloadMemoryCache()
    {
        RawImageUrlTextureManager.instance.UnloadMemoryCache();
    }

    public static void CacheUrl(string url)
    {
        RawImageUrlTextureManager.instance.LoadTextureAsync(url, OnUrlTextureCached);
    }

    public static void OnUrlTextureCached(RawImageUrlTextureCacheMemory.MemoryEntry tex)
    {
#if UNITY_EDITOR
        Debug.Log(tex.url + " has been cached！");
#endif
    }

    public static void SetGray(uint windowId, bool state)
    {
        var uiImage = PanelService.GetWindowComponent<RawImage>(windowId);
        if (null == uiImage)
        {
            return;
        }

        var grayEffect = uiImage.GetOrAddComponent<UIImageGrayEffect>();
        grayEffect.Gray = state;
    }

    public static void LoadLocalTexture(uint windowId, string path, bool sync = false)
    {
        var uiImage = PanelService.GetWindowComponent<RawImage>(windowId);
        if (null == uiImage)
        {
            return;
        }

        var jpgloader = uiImage.GetComponent<RawImageLocalLoader>();
        if (null == jpgloader)
        {
            jpgloader = uiImage.gameObject.AddComponent<RawImageLocalLoader>();
        }

        jpgloader.LoadJpg(path, sync);
    }

    public static void LoadAssetTexture(uint windowId, string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("LoadRawImageFromInternnal Error: input is invalid");
            return;
        }

        var smartRawImage = PanelService.GetWindowComponent<SmartRawImage>(windowId);
        if (null != smartRawImage)
        {
            smartRawImage.texturePath = path;
            return;
        }

        var rawImage = PanelService.GetWindowComponent<RawImage>(windowId);
        if (null != rawImage)
        {
            rawImage.texture = ResourceService.Load<Texture>(path);
        }
    }

    public static void ClearTextureRef(uint windowId)
    {
        var uiImage = PanelService.GetWindowComponent<RawImage>(windowId);
        if (null == uiImage)
        {
            return;
        }

        uiImage.texture = null;

        var jpgloader = uiImage.GetComponent<RawImageLocalLoader>();
        if (null != jpgloader)
        {
            jpgloader.Clear();
        }
    }

    public static void SetMaterialFloat(uint windowId, string propety, float value)
    {
        var uiImage = PanelService.GetWindowComponent<Image>(windowId);
        var rawImage = PanelService.GetWindowComponent<RawImage>(windowId);
        if (null == uiImage && null == rawImage)
        {
            return;
        }

        var mat = null != uiImage ? uiImage.material : rawImage.material;

        if (mat != null)
        {
            mat.SetFloat(propety, value);
        }
    }

    public static void SetNativeSize(uint windowId)
    {
        var rawImage = PanelService.GetWindowComponent<RawImage>(windowId);
        if (null == rawImage)
        {
            return;
        }

        rawImage.SetNativeSize();
    }

    public static void DestroyTexture(uint windowId)
    {
        var rawImage = PanelService.GetWindowComponent<RawImage>(windowId);
        if (null == rawImage)
        {
            return;
        }

        Object.Destroy(rawImage.texture);
    }
}