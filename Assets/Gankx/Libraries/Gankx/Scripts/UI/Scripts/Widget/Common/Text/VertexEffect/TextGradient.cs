﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/TextGradient")]
public class TextGradient : BaseMeshEffect
{
    public Color32 TopColor = Color.white;
    public Color32 BottomColor = Color.black;
    [Range(-90,180)]
    public float Theta = 0;
    [SerializeField, Range(-1f, 1f)]
    public float m_gradientOffsetVertical = 0f;

    public void SetDirty()
    {
        if (graphic != null)
            graphic.SetVerticesDirty();
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
        {
            return;
        }
        
        var vertexList = new List<UIVertex>();
        vh.GetUIVertexStream(vertexList);

        int count = vertexList.Count;
        if (count > 0)
        {
            float a = Mathf.Deg2Rad * Theta;
            Vector3 r = new Vector3(Mathf.Sin(a), Mathf.Cos(a));
            List<float> d = new List<float>();

            for (int i = 0; i < count; i++)
            {
                float value = Vector3.Dot(vertexList[i].position, r);
                d.Add(value);
            }

            float min, max;
            var dis = d.ToArray();
            FindMaxAndMinMethod(dis, 0, dis.Length, out max, out min);
            float maxDis = max - min;

            for (int i = 0; i < count; i++)
            {
                UIVertex v = vertexList[i];
                v.color = Color32.Lerp(BottomColor, TopColor, (d[i] - min) / maxDis + m_gradientOffsetVertical);
                vertexList[i] = v;
            }
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(vertexList);
    }
  
    void FindMaxAndMinMethod(float[] pArr, int nStart, int nEnd, out float max,out float min)
    {
        if (nEnd - nStart <= 2)
        {
            if (pArr[nStart] > pArr[nEnd - 1])
            {
                max = pArr[nStart];
                min = pArr[nEnd - 1];
            }
            else
            {
                max = pArr[nEnd - 1];
                min = pArr[nStart];
            }
            return;
        }

        float nLeftMax = 0;
        float nLeftMin = 0;
        float nRightMax = 0;
        float nRightMin = 0;
        FindMaxAndMinMethod(pArr, nStart, nStart + (nEnd - nStart) / 2, out nLeftMax, out nLeftMin);
        FindMaxAndMinMethod(pArr, nStart + (nEnd - nStart) / 2 + 1, nEnd, out nRightMax, out nRightMin);

        max = nLeftMax > nRightMax ? nLeftMax : nRightMax;
        min = nLeftMin < nRightMin ? nLeftMin : nRightMin;
    }
}