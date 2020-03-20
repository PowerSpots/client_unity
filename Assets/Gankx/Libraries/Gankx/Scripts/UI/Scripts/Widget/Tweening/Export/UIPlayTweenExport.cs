using Gankx.UI;
using Gankx.UI.AnimationOrTween;

public static class UIPlayTweenExport
{
    public static void Play(uint windowId, bool forward)
    {
        PlayTween playTween = PanelService.GetWindowComponent<PlayTween>(windowId);
        if (null != playTween)
        {
            playTween.Play(forward);
        }
    }

    public static void SetIncludeChildren(uint windowId, bool isIncludeChildren)
    {
        PlayTween playTween = PanelService.GetWindowComponent<PlayTween>(windowId);
        if (null != playTween)
        {
            playTween.includeChildren = isIncludeChildren;
        }
    }

    public static void SetPlayDirection(uint windowId, int direction)
    {
        PlayTween playTween = PanelService.GetWindowComponent<PlayTween>(windowId);
        if (null != playTween)
        {
            playTween.playDirection = (Direction) direction;
        }
    }

    public static void SetResetOnPlay(uint windowId, bool resetOnPlay)
    {
        PlayTween playTween = PanelService.GetWindowComponent<PlayTween>(windowId);
        if (null != playTween)
        {
            playTween.resetOnPlay = resetOnPlay;
        }
    }

    public static void SetResetIfDisabled(uint windowId, bool resetIfDisabled)
    {
        PlayTween playTween = PanelService.GetWindowComponent<PlayTween>(windowId);
        if (null != playTween)
        {
            playTween.resetIfDisabled = resetIfDisabled;
        }
    }

    public static void SetEnabled(uint windowId, bool enabled)
    {
        PlayTween playTween = PanelService.GetWindowComponent<PlayTween>(windowId);
        if (null != playTween)
        {
            playTween.enabled = enabled;
        }
    }

    public static void SetFinishedReceiver(uint windowId, bool value)
    {
        PlayTween playTween = PanelService.GetWindowComponent<PlayTween>(windowId);
        if (null == playTween)
        {
            return;
        }

#if COMPATIBILITY_NGUI
        //playTween.SetFinishedReceiver(value);
#endif
    }

    public static bool GetFinishedReceiver(uint windowId)
    {
        PlayTween playTween = PanelService.GetWindowComponent<PlayTween>(windowId);
        if (null == playTween)
        {
            return false;
        }

#if COMPATIBILITY_NGUI
        //        return playTween.GetFinishedReceiver();
#endif
        return false;
    }
}