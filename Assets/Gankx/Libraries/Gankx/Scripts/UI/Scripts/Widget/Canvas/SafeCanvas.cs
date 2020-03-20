using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeCanvas : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
        yield return null;
        RectTransform rc = GetComponent<RectTransform>();
        if (rc != null) {
            rc.anchorMin = Gankx.UI.PanelService.SafeAreaRect.min;
            rc.anchorMax = Gankx.UI.PanelService.SafeAreaRect.size;
            rc.anchoredPosition = Vector2.zero;
            rc.sizeDelta = Vector2.zero;
        }
    }
}
