using Gankx;
using Gankx.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelServiceExport
{
    public static uint INVALID_ID = Window.InvalidId;

    public static uint LoadPanel(string panelName, string panelPath)
    {
        if (null == PanelService.instance)
        {
            return INVALID_ID;
        }

        var panelId = PanelService.instance.LoadPanel(panelName, panelPath);

        return panelId;
    }

    public static bool LoadPanelAsync(string panelName, string panelPath)
    {
        if (null == PanelService.instance)
        {
            return false;
        }

        return PanelService.instance.LoadPanelAsync(panelName, panelPath);
    }

    public static void CancelLoadPanelAsync(string panelName)
    {
        if (null == PanelService.instance)
        {
            return;
        }

        PanelService.instance.CancelLoadPanelAsync(panelName);
    }

    public static bool UnloadPanel(string panelName)
    {
        if (null == PanelService.instance)
        {
            return false;
        }

        return PanelService.instance.UnloadPanel(panelName);
    }

    public static void CachePanel(string panelName, string panelPath)
    {
        if (null == PanelService.instance)
        {
            return;
        }

        PanelService.instance.CachePanel(panelName, panelPath);
    }

    public static int GetScreenHeight(bool considerRT)
    {
        if (ScreenRTMgr.instance.screenRT != null && considerRT)
        {
            return ScreenRTMgr.instance.screenRT.height;
        }

        return Screen.height;
    }

    public static int GetScreenWidth(bool considerRT)
    {
        if (ScreenRTMgr.instance.screenRT != null && considerRT)
        {
            return ScreenRTMgr.instance.screenRT.width;
        }

        return Screen.width;
    }

    public static float GetCanvasHeight()
    {
        return UIScreenAdaptor.GetCanvasHeigth();
    }

    public static float GetCanvasWidth()
    {
        return UIScreenAdaptor.GetCanvasWidth();
    }

    public static int GetScreenSafeWidth()
    {
        var m_safeRect = PanelService.SafeAreaRect;
        return (int) (GetScreenWidth(false) * (m_safeRect.width - m_safeRect.x));
    }

    public static float GetCanvasSafeWidth()
    {
        var m_safeRect = PanelService.SafeAreaRect;
        return UIScreenAdaptor.GetCanvasWidth() * (m_safeRect.width - m_safeRect.x);
    }

    public static void ShowGroup(int group)
    {
        var layerType = (PanelLayerType) group;
        PanelService.instance.ShowLayer(layerType);
    }

    public static void SetAFKState(bool bAFK)
    {
        PanelService.instance.SetAfkState(bAFK);
    }

    public static void HideGroup(int group)
    {
        var layerType = (PanelLayerType) group;
        PanelService.instance.HideLayer(layerType);
    }

    public static void CheckAtlasDependency(string panelName, string packtag)
    {
        var go = Object.Instantiate(ResourceService.Load<GameObject>("ui/panel/" + panelName));
        var images = go.GetComponentsInChildren<Image>(true);
        foreach (var image in images)
        {
            var name = image.mainTexture.name;
            if (name.Contains(packtag) || name.Contains("common") || !name.Contains("SpriteAtlasTexture"))
            {
                continue;
            }

            Debug.LogError(
                string.Format("WrongDep panel:{0} image:{1} altas:{2}", panelName, image.gameObject.name, name),
                image.gameObject);
        }
    }

    public static void ChangeParent(uint windowId, string path)
    {
        var obj = PanelService.GetWindowObject(windowId);
        if (null == obj)
        {
            return;
        }

        var parent = GameObject.Find(path);
        if (parent != null)
        {
            obj.transform.SetParent(parent.transform, false);
        }
    }

    public static void ReaddPanel(uint windowId, int group, int sortOrder, int depth)
    {
        var obj = PanelService.GetWindowObject(windowId);
        if (null == obj)
        {
            return;
        }

        var panelLayer = PanelService.instance.GetLayer((PanelLayerType) group);
        if (null == panelLayer)
        {
            return;
        }

        var panelControl = obj.GetOrAddComponent<PanelControl>();
        panelControl.layer = (PanelLayerType) group;
        panelControl.sortOrder = sortOrder;
        panelControl.depth = depth;

        panelLayer.ReaddPanel(obj, sortOrder, depth);
    }

    public static int GetPanelGroup(uint windowId)
    {
        var obj = PanelService.GetWindowObject(windowId);
        if (null == obj)
        {
            return (int) PanelLayerType.Invalid;
        }

        var panelControl = obj.GetComponent<PanelControl>();
        if (null != panelControl)
        {
            return (int) panelControl.layer;
        }

        return (int) PanelLayerType.Foreground;
    }

    public static int GetPanelSortOrder(uint windowId)
    {
        var obj = PanelService.GetWindowObject(windowId);
        if (null == obj)
        {
            return 0;
        }

        var panelControl = obj.GetComponent<PanelControl>();
        if (null != panelControl)
        {
            return panelControl.sortOrder;
        }

        return 0;
    }

    public static int GetPanelDepth(uint windowId)
    {
        var obj = PanelService.GetWindowObject(windowId);
        if (null == obj)
        {
            return 0;
        }

        var panelControl = obj.GetComponent<PanelControl>();
        if (null != panelControl)
        {
            return panelControl.depth;
        }

        return 0;
    }
}