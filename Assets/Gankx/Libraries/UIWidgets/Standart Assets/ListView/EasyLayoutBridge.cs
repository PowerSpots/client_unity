﻿using UnityEngine;
using UnityEngine.UI;

namespace UIWidgets
{
    /// <summary>
    /// Bridge to EasyLayout class.
    /// </summary>
    public class EasyLayoutBridge : ILayoutBridge {
		bool isHorizontal;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is horizontal.
		/// </summary>
		/// <value><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</value>
		public bool IsHorizontal {
			get {
				return isHorizontal;
			}
			set {
				isHorizontal = value;
				UpdateDirection();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UIWidgets.EasyLayoutBridge"/> update content size fitter.
		/// </summary>
		/// <value><c>true</c> if update content size fitter; otherwise, <c>false</c>.</value>
		public bool UpdateContentSizeFitter {
			get; set;
		}
		
		EasyLayout.EasyLayout Layout;
		
		RectTransform DefaultItem;

		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.EasyLayoutBridge"/> class.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="defaultItem">Default item.</param>
		public EasyLayoutBridge(EasyLayout.EasyLayout layout, RectTransform defaultItem)
		{
			Layout = layout;
			DefaultItem = defaultItem;
		}
		
		void UpdateDirection()
		{
			Layout.Stacking = isHorizontal ? EasyLayout.Stackings.Vertical : EasyLayout.Stackings.Horizontal;
			
			if (UpdateContentSizeFitter)
			{
				var fitter = Layout.GetComponent<ContentSizeFitter>();
				if (fitter)
				{
					fitter.horizontalFit = (IsHorizontal) ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;
					fitter.verticalFit = (!IsHorizontal) ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;
				}
			}
			
			var layout_rect_transform = Layout.transform as RectTransform;
			layout_rect_transform.pivot = new Vector2(0, 1);
			if (isHorizontal)
			{
				layout_rect_transform.anchorMin = new Vector2(0, 0);
				layout_rect_transform.anchorMax = new Vector2(0, 1);
			}
			else
			{
				layout_rect_transform.anchorMin = new Vector2(0, 1);
				layout_rect_transform.anchorMax = new Vector2(1, 1);
			}
			layout_rect_transform.sizeDelta = new Vector2(0, 0);
		}

		/// <summary>
		/// Updates the layout.
		/// </summary>
		public void UpdateLayout()
		{
			Layout.UpdateLayout();
		}

		/// <summary>
		/// Sets the filler.
		/// </summary>
		/// <param name="first">First.</param>
		/// <param name="last">Last.</param>
		public void SetFiller(float first, float last)
		{
			var padding = IsHorizontal
				? new EasyLayout.Padding(first, last, 0, 0)
				: new EasyLayout.Padding(0, 0, first, last);
			
			Layout.PaddingInner = padding;
		}

		/// <summary>
		/// Gets the size of the item.
		/// </summary>
		/// <returns>The item size.</returns>
		public Vector2 GetItemSize()
		{
			return new Vector2(DefaultItem.GetWidth() * DefaultItem.localScale.x, DefaultItem.GetHeight() * DefaultItem.localScale.y);
		}

		/// <summary>
		/// Gets the left or top margin.
		/// </summary>
		/// <returns>The margin.</returns>
		public float GetMargin()
		{
			return IsHorizontal ? Layout.GetMarginLeft() : Layout.GetMarginTop();
		}

		/// <summary>
		/// Gets the full margin.
		/// </summary>
		/// <returns>The full margin.</returns>
		public float GetFullMargin()
		{
			return IsHorizontal ? Layout.GetMarginLeft() + Layout.GetMarginRight() : Layout.GetMarginTop() + Layout.GetMarginBottom();
		}

        public float GetFullMarginX()
        {
            return Layout.GetMarginLeft() + Layout.GetMarginRight();
        }

        public float GetFullMarginY()
        {
            return Layout.GetMarginTop() + Layout.GetMarginBottom();
        }

        /// <summary>
        /// Gets the spacing between items.
        /// </summary>
        /// <returns>The spacing.</returns>
        public float GetSpacing()
		{
			return IsHorizontal ? Layout.Spacing.x : Layout.Spacing.y;
		}

	    public float GetSpacingX()
	    {
	        return Layout.Spacing.x;

	    }

        public float GetSpacingY()
        {
            return Layout.Spacing.y;
        }
    }
}