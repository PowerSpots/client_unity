﻿using UnityEngine;

namespace Gankx.UI
{
    /// <summary>
    /// Tween the object's position, rotation and scale.
    /// </summary>
    [AddComponentMenu("UI/Tween/Tween Transform")]
    public class TweenTransform : Tweener
    {
        public Transform from;
        public Transform to;
        public bool parentWhenFinished = false;

        Transform mTrans;
        Vector3 mPos;
        Quaternion mRot;
        Vector3 mScale;

        /// <summary>
        /// Interpolate the position, scale, and rotation.
        /// </summary>
        protected override void OnUpdate(float factor, bool isFinished)
        {
            if (to != null)
            {
                if (mTrans == null)
                {
                    mTrans = transform;
                    mPos = mTrans.position;
                    mRot = mTrans.rotation;
                    mScale = mTrans.localScale;
                }

                if (from != null)
                {
                    mTrans.position = from.position * (1f - factor) + to.position * factor;
                    mTrans.localScale = from.localScale * (1f - factor) + to.localScale * factor;
                    mTrans.rotation = Quaternion.Slerp(from.rotation, to.rotation, factor);
                }
                else
                {
                    mTrans.position = mPos * (1f - factor) + to.position * factor;
                    mTrans.localScale = mScale * (1f - factor) + to.localScale * factor;
                    mTrans.rotation = Quaternion.Slerp(mRot, to.rotation, factor);
                }

                // Change the parent when finished, if requested
                if (parentWhenFinished && isFinished) mTrans.parent = to;
            }
        }

        /// <summary>
        /// Start the tweening operation from the current position/rotation/scale to the target transform.
        /// </summary>
        public static TweenTransform Begin(GameObject go, float duration, Transform to)
        {
            return Begin(go, duration, null, to);
        }

        /// <summary>
        /// Start the tweening operation.
        /// </summary>
        public static TweenTransform Begin(GameObject go, float duration, Transform from, Transform to)
        {
            TweenTransform comp = Tweener.Begin<TweenTransform>(go, duration);
            comp.from = from;
            comp.to = to;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }
    }
}