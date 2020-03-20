using Gankx.UI;

public class UITweenerExport<T> where T : Tweener
{
    public static void Play(uint windowId, bool forward, bool toBeginning)
    {
        T uiTweener = PanelService.GetWindowComponent<T>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        if (toBeginning)
        {
            uiTweener.ResetToBeginning();
        }

        uiTweener.Play(forward);
    }

    public static void PlayFromCurrentValue(uint windowId, bool forward, bool toBeginning)
    {
        T uiTweener = PanelService.GetWindowComponent<T>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.SetStartToCurrentValue();

        if (toBeginning)
        {
            uiTweener.ResetToBeginning();
        }

        uiTweener.Play(forward);
    }

    public static void SetEnabled(uint windowId, bool enabled)
    {
        T uiTweener = PanelService.GetWindowComponent<T>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.enabled = enabled;
    }

    public static bool GetEnabled(uint windowId)
    {
        T uiTweener = PanelService.GetWindowComponent<T>(windowId);
        if (null == uiTweener)
        {
            return false;
        }

        return uiTweener.enabled;
    }

    public static void SetDuration(uint windowId, float duration)
    {
        T uiTweener = PanelService.GetWindowComponent<T>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.duration = duration;
    }

    public static float GetDuration(uint windowId)
    {
        T uiTweener = PanelService.GetWindowComponent<T>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.duration;
    }

    public static void SetDelay(uint windowId, float delay)
    {
        T uiTweener = PanelService.GetWindowComponent<T>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.delay = delay;
    }

    public static float GetDelay(uint windowId)
    {
        T uiTweener = PanelService.GetWindowComponent<T>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.delay;
    }

    public static void SetFinishedReceiver(uint windowId, bool value)
    {
        T uiTweener = PanelService.GetWindowComponent<T>(windowId);
        if (null == uiTweener)
        {
            return;
        }

#if COMPATIBILITY_NGUI
        uiTweener.SetFinishedReceiver(value);
#endif
    }

    public static bool GetFinishedReceiver(uint windowId)
    {
        T uiTweener = PanelService.GetWindowComponent<T>(windowId);
        if (null == uiTweener)
        {
            return false;
        }

#if COMPATIBILITY_NGUI
        return uiTweener.GetFinishedReceiver();
#else
        return false;
#endif
    }

    public static void SetStyle(uint windowId, uint styleVal)
    {
        T uiTweener = PanelService.GetWindowComponent<T>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.style = (Tweener.Style) styleVal;
    }
}