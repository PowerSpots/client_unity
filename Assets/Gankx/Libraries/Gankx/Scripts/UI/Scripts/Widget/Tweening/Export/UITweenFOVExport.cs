using Gankx.UI;

/// <summary>
/// Export all UITweenFOV interfaces to lua
/// </summary>
public class UITweenFOVExport : UITweenerExport<TweenFOV>
{
    public static void SetTo(uint windowId, float to)
    {
        TweenFOV uiTweener = PanelService.GetWindowComponent<TweenFOV>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.to = to;
    }

    public static void SetFrom(uint windowId, float from)
    {
        TweenFOV uiTweener = PanelService.GetWindowComponent<TweenFOV>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.from = from;
    }

    public static void Reset(uint windowId, float duration, float from, float to)
    {
        TweenFOV uiTweener = PanelService.GetWindowComponent<TweenFOV>(windowId);
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
        TweenFOV uiTweener = PanelService.GetWindowComponent<TweenFOV>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.from = from;
        uiTweener.to = to;
    }

    public static float GetFrom(uint windowId)
    {
        TweenFOV uiTweener = PanelService.GetWindowComponent<TweenFOV>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.from;
    }

    public static float GetTo(uint windowId)
    {
        TweenFOV uiTweener = PanelService.GetWindowComponent<TweenFOV>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.to;
    }
}