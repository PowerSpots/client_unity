using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gankx.UI
{
    public class PointerClickDispatcher : EventDispatcher,
        IPointerClickHandler,
        IEndDragHandler,
        IBeginDragHandler,
        IPointerDownHandler,
        IPointerUpHandler
    {
        [HideInInspector]
        [NonSerialized]
        public Predicate<PointerEventData> ShouldBlockClick = null;

        private bool myDraging;

        public void OnBeginDrag(PointerEventData eventData)
        {
            myDraging = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            myDraging = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (myDraging)
            {
                return;
            }

            if (ShouldBlockClick != null && ShouldBlockClick(eventData))
            {
            }
            else
            {
                SendPanelMessage("OnPointerClick", eventData.clickCount);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnPointerUp(PointerEventData eventData)
        {
        }
    }
}