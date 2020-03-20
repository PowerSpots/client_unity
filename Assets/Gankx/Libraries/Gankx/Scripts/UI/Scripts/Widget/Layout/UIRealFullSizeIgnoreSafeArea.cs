using System.Collections;
using System.Collections.Generic;
using Gankx.UI;
using UnityEngine;

public class UIRealFullSizeIgnoreSafeArea : MonoBehaviour {

    private RectTransform rectTransform;
    private Vector2 offsetMin, offsetMax;

    void OnEnable () {
        ChangeSize();
    }

    void Start() {
        ChangeSize();
    }

    void ChangeSize() {
        if(!PanelService.ContainsInstance()) return;
        if (PanelService.instance.SafeWidthEdge > 0)
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
                offsetMin = rectTransform.offsetMin;
                offsetMax = rectTransform.offsetMax;

                offsetMin.x -= PanelService.instance.SafeWidthEdge;
                offsetMax.x += PanelService.instance.SafeWidthEdge;
            }

            if (rectTransform != null)
            {
                rectTransform.offsetMin = offsetMin;
                rectTransform.offsetMax = offsetMax;
            }
        }
    }
}
