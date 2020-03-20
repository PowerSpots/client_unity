using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Gankx.UI
{
    /// <summary>
    /// This script is able to fill in the label's text gradually, giving the effect of someone typing or fading in the content over time.
    /// </summary>
    [RequireComponent(typeof(Text))]
    [AddComponentMenu("UI/Interaction/Typewriter Effect")]
    public class TypewriterEffect : MonoBehaviour
    {
        public static TypewriterEffect current;

        struct FadeEntry
        {
            public int index;
            public string text;
            public float alpha;
        }
        private int _colorIndex = 0; // 记录带颜色文本的index
        private string _colorText = string.Empty; // 记录带颜色的文本
        private bool _beginUseColor = false; // 标记带颜色文本的开始
        private bool _addColor = false; // 标记当前文本是否应加上颜色

        private void ResetColorTextParams()
        {
            _colorIndex = 0;
            _colorText = string.Empty;
            _beginUseColor = false;
            _addColor = false;
        }

        /// <summary>
        /// How many characters will be printed per second.
        /// </summary>
        public int charsPerSecond = 20;

        /// <summary>
        /// How long it takes for each character to fade in.
        /// </summary>
        public float fadeInTime = 0f;

        /// <summary>
        /// How long to pause when a period is encountered (in seconds).
        /// </summary>
        public float delayOnPeriod = 0f;

        /// <summary>
        /// How long to pause when a new line character is encountered (in seconds).
        /// </summary>
        public float delayOnNewLine = 0f;

#if COMPATIBILITY_NGUI
    /// <summary>
    /// If a scroll view is specified, its UpdatePosition() function will be called every time the text is updated.
    /// </summary>

    public UIScrollView scrollView;
#endif

        /// <summary>
        /// If set to 'true', the text's dimensions will be that of a fully faded-in content.
        /// </summary>
        public bool keepFullDimensions = false;

        /// <summary>
        /// Event delegate triggered when the typewriter effect finishes.
        /// </summary>
        public List<EventDelegate> onFinished = new List<EventDelegate>();

        Text mText;
        string mFullText = "";
        int mCurrentOffset = 0;
        float mNextChar = 0f;
        bool mReset = true;
        bool mActive = false;

        BetterList<FadeEntry> mFade = new BetterList<FadeEntry>();

        /// <summary>
        /// Whether the typewriter effect is currently active or not.
        /// </summary>
        public bool isActive
        {
            get { return mActive; }
        }

        /// <summary>
        /// Reset the typewriter effect to the beginning of the label.
        /// </summary>
        [ContextMenu("ResetToBeginning")]
        public void ResetToBeginning()
        {
            Finish();
            mReset = true;
            mActive = true;
            mNextChar = 0f;
            mCurrentOffset = 0;
            mText = GetComponent<Text>();
            mText.enabled = false;
            ResetColorTextParams();

            //            Update();
        }

        /// <summary>
        /// Finish the typewriter operation and show all the text right away.
        /// </summary>
        public void Finish()
        {
            if (mActive)
            {
                mActive = false;

                if (!mReset)
                {
                    mCurrentOffset = mFullText.Length;
                    mFade.Clear();
                    mText.text = mFullText;
                }

#if COMPATIBILITY_NGUI
			if (keepFullDimensions && scrollView != null)
				scrollView.UpdatePosition();
#endif

                current = this;
                EventDelegate.Execute(onFinished);
                current = null;
            }

            ResetColorTextParams();
        }

        void OnEnable()
        {
//            mReset = true;
//            mActive = true;
            ResetToBeginning();
        }


        
        void Update()
        {
            if (!mActive) return;

            if (mReset)
            {
                mCurrentOffset = 0;
                mReset = false;
                mText = GetComponent<Text>();
                mFullText = mText.text;
                mText.enabled = true;
                mFade.Clear();

#if COMPATIBILITY_NGUI
			if (keepFullDimensions && scrollView != null) scrollView.UpdatePosition();
#endif
            }

            while (mCurrentOffset < mFullText.Length && mNextChar <= Time.time)
            {
                int lastOffset = mCurrentOffset;
                charsPerSecond = Mathf.Max(1, charsPerSecond);

                // Automatically skip all symbols
#if COMPATIBILITY_NGUI
            while (NGUIText.ParseSymbol(mFullText, ref mCurrentOffset)) { }
#endif
                ++mCurrentOffset;

                // Reached the end? We're done.
                if (mCurrentOffset > mFullText.Length) break;

                // Periods and end-of-line characters should pause for a longer time.
                float delay = 1f / charsPerSecond;
                char c = (lastOffset < mFullText.Length) ? mFullText[lastOffset] : '\n';

                if (c == '\n')
                {
                    delay += delayOnNewLine;
                }
                else if (_beginUseColor)
                {
                    if (c == '>')
                    {
                        _addColor = true;
                    }
                    if (c == '<')
                    {
                        _addColor = false;
                    }
                    _colorText = _colorText + c;
                    if (_colorText.Contains("</color>"))
                    {
                        ResetColorTextParams();
                    }
                }
                else if (c == '<' && _beginUseColor == false)
                {
                    _beginUseColor = true;
                    _colorText = _colorText + c;
                    _colorIndex = lastOffset;
                }
                
                else if (lastOffset + 1 == mFullText.Length || mFullText[lastOffset + 1] <= ' ')
                {
                    if (c == '.')
                    {
                        if (lastOffset + 2 < mFullText.Length && mFullText[lastOffset + 1] == '.' &&
                            mFullText[lastOffset + 2] == '.')
                        {
                            delay += delayOnPeriod*3f;
                            lastOffset += 2;
                        }
                        else
                        {
                            delay += delayOnPeriod;
                        }
                    }
                    else if (c == '!' || c == '?')
                    {
                        delay += delayOnPeriod;
                    }
                }

                if (mNextChar == 0f)
                {
                    mNextChar = Time.time + delay;
                }
                else if (_beginUseColor && !_addColor)
                {
                    
                }
                else
                {
                    mNextChar += delay;
                }

                if (fadeInTime != 0f)
                {
                    // There is smooth fading involved
                    FadeEntry fe = new FadeEntry();
                    fe.index = lastOffset;
                    fe.alpha = 0f;
                    if (_beginUseColor)
                    {
                        fe.text = _addColor ? mFullText.Substring(lastOffset, mCurrentOffset - lastOffset) + "</color>"
                            : mFullText.Substring(_colorIndex, mCurrentOffset - _colorIndex);
                    }
                    else
                    {
                        fe.text = mFullText.Substring(lastOffset, mCurrentOffset - lastOffset);
                    }
                    
                    mFade.Add(fe);
                }
                else
                {
                    // No smooth fading necessary
                    if (_beginUseColor)
                    {
                        mText.text = _addColor ? mFullText.Substring(0, mCurrentOffset) + "</color>" 
                            : mFullText.Substring(0, _colorIndex);
                    }
                    else
                    {
                        mText.text = keepFullDimensions
                        ? mFullText.Substring(0, mCurrentOffset) + "[00]" + mFullText.Substring(mCurrentOffset)
                        : mFullText.Substring(0, mCurrentOffset);
                    }
#if COMPATIBILITY_NGUI
    // If a scroll view was specified, update its position
				if (!keepFullDimensions && scrollView != null) scrollView.UpdatePosition();
#endif
                }
            }

            // Alpha-based fading
            if (mFade.size != 0)
            {
                for (int i = 0; i < mFade.size;)
                {
                    FadeEntry fe = mFade[i];
                    fe.alpha += RealTime.deltaTime / fadeInTime;

                    if (fe.alpha < 1f)
                    {
                        mFade[i] = fe;
                        ++i;
                    }
                    else mFade.RemoveAt(i);
                }

                if (mFade.size == 0)
                {
                    if (keepFullDimensions)
                        mText.text = mFullText.Substring(0, mCurrentOffset) + "[00]" +
                                     mFullText.Substring(mCurrentOffset);
                    else mText.text = mFullText.Substring(0, mCurrentOffset);
                }
                else
                {
                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < mFade.size; ++i)
                    {
                        FadeEntry fe = mFade[i];

                        if (i == 0)
                        {
                            sb.Append(mFullText.Substring(0, fe.index));
                        }

#if COMPATIBILITY_NGUI
					sb.Append('[');
					sb.Append(NGUIText.EncodeAlpha(fe.alpha));
					sb.Append(']');
#endif
                        sb.Append(fe.text);
                    }

                    if (keepFullDimensions)
                    {
                        sb.Append("[00]");
                        sb.Append(mFullText.Substring(mCurrentOffset));
                    }

                    mText.text = sb.ToString();
                }
            }
            else if (mCurrentOffset == mFullText.Length)
            {
                current = this;
                EventDelegate.Execute(onFinished);
                current = null;
                mActive = false;
            }
        }
    }
}