using UnityEngine.EventSystems;

namespace Gankx.UI
{
    public class PointerDownDispatcher : EventDispatcher, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            SendPanelMessage("OnPointerDown", eventData.position.x, eventData.position.y);
        }
    }
}