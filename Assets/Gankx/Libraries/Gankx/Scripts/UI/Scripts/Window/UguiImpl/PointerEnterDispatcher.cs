using UnityEngine.EventSystems;

namespace Gankx.UI
{
    public class PointerEnterDispatcher : EventDispatcher, IPointerEnterHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            SendPanelMessage("OnPointerEnter");
        }
    }
}