using Gankx.UI;
using UnityEngine.UI;

public static class LayoutElementExport
{
    public static void SetMinHeight(uint windowId, float height, bool forceSelf = false)
    {
        var layoutElement = PanelService.GetWindowComponent<LayoutElement>(windowId, forceSelf);
        if (null == layoutElement)
        {
            return;
        }

        layoutElement.minHeight = height;
    }

    public static void SetMinWidth(uint windowId, float width, bool forceSelf = false)
    {
        var layoutElement = PanelService.GetWindowComponent<LayoutElement>(windowId, forceSelf);
        if (null == layoutElement)
        {
            return;
        }

        layoutElement.minWidth = width;
    }

    public static void SetPreferredHeight(uint windowId, float height, bool forceSelf = false)
    {
        var layoutElement = PanelService.GetWindowComponent<LayoutElement>(windowId, forceSelf);
        if (null == layoutElement)
        {
            return;
        }

        layoutElement.preferredHeight = height;
    }

    public static void SetPreferredWidth(uint windowId, float width, bool forceSelf = false)
    {
        var layoutElement = PanelService.GetWindowComponent<LayoutElement>(windowId, forceSelf);
        if (null == layoutElement)
        {
            return;
        }

        layoutElement.preferredWidth = width;
    }

    public static float GetPreferredWidth(uint windowId)
    {
        var layoutElement = PanelService.GetWindowComponent<LayoutElement>(windowId);
        if (null == layoutElement)
        {
            return 0;
        }

        return layoutElement.preferredWidth;
    }

    public static float GetPreferredHeight(uint windowId)
    {
        var layoutElement = PanelService.GetWindowComponent<LayoutElement>(windowId);
        if (null == layoutElement)
        {
            return 0;
        }

        return layoutElement.preferredHeight;
    }
}