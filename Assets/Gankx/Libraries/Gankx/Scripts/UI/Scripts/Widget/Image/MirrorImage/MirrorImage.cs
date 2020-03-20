using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Gankx.UI
{
    [AddComponentMenu("UI/Image/Mirror Image")]
    public class MirrorImage : BaseMeshEffect
    {
        public enum MirrorType
        {
            Horizontal,
            Vertical,
            Quarter
        }

        [SerializeField]
        [FormerlySerializedAs("m_MirrorType")]
        private MirrorType myMirrorType = MirrorType.Horizontal;

        [NonSerialized]
        private RectTransform myRectTransform;

        public MirrorType mirrorType
        {
            get { return myMirrorType; }
            set
            {
                if (myMirrorType != value)
                {
                    myMirrorType = value;
                    if (graphic != null)
                    {
                        graphic.SetVerticesDirty();
                    }
                }
            }
        }

        public RectTransform rectTransform
        {
            get { return myRectTransform ?? (myRectTransform = GetComponent<RectTransform>()); }
        }

        public void SetNativeSize()
        {
            var graphicImage = graphic as Image;
            if (graphicImage != null)
            {
                var overrideSprite = graphicImage.overrideSprite;

                if (overrideSprite != null)
                {
                    var w = overrideSprite.rect.width / graphicImage.pixelsPerUnit;
                    var h = overrideSprite.rect.height / graphicImage.pixelsPerUnit;
                    rectTransform.anchorMax = rectTransform.anchorMin;

                    switch (myMirrorType)
                    {
                        case MirrorType.Horizontal:
                            rectTransform.sizeDelta = new Vector2(w * 2, h);
                            break;
                        case MirrorType.Vertical:
                            rectTransform.sizeDelta = new Vector2(w, h * 2);
                            break;
                        case MirrorType.Quarter:
                            rectTransform.sizeDelta = new Vector2(w * 2, h * 2);
                            break;
                    }

                    graphic.SetVerticesDirty();
                }
            }
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
            {
                return;
            }

            var output = ListPool<UIVertex>.Get();
            vh.GetUIVertexStream(output);

            var count = output.Count;

            var graphicImage = graphic as Image;
            if (graphicImage != null)
            {
                var type = graphicImage.type;

                switch (type)
                {
                    case Image.Type.Simple:
                        DrawSimple(output, count);
                        break;
                    case Image.Type.Sliced:
                        DrawSliced(output, count);
                        break;
                    case Image.Type.Tiled:

                        break;
                    case Image.Type.Filled:

                        break;
                }
            }
            else
            {
                DrawSimple(output, count);
            }

            vh.Clear();
            vh.AddUIVertexTriangleStream(output);
        }

        protected void DrawSimple(List<UIVertex> output, int count)
        {
            var rect = graphic.GetPixelAdjustedRect();

            SimpleScale(rect, output, count);

            switch (myMirrorType)
            {
                case MirrorType.Horizontal:
                    ExtendCapacity(output, count);
                    MirrorVerts(rect, output, count);
                    break;
                case MirrorType.Vertical:
                    ExtendCapacity(output, count);
                    MirrorVerts(rect, output, count, false);
                    break;
                case MirrorType.Quarter:
                    ExtendCapacity(output, count * 3);
                    MirrorVerts(rect, output, count);
                    MirrorVerts(rect, output, count * 2, false);
                    break;
            }
        }

        protected void DrawSliced(List<UIVertex> output, int count)
        {
            var graphicImage = graphic as Image;
            if (null == graphicImage || !graphicImage.hasBorder)
            {
                DrawSimple(output, count);
            }

            var rect = graphic.GetPixelAdjustedRect();

            SlicedScale(rect, output, count);
            count = SliceExcludeVerts(output, count);

            switch (myMirrorType)
            {
                case MirrorType.Horizontal:
                    ExtendCapacity(output, count);
                    MirrorVerts(rect, output, count);
                    break;
                case MirrorType.Vertical:
                    ExtendCapacity(output, count);
                    MirrorVerts(rect, output, count, false);
                    break;
                case MirrorType.Quarter:
                    ExtendCapacity(output, count * 3);
                    MirrorVerts(rect, output, count);
                    MirrorVerts(rect, output, count * 2, false);
                    break;
            }
        }

        protected void ExtendCapacity(List<UIVertex> verts, int addCount)
        {
            var neededCapacity = verts.Count + addCount;
            if (verts.Capacity < neededCapacity)
            {
                verts.Capacity = neededCapacity;
            }
        }

        protected void SimpleScale(Rect rect, List<UIVertex> verts, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var vertex = verts[i];
                var position = vertex.position;
                if (myMirrorType == MirrorType.Horizontal || myMirrorType == MirrorType.Quarter)
                {
                    position.x = (position.x + rect.x) * 0.5f;
                }

                if (myMirrorType == MirrorType.Vertical || myMirrorType == MirrorType.Quarter)
                {
                    position.y = (position.y + rect.y) * 0.5f;
                }

                vertex.position = position;
                verts[i] = vertex;
            }
        }

        protected void SlicedScale(Rect rect, List<UIVertex> verts, int count)
        {
            var border = GetAdjustedBorders(rect);

            var halfWidth = rect.width * 0.5f;

            var halfHeight = rect.height * 0.5f;

            for (var i = 0; i < count; i++)
            {
                var vertex = verts[i];

                var position = vertex.position;

                if (myMirrorType == MirrorType.Horizontal || myMirrorType == MirrorType.Quarter)
                {
                    if (halfWidth < border.x && position.x >= rect.center.x)
                    {
                        position.x = rect.center.x;
                    }
                    else if (position.x >= border.x)
                    {
                        position.x = (position.x + rect.x) * 0.5f;
                    }
                }

                if (myMirrorType == MirrorType.Vertical || myMirrorType == MirrorType.Quarter)
                {
                    if (halfHeight < border.y && position.y >= rect.center.y)
                    {
                        position.y = rect.center.y;
                    }
                    else if (position.y >= border.y)
                    {
                        position.y = (position.y + rect.y) * 0.5f;
                    }
                }

                vertex.position = position;

                verts[i] = vertex;
            }
        }

        protected static void MirrorVerts(Rect rect, List<UIVertex> verts, int count, bool isHorizontal = true)
        {
            for (var i = 0; i < count; i++)
            {
                var vertex = verts[i];

                var position = vertex.position;

                if (isHorizontal)
                {
                    position.x = rect.center.x * 2 - position.x;
                }
                else
                {
                    position.y = rect.center.y * 2 - position.y;
                }

                vertex.position = position;

                verts.Add(vertex);
            }
        }

        protected static int SliceExcludeVerts(List<UIVertex> verts, int count)
        {
            var realCount = count;

            var i = 0;

            while (i < realCount)
            {
                var v1 = verts[i];
                var v2 = verts[i + 1];
                var v3 = verts[i + 2];

                if (v1.position == v2.position || v2.position == v3.position || v3.position == v1.position)
                {
                    verts[i] = verts[realCount - 3];
                    verts[i + 1] = verts[realCount - 2];
                    verts[i + 2] = verts[realCount - 1];

                    realCount -= 3;
                    continue;
                }

                i += 3;
            }

            if (realCount < count)
            {
                verts.RemoveRange(realCount, count - realCount);
            }

            return realCount;
        }

        protected Vector4 GetAdjustedBorders(Rect rect)
        {
            var graphicImage = graphic as Image;
            if (null == graphicImage)
            {
                return Vector4.zero;
            }

            var overrideSprite = graphicImage.overrideSprite;

            var border = overrideSprite.border;

            border = border / graphicImage.pixelsPerUnit;

            for (var axis = 0; axis <= 1; axis++)
            {
                var combinedBorders = border[axis] + border[axis + 2];
                if (rect.size[axis] < combinedBorders && Math.Abs(combinedBorders) > float.Epsilon)
                {
                    var borderScaleRatio = rect.size[axis] / combinedBorders;
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }
            }

            return border;
        }
    }
}