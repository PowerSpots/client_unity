using Gankx.UI;
using UnityEngine;
using UnityEngine.UI;

public class ContentSizeFitterExport {

    public static void SetLayoutVertical(uint windowId)
    {
        var contentSizeFitter = PanelService.GetWindowComponent<ContentSizeFitter>(windowId);

        if (null == contentSizeFitter)
        {
            return;
        }

        contentSizeFitter.SetLayoutVertical();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentSizeFitter.GetComponent<RectTransform>());
    }

    public static void SetLayoutHorizontal(uint windowId)
    {
        var contentSizeFitter = PanelService.GetWindowComponent<ContentSizeFitter>(windowId);

        if (null == contentSizeFitter)
        {
            return;
        }

        contentSizeFitter.SetLayoutHorizontal();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentSizeFitter.GetComponent<RectTransform>());

    }

    public static void SetLayoutImmediate(uint windowId)
    {
        var contentSizeFitter = PanelService.GetWindowComponent<ContentSizeFitter>(windowId);

        if (null == contentSizeFitter)
        {
            return;
        }

        contentSizeFitter.SetLayoutHorizontal();
        contentSizeFitter.SetLayoutVertical();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentSizeFitter.GetComponent<RectTransform>());
    }
}
