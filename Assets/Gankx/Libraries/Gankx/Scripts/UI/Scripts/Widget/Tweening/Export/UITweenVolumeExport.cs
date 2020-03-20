using Gankx.UI;

/// <summary>
/// 导出给Lua使用UITweenVolume相关的接口
/// </summary>
public class UITweenVolumeExport : UITweenerExport<TweenVolume>
{
    public static void SetTo(uint windowId, float to)
    {
        TweenVolume uiTweener = PanelService.GetWindowComponent<TweenVolume>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.to = to;
    }

    public static void SetFrom(uint windowId, float from)
    {
        TweenVolume uiTweener = PanelService.GetWindowComponent<TweenVolume>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.from = from;
    }

    public static void Reset(uint windowId, float duration, float from, float to)
    {
        TweenVolume uiTweener = PanelService.GetWindowComponent<TweenVolume>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.duration = duration;
        
        uiTweener.from = from;
        uiTweener.to = to;

        uiTweener.ResetToBeginning();
    }

    public static void SetFromTo(uint windowId, float from, float to)
    {
        TweenVolume uiTweener = PanelService.GetWindowComponent<TweenVolume>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.from = from;
        uiTweener.to = to;
    }

    public static float GetFrom(uint windowId)
    {
        TweenVolume uiTweener = PanelService.GetWindowComponent<TweenVolume>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.from;
    }

    public static float GetTo(uint windowId)
    {
        TweenVolume uiTweener = PanelService.GetWindowComponent<TweenVolume>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.to;
    }
}