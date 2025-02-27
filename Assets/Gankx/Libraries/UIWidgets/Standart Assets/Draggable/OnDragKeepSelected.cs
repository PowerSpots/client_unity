﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UIWidgets
{
    /// <summary>
    /// No more useful.
    /// Return selection to last selected object after drag.
    /// </summary>
    [Obsolete("No more useful.")]
	public class OnDragKeepSelected : MonoBehaviour, IEndDragHandler
	{
		/// <summary>
		/// Raises the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnEndDrag(PointerEventData eventData)
		{
			#if UNITY_5_2 || UNITY_5_3 || UNITY_5_4
			#else
			EventSystem.current.SetSelectedGameObject(EventSystem.current.lastSelectedGameObject);
			#endif
		}
	}
}