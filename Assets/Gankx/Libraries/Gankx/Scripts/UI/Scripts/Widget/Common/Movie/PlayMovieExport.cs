using System.Collections;
using System.Collections.Generic;
using Gankx.UI;
using UnityEngine;

public static class PlayMovieExport  {

    public static void Play(uint windowId, string name)
    {
        CGPanel moviceCtrl = PanelService.GetWindowComponent<CGPanel>(windowId);
        if (null == moviceCtrl)
        {
            return;
        }

        moviceCtrl.Play(name);
    }

    public static void Stop(uint windowId)
    {
        CGPanel moviceCtrl = PanelService.GetWindowComponent<CGPanel>(windowId);
        if (null == moviceCtrl)
        {
            return;
        }

        moviceCtrl.OnPlayEnd();
    }

    public static bool IsFristFrameReady(uint windowId)
    {
        CGPanel moviceCtrl = PanelService.GetWindowComponent<CGPanel>(windowId);
        if (null == moviceCtrl)
        {
            return false;
        }

        return moviceCtrl.IsFirstFrameReady();
    }
}
