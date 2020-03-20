using Gankx;
using Gankx.UI;
using UnityEngine;
using UnityEngine.UI;

public static class UISelectableExport
{   
    public static void SetInteractable(uint windowId, bool isInteractable)
    {
        Selectable selectable = PanelService.GetWindowComponent<Selectable>(windowId);
        if (null == selectable)
        {
            Debug.LogError(string.Format("{0}.SetInteractable: No Selectable Component is found!!!", PanelService.GetWindowPath(windowId)));
            return;
        }

        selectable.interactable = isInteractable;        
    }

    public static bool GetInteractable(uint windowId)
    {
        Selectable selectable = PanelService.GetWindowComponent<Selectable>(windowId);
        if (null == selectable)
        {
            Debug.LogError(string.Format("{0}.GetInteractable: No Selectable Component is found!!!", PanelService.GetWindowPath(windowId)));
            return false;
        }        

        return selectable.interactable;
    }
}
