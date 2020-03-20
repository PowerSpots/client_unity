using UnityEngine;
using UnityEngine.UI;

namespace Gankx.UI
{
    public class ButtonClickDispatcher :  EventDispatcher
    {
        private Button button;

        protected override void OnInit()
        {
            button = GetComponent<Button>();
            if (null == button)
            {
                button = AddComponent<Button>();
            }

            if (null == button)
            {
                Debug.LogError(gameObject.name + " Button com conflict!!!!!!");
                return;
            }

            button.onClick.AddListener(OnButtonClick);            
        }

        public void OnButtonClick()
        {
            SendPanelMessage("OnClick");
        }        
    }
}

