using UnityEngine;

namespace UIExtention {
    public static class HashUtility {
        public static int GetValueHashCode(RectTransform rectTransform) {
            int result = rectTransform.GetHashCode();
            MergeValueHashCode(ref result, rectTransform.position);
            MergeValueHashCode(ref result, rectTransform.rotation);
            MergeValueHashCode(ref result, rectTransform.lossyScale);
            return result;
        }

        private static void MergeValueHashCode(ref int hash, Vector3 src) {
            RotateHashCode(ref hash);
            hash = hash ^ src.x.GetHashCode();
            RotateHashCode(ref hash);
            hash = hash ^ src.y.GetHashCode();
            RotateHashCode(ref hash);
            hash = hash ^ src.z.GetHashCode();
        }

        private static void MergeValueHashCode(ref int hash, Quaternion src) {
            RotateHashCode(ref hash);
            hash = hash ^ src.x.GetHashCode();
            RotateHashCode(ref hash);
            hash = hash ^ src.y.GetHashCode();
            RotateHashCode(ref hash);
            hash = hash ^ src.z.GetHashCode();
            RotateHashCode(ref hash);
            hash = hash ^ src.w.GetHashCode();
        }

        private static void RotateHashCode(ref int hash) {
            hash = (int) (((uint) hash >> 15) | ((uint) hash << 17));
        }
    }
}