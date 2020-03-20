using UnityEngine;
using UnityEngine.UI;

/// <inheritdoc />
/// <summary>
/// UGUI拓展 - 更好用的文字缩放ShrinkText @see https://www.jianshu.com/p/45a1bb73969c
/// </summary>
[AddComponentMenu("UI/ShrinkText", 50)]
// ReSharper disable once UnusedMember.Global
// ReSharper disable once CheckNamespace
public class ShrinkText : Text
{
    private readonly UIVertex[] _tmpVerts = new UIVertex[4];

    /// <summary>
    ///     当前可见的文字行数
    /// </summary>
    public int VisibleLines { get; private set; }

    private void _UseFitSettings()
    {
        var settings = GetGenerationSettings(rectTransform.rect.size);
        settings.resizeTextForBestFit = false;

        if (!resizeTextForBestFit)
        {
            cachedTextGenerator.Populate(text, settings);
            return;
        }

        var minSize = resizeTextMinSize;
        var txtLen = text.Length;
        for (var i = resizeTextMaxSize; i >= minSize; --i)
        {
            settings.fontSize = i;
            cachedTextGenerator.Populate(text, settings);
            if (cachedTextGenerator.characterCountVisible == txtLen) break;
        }
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        if (null == font) return;

        m_DisableFontTextureRebuiltCallback = true;
        _UseFitSettings();
        var rect = rectTransform.rect;

        var textAnchorPivot = GetTextAnchorPivot(alignment);
        var zero = Vector2.zero;
        zero.x = Mathf.Lerp(rect.xMin, rect.xMax, textAnchorPivot.x);
        zero.y = Mathf.Lerp(rect.yMin, rect.yMax, textAnchorPivot.y);
        var vector2 = PixelAdjustPoint(zero) - zero;
        var verts = cachedTextGenerator.verts;
        var num1 = 1f / pixelsPerUnit;
        var num2 = verts.Count - 4;
        toFill.Clear();
        if (vector2 != Vector2.zero)
            for (var index1 = 0; index1 < num2; ++index1)
            {
                var index2 = index1 & 3;
                _tmpVerts[index2] = verts[index1];
                _tmpVerts[index2].position *= num1;
                _tmpVerts[index2].position.x += vector2.x;
                _tmpVerts[index2].position.y += vector2.y;
                if (index2 == 3)
                    toFill.AddUIVertexQuad(_tmpVerts);
            }
        else
            for (var index1 = 0; index1 < num2; ++index1)
            {
                var index2 = index1 & 3;
                _tmpVerts[index2] = verts[index1];
                _tmpVerts[index2].position *= num1;
                if (index2 == 3)
                    toFill.AddUIVertexQuad(_tmpVerts);
            }

        m_DisableFontTextureRebuiltCallback = false;
        VisibleLines = cachedTextGenerator.lineCount;
    }
}