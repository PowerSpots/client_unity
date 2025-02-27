﻿using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UIWidgets
{
    /// <summary>
    /// Base class for custom ListView with dynamic items heights.
    /// </summary>
    public class ListViewCustomHeight<TComponent,TItem> : ListViewCustom<TComponent,TItem>
		where TComponent : ListViewItem
		where TItem: IItemHeight
	{
		/// <summary>
		/// Calculate height automaticly without using IListViewItemHeight.Height.
		/// </summary>
		[SerializeField]
		[Tooltip("Calculate height automaticly without using IListViewItemHeight.Height.")]
		bool ForceAutoHeightCalculation = true;

		TComponent defaultItemCopy;
		RectTransform defaultItemCopyRect;

		/// <summary>
		/// Gets the default item copy.
		/// </summary>
		/// <value>The default item copy.</value>
		protected TComponent DefaultItemCopy {
			get {
				if (defaultItemCopy==null)
				{
					defaultItemCopy = Instantiate(DefaultItem) as TComponent;
					defaultItemCopy.transform.SetParent(DefaultItem.transform.parent);
					Utilites.FixInstantiated(DefaultItem, defaultItemCopy);
					defaultItemCopy.gameObject.name = "DefaultItemCopy";
					defaultItemCopy.gameObject.SetActive(false);
				}
				return defaultItemCopy;
			}
		}

		/// <summary>
		/// Gets the RectTransform of DefaultItemCopy.
		/// </summary>
		/// <value>RectTransform.</value>
		protected RectTransform DefaultItemCopyRect {
			get {
				if (defaultItemCopyRect==null)
				{
					defaultItemCopyRect = defaultItemCopy.transform as RectTransform;
				}
				return defaultItemCopyRect;
			}
		}

		bool IsCanCalculateHeight;
		public ListViewCustomHeight()
		{
			IsCanCalculateHeight = typeof(IListViewItemHeight).IsAssignableFrom(typeof(TComponent));
		}

		/// <summary>
		/// Awake this instance.
		/// </summary>
		protected override void Awake()
		{
			Start(); 
		}

		/// <summary>
		/// Calculates the max count of visible items.
		/// </summary>
		protected override void CalculateMaxVisibleItems()
		{
			SetItemsHeight(DataSource);

			var height = scrollHeight;
			maxVisibleItems = DataSource.OrderBy<TItem,float>(GetItemHeight).TakeWhile(x => {
				height -= x.Height;
				return height >= 0;
			}).Count() + 2;
		}

		/// <summary>
		/// Calculates the size of the item.
		/// </summary>
		protected override void CalculateItemSize()
		{
			var rect = DefaultItem.transform as RectTransform;
			#if UNITY_4_6 || UNITY_4_7
			var layout_elements = rect.GetComponents<Component>().OfType<ILayoutElement>();
			#else
			var layout_elements = rect.GetComponents<ILayoutElement>();
			#endif
			if (itemHeight==0)
			{
				var preffered_height = layout_elements.Max(x => Mathf.Max(x.minHeight, x.preferredHeight));
				itemHeight = (preffered_height > 0) ? preffered_height : rect.rect.height;
			}
			if (itemWidth==0)
			{
				var preffered_width = layout_elements.Max(x => Mathf.Max(x.minWidth, x.preferredWidth));
				itemWidth = (preffered_width > 0) ? preffered_width : rect.rect.width;
			}
		}
		
		/// <summary>
		/// Scrolls to item with specifid index.
		/// </summary>
		/// <param name="index">Index.</param>
		protected override void ScrollTo(int index)
		{
			if (!CanOptimize())
			{
				return ;
			}

			var top = GetScrollValue();
			var bottom = GetScrollValue() + scrollHeight;

			var item_starts = ItemStartAt(index);

			var item_ends = ItemEndAt(index) + LayoutBridge.GetMargin();

			if (item_starts < top)
			{
				SetScrollValue(item_starts);
			}
			else if (item_ends > bottom)
			{
				SetScrollValue(item_ends - GetScrollSize());
			}
		}

		/// <summary>
		/// Calculates the size of the bottom filler.
		/// </summary>
		/// <returns>The bottom filler size.</returns>
		protected override float CalculateBottomFillerSize()
		{
			if (bottomHiddenItems==0)
			{
				return 0f;
			}
			var height = DataSource.GetRange(topHiddenItems + visibleItems, bottomHiddenItems).SumFloat(GetItemHeight);

			return Mathf.Max(0, height + (LayoutBridge.GetSpacing() * (bottomHiddenItems - 1)));
		}

        protected override float GetItemPositionBottom(int index)
        {
            //topHiddenItems = GetFirstVisibleIndex();
            if (index <= topHiddenItems + visibleItems )
            {
                float height = DataSource.GetRange(topHiddenItems,visibleItems).SumFloat(GetItemHeight);
                return base.GetItemPosition(topHiddenItems) + height + LayoutBridge.GetMargin();// -GetScrollSize();
            }
            else
                return base.GetItemPositionBottom( index );
        }

		/// <summary>
		/// Calculates the size of the top filler.
		/// </summary>
		/// <returns>The top filler size.</returns>
		protected override float CalculateTopFillerSize()
		{
			if (topHiddenItems==0)
			{
				return 0f;
			}

			var height = DataSource.GetRange(0, topHiddenItems).SumFloat(GetItemHeight);

			return Mathf.Max(0, height + (LayoutBridge.GetSpacing() * (topHiddenItems - 1)));
		}

		float GetItemHeight(TItem item)
		{
			return item.Height;
		}

		/// <summary>
		/// Total height of items before specified index.
		/// </summary>
		/// <returns>Height.</returns>
		/// <param name="index">Index.</param>
		float ItemStartAt(int index)
		{
			var height = DataSource.GetRange(0, index).SumFloat(GetItemHeight);

			return height + (LayoutBridge.GetSpacing() * index);
		}

		/// <summary>
		/// Total height of items before and with specified index.
		/// </summary>
		/// <returns>The <see cref="System.Single"/>.</returns>
		/// <param name="index">Index.</param>
		float ItemEndAt(int index)
		{
			var height = DataSource.GetRange(0, index + 1).SumFloat(GetItemHeight);
			
			return height + (LayoutBridge.GetSpacing() * index);
		}

		/// <summary>
		/// Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of added item.</returns>
		public override int Add(TItem item)
		{
			if (item==null)
			{
				throw new ArgumentNullException("item", "Item is null.");
			}
			if (item.Height==0)
			{
				item.Height = CalculateItemHeight(item);
			}

			return base.Add(item);
		}

        /// <summary>
        /// Insert the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <returns>Index of added item.</returns>
        public void Insert(int index, TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", "Item is null.");
            }
            if (item.Height == 0)
            {
                item.Height = CalculateItemHeight(item);
            }

            DataSource.Insert(index, item);
        }

		/// <summary>
		/// Calculate and sets the height of the items.
		/// </summary>
		/// <param name="items">Items.</param>
		/// <param name="forceUpdate">If set to <c>true</c> force update.</param>
		void SetItemsHeight(ObservableList<TItem> items, bool forceUpdate = true)
		{
			items.ForEach(x => {
				if ((x.Height==0) || forceUpdate)
				{
					x.Height = CalculateItemHeight(x);
				}
			});
		}

		/// <summary>
		/// Resize this instance.
		/// </summary>
		public override void Resize()
		{
			//defaultItemCopy.gameObject.SetActive(true);
			//(defaultItemCopy.transform as RectTransform).sizeDelta = (DefaultItem.transform as RectTransform).sizeDelta;

			SetItemsHeight(DataSource, true);
			//defaultItemCopy.gameObject.SetActive(false);

			base.Resize();
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		/// <param name="newItems">New items.</param>
		protected override void SetNewItems(ObservableList<TItem> newItems)
		{
			SetItemsHeight(newItems);

			base.SetNewItems(newItems);
		}

		/// <summary>
		/// Gets the height of the index by.
		/// </summary>
		/// <returns>The index by height.</returns>
		/// <param name="height">Height.</param>
		int GetIndexByHeight(float height)
		{
			var spacing = LayoutBridge.GetSpacing();
			return DataSource.TakeWhile((item, index) => {
				height -= item.Height;
				if (index > 0)
				{
					height -= spacing;
				}
				return height >= 0;
			}).Count();
		}

		/// <summary>
		/// Gets the last index of the visible.
		/// </summary>
		/// <returns>The last visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		protected override int GetLastVisibleIndex(bool strict=false)
		{
			var last_visible_index = GetIndexByHeight(GetScrollValue() + scrollHeight);

			return (strict) ? last_visible_index : last_visible_index + 2;
		}

		/// <summary>
		/// Gets the first index of the visible.
		/// </summary>
		/// <returns>The first visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		protected override int GetFirstVisibleIndex(bool strict=false)
		{
			var first_visible_index = GetIndexByHeight(GetScrollValue());

			if (strict)
			{
				return first_visible_index;
			}
			return Mathf.Min(first_visible_index, Mathf.Max(0, DataSource.Count - visibleItems));
		}
		
		LayoutGroup defaultItemlayoutGroup;

		/// <summary>
		/// Gets the height of the item.
		/// </summary>
		/// <returns>The item height.</returns>
		/// <param name="item">Item.</param>
		float CalculateItemHeight(TItem item)
		{
			if (defaultItemlayoutGroup==null)
			{
				defaultItemlayoutGroup = DefaultItemCopy.GetComponent<LayoutGroup>();
			}

			float height = 0f;
			if ((!IsCanCalculateHeight || ForceAutoHeightCalculation) && (defaultItemlayoutGroup!=null))
			{
				DefaultItemCopy.gameObject.SetActive(true);

				SetData(DefaultItemCopy, item);

				defaultItemlayoutGroup.CalculateLayoutInputHorizontal();
				defaultItemlayoutGroup.SetLayoutHorizontal();
				defaultItemlayoutGroup.CalculateLayoutInputVertical();
				defaultItemlayoutGroup.SetLayoutVertical();

				height = LayoutUtility.GetPreferredHeight(DefaultItemCopyRect);

				DefaultItemCopy.gameObject.SetActive(false);
			}
			else
			{
				SetData(DefaultItemCopy, item);

				height = (DefaultItemCopy as IListViewItemHeight).Height;
			}

			return height;
		}

		/// <summary>
		/// Calls specified function with each component.
		/// </summary>
		/// <param name="func">Func.</param>
		public override void ForEachComponent(Action<ListViewItem> func)
		{
			base.ForEachComponent(func);
			func(DefaultItemCopy);
		}

		/// <summary>
		/// 重新设置数据，但不更新scrollbar
		/// </summary>
		/// <param name="newItems"></param>
		public void SetNewItemsAndResize(ObservableList<TItem> newItems)
		{
			SetNewItems(newItems);
			Resize();
		}

		/// <summary>
		/// 滑动到底部
		/// </summary>
		public void ScrollToBottom()
		{
			int count = GetCount();
			if (count > 0)
			{
				ScrollTo(count - 1);
			}
		}

        public void ScrollToTop()
        {
            ScrollTo(0);
        }

		public int GetCount()
		{
			if (null == DataSource)
			{
				return 0;
			}

			return DataSource.Count;
		}

	}
}