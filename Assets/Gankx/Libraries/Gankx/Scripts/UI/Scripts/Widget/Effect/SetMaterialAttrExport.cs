using Gankx.UI;
using UnityEngine;

public class SetMaterialAttrExport : MonoBehaviour {


    public static void SetMaterialAlpha(uint windowId, float a, float duration)
    {
        var obj = PanelService.GetWindowObject(windowId);
        var setMaterialAttr = obj.GetOrAddComponent<SetMaterialAttr>();

        if (setMaterialAttr == null)
        {
            return;
        }
        
        setMaterialAttr.SetMaterialAlpha(a, duration);
    }


}
