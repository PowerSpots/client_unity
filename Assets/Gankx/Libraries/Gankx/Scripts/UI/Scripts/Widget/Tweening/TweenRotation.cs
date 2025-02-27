﻿using UnityEngine;

namespace Gankx.UI
{
    /// <summary>
    /// Tween the object's rotation.
    /// </summary>
    [AddComponentMenu("UI/Tween/Tween Rotation")]
    public class TweenRotation : Tweener
    {
        public Vector3 from;
        public Vector3 to;
        public bool quaternionLerp = false;

        Transform mTrans;

        public Transform cachedTransform
        {
            get
            {
                if (mTrans == null) mTrans = transform;
                return mTrans;
            }
        }

        /// <summary>
        /// Tween's current value.
        /// </summary>
        public Quaternion value
        {
            get { return cachedTransform.localRotation; }
            set { cachedTransform.localRotation = value; }
        }

        /// <summary>
        /// Tween the value.
        /// </summary>
        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = quaternionLerp
                ? Quaternion.Slerp(Quaternion.Euler(from), Quaternion.Euler(to), factor)
                : Quaternion.Euler(new Vector3(
                    Mathf.Lerp(from.x, to.x, factor),
                    Mathf.Lerp(from.y, to.y, factor),
                    Mathf.Lerp(from.z, to.z, factor)));
        }

        /// <summary>
        /// Start the tweening operation.
        /// </summary>
        public static TweenRotation Begin(GameObject go, float duration, Quaternion rot)
        {
            TweenRotation comp = Tweener.Begin<TweenRotation>(go, duration);
            comp.from = comp.value.eulerAngles;
            comp.to = rot.eulerAngles;

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
            from = value.eulerAngles;
        }

        [ContextMenu("Set 'To' to current value")]
        public override void SetEndToCurrentValue()
        {
            to = value.eulerAngles;
        }

        [ContextMenu("Assume value of 'From'")]
        void SetCurrentValueToStart()
        {
            value = Quaternion.Euler(from);
        }

        [ContextMenu("Assume value of 'To'")]
        void SetCurrentValueToEnd()
        {
            value = Quaternion.Euler(to);
        }
    }
}