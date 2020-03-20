using System.Collections;
using System.Collections.Generic;
using Gankx.UI;
using UnityEngine;
using UnityEngine.UI;

public class LayoutGrouptContentSizeFitterExport
{   
    public static void UpdateAll(uint windowId)
    {
        Window window = PanelService.instance.GetWindow(windowId);
        if (null == window)
        {
            Debug.LogError("Cannot find window, windowId: " + windowId);
            return;
        }

        ContentSizeFitter contentSizeFitter = ((Component) window).GetComponent<ContentSizeFitter>();
        if (null == contentSizeFitter)
        {
            Debug.LogError("Cannot Find Content Size Fitter", window.gameObject);
            return;
        }

        LayoutGroup layoutGroup = ((Component) window).GetComponent<LayoutGroup>();
        if (null == layoutGroup)
        {
            Debug.LogError("Cannot Find LayoutGroup", window.gameObject);
            return;
        }        

        layoutGroup.SetLayoutHorizontal();
        layoutGroup.SetLayoutVertical();
        contentSizeFitter.SetLayoutHorizontal();
        contentSizeFitter.SetLayoutVertical();
    }
}
