using UnityEngine;
using UnityEngine.UI;

public class UIScreenAdaptor 
{
    private static readonly float ConstCanvasHeight = 1080;
    private static readonly float ConstCanvasWidth = 1920;
    private static readonly float ConstScreenRadioLine = (16f / 9f - 0.001f);

    public static void SetCanvasScaler(GameObject canvasGo)
    {
        CanvasScaler cs = canvasGo.GetComponent<CanvasScaler>();
        if (cs != null)
        {
            cs.matchWidthOrHeight = GetScreenMatchMode();
        }
    }

    public static float GetScreenMatchMode()
    {
        return GetScreenRadio() < ConstScreenRadioLine ? 0f : 1f;
    }

    public static float GetScreenRadio()
    {
        return Screen.width / (float)Screen.height;
    }

    public static float GetCanvasWidth()
    {
        float matchMode = GetScreenMatchMode();
        if (matchMode == 0f)
        {
            return ConstCanvasWidth;
        }
        else if (matchMode == 1f)
        {
            return ConstCanvasHeight *  GetScreenRadio();
        }

        return ConstCanvasWidth; 
    }

    public static float GetCanvasHeigth()
    {
        float matchMode = GetScreenMatchMode();
        if (matchMode == 0f)
        {
            return ConstCanvasWidth / GetScreenRadio();
        }
        else if (matchMode == 1f)
        {
            return ConstCanvasHeight;
        }

        return ConstCanvasHeight;
    }
}
