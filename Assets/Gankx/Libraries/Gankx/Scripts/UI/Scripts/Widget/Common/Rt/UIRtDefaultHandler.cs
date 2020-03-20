using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRtDefaultHandler : MonoBehaviour, IUIRtHandler
{
    private RawImage rawImage;

    private void CacheRawImage()
    {
        if (null == rawImage)
        {
            rawImage = GetComponent<RawImage>();
        }
    }

    public void InitRt(Camera cam)
    {
        if (null == cam)
        {
            return;
        }

        CacheRawImage();
        if (null == rawImage)
        {
            return;
        }

        cam.targetTexture = RenderTexture.GetTemporary((int)ScreenRTMgr.instance.GetWidth(), (int)ScreenRTMgr.instance.GetHeight(), 24);
        cam.targetTexture.name = "UI Screen RenderTexture";
        rawImage.texture = cam.targetTexture;
    }

    public void UninitRt(Camera cam)
    {
        if (null == cam)
        {
            return;
        }
        
        rawImage.texture = null;

        if (null != cam.targetTexture)
        {
            RenderTexture.ReleaseTemporary(cam.targetTexture);
            cam.targetTexture = null;
        }               
    }
}
