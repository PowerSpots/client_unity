using UnityEngine;
using UnityEngine.UI;

namespace Gankx.UI
{
    public class InputTextEndEditDispatcher : EventDispatcher
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

            _inputField.onEndEdit.AddListener(OnInputTextEndEdit);
        }

        private void OnInputTextEndEdit(string value)
        {
            SendPanelMessage("OnInputTextEndEdit", value);
        }
    }
}
