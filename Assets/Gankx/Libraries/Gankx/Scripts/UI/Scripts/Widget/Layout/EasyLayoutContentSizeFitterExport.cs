using System.Collections;
using System.Collections.Generic;
using Gankx.UI;
using UnityEngine;
using UnityEngine.UI;

public class EasyLayoutContentSizeFitterExport
{
    public static void UpdateAll(uint windowId)
    {
        EasyLayoutContentSizeFitter comp = PanelService.GetWindowComponent<EasyLayoutContentSizeFitter>(windowId);
        if (null == comp)
        {
            return;
        }

        comp.UpdateAll();
    }

    public static float GetSpacingY(uint windowId) {
        EasyLayout.EasyLayout comp = PanelService.GetWindowComponent<EasyLayout.EasyLayout>(windowId);
        if (null == comp) {
            return 0;
        }

        return comp.Spacing.y;
    }

    public static float GetMarginY(uint windowId) {
        EasyLayout.EasyLayout comp = PanelService.GetWindowComponent<EasyLayout.EasyLayout>(windowId);
        if (null == comp) {
            return 0;
        }

        return comp.Margin.y;
    }
}
