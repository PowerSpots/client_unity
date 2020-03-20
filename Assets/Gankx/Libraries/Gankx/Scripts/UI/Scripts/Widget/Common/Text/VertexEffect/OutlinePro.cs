using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//字体勾边效果


[AddComponentMenu("UI/Effects/OutlinePro", 15)]
public class OutlinePro : ShadowPro
{
    public enum OutlineType
    {
        Outline4 ,
        Outline8 ,
        Circle,
    }

    public OutlineType StyleType = OutlineType.Outline4;

    public override  void ModifyVertices(List<UIVertex> verts)
    {
        if (StyleType == OutlineType.Outline8)
        {
            Outline8Handler(verts);
        }
        else if (StyleType == OutlineType.Outline4)
        {
            Outline4Handler(verts);
        }
        else if (StyleType == OutlineType.Circle)
        {
            OutlineCircle(verts);
        }
    }

    public bool m_UseGradient = false;

    [HeaderAttribute("Gradient Settings")]
    public Color32 BottomColor = Color.black;
    public bool m_UseGraphicAlphaInGradiend = true;
    [Range(-1, 1)]
    public float m_gradientOffsetVertical = 0;

    // 所有采样圈数
    private int mCircleCount = 2;
    // 第一圈采样次数
    private int mFirstCircleSampleCount = 4;
    // 每圈采样次数递增
    private int mSampleCountIncrement = 2;

    /// <summary>
    /// 只适合2像素勾边，更多像素勾边需要参数修改
    /// </summary>
    /// <param name="verts"></param>
    private void OutlineCircle(List<UIVertex> verts)
    {
        float maxDis = Mathf.Max(Mathf.Abs(effectDistance.x), Mathf.Abs(effectDistance.y));
        mCircleCount = Mathf.FloorToInt(maxDis);

        int total = (mFirstCircleSampleCount * 2 + mSampleCountIncrement * (mCircleCount - 1)) * mCircleCount / 2;
        int neededCapacity = verts.Count * (total + 1);
        if (verts.Capacity < neededCapacity)
        {
            verts.Capacity = neededCapacity;
        }

        int original = verts.Count;
        int count = 0;
        int sampleCount = mFirstCircleSampleCount;
        float dx = effectDistance.x / mCircleCount;
        float dy = effectDistance.y / mCircleCount;
        for (int i = 1; i <= mCircleCount; i++)
        {
            float rx = dx * i;
            float ry = dy * i;
            float radStep = 2 * Mathf.PI / sampleCount;
            float rad = (i % 2) * radStep * 0.5f;
            for (int j = 0; j < sampleCount; j++)
            {
                int next = count + original;
                ApplyShadowSupportGradient(verts, effectColor, count, next, rx * Mathf.Cos(rad), ry * Mathf.Sin(rad));
                count = next;
                rad += radStep;
            }
            sampleCount += mSampleCountIncrement;
        }
    }

    private void Outline8Handler(List<UIVertex> verts)
    {
        int neededCapacity = verts.Count * 9;
        if (verts.Capacity < neededCapacity)
        {
            verts.Capacity = neededCapacity;
        }

        int original = verts.Count;
        int count = 0;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (!(x == 0 && y == 0))
                {
                    int next = count + original;
                    ApplyShadowSupportGradient(verts, effectColor, count, next, effectDistance.x * x, effectDistance.y * y);
                    count = next;
                }
            }
        }
    }

    private void Outline4Handler(List<UIVertex> verts)
    {
        int neededCpacity = verts.Count * 5;
        if (verts.Capacity < neededCpacity)
        {
            verts.Capacity = neededCpacity;
        }

        int start = 0;
        int end = verts.Count;
        ApplyShadowZeroAllocSupportGradient(verts, effectColor, start, verts.Count, effectDistance.x, effectDistance.y);

        start = end;
        end = verts.Count;
        ApplyShadowZeroAllocSupportGradient(verts, effectColor, start, verts.Count, effectDistance.x, -effectDistance.y);

        start = end;
        end = verts.Count;
        ApplyShadowZeroAllocSupportGradient(verts, effectColor, start, verts.Count, -effectDistance.x, effectDistance.y);

        start = end;
        end = verts.Count;
        ApplyShadowZeroAllocSupportGradient(verts, effectColor, start, verts.Count, -effectDistance.x, -effectDistance.y);
    }

    protected void ApplyShadowZeroAllocSupportGradient(List<UIVertex> verts, Color32 color, int start, int end, float x, float y) {
        if (!m_UseGradient) {
            ApplyShadowZeroAlloc(verts, color, start, end, x, y);
            return;
        }

        int num = verts.Count + end - start;
        if (verts.Capacity < num)
            verts.Capacity = num;

        float min = verts.Min(vert => vert.position.y);
        float max = verts.Max(vert => vert.position.y);
        float dis = max - min;

        for (int index = start; index < end; ++index) {
            UIVertex vert = verts[index];
            verts.Add(vert);
            Vector3 position = vert.position;
            position.x += x;
            position.y += y;
            vert.position = position;
            Color32 color32 = Color32.Lerp(BottomColor, color, (vert.position.y - min) / dis + m_gradientOffsetVertical);
            if (m_UseGraphicAlphaInGradiend) {
                color32.a = (byte)((int)color32.a * (int)verts[index].color.a / (int)byte.MaxValue);
            }
            vert.color = color32;
            verts[index] = vert;
        }
    }

    protected void ApplyShadowSupportGradient(List<UIVertex> verts, Color32 color, int start, int end, float x, float y) {
        this.ApplyShadowZeroAllocSupportGradient(verts, color, start, end, x, y);
    }

}
