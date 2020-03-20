using UnityEngine;

namespace Gankx.UI
{
    /// <summary>
    /// Tween the audio source's volume.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("UI/Tween/Tween Volume")]
    public class TweenVolume : Tweener
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

        AudioSource mSource;

        /// <summary>
        /// Cached version of 'audio', as it's always faster to cache.
        /// </summary>
        public AudioSource audioSource
        {
            get
            {
                if (mSource == null)
                {
                    mSource = GetComponent<AudioSource>();

                    if (mSource == null)
                    {
                        mSource = GetComponent<AudioSource>();

                        if (mSource == null)
                        {
                            Debug.LogError("TweenVolume needs an AudioSource to work with", this);
                            enabled = false;
                        }
                    }
                }
                return mSource;
            }
        }

        /// <summary>
        /// Audio source's current volume.
        /// </summary>
        public float value
        {
            get { return audioSource != null ? mSource.volume : 0f; }
            set
            {
                if (audioSource != null) mSource.volume = value;
            }
        }

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = from * (1f - factor) + to * factor;
            mSource.enabled = (mSource.volume > 0.01f);
        }

        /// <summary>
        /// Start the tweening operation.
        /// </summary>
        public static TweenVolume Begin(GameObject go, float duration, float targetVolume)
        {
            TweenVolume comp = Tweener.Begin<TweenVolume>(go, duration);
            comp.from = comp.value;
            comp.to = targetVolume;
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