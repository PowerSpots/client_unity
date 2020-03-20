using Gankx.UI;
using UnityEngine;

public static class UILookAndFeelExport
{
    public static bool Change(uint windowId, string name)
    {
        LookAndFeel lookAndFeel = PanelService.GetWindowComponent<LookAndFeel>(windowId);
        if (null == lookAndFeel)
        {
            return false;
        }

        Animator animator = lookAndFeel.GetComponent<Animator>();
        if (null == animator)
        {
            return false;
        }

        lookAndFeel.Current = Animator.StringToHash(name);
        return true;
    }
}
