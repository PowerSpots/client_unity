using System.Collections.Generic;
using UnityEngine;

namespace Flux
{
    [FEvent("Camera/Disable Main Camera")]
    public class FDisableMainCameraEvent : FEvent
    {
        private Camera _camera = null;

        [SerializeField]
        private bool _restoreOnFinish = false;

        protected override void OnTrigger(float timeSinceTrigger)
        {
            _camera = Camera.main;
            if (null == _camera)
            {
                return;
            }

            _camera.gameObject.SetActive(false);
        }

        private void Restore()
        {
            if (null == _camera)
            {
                return;
            }

            _camera.gameObject.SetActive(true);
        }

        protected override void OnFinish()
        {
            if (!_restoreOnFinish)
            {
                return;
            }

            Restore();
        }

        protected override void OnStop()
        {
            Restore();
        }
    }
}
