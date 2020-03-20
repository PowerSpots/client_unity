using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdaptiveCanvasScaler : CanvasScaler
{
    private Canvas mCanvas;

    protected override void Awake()
    {
        base.Awake();

        mCanvas = GetComponent<Canvas>();
    }

    protected override void HandleScaleWithScreenSize()
    {
        float width = (float) Screen.width;
        float height = (float) Screen.height;
        if (null != mCanvas.worldCamera)
        {
            width = mCanvas.worldCamera.pixelWidth;
            height = mCanvas.worldCamera.pixelHeight;
        }
        Vector2 vector2 = new Vector2(width, height);
        int targetDisplay = this.mCanvas.targetDisplay;
        if (targetDisplay > 0 && targetDisplay < Display.displays.Length)
        {
            Display display = Display.displays[targetDisplay];
            vector2 = new Vector2((float)display.renderingWidth, (float)display.renderingHeight);
        }
        float scaleFactor = 0.0f;
        switch (this.m_ScreenMatchMode)
        {
            case CanvasScaler.ScreenMatchMode.MatchWidthOrHeight:
                scaleFactor = Mathf.Pow(2f, Mathf.Lerp(Mathf.Log(vector2.x / this.m_ReferenceResolution.x, 2f), Mathf.Log(vector2.y / this.m_ReferenceResolution.y, 2f), this.m_MatchWidthOrHeight));
                break;
            case CanvasScaler.ScreenMatchMode.Expand:
                scaleFactor = Mathf.Min(vector2.x / this.m_ReferenceResolution.x, vector2.y / this.m_ReferenceResolution.y);
                break;
            case CanvasScaler.ScreenMatchMode.Shrink:
                scaleFactor = Mathf.Max(vector2.x / this.m_ReferenceResolution.x, vector2.y / this.m_ReferenceResolution.y);
                break;
        }
        this.SetScaleFactor(scaleFactor);
        this.SetReferencePixelsPerUnit(this.m_ReferencePixelsPerUnit);
    }
}
