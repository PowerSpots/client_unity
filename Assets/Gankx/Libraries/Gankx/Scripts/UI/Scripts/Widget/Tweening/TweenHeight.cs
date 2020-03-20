using UnityEngine;

namespace Gankx.UI
{
    /// <summary>
    /// Tween the window's height.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Tween/Tween Height")]
    public class TweenHeight : Tweener
    {
        public float from = 100;
        public float to = 100;

        RectTransform mRect;

        public RectTransform cachedRect
        {
            get
            {
                if (mRect == null)
                {
                    mRect = GetComponent<RectTransform>();
                }
                return mRect;
            }
        }

        /// <summary>
        /// Tween's current value.
        /// </summary>
        public float value
        {
            get { return cachedRect.rect.height; }
            set { cachedRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value); }
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
        public static TweenHeight Begin(RectTransform rectTrans, float duration, int height)
        {
            TweenHeight comp = Tweener.Begin<TweenHeight>(rectTrans.gameObject, duration);
            comp.from = rectTrans.rect.height;
            comp.to = height;

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