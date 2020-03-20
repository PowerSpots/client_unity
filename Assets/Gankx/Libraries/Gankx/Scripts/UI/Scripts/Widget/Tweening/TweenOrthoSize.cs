using UnityEngine;

namespace Gankx.UI
{
    /// <summary>
    /// Tween the camera's orthographic size.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("UI/Tween/Tween Orthographic Size")]
    public class TweenOrthoSize : Tweener
    {
        public float from = 1f;
        public float to = 1f;

        Camera mCam;

        /// <summary>
        /// Camera that's being tweened.
        /// </summary>
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
	public Camera cachedCamera { get { if (mCam == null) mCam = camera; return mCam; } }
#else
        public Camera cachedCamera
        {
            get
            {
                if (mCam == null) mCam = GetComponent<Camera>();
                return mCam;
            }
        }
#endif

        /// <summary>
        /// Tween's current value.
        /// </summary>
        public float value
        {
            get { return cachedCamera.orthographicSize; }
            set { cachedCamera.orthographicSize = value; }
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
        public static TweenOrthoSize Begin(GameObject go, float duration, float to)
        {
            TweenOrthoSize comp = Tweener.Begin<TweenOrthoSize>(go, duration);
            comp.from = comp.value;
            comp.to = to;

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