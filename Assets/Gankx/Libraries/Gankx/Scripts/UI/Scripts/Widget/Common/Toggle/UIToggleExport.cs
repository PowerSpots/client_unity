using Gankx.UI;
using UnityEngine.UI;

public static class UIToggleExport
{
    public static void SetInteractable(uint windowId, bool isInteractable)
    {
        Toggle toggle = PanelService.GetWindowComponent<Toggle>(windowId);
        if (null == toggle)
        {
            return;
        }

        toggle.interactable = isInteractable;
    }

    public static bool GetInteractable(uint windowId)
    {
        Toggle toggle = PanelService.GetWindowComponent<Toggle>(windowId);
        if (null == toggle)
        {
            return false;
        }
        
        return toggle.interactable;
    }

    public static void SetValue(uint windowId, bool value)
    {
        Toggle toggle = PanelService.GetWindowComponent<Toggle>(windowId);
        if (null == toggle)
        {
            return;
        }

        toggle.isOn = value;
    }

    public static bool GetValue(uint windowId)
    {
        Toggle toggle = PanelService.GetWindowComponent<Toggle>(windowId);
        if (null == toggle)
        {
            return false;
        }

        return toggle.isOn;
    }

    /// <summary>
    ///  重设toggle的togglegroup为父节点
    /// </summary>
    /// <param name="windowId"></param>
    public static void ResetToggleGroup(uint windowId)
    {
        var toggle = PanelService.GetWindowComponent<Toggle>(windowId);
        if (null == toggle)
        {
            return;
        }

        var parent = toggle.transform.parent;
        if (parent && toggle.GetComponent<SlotControlObject>())
        {
            parent = parent.parent;
        }
        toggle.group = parent.GetComponent<ToggleGroup>();

    }
}