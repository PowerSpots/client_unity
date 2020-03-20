using System;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

// ReSharper disable InconsistentNaming

namespace Gankx.UI
{
    [AddComponentMenu("UI/Image/Circle Polygon")]
    public class CirclePolygon : AtlasImage
    {
        [Range(0, 1)]
        public float beginFillPercent = 0.0f;

        [Range(0, 1)]
        public float endFillPercent = 1.0f;

        [Range(3, 100)]
        public int segments = 5;

        [Range(0f, 360f)]
        public float startAngle = 0.0f;

        private Vector2 myUVOffset = Vector2.zero;
        private float[] mySegmentFillAmounts;

        protected override void Awake()
        {
            mySegmentFillAmounts = new float[segments + 1];
            for (var i = 0; i < mySegmentFillAmounts.Length; i++)
            {
                mySegmentFillAmounts[i] = 1.0f;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            base.OnTransformParentChanged();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            var degreeDelta = 2 * Mathf.PI / segments;
            if (fillClockwise)
            {
                degreeDelta = -degreeDelta;
            }

            var beginSegements = (int) (segments * beginFillPercent);
            var endSegements = (int) (segments * endFillPercent);

            var transRectWidth = rectTransform.rect.width;
            var transRectHeight = rectTransform.rect.height;
            var outerRadius = rectTransform.pivot.x * transRectWidth;

            var uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;

            var uvCenterX = (uv.x + uv.z) * 0.5f;
            var uvCenterY = (uv.y + uv.w) * 0.5f;
            var uvScaleX = (uv.z - uv.x) / transRectWidth;
            var uvScaleY = (uv.w - uv.y) / transRectHeight;

            var curDegree = Mathf.PI / 180.0f * startAngle + beginSegements * Mathf.Abs(degreeDelta);

            var curVertice = Vector2.zero;
            var verticeCount = endSegements - beginSegements + 1;
            var uiVertex = new UIVertex();
            uiVertex.color = color;
            uiVertex.position = curVertice;
            uiVertex.uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY) +
                           myUVOffset;
            vh.AddVert(uiVertex);

            for (var i = 1; i < verticeCount; i++)
            {
                var cosA = Mathf.Cos(curDegree);
                var sinA = Mathf.Sin(curDegree);
                curVertice = new Vector2(cosA * outerRadius, sinA * outerRadius) * mySegmentFillAmounts[i];
                curDegree += degreeDelta;

                uiVertex = new UIVertex();
                uiVertex.color = color;
                uiVertex.position = curVertice;
                uiVertex.uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY) +
                               myUVOffset;
                vh.AddVert(uiVertex);
            }

            var triangleCount = (endSegements - beginSegements) * 3;
            for (int i = 0, vIdx = 1; i < triangleCount - 3; i += 3, vIdx++)
            {
                vh.AddTriangle(vIdx, 0, vIdx + 1);
            }

            if (Math.Abs(endFillPercent - 1) < float.Epsilon)
            {
                vh.AddTriangle(verticeCount - 1, 0, 1);
            }
        }

        public void SetUVOffset(Vector2 offset)
        {
            myUVOffset = offset;
        }

        public void SetSegmentFillAmount(int index, float value)
        {
            if (index >= 0 && index < mySegmentFillAmounts.Length)
            {
                mySegmentFillAmounts[index] = value;
            }
        }
    }
}