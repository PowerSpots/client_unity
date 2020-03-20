using UnityEngine;
using UnityEngine.UI;

namespace Gankx.UI
{
    public class SliderChangeDispatcher : EventDispatcher
    {
        private Slider slider;

        private bool mIsEnableEvent = true ;
        public bool EventEnable
        {
            get { return mIsEnableEvent; }
            set { mIsEnableEvent = value; }
        }
        
        protected override void OnInit()
        {
            slider = GetComponent<Slider>();
            if (null == slider)
            {
                Debug.LogError("No Slider found!!!");
                return;
            }

            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        private void OnSliderValueChanged(float value)
        {
            if (mIsEnableEvent)
            {
                SendPanelMessage("OnSliderValueChanged", value);
            }
        }
    }
}