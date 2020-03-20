using Gankx.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GroupColorEffectExport
{
    public static void SetDarkColor(uint windowId, float r, float g, float b, float a)
    {
        var groupColorEffect = PanelService.GetWindowComponent<GroupColorEffect>(windowId);
        if (null == groupColorEffect)
        {
            return;
        }

        groupColorEffect.SetDarkColor(r, g, b, a);
    }

    public static void SetDark(uint windowId, bool isDark)
    {
        var groupColorEffect = PanelService.GetWindowComponent<GroupColorEffect>(windowId);
        if (null == groupColorEffect)
        {
            return;
        }

        groupColorEffect.SetDark(isDark);
    }
}
