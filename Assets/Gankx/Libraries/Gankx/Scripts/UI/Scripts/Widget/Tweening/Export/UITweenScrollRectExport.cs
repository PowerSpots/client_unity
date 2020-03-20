using UnityEngine;
using System.Collections;
using Gankx.UI;

public class UITweenScrollRectExport : UITweenerExport<TweenScrollRect>
{
    public static void SetToX(uint windowId, float to)
    {
        TweenScrollRect uiTweener = PanelService.GetWindowComponent<TweenScrollRect>(windowId);
        if (null == uiTweener) return;
        uiTweener.toX = to;
    }

    public static void SetFromX(uint windowId, float from)
    {
        TweenScrollRect uiTweener = PanelService.GetWindowComponent<TweenScrollRect>(windowId);
        if (null == uiTweener) return;
        uiTweener.fromX = from;
    }
    public static void SetToY(uint windowId, float to)
    {
        TweenScrollRect uiTweener = PanelService.GetWindowComponent<TweenScrollRect>(windowId);
        if (null == uiTweener) return;
        uiTweener.toY = to;
    }

    public static void SetFromY(uint windowId, float from)
    {
        TweenScrollRect uiTweener = PanelService.GetWindowComponent<TweenScrollRect>(windowId);
        if (null == uiTweener) return;
        uiTweener.fromY = from;
    }

    public static void SetFromToX(uint windowId, float from, float to)
    {
        TweenScrollRect uiTweener = PanelService.GetWindowComponent<TweenScrollRect>(windowId);
        if (null == uiTweener) return;
        uiTweener.fromX = from;
        uiTweener.toX = to;
    }

    public static void SetFromToY(uint windowId, float from, float to)
    {
        TweenScrollRect uiTweener = PanelService.GetWindowComponent<TweenScrollRect>(windowId);
        if (null == uiTweener) return;
        uiTweener.fromY = from;
        uiTweener.toY = to;
    }

    public static float GetFromX(uint windowId)
    {
        TweenScrollRect uiTweener = PanelService.GetWindowComponent<TweenScrollRect>(windowId);
        if (null == uiTweener) return 0.0f;

        return uiTweener.fromX;
    }

    public static float GetToX(uint windowId)
    {
        TweenScrollRect uiTweener = PanelService.GetWindowComponent<TweenScrollRect>(windowId);
        if (null == uiTweener) return 0.0f;

        return uiTweener.toX;
    }
    public static float GetFromY(uint windowId)
    {
        TweenScrollRect uiTweener = PanelService.GetWindowComponent<TweenScrollRect>(windowId);
        if (null == uiTweener) return 0.0f;

        return uiTweener.fromY;
    }

    public static float GetToY(uint windowId)
    {
        TweenScrollRect uiTweener = PanelService.GetWindowComponent<TweenScrollRect>(windowId);
        if (null == uiTweener) return 0.0f;

        return uiTweener.toY;
    }
}
