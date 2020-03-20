using Gankx.UI;

public static class UICenterOnChildExport {

    public static void RefreshData(uint windowId) {
        UICenterOnChild centerOnChild = PanelService.GetWindowComponent<UICenterOnChild>(windowId);
        if (centerOnChild == null) {
            return;
        }

        centerOnChild.RefreshData();
    }

    public static void ScrollTo(uint windowId, int index) {
        UICenterOnChild centerOnChild = PanelService.GetWindowComponent<UICenterOnChild>(windowId);
        if (centerOnChild == null) {
            return;
        }

        centerOnChild.ScrollTo(index - 1);
    }

    public static void ForceScrollTo(uint windowId, int index)
    {
        UICenterOnChild centerOnChild = PanelService.GetWindowComponent<UICenterOnChild>(windowId);
        if (centerOnChild == null)
        {
            return;
        }

        centerOnChild.ForceScrollTo(index - 1);
    }

    public static void DelayScrollTo(uint windowId, int index) {
        UICenterOnChild centerOnChild = PanelService.GetWindowComponent<UICenterOnChild>(windowId);
        if (centerOnChild == null) {
            return;
        }

        centerOnChild.DelayScrollTo(index - 1);
    }
}
