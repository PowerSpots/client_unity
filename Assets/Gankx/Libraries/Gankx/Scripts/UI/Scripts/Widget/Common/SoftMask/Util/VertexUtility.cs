﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UIExtention {
    public static class VertexUtility {
        public static UIVertex Lerp(UIVertex a, UIVertex b, float f) {
            return new UIVertex {
                color = Color32.Lerp(a.color, b.color, f),
                normal = Vector3.Lerp(a.normal, b.normal, f),
                position = Vector3.Lerp(a.position, b.position, f),
                tangent = Vector4.Lerp(a.tangent, b.tangent, f),
                uv0 = Vector2.Lerp(a.uv0, b.uv0, f),
                uv1 = Vector2.Lerp(a.uv1, b.uv1, f)
            };
        }

        public static void Lerp(UIVertex a, UIVertex b, float f, out UIVertex result) {
            result = new UIVertex();
            result.color = Color32.Lerp(a.color, b.color, f);
            result.normal = Vector3.Lerp(a.normal, b.normal, f);
            result.position = Vector3.Lerp(a.position, b.position, f);
            result.tangent = Vector4.Lerp(a.tangent, b.tangent, f);
            result.uv0 = Vector2.Lerp(a.uv0, b.uv0, f);
            result.uv1 = Vector2.Lerp(a.uv1, b.uv1, f);
        }

        public static UIVertex WeightedAverage(IEnumerable<UIVertex> vertices, IEnumerable<float> weights) {
            UIVertex result;
            WeightedAverage(vertices, weights, out result);
            return result;
        }

        public static void WeightedAverage(IEnumerable<UIVertex> vertices, IEnumerable<float> weights,
            out UIVertex result) {
            result = new UIVertex();
            result.position = Vector3.zero;
            result.normal = Vector3.zero;
            Color color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            result.uv0 = Vector2.zero;
            result.uv1 = Vector2.zero;
            result.tangent = Vector4.zero;

            IEnumerator<UIVertex> vEnumerator = vertices.GetEnumerator();
            IEnumerator<float> wEnumerator = weights.GetEnumerator();
            while (vEnumerator.MoveNext() && wEnumerator.MoveNext()) {
                UIVertex vertex = vEnumerator.Current;
                float weight = wEnumerator.Current;

                result.position += vertex.position * weight;
                result.normal += vertex.normal * weight;
                color += (Color) vertex.color * weight;
                result.uv0 += vertex.uv0 * weight;
                result.uv1 += vertex.uv1 * weight;
                result.tangent += vertex.tangent * weight;
            }
            result.color = color;
        }

        public static UIVertex MultiplyVertexColor(UIVertex a, UIVertex b) {
            return new UIVertex {
                position = a.position,
                normal = a.normal,
                color = new Color32((byte) (a.color.r * b.color.r / 0xFF)
                    , (byte) (a.color.g * b.color.g / 0xFF)
                    , (byte) (a.color.b * b.color.b / 0xFF)
                    , (byte) (a.color.a * b.color.a / 0xFF)
                ),
                uv0 = a.uv0,
                uv1 = a.uv1,
                tangent = a.tangent
            };
        }

        private static float GetSqrDistance(UIVertex aStart, UIVertex aEnd, UIVertex bStart, UIVertex bEnd) {
            return GetSqrDistance(aStart.position, aEnd.position, bStart.position, bEnd.position);
        }

        private static float GetSqrDistance(Vector3 aStart, Vector3 aEnd, Vector3 bStart, Vector3 bEnd) {
            Array2<float> nearestProgress = GetProgressOfNearestPoint(aStart, aEnd, bStart, bEnd);
            Vector3 aPosition = Vector3.Lerp(aStart, aEnd, nearestProgress.a);
            Vector3 bPosition = Vector3.Lerp(bStart, bEnd, nearestProgress.b);
            Vector3 distance = aPosition - bPosition;
            return distance.sqrMagnitude;
        }

        private static UIVertex? GetCrossPointWithoutPoint(UIVertex aStart, UIVertex aEnd, UIVertex bStart,
            UIVertex bEnd, Func<UIVertex, UIVertex, UIVertex> blend) {
            Array2<float> nearestProgress =
                GetProgressOfNearestPoint(aStart.position, aEnd.position, bStart.position, bEnd.position);
            if (nearestProgress.a < float.Epsilon || 1.0f - nearestProgress.a < float.Epsilon ||
                nearestProgress.b < float.Epsilon || 1.0f - nearestProgress.b < float.Epsilon) return null;
            UIVertex a = Lerp(aStart, aEnd, nearestProgress.a);
            UIVertex b = Lerp(bStart, bEnd, nearestProgress.b);
            if (Vector2.SqrMagnitude((Vector2) a.position - (Vector2) b.position) < Vector2.kEpsilon)
                return blend(a, b);
            return null;
        }

        public static Array2<UIVertex> GetNearestPoint(UIVertex aStart, UIVertex aEnd, UIVertex bStart, UIVertex bEnd) {
            Array2<float> nearestProgress =
                GetProgressOfNearestPoint(aStart.position, aEnd.position, bStart.position, bEnd.position);
            return new Array2<UIVertex>(Lerp(aStart, aEnd, nearestProgress.a)
                , Lerp(aStart, aEnd, nearestProgress.b)
            );
        }

        public static Array2<Vector3> GetNearestPoint(Vector3 aStart, Vector3 aEnd, Vector3 bStart, Vector3 bEnd) {
            Array2<float> nearestProgress = GetProgressOfNearestPoint(aStart, aEnd, bStart, bEnd);
            return new Array2<Vector3>(Vector3.Lerp(aStart, aEnd, nearestProgress.a)
                , Vector3.Lerp(aStart, aEnd, nearestProgress.b)
            );
        }

        public static Array2<float> GetProgressOfNearestPoint(UIVertex aStart, UIVertex aEnd, UIVertex bStart,
            UIVertex bEnd) {
            return GetProgressOfNearestPoint(aStart.position, aEnd.position, bStart.position, bEnd.position);
        }

        public static Array2<float> GetProgressOfNearestPoint(Vector3 aStart, Vector3 aEnd, Vector3 bStart,
            Vector3 bEnd) {
            Vector3 aDirection = aEnd - aStart;
            Vector3 bDirection = bEnd - bStart;

            Vector3 between = aStart - bStart;
            float aSqrLength = aDirection.sqrMagnitude;
            float bSqrLength = bDirection.sqrMagnitude;

            float aProgress, bProgress;
            do {
                if (0.0f == aSqrLength || 0.0f == bSqrLength) {
                    //Point & Point
                    aProgress = 0.0f;
                    bProgress = 0.0f;
                    break;
                }

                float betweenOnBProjection = Vector3.Dot(bDirection, between);
                if (0.0f == aSqrLength) {
                    //Point & Segment
                    aProgress = 0.0f;
                    bProgress = Mathf.Clamp01(betweenOnBProjection / bSqrLength);
                    break;
                }
                float betweenOnAProjection = Vector3.Dot(aDirection, between);
                if (0.0f == bSqrLength) {
                    //Segment & Point
                    aProgress = Mathf.Clamp01(-betweenOnAProjection / aSqrLength);
                    bProgress = 0.0f;
                    break;
                }
                //Segment & Segment
                float bOnAProjection = Vector3.Dot(aDirection, bDirection);
                float denom = aSqrLength * bSqrLength - bOnAProjection * bOnAProjection;
                if (0.0f != denom)
                    aProgress = Mathf.Clamp01(
                        (bOnAProjection * betweenOnBProjection - betweenOnAProjection * bSqrLength) / denom);
                else aProgress = 0.0f;
                float bProgressNom = bOnAProjection * aProgress + betweenOnBProjection;
                if (bProgressNom < 0.0f) {
                    bProgress = 0.0f;
                    aProgress = Mathf.Clamp01(-betweenOnAProjection / aSqrLength);
                }
                else if (bSqrLength < bProgressNom) {
                    bProgress = 1.0f;
                    aProgress = Mathf.Clamp01((bOnAProjection - betweenOnAProjection) / aSqrLength);
                }
                else {
                    bProgress = bProgressNom / bSqrLength;
                }
            } while (false);
            return new Array2<float>(aProgress, bProgress);
        }

        public struct Array2<T> {
            public T a;
            public T b;

            public Array2(T a, T b) {
                this.a = a;
                this.b = b;
            }

            public IEnumerable<T> GetEnumerable() {
                yield return a;
                yield return b;
            }
        }

        public struct Array3<T> {
            public T a;
            public T b;
            public T c;

            public Array3(T a, T b, T c) {
                this.a = a;
                this.b = b;
                this.c = c;
            }

            public IEnumerable<T> GetEnumerable() {
                yield return a;
                yield return b;
                yield return c;
            }
        }

        public static void Intersect(List<UIVertex> vertices, List<UIVertex> mask) {
            Intersect(vertices, mask, MultiplyVertexColor);
        }

        public static void Intersect(List<UIVertex> vertices, List<UIVertex> mask,
            Func<UIVertex, UIVertex, UIVertex> blend) {
            List<int> indices = Enumerable.Range(0, vertices.Count).ToList();
            Intersect(vertices, indices, mask, blend);
            int verticesCount = vertices.Count;
            vertices.AddRange(indices.Select(x => vertices[x]));
            vertices.RemoveRange(0, verticesCount);
        }

        public static void Intersect(List<UIVertex> vertices, List<int> indices, List<UIVertex> mask) {
            Intersect(vertices, indices, mask, MultiplyVertexColor);
        }

        public static void Intersect(List<UIVertex> vertices, List<int> indices, List<UIVertex> mask,
            Func<UIVertex, UIVertex, UIVertex> blend) {
            int indicesOriginalCount = indices.Count;

            IndexedVerticesOffset pertexPack = new IndexedVerticesOffset(vertices, indices);
            VerticesOffset maskPack = new VerticesOffset(mask);
            Vector2[] vertices2d = new Vector2[3];
            Vector2[] mask2d = new Vector2[3];
            for (int m = 0, mMax = maskPack.vertices.Count; m < mMax; m += 3) {
                for (int k = 0, kMax = 3; k < kMax; ++k) mask2d[k] = maskPack.vertices[m + k].position;
                maskPack.start = m;
                if (!IsDegeneracy(mask2d))
                    for (int i = 0, iMax = indicesOriginalCount; i < iMax; i += 3) {
                        for (int k = 0, kMax = 3; k < kMax; ++k)
                            vertices2d[k] = pertexPack.vertices[pertexPack.indices[i + k]].position;
                        if (!IsDegeneracy(vertices2d)) {
                            pertexPack.start = i;
                            MaskTriangles(pertexPack, vertices2d, maskPack, mask2d, blend);
                        }
                    }
            }

            indices.RemoveRange(0, indicesOriginalCount);
        }

        private struct IndexedVerticesOffset {
            public readonly List<UIVertex> vertices;
            public readonly List<int> indices;
            public int start;

            public IndexedVerticesOffset(List<UIVertex> vertices, List<int> indices, int start = 0) {
                this.vertices = vertices;
                this.indices = indices;
                this.start = start;
            }

            public IEnumerable<UIVertex> GetEnumerable() {
                for (int i = start, iMax = indices.Count; i < iMax; ++i) yield return vertices[indices[i]];
            }
        }

        private struct VerticesOffset {
            public readonly List<UIVertex> vertices;
            public int start;

            public VerticesOffset(List<UIVertex> vertices, int start = 0) {
                this.vertices = vertices;
                this.start = start;
            }

            public IEnumerable<UIVertex> GetEnumerable() {
                for (int i = start, iMax = vertices.Count; i < iMax; ++i) yield return vertices[i];
            }
        }

        private static void MaskTriangles(IndexedVerticesOffset vertexPack, Vector2[] vertices2d,
            VerticesOffset maskPack, Vector2[] mask2d, Func<UIVertex, UIVertex, UIVertex> blend) {
            int indicesBaseCount = vertexPack.indices.Count;
            int addCount = AddContainsVertex(vertexPack, vertices2d, maskPack, mask2d, blend);
            if (addCount == 3) return;
            addCount += AddContainsMask(vertexPack, vertices2d, maskPack, mask2d, blend);
            addCount += AddCrossPoint(vertexPack, vertices2d, maskPack, mask2d, blend);
            if (3 < addCount) {
                Vector2 center = Vector2.zero;
                IEnumerable<Vector2> positions = Enumerable.Range(indicesBaseCount, addCount)
                    .Select(x => (Vector2) vertexPack.vertices[vertexPack.indices[x]].position);
                foreach (Vector2 position in positions) {
                    center.x += position.x;
                    center.y += position.y;
                }
                center *= 1.0f / addCount;

                vertexPack.indices.Sort(indicesBaseCount, addCount,
                    new CounterClockWiseUIVertex(vertexPack.vertices, center));
                for (int i = 2; i < addCount; ++i) {
                    vertexPack.indices.Add(vertexPack.indices[indicesBaseCount + 0]);
                    vertexPack.indices.Add(vertexPack.indices[indicesBaseCount + i - 1]);
                    vertexPack.indices.Add(vertexPack.indices[indicesBaseCount + i]);
                }
            }
            else if (addCount == 0 || addCount == 3) {
                return;
            }
            vertexPack.indices.RemoveRange(indicesBaseCount, addCount);
        }

        private static int AddContainsVertex(IndexedVerticesOffset vertexPack, Vector2[] vertices2d,
            VerticesOffset maskPack, Vector2[] mask2d, Func<UIVertex, UIVertex, UIVertex> blend) {
            int result = 0;
            for (int i = 0, iMax = 3; i < iMax; ++i)
                if (ContainsInConvexHull(mask2d, vertices2d[i])) {
                    vertexPack.indices.Add(vertexPack.vertices.Count);
                    UIVertex vertex = vertexPack.vertices[vertexPack.indices[vertexPack.start + i]];
                    UIVertex maskVertex = PickupUIVertexFromTriangle(vertex.position, maskPack, mask2d);
                    vertexPack.vertices.Add(blend(vertex, maskVertex));
                    ++result;
                }
            return result;
        }

        private static int AddContainsMask(IndexedVerticesOffset vertexPack, Vector2[] vertices2d,
            VerticesOffset maskPack, Vector2[] mask2d, Func<UIVertex, UIVertex, UIVertex> blend) {
            int result = 0;
            for (int i = 0, iMax = 3; i < iMax; ++i)
                if (ContainsInConvexHull(vertices2d, mask2d[i])) {
                    UIVertex vertex = PickupUIVertexFromTriangle(mask2d[i], vertexPack, vertices2d);
                    vertexPack.indices.Add(vertexPack.vertices.Count);
                    vertexPack.vertices.Add(blend(vertex, maskPack.vertices[maskPack.start + i]));
                    ++result;
                }
            return result;
        }

        private static int AddCrossPoint(IndexedVerticesOffset vertexPack, Vector2[] vertices2d,
            VerticesOffset maskPack, Vector2[] mask2d, Func<UIVertex, UIVertex, UIVertex> blend) {
            int result = 0;
            for (int i = 0, iMax = 3; i < iMax; ++i) {
                int iNext = i + 1;
                if (iMax <= iNext) iNext = 0;
                for (int k = 0, kMax = 3; k < kMax; ++k) {
                    int kNext = k + 1;
                    if (kMax <= kNext) kNext = 0;

                    UIVertex? crossPoint = GetCrossPointWithoutPoint(
                        vertexPack.vertices[vertexPack.indices[vertexPack.start + i]]
                        , vertexPack.vertices[vertexPack.indices[vertexPack.start + iNext]]
                        , maskPack.vertices[maskPack.start + k]
                        , maskPack.vertices[maskPack.start + kNext]
                        , blend
                    );
                    if (crossPoint.HasValue) {
                        vertexPack.indices.Add(vertexPack.vertices.Count);
                        vertexPack.vertices.Add(crossPoint.Value);
                        ++result;
                    }
                }
            }
            return result;
        }

        private class CounterClockWiseUIVertex : IComparer<int> {
            public CounterClockWiseUIVertex(List<UIVertex> vertices, Vector2 center) {
                m_vertices = vertices;
                m_center = center;
            }

            public int Compare(int x, int y) {
                float compare = Atan2((Vector2) m_vertices[x].position - m_center) -
                                Atan2((Vector2) m_vertices[y].position - m_center);
                return 0.0f < compare
                    ? 1
                    : compare < 0.0f
                        ? -1
                        : 0;
            }

            private float Atan2(Vector2 x) {
                return Mathf.Atan2(x.y, x.x);
            }

            private readonly List<UIVertex> m_vertices;
            private readonly Vector2 m_center;
        }

        public static List<int> Triangulation(List<Vector2> vertices) {
            Vector2[] outerVertices = GetVerticesOfOuterTriangle(vertices);
            List<Triangle> triangles = new List<Triangle>();
            triangles.Add(new Triangle(new[] {-1, -2, -3}, vertices, outerVertices));

            Stack<Side> doubtSides = new Stack<Side>();
            for (int i = 0, iMax = vertices.Count; i < iMax; ++i)
                PlungeVertex(triangles, doubtSides, i, vertices, outerVertices);

            RemoveOuterTriangle(triangles, doubtSides, vertices, outerVertices);

            List<int> result = triangles.SelectMany(x => x.indices)
                .ToList();
            return result;
        }

        private class Triangle {
            public readonly int[] indices; //<=0:Index of vertices, 0<: One's complement index of outerVertices
            public readonly Vector2[] vertices;
            private Vector2 center;
            private float sqrRadius;

            public Triangle(int[] indices, List<Vector2> vertices, Vector2[] outerVertices) {
                this.indices = indices;
                this.vertices = indices.Select(x => GetVertices(x, vertices, outerVertices)).ToArray();
                CacheCircumscribedCircle();
            }

            public bool Valid() {
                return !(float.IsInfinity(sqrRadius) || float.IsNaN(sqrRadius));
            }

            public bool Contains(Vector2 point) {
                return ContainsInConvexHull(vertices, point);
            }

            public bool ContainsInCircumscribedCircle(Vector2 point) {
                bool result = (center - point).sqrMagnitude <= sqrRadius;
                return result;
            }

            public bool ContainsInCircumscribedCircleWithoutEdge(Vector2 point) {
                bool result = (center - point).sqrMagnitude < sqrRadius;
                return result;
            }

            public bool HasSide(Side side) {
                return indices.Length == indices.Concat(side.indices).Distinct().Count();
            }

            public int GetOtherIndex(Side side) {
                return indices.Except(side.indices).First();
            }

            private void CacheCircumscribedCircle() {
                center.y = center.x =
                    1.0f / (2.0f * ((vertices[1].x - vertices[0].x) * (vertices[2].y - vertices[0].y) -
                                    (vertices[1].y - vertices[0].y) * (vertices[2].x - vertices[0].x)));
                center.x *=
                    (vertices[2].y - vertices[0].y) * (vertices[1].x * vertices[1].x - vertices[0].x * vertices[0].x +
                                                       vertices[1].y * vertices[1].y - vertices[0].y * vertices[0].y) +
                    (vertices[0].y - vertices[1].y) * (vertices[2].x * vertices[2].x - vertices[0].x * vertices[0].x +
                                                       vertices[2].y * vertices[2].y - vertices[0].y * vertices[0].y);
                center.y *=
                    (vertices[0].x - vertices[2].x) * (vertices[1].x * vertices[1].x - vertices[0].x * vertices[0].x +
                                                       vertices[1].y * vertices[1].y - vertices[0].y * vertices[0].y) +
                    (vertices[1].x - vertices[0].x) * (vertices[2].x * vertices[2].x - vertices[0].x * vertices[0].x +
                                                       vertices[2].y * vertices[2].y - vertices[0].y * vertices[0].y);
                sqrRadius = (vertices[0] - center).sqrMagnitude;
            }
        }

        private class Side {
            public readonly int[] indices; //<=0:Index of vertices, 0<: One's complement index of outerVertices
            public readonly Vector2[] vertices;

            public Side(int[] indices, List<Vector2> vertices, Vector2[] outerVertices) {
                this.indices = indices;
                this.vertices = indices.Select(x => GetVertices(x, vertices, outerVertices)).ToArray();
            }
        }

        private static Vector2 GetVertices(int index, List<Vector2> vertices, Vector2[] outerVertices) {
            return 0 <= index ? vertices[index] : outerVertices[-index - 1];
        }

        private static Vector2[] GetVerticesOfOuterTriangle(List<Vector2> vertices) {
            float minX = vertices[0].x;
            float minY = vertices[0].y;
            float maxX = vertices[0].x;
            float maxY = vertices[0].y;
            foreach (Vector2 point in vertices.Skip(1)) {
                if (point.x < minX) minX = point.x;
                if (point.y < minY) minY = point.y;
                if (maxX < point.x) maxX = point.x;
                if (maxY < point.y) maxY = point.y;
            }
            Vector2[] result = new Vector2[3];
            result[0].x = maxX;
            result[0].y = maxY;
            float diagonalLength = (result[0] - new Vector2(minX, minY)).magnitude;
            float sideLength = diagonalLength * 1.41422f; //A little bigger than sqrt(2)
            result[1].x = maxX;
            result[1].y = maxY - sideLength;
            result[2].x = maxX - sideLength;
            result[2].y = maxY;
            return result;
        }

        private static void PlungeVertex(List<Triangle> triangles, Stack<Side> doubtSides, int verticesIndex,
            List<Vector2> vertices, Vector2[] outerVertices) {
            Vector2 vertex = vertices[verticesIndex];
            int[] trianglesindices = triangles.Select((x, i) => new {Element = x, Index = i})
                .Where(x => x.Element.Contains(vertex))
                .Select(x => x.Index)
                .Reverse()
                .ToArray();
            foreach (int trianglesIndex in trianglesindices) {
                Triangle removeTriangle = triangles[trianglesIndex];
                triangles.RemoveAt(trianglesIndex);
                for (int i = 0, iMax = removeTriangle.indices.Length; i < iMax; ++i) {
                    int[] indices = {
                        removeTriangle.indices[i],
                        removeTriangle.indices[i < removeTriangle.indices.Length - 1 ? i + 1 : 0], verticesIndex
                    };
                    Triangle addTriangle = new Triangle(indices, vertices, outerVertices);
                    if (addTriangle.Valid()) triangles.Add(addTriangle);
                    Side doubtSide = new Side(new[] {indices[0], indices[1]}, vertices, outerVertices);
                    if (!doubtSides.Contains(doubtSide, new DoubtSideEqualityComparer())) doubtSides.Push(doubtSide);
                }
            }
            while (0 < doubtSides.Count()) {
                Side removeSide = doubtSides.Pop();
                Triangle[] suspectTriangles = triangles.Where(x => x.HasSide(removeSide)).ToArray();
                if (suspectTriangles.Length == 2) {
                    int otherIndex = suspectTriangles[1].GetOtherIndex(removeSide);
                    Vector2 otherVertex = GetVertices(otherIndex, vertices, outerVertices);
                    bool flip = suspectTriangles[0].ContainsInCircumscribedCircleWithoutEdge(otherVertex);
                    if (flip) {
                        triangles.Remove(suspectTriangles[0]);
                        triangles.Remove(suspectTriangles[1]);
                        int moreOtherIndex = suspectTriangles[0].GetOtherIndex(removeSide);
                        triangles.Add(new Triangle(new[] {otherIndex, moreOtherIndex, removeSide.indices[0]}, vertices,
                            outerVertices));
                        triangles.Add(new Triangle(new[] {otherIndex, moreOtherIndex, removeSide.indices[1]}, vertices,
                            outerVertices));
                        doubtSides.Push(new Side(new[] {otherIndex, removeSide.indices[0]}, vertices, outerVertices));
                        doubtSides.Push(new Side(new[] {otherIndex, removeSide.indices[1]}, vertices, outerVertices));
                        doubtSides.Push(
                            new Side(new[] {moreOtherIndex, removeSide.indices[0]}, vertices, outerVertices));
                        doubtSides.Push(
                            new Side(new[] {moreOtherIndex, removeSide.indices[1]}, vertices, outerVertices));
                    }
                }
            }
        }

        private class DoubtSideEqualityComparer : IEqualityComparer<Side> {
            public bool Equals(Side x, Side y) {
                return x.indices.All(z => y.indices.Contains(z));
            }

            public int GetHashCode(Side obj) {
                return obj.indices.Aggregate(0X01F11FE0, (x, y) => x ^ y);
            }
        }

        private static void RemoveOuterTriangle(List<Triangle> triangles, Stack<Side> doubtSides,
            List<Vector2> vertices, Vector2[] outerVertices) {
            IEnumerable<Side> removeSides = triangles.Where(x => x.indices.Where(y => y < 0).Take(2).Count() == 1)
                .Select(x => x.indices.OrderBy(y => y).ToArray())
                .SelectMany(x => new[] {new[] {x[0], x[1]}, new[] {x[0], x[2]}})
                .Distinct()
                .Select(x => new Side(x, vertices, outerVertices));
            foreach (Side removeSide in removeSides) doubtSides.Push(removeSide);
            while (0 < doubtSides.Count()) {
                Side removeSide = doubtSides.Pop();
                Triangle[] suspectTriangles = triangles.Where(x => x.HasSide(removeSide)).ToArray();
                if (suspectTriangles.Length == 2) {
                    int otherIndex = suspectTriangles[0].GetOtherIndex(removeSide);
                    int moreOtherIndex = suspectTriangles[1].GetOtherIndex(removeSide);
                    Vector2[] newTriangleVertices = {
                        removeSide.vertices[0], GetVertices(otherIndex, vertices, outerVertices),
                        GetVertices(moreOtherIndex, vertices, outerVertices)
                    };
                    bool flip = !ContainsInConvexHull(newTriangleVertices, removeSide.vertices[1]);
                    if (flip) {
                        triangles.Remove(suspectTriangles[0]);
                        triangles.Remove(suspectTriangles[1]);
                        triangles.Add(new Triangle(new[] {otherIndex, moreOtherIndex, removeSide.indices[0]}, vertices,
                            outerVertices));
                        triangles.Add(new Triangle(new[] {otherIndex, moreOtherIndex, removeSide.indices[1]}, vertices,
                            outerVertices));
                    }
                }
            }
            triangles.RemoveAll(x => IgnoreTriangle(x));
        }

        public static bool ContainsInConvexHull(Vector2[] vertices, Vector2 point) {
            Func<Vector2, Vector2, float> Vector2Cross = (lhs, rhs) => { return lhs.x * rhs.y - lhs.y * rhs.x; };
            float cross = Vector2Cross(vertices[0] - vertices[vertices.Length - 1],
                point - vertices[vertices.Length - 1]);
            for (int i = 0, iMax = vertices.Length - 1; i < iMax; ++i) {
                float temp = Vector2Cross(vertices[i + 1] - vertices[i], point - vertices[i]);
                if (cross * temp < 0.0f) return false;
                if (cross == 0.0f && temp != 0.0f) cross = temp;
            }
            return true;
        }

        private static bool IgnoreTriangle(Triangle triangle) {
            return triangle.indices.Any(x => x < 0);
        }

        private static float Vector2Cross(Vector2 lhs, Vector2 rhs) {
            return lhs.x * rhs.y - lhs.y * rhs.x;
        }

        private static float GetAreaOfTriangle(Vector2 vertex0, Vector2 vertex1, Vector2 vertex2) {
            Vector2 vector01 = vertex1 - vertex0;
            Vector2 vector02 = vertex2 - vertex0;
            return 0.5f * Mathf.Abs(Vector2Cross(vector01, vector02));
        }

        public static bool IsDegeneracy(Vector2[] vertices) {
            Vector2 vector01 = vertices[1] - vertices[0];
            Vector2 vector02 = vertices[2] - vertices[0];
            return Mathf.Abs(Vector2Cross(vector01, vector02)) < float.Epsilon;
        }

        public static bool IsDegeneracy(Vector2 vertex0, Vector2 vertex1, Vector2 vertex2) {
            Vector2 vector01 = vertex1 - vertex0;
            Vector2 vector02 = vertex2 - vertex0;
            return Vector2Cross(vector01, vector02) < float.Epsilon;
        }

        private static UIVertex PickupUIVertexFromTriangle(Vector2 pickupPosition, IndexedVerticesOffset vertexPack,
            Vector2[] vertices2d) {
            float inverseArea = 1.0f / GetAreaOfTriangle(vertices2d[0], vertices2d[1], vertices2d[2]);
            IEnumerable<UIVertex> vertices = vertexPack.GetEnumerable();
            IEnumerable<float> weights = new Array3<float>(
                GetAreaOfTriangle(pickupPosition, vertices2d[1], vertices2d[2]) * inverseArea
                , GetAreaOfTriangle(vertices2d[0], pickupPosition, vertices2d[2]) * inverseArea
                , GetAreaOfTriangle(vertices2d[0], vertices2d[1], pickupPosition) * inverseArea
            ).GetEnumerable();
            UIVertex result = WeightedAverage(vertices, weights);
            return result;
        }

        private static UIVertex PickupUIVertexFromTriangle(Vector2 pickupPosition, VerticesOffset vertexPack,
            Vector2[] vertices2d) {
            float inverseArea = 1.0f / GetAreaOfTriangle(vertices2d[0], vertices2d[1], vertices2d[2]);
            IEnumerable<UIVertex> vertices = vertexPack.GetEnumerable();
            IEnumerable<float> weights = new Array3<float>(
                GetAreaOfTriangle(pickupPosition, vertices2d[1], vertices2d[2]) * inverseArea
                , GetAreaOfTriangle(vertices2d[0], pickupPosition, vertices2d[2]) * inverseArea
                , GetAreaOfTriangle(vertices2d[0], vertices2d[1], pickupPosition) * inverseArea
            ).GetEnumerable();
            UIVertex result = WeightedAverage(vertices, weights);
            return result;
        }
    }
}