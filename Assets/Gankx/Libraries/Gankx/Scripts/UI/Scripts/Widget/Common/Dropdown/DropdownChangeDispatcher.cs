using UnityEngine;
using UnityEngine.UI;

namespace Gankx.UI
{
    public class DropdownChangeDispatcher : EventDispatcher
    {
        private Dropdown dropDown;
        protected override void OnInit()
        {
            dropDown = GetComponent<Dropdown>();
            if (null == dropDown)
            {
                Debug.LogError("No Dropdown found!!!");
                return;
            }
            
            dropDown.onValueChanged.AddListener(OnDropdownValueChanged);
        }        

        /// <summary>
        /// Scrollbar的OnScrollbarChanged事件注册
        /// </summary>
        /// <param name="value"></param>
        public void OnDropdownValueChanged(int value)
        {
            SendPanelMessage("OnDropdownValueChanged", value);
        }
    }
}
