using UnityEngine;
using UnityEngine.UI;

namespace Gankx.UI
{
    [AddComponentMenu("UI/Tween/Tween ScrollRect")]
    public class TweenScrollRect : Tweener
    {
#if UNITY_3_5
	public float fromX = 1f;
	public float toX = 1f;
    public float fromY = 1f;
	public float toY = 1f;
#else
        [Range(0f, 1f)]
        public float fromX = 1f;

        [Range(0f, 1f)]
        public float toX = 1f;

        [Range(0f, 1f)]
        public float fromY = 1f;

        [Range(0f, 1f)]
        public float toY = 1f;
#endif

        ScrollRect mScrollRect;

        public ScrollRect cachedScrollRect
        {
            get
            {
                if (mScrollRect == null)
                {
                    mScrollRect = GetComponent<ScrollRect>();
                    if (mScrollRect == null)
                    {
                        Debug.LogError("TweenSlider needs an Slider to work with", this);
                        enabled = false;
                    }
                }
                return mScrollRect;
            }
        }

        public float valueX
        {
            get { return cachedScrollRect.horizontalNormalizedPosition; }
            set { cachedScrollRect.horizontalNormalizedPosition = value; }
        }

        public float valueY
        {
            get { return cachedScrollRect.verticalNormalizedPosition; }
            set { cachedScrollRect.verticalNormalizedPosition = value; }
        }

        protected override void OnUpdate(float factor, bool isFinished)
        {
            valueX = Mathf.Lerp(fromX, toX, factor);
            valueY = Mathf.Lerp(fromY, toY, factor);
            cachedScrollRect.onValueChanged.Invoke(new Vector2(valueX, valueY));
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
            fromX = valueX;
            fromY = valueY;
        }

        public override void SetEndToCurrentValue()
        {
            toX = valueX;
            toY = valueY;
        }
    }
}