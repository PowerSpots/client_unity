using System.Collections.Generic;
using Gankx.UI;
using UnityEngine.UI;

public static class UIDropdownExport
{
    public static void SetInteractable(uint windowId, bool isInteractable)
    {
        Dropdown dropdown = PanelService.GetWindowComponent<Dropdown>(windowId);
        if (null == dropdown)
        {
            return;
        }

        dropdown.interactable = isInteractable;
    }

    public static bool GetInteractable(uint windowId)
    {
        Dropdown dropdown = PanelService.GetWindowComponent<Dropdown>(windowId);
        if (null == dropdown)
        {
            return false;
        }
        return dropdown.interactable;
    }

    public static void SetValue(uint windowId, int value)
    {
        Dropdown dropdown = PanelService.GetWindowComponent<Dropdown>(windowId);
        if (null == dropdown)
        {
            return;
        }
        
        dropdown.value = value;
    }

    public static int GetValue(uint windowId)
    {
        Dropdown dropdown = PanelService.GetWindowComponent<Dropdown>(windowId);
        if (null == dropdown)
        {
            return -1;
        }
                
        return dropdown.value;
    }

    public static string GetText(uint windowId)
    {
        Dropdown dropdown = PanelService.GetWindowComponent<Dropdown>(windowId);
        if (null == dropdown)
        {
            return "";
        }

        if (dropdown.options.Count <= dropdown.value)
        {
            return "";
        }

        return dropdown.options[dropdown.value].text;        
    }

    public static void AddOptions(uint windowId, List<string> options)
    {
        Dropdown dropdown = PanelService.GetWindowComponent<Dropdown>(windowId);
        if (null == dropdown)
        {
            return;
        }

        dropdown.AddOptions(options);
    }

    public static void ClearOptions(uint windowId)
    {
        Dropdown dropdown = PanelService.GetWindowComponent<Dropdown>(windowId);
        if (null == dropdown)
        {
            return;
        }

        dropdown.ClearOptions();
    }

    public static void RefreshShownValue(uint windowId)
    {
        Dropdown dropdown = PanelService.GetWindowComponent<Dropdown>(windowId);
        if (null == dropdown)
        {
            return;
        }

        dropdown.RefreshShownValue();
    }

    public static void Show(uint windowId)
    {
        Dropdown dropdown = PanelService.GetWindowComponent<Dropdown>(windowId);
        if (null == dropdown)
        {
            return;
        }

        dropdown.Show();
    }

    public static void Hide(uint windowId)
    {
        Dropdown dropdown = PanelService.GetWindowComponent<Dropdown>(windowId);
        if (null == dropdown)
        {
            return;
        }

        dropdown.Hide();
    }
}