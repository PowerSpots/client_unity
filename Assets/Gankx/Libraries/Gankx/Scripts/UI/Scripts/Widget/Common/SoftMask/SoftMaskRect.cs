using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Gankx;
using Gankx.UI;

namespace UIExtention {
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class SoftMaskRect : MonoBehaviour {
        private int lastRectTransformHashCode;
        private OrientedRect2D m_offsetedOrientedRect2D;
        private OrientedRect2D m_orientedRect2D;
        [SerializeField] protected RectOffset m_padding = new RectOffset();
        private RectTransform m_rectTransform;
        private List<UIVertex> m_vertices;

        public RectOffset padding {
            get { return m_padding; }
            set {
                if (m_padding != value) {
                    SetVerticesDirty();
                }
                m_padding = value;
            }
        }

        public RectTransform rectTransform {
            get {
                if (m_rectTransform == null) {
                    m_rectTransform = transform as RectTransform;
                }
                return m_rectTransform;
            }
        }

        public OrientedRect2D orientedRect2D {
            get {
                if (IsVerticesDirty()) UpdateVertices();
                return m_orientedRect2D;
            }
        }

        public OrientedRect2D offsetedOrientedRect2D {
            get {
                if (IsVerticesDirty()) UpdateVertices();
                return m_offsetedOrientedRect2D;
            }
        }

        public List<UIVertex> Vertices {
            get {
                if (IsVerticesDirty()) UpdateVertices();
                return m_vertices;
            }
        }

        protected virtual void Awake() {
            subordinateChildrenGraphic();
        }

        protected virtual void LateUpdate() {
            int rectTransformHashCode = HashUtility.GetValueHashCode(rectTransform);
            if (lastRectTransformHashCode != rectTransformHashCode) {
                lastRectTransformHashCode = rectTransformHashCode;

                SetVerticesDirty();
                SoftMaskMeshEffect[] softMaskMeshEffects = GetComponentsInChildren<SoftMaskMeshEffect>();
                for (int i = 0; i < softMaskMeshEffects.Length; i++) {
                    SoftMaskMeshEffect effect = softMaskMeshEffects[i];
                    effect.SetVerticesDirty();
                }
            }
        }

        protected virtual void OnDestroy() {
            freeChildrenGraphic();
        }

        protected virtual void OnTransformChildrenChanged() {
            subordinateChildrenGraphic();
        }

        private void subordinateChildrenGraphic() {
            IEnumerable<Graphic> graphicsWithoutEffect = GetComponentsInChildren<Graphic>()
                .Where(x => x.GetComponent<SoftMaskMeshEffect>() == null);
            foreach (Graphic graphic in graphicsWithoutEffect) {
                if (graphic.enabled)
                    graphic.gameObject.AddComponent<SoftMaskMeshEffect>();
            }

#if UNITY_EDITOR
            SlotControl[] uc = GetComponentsInChildren<SlotControl>();
            for (int i = 0; i < uc.Length; i++) {
                uc[i].supportSoftMask = true;
            }
#endif
        }

        private void freeChildrenGraphic() {
            SoftMaskMeshEffect[] softMaskMeshEffects = GetComponentsInChildren<SoftMaskMeshEffect>();
            for (int i = 0; i < softMaskMeshEffects.Length; i++) {
                SoftMaskMeshEffect effect = softMaskMeshEffects[i];
#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying) {
                    DestroyImmediate(effect);
                }
                else //fall through
#endif
                    Destroy(effect);
            }
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying) {
                SlotControl[] uc = GetComponentsInChildren<SlotControl>();
                for (int i = 0; i < uc.Length; i++) {
                    uc[i].supportSoftMask = false;
                }
            }
#endif
        }

        private bool IsVerticesDirty() {
            return m_vertices == null || m_vertices.Count == 0;
        }

        private void SetVerticesDirty() {
            if (m_vertices != null && 0 < m_vertices.Count) {
                m_vertices.Clear();
            }
        }

        private void UpdateVertices() {
            m_orientedRect2D = new OrientedRect2D(rectTransform);

            Rect rect = rectTransform.rect;
            rect.xMin += padding.left;
            rect.yMax -= padding.top;
            rect.xMax -= padding.right;
            rect.yMin += padding.bottom;
            m_offsetedOrientedRect2D = new OrientedRect2D(
                (Vector2) rectTransform.position + Vector2.Scale(rect.center, rectTransform.lossyScale)
                , Vector2.Scale(rect.size, rectTransform.lossyScale)
                , rectTransform.rotation
            );

            if (m_vertices == null) {
                m_vertices = new List<UIVertex>(30);
            }
            else {
                m_vertices.Clear();
            }
            Vector2[] rectVertices = GetVertices(m_orientedRect2D).ToArray();
            Vector2[] rectOffsetedVertices = GetVertices(m_offsetedOrientedRect2D).ToArray();
            if (!VertexUtility.IsDegeneracy(rectOffsetedVertices[0], rectOffsetedVertices[1],
                rectOffsetedVertices[2])) {
                m_vertices.Add(CreateUIVertex(rectOffsetedVertices[0], 1.0f));
                m_vertices.Add(CreateUIVertex(rectOffsetedVertices[1], 1.0f));
                m_vertices.Add(CreateUIVertex(rectOffsetedVertices[2], 1.0f));
            }
            if (!VertexUtility.IsDegeneracy(rectOffsetedVertices[0], rectOffsetedVertices[2],
                rectOffsetedVertices[3])) {
                m_vertices.Add(CreateUIVertex(rectOffsetedVertices[0], 1.0f));
                m_vertices.Add(CreateUIVertex(rectOffsetedVertices[2], 1.0f));
                m_vertices.Add(CreateUIVertex(rectOffsetedVertices[3], 1.0f));
            }
            for (int i = 0, iMax = 4; i < iMax; ++i) {
                int iNext = i + 1;
                if (iMax <= iNext) iNext = 0;
                if (!VertexUtility.IsDegeneracy(rectVertices[i], rectVertices[iNext], rectOffsetedVertices[i])) {
                    m_vertices.Add(CreateUIVertex(rectVertices[i], 0.0f));
                    m_vertices.Add(CreateUIVertex(rectVertices[iNext], 0.0f));
                    m_vertices.Add(CreateUIVertex(rectOffsetedVertices[i], 1.0f));
                }
                if (!VertexUtility.IsDegeneracy(rectVertices[iNext], rectOffsetedVertices[iNext],
                    rectOffsetedVertices[i])) {
                    m_vertices.Add(CreateUIVertex(rectVertices[iNext], 0.0f));
                    m_vertices.Add(CreateUIVertex(rectOffsetedVertices[iNext], 1.0f));
                    m_vertices.Add(CreateUIVertex(rectOffsetedVertices[i], 1.0f));
                }
            }
        }

        private static IEnumerable<Vector2> GetVertices(OrientedRect2D orientedRect) {
            Vector2 rotatedExtents = orientedRect.rotation * orientedRect.extents;
            for (int i = 0, iMax = 4; i < iMax; ++i) {
                Vector2 sign =
                    new Vector2(((i ^ (i >> 1)) & 0x1) == 0 ? 1.0f : -1.0f,
                        i >> 1 == 0 ? 1.0f : -1.0f); //xy:[++, -+, --, +-]
                yield return orientedRect.position + Vector2.Scale(rotatedExtents, sign);
            }
        }

        private static UIVertex CreateUIVertex(Vector2 position, float alpha) {
            UIVertex r = new UIVertex();
            r.position = position;
            r.color = new Color32(0xFF, 0xFF, 0xFF, (byte) (0xFF * alpha));
            return r;
        }

#if UNITY_EDITOR
        protected virtual void OnValidate() {
            SetVerticesDirty();
            foreach (SoftMaskMeshEffect effect in GetComponentsInChildren<SoftMaskMeshEffect>()) {
                effect.SetVerticesDirty();
            }
        }

        protected virtual void Reset() {
            m_padding = new RectOffset(4, 4, 4, 4);
        }
#endif
    }
}