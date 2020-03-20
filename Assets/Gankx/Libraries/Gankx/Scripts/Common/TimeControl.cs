using UnityEngine;

namespace Gankx
{
    public sealed class TimeControl
    {
        public enum Layer
        {
            Sequence = 0,

            Script,

            GM,

            Count
        }

        private static readonly int MyLayerCount = (int) Layer.Count;

        private static readonly float[] MyTimeScale =
        {
            1f, 1f, 1f
        };

        private static void ApplyTimeScale()
        {
            var timeScale = 1f;

            for (var i = 0; i < MyLayerCount; ++i)
            {
                timeScale *= MyTimeScale[i];
            }

            // unity "breaks" if it is outside this range
            // TODO TimeScaleSetting
//            TimeScaleSetting.TimeScale = Mathf.Clamp(timeScale, 0f, 100f);
        }

        public static void SetScale(Layer layer, float timeScale)
        {
            if (layer < 0 || layer >= Layer.Count)
            {
                return;
            }

            MyTimeScale[(int) layer] = timeScale;
            ApplyTimeScale();

            AudioListener.pause = timeScale.Equals(0f);
        }

        public static float GetScale(Layer layer)
        {
            if (layer < 0 || layer >= Layer.Count)
            {
                return 0f;
            }

            return MyTimeScale[(int) layer];
        }

        public static void ResetScale()
        {
            for (var i = 0; i < MyLayerCount; ++i)
            {
                MyTimeScale[i] = 1f;
            }
        }
    }
}