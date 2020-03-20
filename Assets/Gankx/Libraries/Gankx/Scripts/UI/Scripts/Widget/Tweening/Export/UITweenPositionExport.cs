using Gankx.UI;

/// <summary>
/// 导出给Lua使用UITweenPosition相关的接口
/// </summary>
public class UITweenPositionExport : UITweenerExport<TweenPosition>
{
    public static void SetTo(uint windowId, float x, float y, float z)
    {
        TweenPosition uiTweener = PanelService.GetWindowComponent<TweenPosition>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.to.x = x;
        uiTweener.to.y = y;
        uiTweener.to.z = z;
    }

    public static void SetFrom(uint windowId, float x, float y, float z)
    {
        TweenPosition uiTweener = PanelService.GetWindowComponent<TweenPosition>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.from.x = x;
        uiTweener.from.y = y;
        uiTweener.from.z = z;
    }

    public static void Reset(uint windowId, float duration, float xFrom, float yFrom, float zFrom, float xTo, float yTo,
        float zTo)
    {
        TweenPosition uiTweener = PanelService.GetWindowComponent<TweenPosition>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.duration = duration;

        uiTweener.from.x = xFrom;
        uiTweener.from.y = yFrom;
        uiTweener.from.z = zFrom;

        uiTweener.to.x = xTo;
        uiTweener.to.y = yTo;
        uiTweener.to.z = zTo;

        uiTweener.ResetToBeginning();
    }

    public static void SetFromTo(uint windowId, float xFrom, float yFrom, float zFrom, float xTo, float yTo, float zTo)
    {
        TweenPosition uiTweener = PanelService.GetWindowComponent<TweenPosition>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.from.x = xFrom;
        uiTweener.from.y = yFrom;
        uiTweener.from.z = zFrom;

        uiTweener.to.x = xTo;
        uiTweener.to.y = yTo;
        uiTweener.to.z = zTo;
    }

    public static float GetXFrom(uint windowId)
    {
        TweenPosition uiTweener = PanelService.GetWindowComponent<TweenPosition>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.from.x;
    }

    public static float GetYFrom(uint windowId)
    {
        TweenPosition uiTweener = PanelService.GetWindowComponent<TweenPosition>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.from.y;
    }

    public static float GetZFrom(uint windowId)
    {
        TweenPosition uiTweener = PanelService.GetWindowComponent<TweenPosition>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.from.z;
    }

    public static float GetXTo(uint windowId)
    {
        TweenPosition uiTweener = PanelService.GetWindowComponent<TweenPosition>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.to.x;
    }

    public static float GetYTo(uint windowId)
    {
        TweenPosition uiTweener = PanelService.GetWindowComponent<TweenPosition>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.to.y;
    }

    public static float GetZTo(uint windowId)
    {
        TweenPosition uiTweener = PanelService.GetWindowComponent<TweenPosition>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.to.z;
    }

    public static void SetXFrom(uint windowId, float xFrom)
    {
        TweenPosition uiTweener = PanelService.GetWindowComponent<TweenPosition>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.from.x = xFrom;
    }

    public static void SetYFrom(uint windowId, float yFrom)
    {
        TweenPosition uiTweener = PanelService.GetWindowComponent<TweenPosition>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.from.y = yFrom;
    }

    public static void SetZFrom(uint windowId, float zFrom)
    {
        TweenPosition uiTweener = PanelService.GetWindowComponent<TweenPosition>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.from.z = zFrom;
    }

    public static void SetXTo(uint windowId, float xTo)
    {
        TweenPosition uiTweener = PanelService.GetWindowComponent<TweenPosition>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.to.x = xTo;
    }

    public static void SetYTo(uint windowId, float yTo)
    {
        TweenPosition uiTweener = PanelService.GetWindowComponent<TweenPosition>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.to.y = yTo;
    }

    public static void SetZTo(uint windowId, float zTo)
    {
        TweenPosition uiTweener = PanelService.GetWindowComponent<TweenPosition>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.to.z = zTo;
    }
}