using System.Collections;
using System.Collections.Generic;
using Gankx;
using Gankx.UI;
using UnityEngine;

public class AnimationFinishHandler : MonoBehaviour
{
    private Window window;

    void CacheWindow()
    {
        if (null == window)
        {
            window = Window.GetWindow(gameObject);
        }
    }

    void OnAnimationFinish(string key)
    {
        CacheWindow();        
        if (null == window)
        {
            return;
        }

        LuaService.instance.FireEvent(
            "OnPanelMessage", window.panelId, window.id, "OnAnimationFinished", key);
    }
}
