using Gankx.UI.AnimationOrTween;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Gankx.UI
{
    [ExecuteInEditMode]
    [AddComponentMenu("UI/Interaction/Play Tween")]
    public class PlayTween : MonoBehaviour, IPointerDownHandler
    {
        /// <summary>
        ///   <para>Finished Event type used by the PlayTween.</para>
        /// </summary>
        [Serializable]
        public class PlayFinishedEvent : UnityEvent
        {
        }

        public static PlayTween current;

        /// <summary>
        /// Target on which there is one or more tween.
        /// </summary>
        public GameObject tweenTarget;

        /// <summary>
        /// If there are multiple tweens, you can choose which ones get activated by changing their group.
        /// </summary>
        public int tweenGroup = 0;

        /// <summary>
        /// Which event will trigger the tween.
        /// </summary>
        public Trigger trigger = Trigger.OnClick;

        /// <summary>
        /// Direction to tween in.
        /// </summary>
        public Direction playDirection = Direction.Forward;

        /// <summary>
        /// Whether the tween will be reset to the start or end when activated. If not, it will continue from where it currently is.
        /// </summary>
        public bool resetOnPlay = false;

        /// <summary>
        /// Whether the tween will be reset to the start if it's disabled when activated.
        /// </summary>
        public bool resetIfDisabled = false;

        /// <summary>
        /// What to do if the tweenTarget game object is currently disabled.
        /// </summary>
        public EnableCondition ifDisabledOnPlay = EnableCondition.DoNothing;

        /// <summary>
        /// What to do with the tweenTarget after the tween finishes.
        /// </summary>
        public DisableCondition disableWhenFinished = DisableCondition.DoNotDisable;

        /// <summary>
        /// Whether the tweens on the child game objects will be considered.
        /// </summary>
        public bool includeChildren = false;

        /// <summary>
        /// Event delegates called when the animation finishes.
        /// </summary>
        public PlayFinishedEvent onFinished = new PlayFinishedEvent();

        protected Tweener[] mTweens;
        protected bool mStarted = false;
        protected int mActive = 0;
        protected bool mActivated = false;

//        void Awake()
//        {
//        }

        protected void Start()
        {
            mStarted = true;

            if (tweenTarget == null)
            {
                tweenTarget = gameObject;
#if UNITY_EDITOR
                UITools.SetDirty(this);
#endif
            }
        }

        //UGui
        protected void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (trigger == Trigger.OnActivate  || trigger == Trigger.OnActivateTrue)
            {
                Play(true);
            }
        }

#if COMPATIBILITY_NGUI

    void OnEnable ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		if (mStarted) OnHover(UICamera.IsHighlighted(gameObject));

		if (UICamera.currentTouch != null)
		{
			if (trigger == Trigger.OnPress || trigger == Trigger.OnPressTrue)
				mActivated = (UICamera.currentTouch.pressed == gameObject);

			if (trigger == Trigger.OnHover || trigger == Trigger.OnHoverTrue)
				mActivated = (UICamera.currentTouch.current == gameObject);
		}

		UIToggle toggle = GetComponent<UIToggle>();
		if (toggle != null) EventDelegate.Add(toggle.onChange, OnToggle);
	}

	void OnDisable ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		UIToggle toggle = GetComponent<UIToggle>();
		if (toggle != null) EventDelegate.Remove(toggle.onChange, OnToggle);
	}
        
	void OnDragOver () { if (trigger == Trigger.OnHover) OnHover(true); }

	void OnHover (bool isOver)
	{
		if (enabled)
		{
			if (trigger == Trigger.OnHover ||
				(trigger == Trigger.OnHoverTrue && isOver) ||
				(trigger == Trigger.OnHoverFalse && !isOver))
			{
				mActivated = isOver && (trigger == Trigger.OnHover);
				Play(isOver);
			}
		}
	}

	void OnDragOut ()
	{
		if (enabled && mActivated)
		{
			mActivated = false;
			Play(false);
		}
	}

	void OnPress (bool isPressed)
	{
		if (enabled)
		{
			if (trigger == Trigger.OnPress ||
				(trigger == Trigger.OnPressTrue && isPressed) ||
				(trigger == Trigger.OnPressFalse && !isPressed))
			{
				mActivated = isPressed && (trigger == Trigger.OnPress);
				Play(isPressed);
			}
		}
	}

	void OnClick ()
	{
		if (enabled && trigger == Trigger.OnClick)
		{
			Play(true);
		}
	}

	void OnDoubleClick ()
	{
		if (enabled && trigger == Trigger.OnDoubleClick)
		{
			Play(true);
		}
	}

	void OnSelect (bool isSelected)
	{
		if (enabled)
		{
			if (trigger == Trigger.OnSelect ||
				(trigger == Trigger.OnSelectTrue && isSelected) ||
				(trigger == Trigger.OnSelectFalse && !isSelected))
			{
				mActivated = isSelected && (trigger == Trigger.OnSelect);
				Play(isSelected);
			}
		}
	}

	void OnToggle ()
	{
		if (!enabled || UIToggle.current == null) return;
		if (trigger == Trigger.OnActivate ||
			(trigger == Trigger.OnActivateTrue && UIToggle.current.value) ||
			(trigger == Trigger.OnActivateFalse && !UIToggle.current.value))
			Play(UIToggle.current.value);
	}

