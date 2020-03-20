using Gankx.UI;

public static class UILongPressTriggerExport
{
    public static void SetThreshold(uint windowId, float value)
    {
        LongPressTrigger longPressTrigger = PanelService.GetWindowComponent<LongPressTrigger>(windowId);
        if (null == longPressTrigger)
        {
            return;
        }

        longPressTrigger.SetThreshold(value);
    }
}
