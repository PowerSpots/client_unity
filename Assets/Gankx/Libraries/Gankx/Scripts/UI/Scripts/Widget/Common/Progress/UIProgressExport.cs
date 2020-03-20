using Gankx.UI;
using UnityEngine;

public class UIProgressExport : MonoBehaviour {

    public static void SetProgress(uint windowId, float progress1, float progress2)
    {
        var uiProgress = PanelService.GetWindowComponent<UIProgress>(windowId);
        if (null == uiProgress)
        {
            return;
        }

        uiProgress.SetProgress(progress1, progress2);
    }
}
