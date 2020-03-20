using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;
// ReSharper disable InconsistentNaming

namespace Gankx.UI
{
    [AddComponentMenu("UI/Image/Circle Image")]
    public class CircleImage : AtlasImage
    {
        [Tooltip("Circle or fan")]
        [Range(0, 1)]
        public float beginFillPercent = 0.0f;

        [Range(0, 1)]
        public float endFillPercent = 1.0f;

        [Tooltip("Cirle or ring")]
        public bool fill = true;

        [Tooltip("Border thickness")]
        public float thickness = 5;

        [Tooltip("Circle segements")]
        [Range(3, 100)]
        public int segements = 60;

        [Range(0f, 360f)]
        public float startAngle = 0.0f;

        private Vector2 myUVOffset = Vector2.zero;

        private List<Vector3> myInnerVertices;
        private List<Vector3> myOutterVertices;

        protected override void Awake()
        {
            myInnerVertices = new List<Vector3>();
            myOutterVertices = new List<Vector3>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            base.OnTransformParentChanged();
        }

        private void Update()
        {
            thickness = Mathf.Clamp(thickness, 0, rectTransform.rect.width / 2);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            myInnerVertices.Clear();
            myOutterVertices.Clear();

            var degreeDelta = 2 * Mathf.PI / segements;
            if (fillClockwise)
            {
                degreeDelta = -degreeDelta;
            }

            var beginSegements = (int) (segements * beginFillPercent);
            var endSegements = (int) (segements * endFillPercent);

            var transRectWidth = rectTransform.rect.width;
            var transRectHeight = rectTransform.rect.height;
            var outerRadius = rectTransform.pivot.x * transRectWidth;
            var innerRadius = rectTransform.pivot.x * transRectWidth - thickness;

            var uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;

            var uvCenterX = (uv.x + uv.z) * 0.5f;
            var uvCenterY = (uv.y + uv.w) * 0.5f;
            var uvScaleX = (uv.z - uv.x) / transRectWidth;
            var uvScaleY = (uv.w - uv.y) / transRectHeight;

            var curDegree = Mathf.PI / 180.0f * startAngle + beginSegements * Mathf.Abs(degreeDelta);
            UIVertex uiVertex;
            int verticeCount;
            int triangleCount;
            Vector2 curVertice;

            if (fill)
            {
                // circle

                curVertice = Vector2.zero;
                verticeCount = endSegements - beginSegements + 1;
                uiVertex = new UIVertex();
                uiVertex.color = color;
                uiVertex.position = curVertice;
                uiVertex.uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY) +
                               myUVOffset;
                vh.AddVert(uiVertex);

                for (var i = 1; i < verticeCount; i++)
                {
                    var cosA = Mathf.Cos(curDegree);
                    var sinA = Mathf.Sin(curDegree);
                    curVertice = new Vector2(cosA * outerRadius, sinA * outerRadius);
                    curDegree += degreeDelta;

                    uiVertex = new UIVertex();
                    uiVertex.color = color;
                    uiVertex.position = curVertice;
                    uiVertex.uv0 =
                        new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY) +
                        myUVOffset;
                    vh.AddVert(uiVertex);

                    myOutterVertices.Add(curVertice);
                }

                triangleCount = (endSegements - beginSegements) * 3;
                for (int i = 0, vIdx = 1; i < triangleCount - 3; i += 3, vIdx++)
                {
                    vh.AddTriangle(vIdx, 0, vIdx + 1);
                }

                if (Math.Abs(endFillPercent - 1) < float.Epsilon)
                {
                    vh.AddTriangle(verticeCount - 1, 0, 1);
                }
            }
            else
            {
                // ring

                verticeCount = (endSegements - beginSegements) * 2;
                for (var i = 0; i < verticeCount; i += 2)
                {
                    var cosA = Mathf.Cos(curDegree);
                    var sinA = Mathf.Sin(curDegree);
                    curDegree += degreeDelta;

                    curVertice = new Vector3(cosA * innerRadius, sinA * innerRadius);
                    uiVertex = new UIVertex();
                    uiVertex.color = color;
                    uiVertex.position = curVertice;
                    uiVertex.uv0 =
                        new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY) +
                        myUVOffset;
                    vh.AddVert(uiVertex);
                    myInnerVertices.Add(curVertice);

                    curVertice = new Vector3(cosA * outerRadius, sinA * outerRadius);
                    uiVertex = new UIVertex();
                    uiVertex.color = color;
                    uiVertex.position = curVertice;
                    uiVertex.uv0 =
                        new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY) +
                        myUVOffset;
                    vh.AddVert(uiVertex);
                    myOutterVertices.Add(curVertice);
                }

                triangleCount = (endSegements - beginSegements) * 3 * 2;
                for (int i = 0, vIdx = 0; i < triangleCount - 6; i += 6, vIdx += 2)
                {
                    vh.AddTriangle(vIdx + 1, vIdx, vIdx + 3);
                    vh.AddTriangle(vIdx, vIdx + 2, vIdx + 3);
                }

                if (Math.Abs(endFillPercent - 1) < float.Epsilon)
                {
                    vh.AddTriangle(verticeCount - 1, verticeCount - 2, 1);
                    vh.AddTriangle(verticeCount - 2, 0, 1);
                }
            }
        }

        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (overrideSprite == null)
            {
                return true;
            }

            Vector2 local;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out local);
            return Contains(local, myOutterVertices, myInnerVertices);
        }

        private bool Contains(Vector2 p, List<Vector3> outterVertices, List<Vector3> innerVertices)
        {
            var crossNumber = 0;
            RayCrossing(p, innerVertices, ref crossNumber);
            RayCrossing(p, outterVertices, ref crossNumber);
            return (crossNumber & 1) == 1;
        }

        private static void RayCrossing(Vector2 p, List<Vector3> vertices, ref int crossNumber)
        {
            for (int i = 0, count = vertices.Count; i < count; i++)
            {
                var v1 = vertices[i];
                var v2 = vertices[(i + 1) % count];

                if (v1.y <= p.y && v2.y > p.y
                    || v1.y > p.y && v2.y <= p.y)
                {
                    if (p.x < v1.x + (p.y - v1.y) / (v2.y - v1.y) * (v2.x - v1.x))
                    {
                        crossNumber += 1;
                    }
                }
            }
        }

        public void SetUVOffset(Vector2 offset)
        {
            myUVOffset = offset;
        }
    }
}