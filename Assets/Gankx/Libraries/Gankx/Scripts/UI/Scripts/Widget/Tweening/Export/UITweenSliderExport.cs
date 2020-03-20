using Gankx.UI;

/// <summary>
/// 导出给Lua使用UITweenSlider相关的接口
/// </summary>
public class UITweenSliderExport : UITweenerExport<TweenSlider>
{
    public static void SetTo(uint windowId, float to)
    {
        TweenSlider uiTweener = PanelService.GetWindowComponent<TweenSlider>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.to = to;
    }

    public static void SetFrom(uint windowId, float from)
    {
        TweenSlider uiTweener = PanelService.GetWindowComponent<TweenSlider>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.from = from;
    }

    public static void SetValue(uint windowId, float value)
    {
        TweenSlider uiTweener = PanelService.GetWindowComponent<TweenSlider>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.value = value;
    }

    public static void Reset(uint windowId, float duration, float from, float to)
    {
        TweenSlider uiTweener = PanelService.GetWindowComponent<TweenSlider>(windowId);
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
        TweenSlider uiTweener = PanelService.GetWindowComponent<TweenSlider>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.from = from;
        uiTweener.to = to;
    }

    public static float GetFrom(uint windowId)
    {
        TweenSlider uiTweener = PanelService.GetWindowComponent<TweenSlider>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.from;
    }

    public static float GetTo(uint windowId)
    {
        TweenSlider uiTweener = PanelService.GetWindowComponent<TweenSlider>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.to;
    }

    public static float GetValue(uint windowId)
    {
        TweenSlider uiTweener = PanelService.GetWindowComponent<TweenSlider>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.value;
    }
}