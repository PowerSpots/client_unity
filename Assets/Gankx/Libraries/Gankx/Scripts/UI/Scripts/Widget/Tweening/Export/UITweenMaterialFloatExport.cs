using Gankx.UI;


public class UITweenMaterialFloatExport : UITweenerExport<TweenMaterialFloat>
{
    public static void SetTo(uint windowId, float to)
    {
        TweenMaterialFloat uiTweener = PanelService.GetWindowComponent<TweenMaterialFloat>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.to = to;
    }

    public static void SetFrom(uint windowId, float from)
    {
        TweenMaterialFloat uiTweener = PanelService.GetWindowComponent<TweenMaterialFloat>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.from = from;
    }

    public static void Reset(uint windowId, float duration, float from, float to, string key)
    {
        TweenMaterialFloat uiTweener = PanelService.GetWindowComponent<TweenMaterialFloat>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.duration = duration;
        uiTweener.from = from;
        uiTweener.to = to;
        uiTweener.Keyword = key;

        uiTweener.ResetToBeginning();
    }

    public static void SetFromTo(uint windowId, float from, float to, string key)
    {
        TweenMaterialFloat uiTweener = PanelService.GetWindowComponent<TweenMaterialFloat>(windowId);
        if (null == uiTweener)
        {
            return;
        }

        uiTweener.from = from;
        uiTweener.to = to;
        uiTweener.Keyword = key;
    }
    public static void Play(uint windowId)
    {
        TweenMaterialFloat uiTweener = PanelService.GetWindowComponent<TweenMaterialFloat>(windowId);
        if (null == uiTweener)
        {
            return;
        }
        uiTweener.value = uiTweener.from;
        uiTweener.enabled = true;
    }
}
