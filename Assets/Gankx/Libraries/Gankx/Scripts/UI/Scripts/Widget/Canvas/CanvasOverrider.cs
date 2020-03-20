using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasOverrider : MonoBehaviour {

    public Canvas canvas;
    public int sortOrder = 0;
    public string sortingLayerName;

    private void Start()
    {
        if (canvas == null)
        {
            canvas = GetComponent<Canvas>();
        }

        canvas.sortingLayerName = sortingLayerName;
        canvas.sortingLayerID = sortOrder;
    }
}
