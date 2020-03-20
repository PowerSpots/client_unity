using UnityEngine;
using UnityEngine.UI;

namespace Gankx.UI
{
    /// <summary>
    /// 该类用于对UGUI以及第三方UI组件提供的delegate做事件转发
    /// 该类send到lua层的事件命名规范: On + ComponentName + DelegateName
    /// </summary>
    public class ScrollbarChangeDispatcher : EventDispatcher
    {
        private Scrollbar scrollBar;
        protected override void OnInit()
        {            
            scrollBar = GetComponent<Scrollbar>();
            if (null == scrollBar)
            {
                Debug.LogError("No Scollbar found!!!");
                return;
            }

            scrollBar.onValueChanged.AddListener(OnScrollbarChanged);
        }

        /// <summary>
        /// Scrollbar的OnScrollbarChanged事件注册
        /// </summary>
        /// <param name="value"></param>
        public void OnScrollbarChanged(float value)
        {
            SendPanelMessage("OnScrollbarValueChanged", value);
        }
    }
}
