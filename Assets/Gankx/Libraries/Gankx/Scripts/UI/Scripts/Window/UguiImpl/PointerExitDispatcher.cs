using UnityEngine.EventSystems;

namespace Gankx.UI
{
    public class PointerExitDispatcher : EventDispatcher, IPointerExitHandler
    {
        public void OnPointerExit(PointerEventData eventData)
        {
            SendPanelMessage("OnPointerExit");
        }
    }
}