using UnityEngine;
using UnityEngine.UI;

namespace Gankx.UI
{
    public class ToggleChangeDispatcher : EventDispatcher
    {
        private Toggle toggle;
        protected override void OnInit()
        {
            toggle = GetComponent<Toggle>();
            if (null == toggle)
            {
                Debug.LogWarning("No Toggle found!!!");
                return;
            }

            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnToggleValueChanged(bool value)
        {
            SendPanelMessage("OnToggleValueChanged", value);
        }
    }
}
