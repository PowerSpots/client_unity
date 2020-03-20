using UnityEngine;
using UnityEngine.UI;

/// <inheritdoc />
/// <summary>
///     UGUI拓展 - 文字一行中间对齐 超过一行左对齐
/// </summary>
[AddComponentMenu("UI/AlignmentText", 50)]
// ReSharper disable once UnusedMember.Global
// ReSharper disable once CheckNamespace
public class AlignmentText : Text
{

    public override string text
    {
        get { return base.text; }
        set
        {
            base.text = value;
            SetAlignment();
        }
    }
    [ContextMenu("SetAlignment")]
    protected void SetAlignment()
    {
        var settings = GetGenerationSettings(rectTransform.rect.size);
        settings.resizeTextForBestFit = false;

        cachedTextGenerator.Populate(text, settings);
        var v = (int) alignment / 3;

        alignment = (TextAnchor) (cachedTextGenerator.lineCount <= 1 ? v * 3 + 1 : v * 3);
    }


#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        if (resizeTextForBestFit)
        {
            resizeTextForBestFit = false;
        }

        SetAlignment();
    }
#endif
}