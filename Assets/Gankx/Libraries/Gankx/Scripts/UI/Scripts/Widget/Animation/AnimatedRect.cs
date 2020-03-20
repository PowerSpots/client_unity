using UnityEngine;

namespace Gankx.UI
{
    /// <summary>
    /// Makes it possible to animate the window's width and height using Unity's animations.
    /// </summary>
    [ExecuteInEditMode]
    public class AnimatedRect : MonoBehaviour
    {
        public float width = 1f;
        public float height = 1f;

        RectTransform mRectTrans;

        void OnEnable()
        {
            mRectTrans = GetComponent<RectTransform>();
            LateUpdate();
        }

        void LateUpdate()
        {
            if (mRectTrans != null)
            {
                mRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                mRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
        }
    }
}