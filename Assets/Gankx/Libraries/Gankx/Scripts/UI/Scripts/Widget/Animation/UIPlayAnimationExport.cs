using Gankx.UI;

public static class UIPlayAnimationExport
{
    public static void Play(uint windowId)
    {
        PlayAnimation animation = PanelService.GetWindowComponent<PlayAnimation>(windowId);
        if (null == animation)
        {
            return;
        }

        animation.Play(true, false);
    }
}