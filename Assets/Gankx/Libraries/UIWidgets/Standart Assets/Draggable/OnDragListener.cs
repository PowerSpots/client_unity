﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace UIWidgets
{
    /// <summary>
    /// OnDragListener.
    /// </summary>
    public class OnDragListener : MonoBehaviour, IDragHandler {

		/// <summary>
		/// OnDragEvent.
		/// </summary>
		[SerializeField]
		public PointerUnityEvent OnDragEvent = new PointerUnityEvent();

		/// <summary>
		/// Raises the OnDragEvent.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			OnDragEvent.Invoke(eventData);
		}
	}
}