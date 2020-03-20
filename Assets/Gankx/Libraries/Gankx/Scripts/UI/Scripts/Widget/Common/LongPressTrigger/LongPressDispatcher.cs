using UnityEngine.EventSystems;

namespace Gankx.UI
{
    public class LongPressDispatcher : EventDispatcher
    {        
        protected override void OnInit()
        {
            LongPressTrigger trigger = GetComponent<LongPressTrigger>();
            if (null == trigger)
            {
                trigger = AddComponent<LongPressTrigger>();
            }

            trigger.onLongPress.AddListener(OnLongPress);
            trigger.onLongPressStay.AddListener(OnLongPressStay);
            trigger.onLongRelease.AddListener(OnLongRelease);
            trigger.onPointerDown.AddListener(OnPointerDown);

            PointerClickDispatcher pointerClickDispatcher = GetComponent<PointerClickDispatcher>();
            if (null != pointerClickDispatcher)
            {
                pointerClickDispatcher.ShouldBlockClick = ShouldBlockClick;
            }
        }

        private bool shouldBlockClick = false;

        private void OnLongPress()
        {
            SendPanelMessage("OnLongPress");
            shouldBlockClick = true;
        }

        private void OnLongPressStay()
        {
            SendPanelMessage("OnLongPressStay");
        }

        private void OnLongRelease(bool isSame, float pressedTime)
        {
            SendPanelMessage("OnLongRelease", isSame, pressedTime);
        }

        /// <summary>
        /// 每次新的down事件，都刷新block click标记位
        /// </summary>
        private void OnPointerDown()
        {
            shouldBlockClick = false;
        }

        private bool ShouldBlockClick(PointerEventData eventData)
        {
            return shouldBlockClick;
        }
    }
}