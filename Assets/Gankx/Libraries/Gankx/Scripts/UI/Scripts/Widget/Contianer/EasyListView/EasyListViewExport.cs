using Gankx.UI;
using UIWidgets;

public static class EasyListViewExport
{
    public static void SetDataSource(uint windowId, int dataSourceCount)
    {
        EasyListView listView = PanelService.GetWindowComponent<EasyListView>(windowId);
        if (null == listView)
        {
            return;
        }

        listView.SetDataSource(dataSourceCount);
        //listView.Resize();
    }

    public static int GetListItemIndex(uint windowId)
    {
        UnityEngine.GameObject go = PanelService.GetWindowObject(windowId);

        if (go == null)
            return -1;

        ListViewItem listView = go.GetComponent<ListViewItem>();
        if (null == listView)
        {
            return -1;
        }

        return listView.Index;
    }

    public static void CenterScrollTo(uint windowId, int index)
    {
        EasyListView listView = PanelService.GetWindowComponent<EasyListView>(windowId);
        if (null == listView)
        {
            return;
        }
        
        listView.CenterScrollTo(index, 0);
    }

    public static void LazyScrollTo(uint windowId, int index)
    {
        EasyListView listView = PanelService.GetWindowComponent<EasyListView>(windowId);
        if (null == listView)
        {
            return;
        }

        listView.LazyScrollTo(index);
    }

    public static void EnableCenterOnChild(uint windowId)
    {
        EasyListView listView = PanelService.GetWindowComponent<EasyListView>(windowId);
        if (null == listView)
        {
            return;
        }

        listView.EnableCenterOnChild();
    }

    public static int GetCenterIndex(uint windowId)
    {
        EasyListView listView = PanelService.GetWindowComponent<EasyListView>(windowId);
        if (null == listView)
        {
            return 0;
        }

        return listView.GetCenterIndex();
    }

    public static float GetScrollValue(uint windowId)
    {
        EasyListView listView = PanelService.GetWindowComponent<EasyListView>(windowId);
        if (null == listView)
        {
            return 0;
        }

        return listView.GetScrollValue();
    }

    public static void SetScrollValue(uint windowId,float value, bool callScrollUpdate = true)
    {
        EasyListView listView = PanelService.GetWindowComponent<EasyListView>(windowId);
        if (null == listView)
        {
            return;
        }

        listView.SetScrollValue(value, callScrollUpdate);
    }

}