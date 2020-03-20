using Gankx.UI;

/// <summary>
/// 导出给Lua使用UITweenColor相关的接口
/// </summary>
public class UITweenColorExport : UITweenerExport<TweenColor>
{
    public static void SetTo(uint windowId, float r, float g, float b, float a)
    {
        TweenColor uiTweener = PanelService.GetWindowComponent<TweenColor>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.to.r = r;
        uiTweener.to.g = g;
        uiTweener.to.b = b;
        uiTweener.to.a = a;
    }

    public static void SetFrom(uint windowId, float r, float g, float b, float a)
    {
        TweenColor uiTweener = PanelService.GetWindowComponent<TweenColor>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.from.r = r;
        uiTweener.from.g = g;
        uiTweener.from.b = b;
        uiTweener.from.a = a;
    }

    public static void Reset(uint windowId, float duration, float rFrom, float gFrom, float bFrom, float aFrom,
        float rTo, float gTo, float bTo, float aTo)
    {
        TweenColor uiTweener = PanelService.GetWindowComponent<TweenColor>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.duration = duration;

        uiTweener.from.r = rFrom;
        uiTweener.from.g = gFrom;
        uiTweener.from.b = bFrom;
        uiTweener.from.a = aFrom;

        uiTweener.to.r = rTo;
        uiTweener.to.g = gTo;
        uiTweener.to.b = bTo;
        uiTweener.to.a = aTo;

        uiTweener.ResetToBeginning();
    }

    public static void SetFromTo(uint windowId, float rFrom, float gFrom, float bFrom, float aFrom, float rTo, float gTo,
        float bTo, float aTo)
    {
        TweenColor uiTweener = PanelService.GetWindowComponent<TweenColor>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.from.r = rFrom;
        uiTweener.from.g = gFrom;
        uiTweener.from.b = bFrom;
        uiTweener.from.a = aFrom;

        uiTweener.to.r = rTo;
        uiTweener.to.g = gTo;
        uiTweener.to.b = bTo;
        uiTweener.to.a = aTo;
    }

    public static float GetRFrom(uint windowId)
    {
        TweenColor uiTweener = PanelService.GetWindowComponent<TweenColor>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.from.r;
    }

    public static float GetGFrom(uint windowId)
    {
        TweenColor uiTweener = PanelService.GetWindowComponent<TweenColor>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.from.g;
    }

    public static float GetBFrom(uint windowId)
    {
        TweenColor uiTweener = PanelService.GetWindowComponent<TweenColor>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }
        
        return uiTweener.from.b;
    }

    public static float GetAFrom(uint windowId)
    {
        TweenColor uiTweener = PanelService.GetWindowComponent<TweenColor>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.from.a;
    }

    public static float GetRTo(uint windowId)
    {
        TweenColor uiTweener = PanelService.GetWindowComponent<TweenColor>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.to.r;
    }

    public static float GetGTo(uint windowId)
    {
        TweenColor uiTweener = PanelService.GetWindowComponent<TweenColor>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.to.g;
    }

    public static float GetBTo(uint windowId)
    {
        TweenColor uiTweener = PanelService.GetWindowComponent<TweenColor>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.to.b;
    }

    public static float GetATo(uint windowId)
    {
        TweenColor uiTweener = PanelService.GetWindowComponent<TweenColor>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.to.a;
    }
}