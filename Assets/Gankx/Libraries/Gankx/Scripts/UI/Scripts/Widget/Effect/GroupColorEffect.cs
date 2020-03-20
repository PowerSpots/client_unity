using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupColorEffect : MonoBehaviour
{
    [SerializeField]
    public Color darkColor; // 压暗颜色
    private bool isInited = false; // 是否已经初始化过

    private class ColorInfo
    {
        public MaskableGraphic graphic;
        public Color color;
    }
    private List<ColorInfo> srcColorInfos = new List<ColorInfo>();

    private void Init()
    {
        if (isInited)
        {
            return;
        }
        isInited = true;

        MaskableGraphic[] maskableGraphics = GetComponentsInChildren<MaskableGraphic>(true);
        foreach (MaskableGraphic graphic in maskableGraphics)
        {
            srcColorInfos.Add(new ColorInfo() { graphic = graphic, color = graphic.color });
        }
    }

    [ContextMenu("SetDark")]
    public void Dark()
    {
        SetDark(true);
    }

    [ContextMenu("SetNormal")]
    public void Normal()
    {
        SetDark(false);
    }

    public void SetDark(bool isDark)
    {
        Init();
        if (isDark)
        {
            foreach (ColorInfo info in srcColorInfos)
            {
                Color destColor  = (info.color * (1.0f - darkColor.a)) + (darkColor * darkColor.a);
                destColor.a = 1.0f;
                info.graphic.color = destColor;
            }
        }
        else
        {
            foreach (ColorInfo info in srcColorInfos)
            {
                info.graphic.color = info.color;
            }
        }
    }

    public void SetDarkColor(float r, float g, float b, float a)
    {
        darkColor = new Color(r, g, b, a);
    }
}
