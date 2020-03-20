using Gankx.UI;
using UnityEngine.UI;

public static class UIScrollbarExport
{
    public static void SetInteractable(uint windowId, bool isInteractable)
    {
        Scrollbar scrollbar = PanelService.GetWindowComponent<Scrollbar>(windowId);
        if (null == scrollbar)
        {
            return;
        }

        scrollbar.interactable = isInteractable;
    }

    public static bool GetInteractable(uint windowId)
    {
        Scrollbar scrollbar = PanelService.GetWindowComponent<Scrollbar>(windowId);
        if (null == scrollbar)
        {
            return false;
        }

        return scrollbar.interactable;
    }

    public static float GetValue(uint windowId)
    {
        Scrollbar scrollbar = PanelService.GetWindowComponent<Scrollbar>(windowId);
        if (null == scrollbar)
        {
            return 0f;
        }

        return scrollbar.value;
    }

    public static void SetValue(uint windowId, float value)
    {
        Scrollbar scrollbar = PanelService.GetWindowComponent<Scrollbar>(windowId);
        if (null == scrollbar)
        {
            return;
        }

        scrollbar.value = value;
    }
}