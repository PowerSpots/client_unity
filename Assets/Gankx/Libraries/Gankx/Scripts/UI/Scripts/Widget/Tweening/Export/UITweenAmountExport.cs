using Gankx.UI;

/// <summary>
/// 导出给Lua使用UITweenAmount相关的接口
/// </summary>
public class UITweenAmountExport : UITweenerExport<TweenAmount>
{
    public static void SetTo(uint windowId, float to)
    {
        TweenAmount uiTweener = PanelService.GetWindowComponent<TweenAmount>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.to = to;
    }

    public static void SetFrom(uint windowId, float from)
    {
        TweenAmount uiTweener = PanelService.GetWindowComponent<TweenAmount>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.from = from;
    }

    public static void SetValue(uint windowId, float value)
    {
        TweenAmount uiTweener = PanelService.GetWindowComponent<TweenAmount>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.value = value;
    }

    public static float GetValue(uint windowId)
    {
        TweenAmount uiTweener = PanelService.GetWindowComponent<TweenAmount>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.value;
    }

    public static void Reset(uint windowId, float duration, float from, float to)
    {
        TweenAmount uiTweener = PanelService.GetWindowComponent<TweenAmount>(windowId);
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
        TweenAmount uiTweener = PanelService.GetWindowComponent<TweenAmount>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.from = from;
        uiTweener.to = to;
    }

    public static float GetFrom(uint windowId)
    {
        TweenAmount uiTweener = PanelService.GetWindowComponent<TweenAmount>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.from;
    }

    public static float GetTo(uint windowId)
    {
        TweenAmount uiTweener = PanelService.GetWindowComponent<TweenAmount>(windowId);
        if (null == uiTweener)
        {
            return 0.0f;
        }

        return uiTweener.to;
    }
}