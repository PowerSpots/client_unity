using UnityEngine;
using UnityEngine.UI;

namespace Gankx.UI
{
    public class InputTextChangeDispatcher : EventDispatcher
    {
        private InputField _inputField;
        protected override void OnInit()
        {
            _inputField = GetComponent<InputField>();
            if (null == _inputField)
            {
                Debug.LogError("No InputField found!!!");
                return;
            }

            _inputField.onValueChanged.AddListener(OnInputTextValueChanged);
        }

        private void OnInputTextValueChanged(string value)
        {
            SendPanelMessage("OnInputTextValueChanged", value);
        }
    }
}
