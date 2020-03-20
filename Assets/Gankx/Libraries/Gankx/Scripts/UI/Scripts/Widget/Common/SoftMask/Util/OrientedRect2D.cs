using System.Linq;
using UnityEngine;

namespace UIExtention {
    public struct OrientedRect2D {
        public OrientedRect2D(OrientedRect2D source) {
            position = source.position;
            extents = source.extents;
            rotation = source.rotation;
        }

        public OrientedRect2D(Rect rect, Quaternion rotation) : this(rect.center, rect.size, rotation) {
        }

        public OrientedRect2D(Vector2 position, Vector2 size, Quaternion rotation) {
            this.position = position;
            extents = size * 0.5f;
            this.rotation = rotation;
        }

        public OrientedRect2D(float x, float y, float width, float height, Quaternion rotation) {
            position = new Vector2(x, y);
            extents = new Vector2(width * 0.5f, height * 0.5f);
            this.rotation = rotation;
        }

        public OrientedRect2D(RectTransform rectTransform) {
            Rect rect = rectTransform.rect;
            position = (Vector2) rectTransform.position + Vector2.Scale(rect.center, rectTransform.lossyScale);
            extents = Vector2.Scale(rect.size, rectTransform.lossyScale) * 0.5f;
            rotation = rectTransform.rotation;
        }

        public Vector2 position;
        public Vector2 extents;
        public Quaternion rotation;

        public Vector2 center {
            get { return position; }
            set { position = value; }
        }

        public float x {
            get { return position.x; }
            set { position = new Vector2(value, position.y); }
        }

        public float y {
            get { return position.y; }
            set { position = new Vector2(position.x, value); }
        }

        public Vector2 size {
            get { return extents * 2.0f; }
            set { extents = value * 0.5f; }
        }

        public float height {
            get { return extents.y * 2.0f; }
            set { extents = new Vector2(extents.x, value * 0.5f); }
        }

        public float width {
            get { return extents.x * 2.0f; }
            set { extents = new Vector2(value * 0.5f, extents.y); }
        }

        public Vector2 max {
            get { return position + extents; }
            set {
                Vector2 m = min;
                position = new Vector2((value.x + m.x) * 0.5f, (value.y - m.y) * 0.5f);
                size = new Vector2(Mathf.Abs(value.x - m.x), Mathf.Abs(value.y - m.y));
            }
        }

        public Vector2 min {
            get { return position - extents; }
            set {
                Vector2 m = max;
                position = new Vector2((m.x + value.x) * 0.5f, (m.y - value.y) * 0.5f);
                size = new Vector2(Mathf.Abs(m.x - value.x), Mathf.Abs(m.y - value.y));
            }
        }

        public float xMax {
            get { return position.x + extents.x; }
        }

        public float xMin {
            get { return position.x - extents.x; }
        }

        public float yMax {
            get { return position.y + extents.y; }
        }

        public float yMin {
            get { return position.y - extents.y; }
        }

        public static OrientedRect2D MinMaxOrientedRect(float xmin, float ymin, float xmax, float ymax,
            Quaternion rotation) {
            return new OrientedRect2D((xmax + xmin) * 0.5f, (ymax + ymin) * 0.5f, Mathf.Abs(xmax - xmin),
                Mathf.Abs(ymax - ymin), rotation);
        }

        public bool Contains(Vector3 point) {
            Vector3 localPoint = Quaternion.Inverse(rotation) * point;
            if (extents.x < Mathf.Abs(localPoint.x - position.x)) return false;
            if (extents.y < Mathf.Abs(localPoint.y - position.y)) return false;
            return true;
        }

        public bool Contains(Vector2 point) {
            return Contains((Vector3) point);
        }

        public bool Overlaps(OrientedRect2D other) {
            Vector2 distanceAxis = position - other.position;
            AxisPack3 thisUnitAxis = new AxisPack3(rotation);
            AxisPack3 thisExtentsAxis = new AxisPack3(extents, rotation);
            AxisPack3 otherUnitAxis = new AxisPack3(other.rotation);
            AxisPack3 otherExtentsAxis = new AxisPack3(other.extents, other.rotation);

            //this
            for (int i = 0, iMax = 2; i < iMax; ++i) {
                Vector3 splitAxis = thisUnitAxis[i];
                float distance = GetVectorLengthOfProjection(distanceAxis, splitAxis);
                distance -= extents[i];
                distance -= GetVectorLengthOfProjection(otherExtentsAxis, splitAxis);
                if (0.0f < distance) return false;
            }
            //other
            for (int i = 0, iMax = 2; i < iMax; ++i) {
                Vector3 splitAxis = otherUnitAxis[i];
                float distance = GetVectorLengthOfProjection(distanceAxis, splitAxis);
                distance -= GetVectorLengthOfProjection(thisExtentsAxis, splitAxis);
                distance -= other.extents[i];
                if (0.0f < distance) return false;
            }
            //3rd split axis
            for (int i = 0, iMax = 2; i < iMax; ++i)
            for (int k = 0, kMax = 2; k < kMax; ++k) {
                Vector3 splitAxis = Vector3.Cross(thisUnitAxis[i], otherUnitAxis[k]);
                float distance = GetVectorLengthOfProjection(distanceAxis, splitAxis);
                distance -= GetVectorLengthOfProjection(thisExtentsAxis, splitAxis);
                distance -= GetVectorLengthOfProjection(otherExtentsAxis, splitAxis);
                if (0.0f < distance) return false;
            }

            //Hit
            return true;
        }

        private class AxisPack3 {
            private readonly Vector3[] axis;

            public Vector3 this[int i] {
                set { axis[i] = value; }
                get { return axis[i]; }
            }

            public Vector3 right {
                set { axis[0] = value; }
                get { return axis[0]; }
            }

            public Vector3 up {
                set { axis[1] = value; }
                get { return axis[1]; }
            }

            public Vector3 forward {
                set { axis[2] = value; }
                get { return axis[2]; }
            }

            public AxisPack3() : this(Vector3.one, Quaternion.identity) {
            }

            public AxisPack3(Vector3 scale) : this(scale, Quaternion.identity) {
            }

            public AxisPack3(Quaternion rotation) : this(Vector3.one, rotation) {
            }

            public AxisPack3(Vector3 scale, Quaternion rotation) {
                axis = new[] {Vector3.right * scale.x, Vector3.up * scale.y, Vector3.forward * scale.z}
                    .Select(x => rotation * x).ToArray();
            }

            public AxisPack3(Vector3 right, Vector3 up, Vector3 forward) {
                axis = new[] {right, up, forward}.ToArray();
            }
        }

        private static float GetVectorLengthOfProjection(Vector3 src, Vector3 projection) {
            return Mathf.Abs(Vector3.Dot(projection, src));
        }

        private static float GetVectorLengthOfProjection(AxisPack3 axis, Vector3 projection) {
            float result = Enumerable.Range(0, 3)
                .Select(x => GetVectorLengthOfProjection(projection, axis[x]))
                .Sum();
            return result;
        }
    }
}