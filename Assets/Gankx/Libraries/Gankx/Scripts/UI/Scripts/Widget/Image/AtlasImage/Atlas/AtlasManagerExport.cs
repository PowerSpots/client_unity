using Gankx.UI;

public class AtlasManagerExport
{
    public static void UnloadAllAssets()
    {
        AtlasManager.instance.UnloadUnusedAtlases();
    }
}