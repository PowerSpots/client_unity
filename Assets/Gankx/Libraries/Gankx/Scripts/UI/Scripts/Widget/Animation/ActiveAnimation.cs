using System;
using System.Collections.Generic;
using Gankx.UI.AnimationOrTween;
using UnityEngine;

namespace Gankx.UI
{
    [AddComponentMenu("UI/Animation/Active Animation")]
    public class ActiveAnimation : MonoBehaviour
    {
        public static ActiveAnimation current;

        public List<EventDelegate> onFinished = new List<EventDelegate>();

        [HideInInspector]
        public GameObject eventReceiver;

        [HideInInspector]
        public string callWhenFinished;

        private Animation myAnimation;
        private Direction myLastDirection = Direction.Toggle;
        private Direction myDisableDirection = Direction.Toggle;
        private bool myNotify;

        private Animator myAnimator;
        private string myAnimationClip = "";

        private float playbackTime
        {
            get
            {
                var state = myAnimator.GetCurrentAnimatorStateInfo(0);
                return Mathf.Clamp01(state.normalizedTime);
            }
        }

        public bool isPlaying
        {
            get
            {
                if (myAnimation == null)
                {
                    if (myAnimator != null)
                    {
                        if (myLastDirection == Direction.Reverse)
                        {
                            if (Math.Abs(playbackTime) < float.Epsilon)
                            {
                                return false;
                            }
                        }
                        else if (Math.Abs(playbackTime - 1f) < float.Epsilon)
                        {
                            return false;
                        }

                        return true;
                    }

                    return false;
                }

                foreach (AnimationState state in myAnimation)
                {
                    if (!myAnimation.IsPlaying(state.name))
                    {
                        continue;
                    }

                    if (myLastDirection == Direction.Forward)
                    {
                        if (state.time < state.length)
                        {
                            return true;
                        }
                    }
                    else if (myLastDirection == Direction.Reverse)
                    {
                        if (state.time > 0f)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public void Finish()
        {
            if (myAnimation != null)
            {
                foreach (AnimationState state in myAnimation)
                {
                    if (myLastDirection == Direction.Forward)
                    {
                        state.time = state.length;
                    }
                    else if (myLastDirection == Direction.Reverse)
                    {
                        state.time = 0f;
                    }
                }

                myAnimation.Sample();
            }
            else if (myAnimator != null)
            {
                myAnimator.Play(myAnimationClip, 0, myLastDirection == Direction.Forward ? 1f : 0f);
            }
        }

        public void Reset()
        {
            if (myAnimation != null)
            {
                foreach (AnimationState state in myAnimation)
                {
                    if (myLastDirection == Direction.Reverse)
                    {
                        state.time = state.length;
                    }
                    else if (myLastDirection == Direction.Forward)
                    {
                        state.time = 0f;
                    }
                }
            }
            else if (myAnimator != null)
            {
                myAnimator.Play(myAnimationClip, 0, myLastDirection == Direction.Reverse ? 1f : 0f);
            }
        }

        private void Start()
        {
            if (eventReceiver != null && EventDelegate.IsValid(onFinished))
            {
                eventReceiver = null;
                callWhenFinished = null;
            }
        }

        private void Update()
        {
            var delta = RealTime.deltaTime;
            if (Math.Abs(delta) < float.Epsilon)
            {
                return;
            }

            if (myAnimator != null)
            {
                myAnimator.Update(myLastDirection == Direction.Reverse ? -delta : delta);
                if (isPlaying)
                {
                    return;
                }

                myAnimator.enabled = false;
                enabled = false;
            }
            else if (myAnimation != null)
            {
                var playing = false;

                foreach (AnimationState state in myAnimation)
                {
                    if (!myAnimation.IsPlaying(state.name))
                    {
                        continue;
                    }

                    var movement = state.speed * delta;
                    state.time += movement;

                    if (movement < 0f)
                    {
                        if (state.time > 0f)
                        {
                            playing = true;
                        }
                        else
                        {
                            state.time = 0f;
                        }
                    }
                    else
                    {
                        if (state.time < state.length)
                        {
                            playing = true;
                        }
                        else
                        {
                            state.time = state.length;
                        }
                    }
                }

                myAnimation.Sample();
                if (playing)
                {
                    return;
                }

                enabled = false;
            }
            else
            {
                enabled = false;
                return;
            }

            if (myNotify)
            {
                myNotify = false;

                if (current == null)
                {
                    current = this;
                    EventDelegate.Execute(onFinished);

                    if (eventReceiver != null && !string.IsNullOrEmpty(callWhenFinished))
                    {
                        eventReceiver.SendMessage(callWhenFinished, SendMessageOptions.DontRequireReceiver);
                    }

                    current = null;
                }

                if (myDisableDirection != Direction.Toggle && myLastDirection == myDisableDirection)
                {
                    UITools.SetActive(gameObject, false);
                }
            }
        }

        private void Play(string clipName, Direction playDirection)
        {
            if (playDirection == Direction.Toggle)
            {
                playDirection = myLastDirection != Direction.Forward ? Direction.Forward : Direction.Reverse;
            }

            if (myAnimation != null)
            {
                enabled = true;
                myAnimation.enabled = false;

                var noName = string.IsNullOrEmpty(clipName);

                if (noName)
                {
                    if (!myAnimation.isPlaying)
                    {
                        myAnimation.Play();
                    }
                }
                else if (!myAnimation.IsPlaying(clipName))
                {
                    myAnimation.Play(clipName);
                }

                foreach (AnimationState state in myAnimation)
                {
                    if (string.IsNullOrEmpty(clipName) || state.name == clipName)
                    {
                        var speed = Mathf.Abs(state.speed);
                        state.speed = speed * (int) playDirection;

                        if (playDirection == Direction.Reverse && Math.Abs(state.time) < float.Epsilon)
                        {
                            state.time = state.length;
                        }
                        else if (playDirection == Direction.Forward && Math.Abs(state.time - state.length) < float.Epsilon)
                        {
                            state.time = 0f;
                        }
                    }
                }

                myLastDirection = playDirection;
                myNotify = true;
                myAnimation.Sample();
            }
            else if (myAnimator != null)
            {
                if (enabled && isPlaying)
                {
                    if (myAnimationClip == clipName)
                    {
                        myLastDirection = playDirection;
                        return;
                    }
                }

                enabled = true;
                myNotify = true;
                myLastDirection = playDirection;
                myAnimationClip = clipName;
                myAnimator.Play(myAnimationClip, 0, playDirection == Direction.Forward ? 0f : 1f);
            }
        }

        public static ActiveAnimation Play(Animation anim, string clipName, Direction playDirection,
            EnableCondition enableBeforePlay = EnableCondition.DoNothing, DisableCondition disableCondition = DisableCondition.DoNotDisable)
        {
            if (!UITools.GetActive(anim.gameObject))
            {
                if (enableBeforePlay != EnableCondition.EnableThenPlay)
                {
                    return null;
                }

                UITools.SetActive(anim.gameObject, true);
            }

            var aa = anim.GetComponent<ActiveAnimation>();
            if (aa == null)
            {
                aa = anim.gameObject.AddComponent<ActiveAnimation>();
            }

            aa.myAnimation = anim;
            aa.myDisableDirection = (Direction) (int) disableCondition;
            aa.onFinished.Clear();
            aa.Play(clipName, playDirection);

            if (aa.myAnimation != null)
            {
                aa.myAnimation.Sample();
            }
            else if (aa.myAnimator != null)
            {
                aa.myAnimator.Update(0f);
            }

            return aa;
        }

        public static ActiveAnimation Play(Animation anim, Direction playDirection)
        {
            return Play(anim, null, playDirection);
        }

        public static ActiveAnimation Play(Animator anim, string clipName, Direction playDirection,
            EnableCondition enableBeforePlay, DisableCondition disableCondition)
        {
            if (enableBeforePlay != EnableCondition.IgnoreDisabledState && !UITools.GetActive(anim.gameObject))
            {
                if (enableBeforePlay != EnableCondition.EnableThenPlay)
                {
                    return null;
                }

                UITools.SetActive(anim.gameObject, true);
            }

            var aa = anim.GetComponent<ActiveAnimation>();
            if (aa == null)
            {
                aa = anim.gameObject.AddComponent<ActiveAnimation>();
            }

            aa.myAnimator = anim;
            aa.myDisableDirection = (Direction) (int) disableCondition;
            aa.onFinished.Clear();
            aa.Play(clipName, playDirection);

            if (aa.myAnimation != null)
            {
                aa.myAnimation.Sample();
            }
            else if (aa.myAnimator != null)
            {
                aa.myAnimator.Update(0f);
            }

            return aa;
        }
    }
}