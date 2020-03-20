using UnityEngine;
using UnityEngine.UI;

namespace Gankx.UI
{
    /// <summary>
    /// 该类用于对UGUI以及第三方UI组件提供的delegate做事件转发
    /// 该类send到lua层的事件命名规范: On + ComponentName + DelegateName
    /// </summary>
    public class ScrollRectChangeDispatcher : EventDispatcher
    {
        private ScrollRect scrollRect;
        protected override void OnInit()
        {
            scrollRect = GetComponent<ScrollRect>();
            if (null == scrollRect)
            {
                Debug.LogError("No Scollbar found!!!");
                return;
            }

            scrollRect.onValueChanged.AddListener(OnScrollrectChanged);
        }

        /// <summary>
        /// Scrollbar的OnScrollbarChanged事件注册
        /// </summary>
        /// <param name="value"></param>
        public void OnScrollrectChanged(Vector2 call)
        {
            SendPanelMessage("OnScrollrectValueChanged", call.x, call.y);
        }
    }
}
