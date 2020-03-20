using UnityEngine.EventSystems;

namespace Gankx.UI
{
    public class OnSelectDispatcher : EventDispatcher, ISelectHandler
    {
        public void OnSelect(BaseEventData eventData)
        {
            SendPanelMessage("OnSelect");
        }
    }
}