﻿using UnityEngine;

namespace Gankx.UI
{
    /// <summary>
    /// Tween the object's local scale.
    /// </summary>
    [AddComponentMenu("UI/Tween/Tween Scale")]
    public class TweenScale : Tweener
    {
        public Vector3 from = Vector3.one;
        public Vector3 to = Vector3.one;
        public bool updateTable = false;

        Transform mTrans;

        public Transform cachedTransform
        {
            get
            {
                if (mTrans == null) mTrans = transform;
                return mTrans;
            }
        }

        public Vector3 value
        {
            get { return cachedTransform.localScale; }
            set { cachedTransform.localScale = value; }
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
        public static TweenScale Begin(GameObject go, float duration, Vector3 scale)
        {
            TweenScale comp = Tweener.Begin<TweenScale>(go, duration);
            comp.from = comp.value;
            comp.to = scale;

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