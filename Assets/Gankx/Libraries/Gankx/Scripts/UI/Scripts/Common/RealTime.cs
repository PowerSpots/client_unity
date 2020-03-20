using UnityEngine;

namespace Gankx.UI
{
    public static class RealTime
    {
        public static float time
        {
            get { return Time.unscaledTime; }
        }

        public static float deltaTime
        {
            get { return Time.unscaledDeltaTime; }
        }
    }
}