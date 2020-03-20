using Gankx.UI;
using UnityEngine.UI;
using UnityEngine;

public static class UICanvasGroupExport
{
    public static void SetAlpha(uint windowId, float value)
    {
        CanvasGroup canvasGroup = PanelService.GetWindowComponent<CanvasGroup>(windowId);
        if (null == canvasGroup)
        {
            return;
        }

        canvasGroup.alpha = value;
    }

    public static float GetAlpha(uint windowId)
    {
        CanvasGroup canvasGroup = PanelService.GetWindowComponent<CanvasGroup>(windowId);
        if (null == canvasGroup)
        {
            return 1;
        }

        return canvasGroup.alpha;
    }

}