#endif

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (enabled && trigger == Trigger.OnClick)
            {
                Play(true);
            }
        }

        protected void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (disableWhenFinished != DisableCondition.DoNotDisable && mTweens != null)
            {
                bool isFinished = true;
                bool properDirection = true;

                for (int i = 0, imax = mTweens.Length; i < imax; ++i)
                {
                    Tweener tw = mTweens[i];
                    if (tw.tweenGroup != tweenGroup) continue;

                    if (tw.enabled)
                    {
                        isFinished = false;
                        break;
                    }
                    else if ((int) tw.direction != (int) disableWhenFinished)
                    {
                        properDirection = false;
                    }
                }

                if (isFinished)
                {
                    if (properDirection) UITools.SetActive(tweenTarget, false);
                    mTweens = null;
                }
            }
        }

        /// <summary>
        /// Activate the tweeners.
        /// </summary>
        public void Play(bool forward)
        {
            mActive = 0;
            GameObject go = (tweenTarget == null) ? gameObject : tweenTarget;

            if (!UITools.GetActive(go))
            {
                // If the object is disabled, don't do anything
                if (ifDisabledOnPlay != EnableCondition.EnableThenPlay) return;

                // Enable the game object before tweening it
                UITools.SetActive(go, true);
            }

            // Gather the tweening components
            mTweens = includeChildren ? go.GetComponentsInChildren<Tweener>() : go.GetComponents<Tweener>();

            if (mTweens.Length == 0)
            {
                // No tweeners found -- should we disable the object?
                if (disableWhenFinished != DisableCondition.DoNotDisable)
                    UITools.SetActive(tweenTarget, false);
            }
            else
            {
                bool activated = false;
                if (playDirection == Direction.Reverse) forward = !forward;

                // Run through all located tween components
                for (int i = 0, imax = mTweens.Length; i < imax; ++i)
                {
                    Tweener tw = mTweens[i];

                    // If the tweener's group matches, we can work with it
                    if (tw.tweenGroup == tweenGroup)
                    {
                        // Ensure that the game objects are enabled
                        if (!activated && !UITools.GetActive(go))
                        {
                            activated = true;
                            UITools.SetActive(go, true);
                        }

                        ++mActive;

                        // Toggle or activate the tween component
                        if (playDirection == Direction.Toggle)
                        {
                            // Listen for tween finished messages, just one shot
                            tw.onFinished.AddListener(OnFinished);

                            tw.Toggle();
                        }
                        else
                        {
                            if (resetOnPlay || (resetIfDisabled && !tw.enabled))
                            {
                                tw.Play(forward);
                                tw.ResetToBeginning();
                            }

                            // Listen for tween finished messages, just one shot
                            tw.onFinished.AddListener(OnFinished);

                            tw.Play(forward);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Callback triggered when each tween executed by this script finishes.
        /// </summary>
        protected void OnFinished()
        {
            // Just one shot
            Tweener.current.onFinished.RemoveListener(OnFinished);

            if (--mActive == 0 && current == null)
            {
                current = this;

                onFinished.Invoke();

                current = null;
            }
        }
    }
}