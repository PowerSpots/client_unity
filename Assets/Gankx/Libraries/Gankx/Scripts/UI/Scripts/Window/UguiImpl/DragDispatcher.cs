using UnityEngine.EventSystems;

namespace Gankx.UI
{
    public class DragDispatcher : EventDispatcher, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public void OnBeginDrag(PointerEventData eventData)
        {
            SendPanelMessage("OnBeginDrag", eventData.position.x, eventData.position.y);
        }

        public void OnDrag(PointerEventData eventData)
        {
            SendPanelMessage("OnDrag", eventData.position.x, eventData.position.y);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            SendPanelMessage("OnEndDrag", eventData.position.x, eventData.position.y);
        }
    }
}