using Gankx.UI;

/// <summary>
/// 导出给Lua使用UITweenWidth相关的接口
/// </summary>
public class UITweenWidthExport : UITweenerExport<TweenWidth>
{
    public static void SetTo(uint windowId, int to)
    {
        TweenWidth uiTweener = PanelService.GetWindowComponent<TweenWidth>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.to = to;
    }

    public static void SetFrom(uint windowId, int from)
    {
        TweenWidth uiTweener = PanelService.GetWindowComponent<TweenWidth>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.from = from;
    }

    public static void Reset(uint windowId, int duration, int from, int to)
    {
        TweenWidth uiTweener = PanelService.GetWindowComponent<TweenWidth>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.duration = duration;

        uiTweener.from = from;
        uiTweener.to = to;

        uiTweener.ResetToBeginning();
    }

    public static void SetFromTo(uint windowId, int from, int to)
    {
        TweenWidth uiTweener = PanelService.GetWindowComponent<TweenWidth>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.from = from;
        uiTweener.to = to;
    }

    public static int GetFrom(uint windowId)
    {
        TweenWidth uiTweener = PanelService.GetWindowComponent<TweenWidth>(windowId);
        if (null == uiTweener)
        {
            return 0;
        }

        return (int) uiTweener.from;
    }

    public static int GetTo(uint windowId)
    {
        TweenWidth uiTweener = PanelService.GetWindowComponent<TweenWidth>(windowId);
        if (null == uiTweener)
        {
            return 0;
        }

        return (int) uiTweener.to;
    }
}