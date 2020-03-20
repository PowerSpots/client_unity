using UnityEngine;

namespace Gankx.UI
{
    /// <summary>
    /// Tween the window's width.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Tween/Tween Width")]
    public class TweenWidth : Tweener
    {
        public float from = 100f;
        public float to = 100f;

        RectTransform mRectTrans;

        public RectTransform cachedRectTrans
        {
            get
            {
                if (mRectTrans == null) mRectTrans = GetComponent<RectTransform>();
                return mRectTrans;
            }
        }

        /// <summary>
        /// Tween's current value.
        /// </summary>
        public float value
        {
            get { return cachedRectTrans.rect.width; }
            set { cachedRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value); }
        }

        /// <summary>
        /// Tween the value.
        /// </summary>
        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = from * (1f - factor) + to * factor;
        }

        /// <summary>
        /// Start the tweening operation.
        /// </summary>
        public static TweenWidth Begin(RectTransform rectTrans, float duration, int width)
        {
            TweenWidth comp = Tweener.Begin<TweenWidth>(rectTrans.gameObject, duration);
            comp.from = rectTrans.rect.width;
            comp.to = width;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }

        [ContextMenu("Set 'From' to current value")]
        public override void SetStartToCurrentValue()
        {
            from = value;
        }

        [ContextMenu("Set 'To' to current value")]
        public override void SetEndToCurrentValue()
        {
            to = value;
        }

        [ContextMenu("Assume value of 'From'")]
        void SetCurrentValueToStart()
        {
            value = from;
        }

        [ContextMenu("Assume value of 'To'")]
        void SetCurrentValueToEnd()
        {
            value = to;
        }
    }
}