using Gankx.UI;
using UnityEngine.UI;

public static class UIButtonExport
{
    public static void SetInteractable(uint windowId, bool isInteractable)
    {
        Button btn = PanelService.GetWindowComponent<Button>(windowId);
        if (null == btn)
        {
            return;
        }

        btn.interactable = isInteractable;
    }

    public static bool GetInteractable(uint windowId)
    {
        Button btn = PanelService.GetWindowComponent<Button>(windowId);

        if (null == btn)
        {
            return false;
        }

        return btn.interactable;
    }

    public static void SetGray(uint windowId, bool state)
    {
        var btn = PanelService.GetWindowComponent<Button>(windowId);

        if (null == btn)
        {
            return;
        }

        var uiImage = btn.targetGraphic as MaskableGraphic;

        if (null == uiImage)
        {
            return;
        }

        var grayEffect = uiImage.GetOrAddComponent<UIImageGrayEffect>();
        grayEffect.Gray = state;
    }
}