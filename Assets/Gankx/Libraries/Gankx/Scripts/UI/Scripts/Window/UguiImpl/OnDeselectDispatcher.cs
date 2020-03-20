using UnityEngine.EventSystems;

namespace Gankx.UI
{
    public class OnDeselectDispatcher : EventDispatcher, IDeselectHandler
    {
        public void OnDeselect(BaseEventData eventData)
        {
            SendPanelMessage("OnDeselect");
        }
    }
}