using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragListener : MonoBehaviour, IBeginDragHandler, IEndDragHandler {

    public Action BeginDragEvent = null;
    public Action EndDragEvent = null;

    public void OnBeginDrag(PointerEventData eventData) {
        if(BeginDragEvent != null) {
            BeginDragEvent();
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (EndDragEvent != null) {
            EndDragEvent();
        }
    }    
}
