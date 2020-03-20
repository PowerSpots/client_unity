using Gankx.UI;
using UnityEngine.UI;

public static class UISliderExport
{
    public static void SetInteractable(uint windowId, bool isInteractable)
    {
        Slider slider = PanelService.GetWindowComponent<Slider>(windowId);
        if (null == slider)
        {
            return;
        }

        slider.interactable = isInteractable;
    }

    public static bool GetInteractable(uint windowId)
    {
        Slider slider = PanelService.GetWindowComponent<Slider>(windowId);
        if (null == slider)
        {
            return false;
        }

        return slider.interactable;
    }

    public static void SetValue(uint windowId, float value)
    {
        Slider slider = PanelService.GetWindowComponent<Slider>(windowId);
        if (null == slider)
        {
            return;
        }

        slider.value = value;
    }

    public static float GetValue(uint windowId)
    {
        Slider slider = PanelService.GetWindowComponent<Slider>(windowId);
        if (null == slider)
        {
            return 0;
        }

        return slider.value;
    }

    public static void  SetEventEnable(uint windowId , bool enable)
    {
        Slider slider = PanelService.GetWindowComponent<Slider>(windowId);
        if (null == slider)
        {
            return ;
        }

        SliderChangeDispatcher dispatcher = slider.transform.GetComponentInChildren<SliderChangeDispatcher>();
        if (dispatcher != null)
        {
            dispatcher.EventEnable = enable;
        }
    }
}