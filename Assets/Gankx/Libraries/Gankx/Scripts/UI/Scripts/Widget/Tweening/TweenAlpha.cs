using UnityEngine;
using UnityEngine.UI;

namespace Gankx.UI
{
    [AddComponentMenu("UI/Tween/Tween Alpha")]
    public class TweenAlpha : Tweener
    {
        [Range(0f, 1f)]
        public float from = 1f;

        [Range(0f, 1f)]
        public float to = 1f;

        bool mCached = false;

        private MaskableGraphic mMaskableGraphic;
        private CanvasGroup     mCanvasGroup;

        Material mMat;
        SpriteRenderer mSr;

        void Cache()
        {
            mCached = true;
            mMaskableGraphic    = GetComponent<MaskableGraphic>();
            mCanvasGroup        = GetComponent<CanvasGroup>();
            mSr                 = GetComponent<SpriteRenderer>();

            if (mMaskableGraphic == null && mSr == null)
            {
                Renderer ren = GetComponent<Renderer>();
                if (ren != null) mMat = ren.material;
                if (mMat == null) mMaskableGraphic = GetComponentInChildren<MaskableGraphic>();
            }
        }

        /// <summary>
        /// Tween's current value.
        /// </summary>
        public float value
        {
            get
            {
                if (!mCached) Cache();
                if (mCanvasGroup != null) return mCanvasGroup.alpha;
                if (mMaskableGraphic != null) return mMaskableGraphic.color.a;
                if (mSr != null) return mSr.color.a;
                return mMat != null ? mMat.color.a : 1f;
            }
            set
            {
                if (!mCached) Cache();

                if (mCanvasGroup != null)
                {
                    mCanvasGroup.alpha = value;
                }
                else if (mMaskableGraphic != null)
                {
                    Color c = mMaskableGraphic.color;
                    c.a = value;
                    mMaskableGraphic.color = c;
                }
                else if (mSr != null)
                {
                    Color c = mSr.color;
                    c.a = value;
                    mSr.color = c;
                }
                else if (mMat != null)
                {
                    Color c = mMat.color;
                    c.a = value;
                    mMat.color = c;
                }
            }
        }

        /// <summary>
        /// Tween the value.
        /// </summary>
        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = Mathf.Lerp(from, to, factor);
        }

        /// <summary>
        /// Start the tweening operation.
        /// </summary>
        public static TweenAlpha Begin(GameObject go, float duration, float alpha)
        {
            TweenAlpha comp = Tweener.Begin<TweenAlpha>(go, duration);
            comp.from = comp.value;
            comp.to = alpha;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }

        public override void SetStartToCurrentValue()
        {
            from = value;
        }

        public override void SetEndToCurrentValue()
        {
            to = value;
        }

        [ContextMenu("ResetOnce")]
        public void ResetOnce()
        {
            value = from;
            tweenFactor = 0;
        }
    }
}