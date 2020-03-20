using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gankx.UI;


public static class TypewriterEffectExport
{
    public static void Stop(uint windowId)
    {
        TypewriterEffect typewriter = PanelService.GetWindowComponent<TypewriterEffect>(windowId);
        if (null != typewriter)
        {
            typewriter.Finish();
        }
    }
}