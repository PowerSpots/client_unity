using Gankx.UI;
using UnityEngine;

public class UIAnchorExport
{
    public static void SetAnchorTarget(uint windowId, uint targetWindowId)
    {
        UIAnchor uiAnchor = PanelService.GetWindowComponent<UIAnchor>(windowId);
        if (null == uiAnchor)
        {
            return;
        }

        RectTransform targetRectTrans = PanelService.GetWindowComponent<RectTransform>(targetWindowId);
        if (null == targetRectTrans)
        {
            return;
        }

        uiAnchor.AnchorTarget = targetRectTrans;
        uiAnchor.DoAnchor();
    }
}
