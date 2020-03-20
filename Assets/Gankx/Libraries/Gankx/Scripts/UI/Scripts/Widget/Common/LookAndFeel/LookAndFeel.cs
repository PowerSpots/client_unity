using System;
using UnityEngine;

namespace Gankx.UI
{
    /// <summary>
    /// 可以通过Animator进行多种样式的配置
    /// </summary>
    public class LookAndFeel : MonoBehaviour
    {
        public const string LookAndFeelLayerName = "LookAndFeel";

        /// <summary>
        /// 当前应用的样式
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private int _current = -1;

        public int Current
        {
            get { return _current; }
            set
            {
                if (_current == value)
                {
                    return;
                }

                _current = value;

                ApplyCurrent();
            }
        }

        /// <summary>
        /// 应用当前选中的样式
        /// </summary>
        private void ApplyCurrent()
        {
            // 当前尚未激活无法应用样式
            if (!isActiveAndEnabled)
            {
                return;
            }

            Animator animator = GetComponent<Animator>();
            if (null == animator)
            {
                Debug.LogWarning(
                    string.Format("LookAndFeel.ApplyLookAndFeel on [{0}] occurred error: cannot find Animator at control object!",
                        gameObject.name), gameObject);
                return;
            }

            int layerIndex = animator.GetLayerIndex(LookAndFeelLayerName);
            if (layerIndex < 0)
            {
                Debug.LogWarning(
                    string.Format("LookAndFeel.ApplyLookAndFeel on [{0}] occurred error: cannot find \'LookAndFeel\' layer at Animator of control object!",
                        gameObject.name), gameObject);
                return;
            }

            if (!animator.HasState(layerIndex, Current))
            {
                return;
            }

            animator.enabled = true;
            animator.Play(Current, layerIndex, 1f);

#if UNITY_EDITOR
            // 在编辑状态下可以直接预览LookAndFeel
            if (!Application.isPlaying)
            {
                try
                {
                    if (animator.recorderMode == AnimatorRecorderMode.Playback)
                    {
                        animator.StopPlayback();
                    }

                    animator.StartRecording(-1);

                    bool success = animator.recorderMode == AnimatorRecorderMode.Record;
                    if (success)
                    {
                        animator.Update(0f);
                        animator.StopRecording();

                        animator.StartPlayback();
                        animator.playbackTime = 0f;
                        animator.Update(0f);
                    }
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
#endif
        }

        /// <summary>
        /// 激活时需要重新应用下样式
        /// </summary>
        private void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            ApplyCurrent();
        }
    }
}
