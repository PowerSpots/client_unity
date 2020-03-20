using UnityEngine;
using UnityEngine.UI;

public class HighlightMask : Graphic, ICanvasRaycastFilter {

    public Camera m_UICamera;
    public RectTransform m_TargetRect = null;

    private Vector3 m_TargetRectPos = Vector3.zero;
    private Vector2 m_TargetRectSize = Vector2.zero;

    private Vector2 m_CenterPoint = Vector2.zero;

    private bool m_NeedRebuild = true;

    void Update() {
        if (m_UICamera == null || m_TargetRect == null || m_TargetRect.gameObject == null) {
            if (!m_NeedRebuild) {
                m_NeedRebuild = true;
                SetAllDirty();
            }
            return;
        }
        if (m_NeedRebuild) {
            m_NeedRebuild = false;
            SetAllDirty();
        }

        if (m_TargetRectPos != m_TargetRect.transform.position || m_TargetRectSize != m_TargetRect.sizeDelta) {
            m_TargetRectPos = m_TargetRect.transform.position;
            m_TargetRectSize = m_TargetRect.rect.size;
            SetAllDirty();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, RectTransformUtility.WorldToScreenPoint(m_UICamera, m_TargetRectPos), m_UICamera, out m_CenterPoint);
        }
    }

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera) {
        if (m_TargetRect == null || m_TargetRect.gameObject == null) {
            return false;
        }
        bool isRaycastValid = RectTransformUtility.RectangleContainsScreenPoint(m_TargetRect, sp, eventCamera);
        return !isRaycastValid;
    }

    protected override void OnPopulateMesh(VertexHelper vh) {
        vh.Clear();

        if(m_NeedRebuild) return;
        
        Vector4 outer = new Vector4(-rectTransform.pivot.x * rectTransform.rect.width,
                                    -rectTransform.pivot.y * rectTransform.rect.height,
                                    (1 - rectTransform.pivot.x) * rectTransform.rect.width,
                                    (1 - rectTransform.pivot.y) * rectTransform.rect.height);
        Vector4 inner = new Vector4(m_CenterPoint.x - m_TargetRectSize.x / 2,
                                    m_CenterPoint.y - m_TargetRectSize.y / 2,
                                    m_CenterPoint.x + m_TargetRectSize.x * 0.5f,
                                    m_CenterPoint.y + m_TargetRectSize.y * 0.5f);

        //   (x,w)                 (z,w)
        //    o2--------------------o3
        //     |                     |
        //     |      i2----i3       |
        //     |      |      |       |
        //     |      i1----i4       |
        //     |                     |
        //    o1--------------------o4
        //   (x,y)                 (z,y)

        UIVertex vert = UIVertex.simpleVert;

        vert.position = new Vector2(outer.x, outer.y);
        vert.color = color;
        vh.AddVert(vert);
        vert.position = new Vector2(outer.x, outer.w);
        vert.color = color;
        vh.AddVert(vert);
        vert.position = new Vector2(outer.z, outer.w);
        vert.color = color;
        vh.AddVert(vert);
        vert.position = new Vector2(outer.z, outer.y);
        vert.color = color;
        vh.AddVert(vert);

        vert.position = new Vector2(inner.x, inner.y);
        vert.color = color;
        vh.AddVert(vert);
        vert.position = new Vector2(inner.x, inner.w);
        vert.color = color;
        vh.AddVert(vert);
        vert.position = new Vector2(inner.z, inner.w);
        vert.color = color;
        vh.AddVert(vert);
        vert.position = new Vector2(inner.z, inner.y);
        vert.color = color;
        vh.AddVert(vert);

        vh.AddTriangle(0, 1, 5);
        vh.AddTriangle(0, 4, 5);

        vh.AddTriangle(1, 2, 6);
        vh.AddTriangle(1, 5, 6);

        vh.AddTriangle(2, 3, 7);
        vh.AddTriangle(2, 6, 7);

        vh.AddTriangle(3, 0, 4);
        vh.AddTriangle(3, 7, 4);
    }
}
