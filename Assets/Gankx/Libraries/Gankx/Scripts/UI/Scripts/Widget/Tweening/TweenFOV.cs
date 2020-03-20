using UnityEngine;

namespace Gankx.UI
{
    /// <summary>
    /// Tween the camera's field of view.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("UI/Tween/Tween Field of View")]
    public class TweenFOV : Tweener
    {
        public float from = 45f;
        public float to = 45f;

        Camera mCam;

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
            get { return cachedCamera.fieldOfView; }
            set { cachedCamera.fieldOfView = value; }
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
        public static TweenFOV Begin(GameObject go, float duration, float to)
        {
            TweenFOV comp = Tweener.Begin<TweenFOV>(go, duration);
            comp.from = comp.value;
            comp.to = to;

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