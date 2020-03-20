using Gankx.UI;
using UnityEngine;

public static class UICirclePolygonExport
{
    public static void SetBeginFillAmount(uint windowId, float fillAmount)
    {
        var uiImage = PanelService.GetWindowComponent<CirclePolygon>(windowId);
        if (null == uiImage)
        {
            return;
        }

        uiImage.beginFillPercent = fillAmount;
    }

    public static void SetEndFillAmount(uint windowId, float fillAmount)
    {
        var uiImage = PanelService.GetWindowComponent<CirclePolygon>(windowId);
        if (null == uiImage)
        {
            return;
        }

        uiImage.endFillPercent = fillAmount;
    }

    public static void SetSegments(uint windowId, int segments)
    {
        var uiImage = PanelService.GetWindowComponent<CirclePolygon>(windowId);
        if (null == uiImage)
        {
            return;
        }

        uiImage.segments = segments;
    }

    public static void SetSegmentFillAmount(uint windowId, int index, float fillAmount)
    {
        var uiImage = PanelService.GetWindowComponent<CirclePolygon>(windowId);
        if (null == uiImage)
        {
            return;
        }

        uiImage.SetSegmentFillAmount(index, fillAmount);
    }

    public static void SetStartAngle(uint windowId, float startAngle)
    {
        var uiImage = PanelService.GetWindowComponent<CirclePolygon>(windowId);
        if (null == uiImage)
        {
            return;
        }

        uiImage.startAngle = startAngle;
    }

    public static void SetUVOffset(uint windowId, float uvOffsetX, float uvOffsetY)
    {
        var uiImage = PanelService.GetWindowComponent<CirclePolygon>(windowId);
        if (null == uiImage)
        {
            return;
        }

        uiImage.SetUVOffset(new Vector2(uvOffsetX, uvOffsetY));
    }

    public static void SetVerticesDirty(uint windowId)
    {
        var uiImage = PanelService.GetWindowComponent<CirclePolygon>(windowId);
        if (null == uiImage)
        {
            return;
        }

        uiImage.SetVerticesDirty();
    }
}