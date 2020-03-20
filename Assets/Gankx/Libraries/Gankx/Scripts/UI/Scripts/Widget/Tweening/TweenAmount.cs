using UnityEngine;
using UnityEngine.UI;

namespace Gankx.UI
{
    /// <summary>
    /// Tween the object's alpha.
    /// </summary>
    [AddComponentMenu("UI/Tween/Tween Amount")]
    public class TweenAmount : Tweener
    {
#if UNITY_3_5
	public float from = 1f;
	public float to = 1f;
#else
        [Range(0f, 1f)]
        public float from = 1f;

        [Range(0f, 1f)]
        public float to = 1f;
#endif

        Image mImage;

        public Image cachedImage
        {
            get
            {
                if (mImage == null)
                {
                    mImage = GetComponent<AtlasImage>();
                }
                return mImage;
            }
        }

        /// <summary>
        /// Tween's current value.
        /// </summary>
        public float value
        {
            get { return cachedImage.fillAmount; }
            set { cachedImage.fillAmount = value; }
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
        public static TweenAmount Begin(GameObject go, float duration, float amount)
        {
            TweenAmount comp = Tweener.Begin<TweenAmount>(go, duration);
            comp.from = comp.value;
            comp.to = amount;

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
    }
}