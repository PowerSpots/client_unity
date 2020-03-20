using Gankx.UI;

public static class RawImageLocalLoaderExport
{
    public static void Load(uint windowId, string path)
    {
        var loader = PanelService.GetWindowComponent<RawImageLocalLoader>(windowId);
        if (null == loader)
        {
            return;
        }

        loader.LoadJpg(path);
    }
}