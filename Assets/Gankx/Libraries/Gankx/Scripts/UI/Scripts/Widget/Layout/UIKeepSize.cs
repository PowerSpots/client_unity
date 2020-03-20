using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(RectTransform))]
public class UIKeepSize : MonoBehaviour
{
    public enum UIKeepSizeType
    {
        KeepWidth,
        KeepHeight,
    }


    public UIKeepSizeType keepType = UIKeepSizeType.KeepWidth;
    public bool IgnoreOtherSide = false;

    private float origWidth;
    private float origHeight;
    private RectTransform rectTrans;

    void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        origWidth = rectTrans.GetWidth();
        origHeight = rectTrans.GetHeight();

        float widthRatio = UIScreenAdaptor.GetCanvasWidth() / 1920f;
        float heightRatio = UIScreenAdaptor.GetCanvasHeigth() / 1080f;

        switch (keepType)
        {
            case UIKeepSizeType.KeepWidth: {
                rectTrans.SetWidth(origWidth * widthRatio);
                if (!IgnoreOtherSide)
                    rectTrans.SetHeight(origHeight * widthRatio);
                else {
                    rectTrans.SetHeight(origHeight * heightRatio);
                }
            }
                break;
            case UIKeepSizeType.KeepHeight: {
                rectTrans.SetHeight(origHeight * heightRatio);
                if (!IgnoreOtherSide)
                    rectTrans.SetWidth(origWidth * heightRatio);
                else {
                    rectTrans.SetWidth(origWidth * widthRatio);
                }
            }
                break;
        }
    }

}

