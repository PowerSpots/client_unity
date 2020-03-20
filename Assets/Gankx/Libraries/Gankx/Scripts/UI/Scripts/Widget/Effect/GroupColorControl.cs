using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class GroupColorControl : MonoBehaviour
{
    public Color color;
    private Color lastColor;
    private MaskableGraphic[] maskableGraphics;

    // Update is called once per frame
    void Update()
    {
        if (lastColor != color)
        {
            ChangeGroupColor(color);
        }
        lastColor = color;
    }

    void ChangeGroupColor(Color newColor)
    {
        MaskableGraphic[] maskableGraphics = GetComponentsInChildren<MaskableGraphic>(true);
        if (null == maskableGraphics || maskableGraphics.Length <= 0)
            return;

        for (int i = 0; i < maskableGraphics.Length; ++i)
        {
            MaskableGraphic mg = maskableGraphics[i];
            if (null != mg)
            {                
                mg.color = newColor;                
            }
        }
    }
}
