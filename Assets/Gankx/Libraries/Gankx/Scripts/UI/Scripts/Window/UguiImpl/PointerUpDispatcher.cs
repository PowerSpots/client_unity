using UnityEngine.EventSystems;

namespace Gankx.UI
{
    public class PointerUpDispatcher : EventDispatcher, IPointerUpHandler
    {
        public void OnPointerUp(PointerEventData eventData)
        {
            SendPanelMessage("OnPointerUp", eventData.position.x, eventData.position.y);
        }
    }
}