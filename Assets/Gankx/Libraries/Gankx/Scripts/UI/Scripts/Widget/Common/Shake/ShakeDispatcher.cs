using UnityEngine;

namespace Gankx.UI
{
    public class ShakeDispatcher : EventDispatcher
    {
        protected override void OnInit()
        {
            ShakeDetector shakeDetector = GetComponent<ShakeDetector>();
            if (null == shakeDetector)
            {
                shakeDetector = AddComponent<ShakeDetector>();
            }

            shakeDetector.onShake.AddListener(OnShake);
        }

        public void OnShake()
        {
            SendPanelMessage("OnShake");
        }
    }
}