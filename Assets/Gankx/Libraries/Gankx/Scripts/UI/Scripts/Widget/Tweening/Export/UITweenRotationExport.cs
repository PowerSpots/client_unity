using Gankx.UI;

/// <summary>
/// 导出给Lua使用UITweenRotation相关的接口
/// </summary>
public class UITweenRotationExport : UITweenerExport<TweenRotation>
{
    public static void SetTo(uint windowId, float x, float y, float z)
    {
        TweenRotation uiTweener = PanelService.GetWindowComponent<TweenRotation>(windowId);
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
        TweenRotation uiTweener = PanelService.GetWindowComponent<TweenRotation>(windowId);
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
        TweenRotation uiTweener = PanelService.GetWindowComponent<TweenRotation>(windowId);
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
        TweenRotation uiTweener = PanelService.GetWindowComponent<TweenRotation>(windowId);
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
        TweenRotation uiTweener = PanelService.GetWindowComponent<TweenRotation>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.from.x;
    }

    public static float GetYFrom(uint windowId)
    {
        TweenRotation uiTweener = PanelService.GetWindowComponent<TweenRotation>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.from.y;
    }

    public static float GetZFrom(uint windowId)
    {
        TweenRotation uiTweener = PanelService.GetWindowComponent<TweenRotation>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.from.z;
    }

    public static float GetXTo(uint windowId)
    {
        TweenRotation uiTweener = PanelService.GetWindowComponent<TweenRotation>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.to.x;
    }

    public static float GetYTo(uint windowId)
    {
        TweenRotation uiTweener = PanelService.GetWindowComponent<TweenRotation>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.to.y;
    }

    public static float GetZTo(uint windowId)
    {
        TweenRotation uiTweener = PanelService.GetWindowComponent<TweenRotation>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.to.z;
    }
}