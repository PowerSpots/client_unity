using UnityEngine;
using UnityEngine.UI;

namespace Gankx.UI
{
    [AddComponentMenu("UI/Tween/Tween Slider")]
    public class TweenSlider : Tweener
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

        Slider mSlider;

        public Slider cachedSlider
        {
            get
            {
                if (mSlider == null)
                {
                    mSlider = GetComponent<Slider>();
                    if (mSlider == null)
                    {
                        Debug.LogError("TweenSlider needs an Slider to work with", this);
                        enabled = false;
                    }
                }
                return mSlider;
            }
        }

        public float value
        {
            get { return cachedSlider.value; }
            set { cachedSlider.value = value; }
        }

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = Mathf.Lerp(from, to, factor);
        }

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