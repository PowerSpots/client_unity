using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace UIExtention {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class SoftMaskMeshEffect : BaseMeshEffect {
        public RectTransform rectTransform {
            get {
                if (m_rectTransform == null) {
                    m_rectTransform = transform as RectTransform;
                }
                return m_rectTransform;
            }
        }

        private RectTransform m_rectTransform;

        public OrientedRect2D orientedRect2D {
            get {
                if (!m_orientedRect2D.HasValue) {
                    m_orientedRect2D = new OrientedRect2D(rectTransform);
                }
                return m_orientedRect2D.Value;
            }
        }

        private OrientedRect2D? m_orientedRect2D;

        public SoftMaskRect softMaskRect {
            get {
                if (m_softMaskRect == null) {
                    m_softMaskRect = GetComponentInParent<SoftMaskRect>();
                }
                return m_softMaskRect;
            }
        }

        private SoftMaskRect m_softMaskRect;

        public void SetVerticesDirty() {
            graphic.SetVerticesDirty();
        }

        public override void ModifyMesh(VertexHelper vh) {
            if (!IsActive() || softMaskRect == null) {
                return;
            }

            if (!softMaskRect.orientedRect2D.Overlaps(orientedRect2D)) {
                vh.Clear();
                return;
            }

            List<UIVertex> vertices = new List<UIVertex>();
            vh.GetUIVertexStream(vertices);
            vh.Clear();
            List<int> indices = Enumerable.Range(0, vertices.Count).ToList();
            for (int i = 0, iMax = vertices.Count; i < iMax; ++i) {
                vertices[i] = SetPosition(vertices[i], rectTransform.TransformPoint(vertices[i].position));
            }
            VertexUtility.Intersect(vertices, indices, softMaskRect.Vertices);
            for (int i = 0, iMax = vertices.Count; i < iMax; ++i) {
                vertices[i] = SetPosition(vertices[i], rectTransform.InverseTransformPoint(vertices[i].position));
            }
            vh.AddUIVertexStream(vertices, indices);
        }

        private static UIVertex SetPosition(UIVertex vertex, Vector3 position) {
            vertex.position = position;
            return vertex;
        }

        protected override void OnTransformParentChanged() {
            base.OnTransformParentChanged();
            SetVerticesDirty();
            transferToSoftMaskRect();
        }

        private void transferToSoftMaskRect() {
            m_softMaskRect = null;
            if (softMaskRect == null) {
#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying) {
                    DestroyImmediate(this);
                }
                else //fall through
#endif
                    Destroy(this);
            }
        }

        protected virtual void LateUpdate() {
            int rectTransformHashCode = HashUtility.GetValueHashCode(rectTransform);
           
            if (lastRectTransformHashCode != rectTransformHashCode) {
                lastRectTransformHashCode = rectTransformHashCode;

                m_orientedRect2D = null;
                bool verticesDirty = softMaskRect.orientedRect2D.Overlaps(orientedRect2D);
                if (verticesDirty || lastVerticesDirty) {
                    SetVerticesDirty();
                }
                lastVerticesDirty = verticesDirty;
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate() {
            base.OnValidate();
        }

        protected override void Reset() {
            base.Reset();
        }

        [UnityEditor.MenuItem("CONTEXT/RectTransform/AddMaskEffect", false, 150)]
        public static void AddMaskMeshEffect() {
            if (UnityEditor.Selection.activeObject == null) return;;
            MaskableGraphic[] graphicses = (UnityEditor.Selection.activeObject as GameObject).transform
                .GetComponentsInChildren<MaskableGraphic>(true);
            for (int i = 0; i < graphicses.Length; i++) {
                SoftMaskMeshEffect effect = graphicses[i].GetComponent<SoftMaskMeshEffect>();
                if (effect == null) {
                    graphicses[i].gameObject.AddComponent<SoftMaskMeshEffect>();
                }
                graphicses[i].material = null;
            }
        }
#endif

        private int lastRectTransformHashCode;
        private bool lastVerticesDirty;
        private Vector3 lastRectTransformPos;
    }
}