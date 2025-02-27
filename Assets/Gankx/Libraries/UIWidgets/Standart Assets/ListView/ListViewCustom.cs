﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UIWidgets
{
    /// <summary>
    /// ListView direction.
    /// Direction to scroll items. Used for optimization or TileView.
    /// Horizontal - Horizontal.
    /// Vertical - Vertical.
    /// </summary>
    public enum ListViewDirection {
		Horizontal = 0,
		Vertical = 1,
	}

	/// <summary>
	/// Custom ListView event.
	/// </summary>
	[Serializable]
	public class ListViewCustomEvent : UnityEvent<int> {
		
	}

	/// <summary>
	/// Base class for custom ListViews.
	/// </summary>
	public class ListViewCustom<TComponent,TItem> : ListViewBase where TComponent : ListViewItem {

		/// <summary>
		/// The items.
		/// </summary>
		[SerializeField]
		protected List<TItem> customItems = new List<TItem>();

		//[SerializeField]
		//[HideInInspector]
		ObservableList<TItem> dataSource;

		/// <summary>
		/// Gets or sets the data source.
		/// </summary>
		/// <value>The data source.</value>
		public ObservableList<TItem> DataSource {
			get {
				if (dataSource==null)
				{
					#pragma warning disable 0618
					dataSource = new ObservableList<TItem>(customItems);
					dataSource.OnChange += UpdateItems;
					customItems = null;
					#pragma warning restore 0618
				}
				return dataSource;
			}
			set {
				SetNewItems(value);
				SetScrollValue(0f);
                Resize();
			}
		}

		/// <summary>
		/// Gets or sets the items.
		/// </summary>
		/// <value>Items.</value>
		[Obsolete("Use DataSource instead.")]
		new public List<TItem> Items {
			get {
				return new List<TItem>(DataSource);
			}
			set {
				SetNewItems(new ObservableList<TItem>(value));
				SetScrollValue(0f);
			}
		}

		/// <summary>
		/// The default item.
		/// </summary>
		[SerializeField]
		public TComponent DefaultItem;

		/// <summary>
		/// The components list.
		/// </summary>
		protected List<TComponent> components = new List<TComponent>();

		/// <summary>
		/// The components cache list.
		/// </summary>
		protected List<TComponent> componentsCache = new List<TComponent>();

		Dictionary<int,UnityAction<PointerEventData>> callbacksEnter = new Dictionary<int,UnityAction<PointerEventData>>();

		Dictionary<int,UnityAction<PointerEventData>> callbacksExit = new Dictionary<int,UnityAction<PointerEventData>>();

		/// <summary>
		/// Gets the selected item.
		/// </summary>
		/// <value>The selected item.</value>
		public TItem SelectedItem {
			get {
				if (SelectedIndex==-1)
				{
					return default(TItem);
				}
				return DataSource[SelectedIndex];
			}
		}

		/// <summary>
		/// Gets the selected items.
		/// </summary>
		/// <value>The selected items.</value>
		public List<TItem> SelectedItems {
			get {
				return SelectedIndicies.Convert<int,TItem>(GetDataItem);
			}
		}

		[SerializeField]
		[FormerlySerializedAs("Sort")]
		bool sort = true;
		
		/// <summary>
		/// Sort items.
		/// Advice to use DataSource.Comparison instead Sort and SortFunc.
		/// </summary>
		public bool Sort {
			get {
				return sort;
			}
			set {
				sort = value;
				if (Sort && isStartedListViewCustom)
				{
					UpdateItems();
				}
			}
		}

		Func<IEnumerable<TItem>,IEnumerable<TItem>> sortFunc;

		/// <summary>
		/// Sort function.
		/// Advice to use DataSource.Comparison instead Sort and SortFunc.
		/// </summary>
		public Func<IEnumerable<TItem>, IEnumerable<TItem>> SortFunc {
			get {
				return sortFunc;
			}
			set {
				sortFunc = value;
				if (Sort && isStartedListViewCustom)
				{
					UpdateItems();
				}
			}
		}
		
		/// <summary>
		/// What to do when the object selected.
		/// </summary>
		public ListViewCustomEvent OnSelectObject = new ListViewCustomEvent();
		
		/// <summary>
		/// What to do when the object deselected.
		/// </summary>
		public ListViewCustomEvent OnDeselectObject = new ListViewCustomEvent();
		
		/// <summary>
		/// What to do when the event system send a pointer enter Event.
		/// </summary>
		public ListViewCustomEvent OnPointerEnterObject = new ListViewCustomEvent();
		
		/// <summary>
		/// What to do when the event system send a pointer exit Event.
		/// </summary>
		public ListViewCustomEvent OnPointerExitObject = new ListViewCustomEvent();
		
		[SerializeField]
		Color defaultBackgroundColor = Color.white;
		
		[SerializeField]
		Color defaultColor = Color.black;
		
		/// <summary>
		/// Default background color.
		/// </summary>
		public Color DefaultBackgroundColor {
			get {
				return defaultBackgroundColor;
			}
			set {
				defaultBackgroundColor = value;
				UpdateColors();
			}
		}
		
		/// <summary>
		/// Default text color.
		/// </summary>
		public Color DefaultColor {
			get {
				return defaultColor;
			}
			set {
				DefaultColor = value;
				UpdateColors();
			}
		}
		
		/// <summary>
		/// Color of background on pointer over.
		/// </summary>
		[SerializeField]
		public Color HighlightedBackgroundColor = new Color(203, 230, 244, 255);
		
		/// <summary>
		/// Color of text on pointer text.
		/// </summary>
		[SerializeField]
		public Color HighlightedColor = Color.black;
		
		[SerializeField]
		Color selectedBackgroundColor = new Color(53, 83, 227, 255);
		
		[SerializeField]
		Color selectedColor = Color.black;
		
		/// <summary>
		/// Background color of selected item.
		/// </summary>
		public Color SelectedBackgroundColor {
			get {
				return selectedBackgroundColor;
			}
			set {
				selectedBackgroundColor = value;
				UpdateColors();
			}
		}
		
		/// <summary>
		/// Text color of selected item.
		/// </summary>
		public Color SelectedColor {
			get {
				return selectedColor;
			}
			set {
				selectedColor = value;
				UpdateColors();
			}
		}

		/// <summary>
		/// The ScrollRect.
		/// </summary>
		[SerializeField]
		protected ScrollRect scrollRect;

		/// <summary>
		/// Gets or sets the ScrollRect.
		/// </summary>
		/// <value>The ScrollRect.</value>
		public ScrollRect ScrollRect {
			get {
				return scrollRect;
			}
			set {
				if (scrollRect!=null)
				{
					var r = scrollRect.GetComponent<ResizeListener>();
					if (r!=null)
					{
						r.OnResize.RemoveListener(SetNeedResize);
					}
					scrollRect.onValueChanged.RemoveListener(OnScrollRectUpdate);
				}
				scrollRect = value;
				if (scrollRect!=null)
				{
					var r = scrollRect.GetComponent<ResizeListener>() ?? scrollRect.gameObject.AddComponent<ResizeListener>();
					r.OnResize.AddListener(SetNeedResize);

					scrollRect.onValueChanged.AddListener(OnScrollRectUpdate);
				}
			}
		}

		/// <summary>
		/// The height of the DefaultItem.
		/// </summary>
		[SerializeField]
		[Tooltip("Minimal height of item")]
		protected float itemHeight;

		/// <summary>
		/// The width of the DefaultItem.
		/// </summary>
		[SerializeField]
		[Tooltip("Minimal width of item")]
		protected float itemWidth;

		/// <summary>
		/// The height of the ScrollRect.
		/// </summary>
		protected float scrollHeight;

		/// <summary>
		/// The width of the ScrollRect.
		/// </summary>
		protected float scrollWidth;

		/// <summary>
		/// Count of visible items.
		/// </summary>
		protected int maxVisibleItems;

		/// <summary>
		/// Count of visible items.
		/// </summary>
		protected int visibleItems;

		/// <summary>
		/// Count of hidden items by top filler.
		/// </summary>
		protected int topHiddenItems;

		/// <summary>
		/// Count of hidden items by bottom filler.
		/// </summary>
		protected int bottomHiddenItems;

		[SerializeField]
		ListViewDirection direction = ListViewDirection.Vertical;

		/// <summary>
		/// The set ContentSizeFitter parametres according direction.
		/// </summary>
		protected bool setContentSizeFitter = true;

		/// <summary>
		/// Gets or sets the direction.
		/// </summary>
		/// <value>The direction.</value>
		public ListViewDirection Direction {
			get {
				return direction;
			}
			set {
				direction = value;

				if (scrollRect)
				{
					scrollRect.horizontal = IsHorizontal();
					scrollRect.vertical = !IsHorizontal();
				}
				if (CanOptimize() && (layout is EasyLayout.EasyLayout))
				{
					LayoutBridge.IsHorizontal = IsHorizontal();

					if (isStartedListViewCustom)
					{
						CalculateMaxVisibleItems();
					}
				}
				if (isStartedListViewCustom)
				{
					UpdateView();
				}
			}
		}

		/// <summary>
		/// Awake this instance.
		/// </summary>
		protected virtual void Awake()
		{
			Start();
		}

		[System.NonSerialized]
		public bool isStartedListViewCustom = false;

		/// <summary>
		/// The layout.
		/// </summary>
		protected LayoutGroup layout;

		/// <summary>
		/// Gets the layout.
		/// </summary>
		/// <value>The layout.</value>
		public EasyLayout.EasyLayout Layout {
			get {
				return layout as EasyLayout.EasyLayout;
			}
		}

		/// <summary>
		/// Selected items cache (to keep valid selected indicies with updates).
		/// </summary>
		protected List<TItem> SelectedItemsCache;

		/// <summary>
		/// LayoutBridge.
		/// </summary>
		protected ILayoutBridge LayoutBridge;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public override void Start()
		{
			if (isStartedListViewCustom)
			{
				return ;
			}

			base.Start();
			base.Items = new List<ListViewItem>();

			SelectedItemsCache = SelectedItems;

			SetItemIndicies = false;

			DestroyGameObjects = false;

			if (DefaultItem==null)
			{
				throw new NullReferenceException(String.Format("DefaultItem is null. Set component of type {0} to DefaultItem.", typeof(TComponent).FullName));
			}
			DefaultItem.gameObject.SetActive(true);


			if (CanOptimize())
			{
				ScrollRect = scrollRect;

				var scroll_rect_transform = scrollRect.transform as RectTransform;
				scrollHeight = scroll_rect_transform.rect.height;
				scrollWidth = scroll_rect_transform.rect.width;

				layout = Container.GetComponent<LayoutGroup>();

				if (layout is EasyLayout.EasyLayout)
				{
					LayoutBridge = new EasyLayoutBridge(layout as EasyLayout.EasyLayout, DefaultItem.transform as RectTransform);
					LayoutBridge.IsHorizontal = IsHorizontal();
				}
				else if (layout is HorizontalOrVerticalLayoutGroup)
				{
					LayoutBridge = new StandardLayoutBridge(layout as HorizontalOrVerticalLayoutGroup, DefaultItem.transform as RectTransform);
				}

				CalculateItemSize();
				CalculateMaxVisibleItems();

				var r = scrollRect.gameObject.GetOrAddComponent<ResizeListener>();
				r.OnResize.AddListener(SetNeedResize);
			}

			DefaultItem.gameObject.SetActive(false);

			Direction = direction;

			UpdateItems();

			OnSelect.AddListener(OnSelectCallback);
			OnDeselect.AddListener(OnDeselectCallback);

			isStartedListViewCustom = true;
		}

		/// <summary>
		/// Gets the item.
		/// </summary>
		/// <returns>The item.</returns>
		/// <param name="index">Index.</param>
		protected TItem GetDataItem(int index)
		{
			return DataSource[index];
		}

		/// <summary>
		/// Calculates the size of the item.
		/// </summary>
		protected virtual void CalculateItemSize()
		{
			if (LayoutBridge==null)
			{
				return ;
			}
			var size = LayoutBridge.GetItemSize();
			if (itemHeight==0f)
			{
				itemHeight = size.y;
			}
			if (itemWidth==0f)
			{
				itemWidth = size.x;
			}
		}

		/// <summary>
		/// Determines whether this instance is horizontal.
		/// </summary>
		/// <returns><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</returns>
		protected bool IsHorizontal()
		{
			return direction==ListViewDirection.Horizontal;
		}

		/// <summary>
		/// Calculates the max count of visible items.
		/// </summary>
		protected virtual void CalculateMaxVisibleItems()
		{
			if (IsHorizontal())
			{
				maxVisibleItems = Mathf.CeilToInt(scrollWidth / itemWidth);
			}
			else
			{
				maxVisibleItems = Mathf.CeilToInt(scrollHeight / itemHeight);
			}
			maxVisibleItems = Mathf.Max(maxVisibleItems, 1) + 1;
		}

		/// <summary>
		/// Resize this instance.
		/// </summary>
		public virtual void Resize()
		{
			needResize = false;
			
			var scroll_rect_transform = scrollRect.transform as RectTransform;
			scrollHeight = scroll_rect_transform.rect.height;
			scrollWidth = scroll_rect_transform.rect.width;

			itemHeight = 0;
			itemWidth = 0;
			CalculateItemSize();
			CalculateMaxVisibleItems();
			UpdateView();

			components.Sort(ComponentsComparer);
			components.ForEach(SetComponentAsLastSibling);
		}

		/// <summary>
		/// Determines whether this instance can optimize.
		/// </summary>
		/// <returns><c>true</c> if this instance can optimize; otherwise, <c>false</c>.</returns>
		protected virtual bool CanOptimize()
		{
			var scrollRectSpecified = scrollRect!=null;
			var containerSpecified = Container!=null;
			var currentLayout = containerSpecified ? (layout ?? Container.GetComponent<LayoutGroup>()) : null;
			var validLayout = currentLayout ? ((currentLayout is EasyLayout.EasyLayout) || (currentLayout is HorizontalOrVerticalLayoutGroup)) : false;
			
			return scrollRectSpecified && validLayout;
		}

		/// <summary>
		/// Raises the select callback event.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="item">Item.</param>
		void OnSelectCallback(int index, ListViewItem item)
		{
            if (SelectedItemsCache != null && DataSource != null && DataSource.Count > index )
			{
				SelectedItemsCache.Add(DataSource[index]);
			}

			OnSelectObject.Invoke(index);

			if (item!=null)
			{
				SelectColoring(item);
			}
		}

		/// <summary>
		/// Raises the deselect callback event.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="item">Item.</param>
		void OnDeselectCallback(int index, ListViewItem item)
		{
			if (SelectedItemsCache!=null)
			{
				SelectedItemsCache.Remove(DataSource[index]);
			}

			OnDeselectObject.Invoke(index);

			if (item!=null)
			{
				DefaultColoring(item);
			}
		}

		/// <summary>
		/// Raises the pointer enter callback event.
		/// </summary>
		/// <param name="item">Item.</param>
		void OnPointerEnterCallback(ListViewItem item)
		{
			OnPointerEnterObject.Invoke(item.Index);

			if (!IsSelected(item.Index))
			{
				HighlightColoring(item);
			}
		}

		/// <summary>
		/// Raises the pointer exit callback event.
		/// </summary>
		/// <param name="item">Item.</param>
		void OnPointerExitCallback(ListViewItem item)
		{
			OnPointerExitObject.Invoke(item.Index);

			if (!IsSelected(item.Index))
			{
				DefaultColoring(item);
			}
		}
		
		/// <summary>
		/// Updates thitemsms.
		/// </summary>
		public override void UpdateItems()
		{
			SetNewItems(DataSource);
		}

		/// <summary>
		/// Clear items of this instance.
		/// </summary>
		public override void Clear()
		{
			DataSource.Clear();
			SetScrollValue(0f);
		}

		/// <summary>
		/// Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of added item.</returns>
		public virtual int Add(TItem item)
		{
			if (item==null)
			{
				throw new ArgumentNullException("item", "Item is null.");
			}

			DataSource.Add(item);
			
			return DataSource.IndexOf(item);
		}
		
		/// <summary>
		/// Remove the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of removed TItem.</returns>
		public virtual int Remove(TItem item)
		{
			var index = DataSource.IndexOf(item);
			if (index==-1)
			{
				return index;
			}

			DataSource.RemoveAt(index);

			return index;
		}

		/// <summary>
		/// Remove item by specifieitemsex.
		/// </summary>
		/// <returns>Index of removed item.</returns>
		/// <param name="index">Index.</param>
		public virtual void Remove(int index)
		{
			DataSource.RemoveAt(index);		
		}

        /// <summary>
        /// Sets the scroll value.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="callScrollUpdate">Call ScrollUpdate() if position changed.</param>
        public void SetScrollValue(float value, bool callScrollUpdate=true)
		{
			if (scrollRect == null || scrollRect.content==null)
			{
				return ;
			}
			var current_position = scrollRect.content.anchoredPosition;
			var new_position = IsHorizontal()
				? new Vector2(value, current_position.y)
				: new Vector2(current_position.x, value);

			var diff_x = IsHorizontal() ? Mathf.Abs(current_position.x - new_position.x) > 0.1f : false;
			var diff_y = IsHorizontal() ? false : Mathf.Abs(current_position.y - new_position.y) > 0.1f;
			if (diff_x || diff_y)
			{
				scrollRect.content.anchoredPosition = new_position;
				if (callScrollUpdate)
				{
					ScrollUpdate();
				}
			}
		}

		/// <summary>
		/// Gets the scroll value.
		/// </summary>
		/// <returns>The scroll value.</returns>
		public float GetScrollValue()
		{
			var pos = scrollRect.content.anchoredPosition;
			return Mathf.Max(0f, (IsHorizontal()) ? -pos.x : pos.y);
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

			var first_visible = GetFirstVisibleIndex(true);
			var last_visible = GetLastVisibleIndex(true);

			if (first_visible > index)
			{
				SetScrollValue(GetItemPosition(index));
			}
			else if (last_visible < index)
			{
				SetScrollValue(GetItemPositionBottom(index));
			}
		}

		/// <summary>
		/// Gets the item position.
		/// </summary>
		/// <returns>The item position.</returns>
		/// <param name="index">Index.</param>
		protected virtual float GetItemPosition(int index)
		{
			return index * GetItemSize();
		}

		/// <summary>
		/// Gets the item position bottom.
		/// </summary>
		/// <returns>The item position bottom.</returns>
		/// <param name="index">Index.</param>
		protected virtual float GetItemPositionBottom(int index)
		{
			return GetItemPosition(index) + GetItemSize() - LayoutBridge.GetSpacing() + LayoutBridge.GetMargin() - GetScrollSize();
		}

		/// <summary>
		/// Removes the callbacks.
		/// </summary>
		protected void RemoveCallbacks()
		{
			base.Items.ForEach(RemoveCallback);
		}

		/// <summary>
		/// Adds the callbacks.
		/// </summary>
		protected void AddCallbacks()
		{
			base.Items.ForEach(AddCallback);
		}

		/// <summary>
		/// Removes the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="index">Index.</param>
		protected virtual void RemoveCallback(ListViewItem item, int index)
		{
			if (callbacksEnter.ContainsKey(index))
			{
				if (item!=null)
				{
					item.onPointerEnter.RemoveListener(callbacksEnter[index]);
				}
				callbacksEnter.Remove(index);
			}
			if (callbacksExit.ContainsKey(index))
			{
				if (item!=null)
				{
					item.onPointerExit.RemoveListener(callbacksExit[index]);
				}
				callbacksExit.Remove(index);
			}
		}

		/// <summary>
		/// Adds the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="index">Index.</param>
		protected virtual void AddCallback(ListViewItem item, int index)
		{
			callbacksEnter.Add(index, ev => OnPointerEnterCallback(item));
			callbacksExit.Add(index, ev => OnPointerExitCallback(item));
			
			item.onPointerEnter.AddListener(callbacksEnter[index]);
			item.onPointerExit.AddListener(callbacksExit[index]);
		}

		/// <summary>
		/// Set the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="allowDuplicate">If set to <c>true</c> allow duplicate.</param>
		/// <returns>Index of item.</returns>
		public int Set(TItem item, bool allowDuplicate=true)
		{
			int index;
			if (!allowDuplicate)
			{
				index = DataSource.IndexOf(item);
				if (index==-1)
				{
					index = Add(item);
				}
			}
			else
			{
				index = Add(item);
			}
			Select(index);
			
			return index;
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="item">Item.</param>
		protected virtual void SetData(TComponent component, TItem item)
		{
		}

		/// <summary>
		/// Updates the components count.
		/// </summary>
		protected void UpdateComponentsCount()
		{
			components.RemoveAll(IsNullComponent);

			if (components.Count==visibleItems)
			{
				return ;
			}

			if (components.Count < visibleItems)
			{
				componentsCache.RemoveAll(IsNullComponent);

				Enumerable.Range(0, visibleItems - components.Count).ForEach(AddComponent);
			}
			else
			{
				var to_cache = components.GetRange(visibleItems, components.Count - visibleItems).OrderByDescending<TComponent,int>(GetComponentIndex);

				to_cache.ForEach(DeactivateComponent);
				componentsCache.AddRange(to_cache);
				components.RemoveRange(visibleItems, components.Count - visibleItems);
			}

			base.Items = components.Convert(x => x as ListViewItem);
		}

		bool IsNullComponent(TComponent component)
		{
			return component==null;
		}

		void AddComponent(int index)
		{
			TComponent component;
			if (componentsCache.Count > 0)
			{
				component = componentsCache[componentsCache.Count - 1];
				componentsCache.RemoveAt(componentsCache.Count - 1);
			}
			else
			{
				component = Instantiate(DefaultItem) as TComponent;
				Utilites.FixInstantiated(DefaultItem, component);
			}
			component.Index = -1;
			component.transform.SetAsLastSibling();
			component.gameObject.SetActive(true);
			components.Add(component);
		}

		void DeactivateComponent(TComponent component)
		{
			RemoveCallback(component, component.Index);
			if (component!=null)
			{
				component.MovedToCache();
				component.Index = -1;
				component.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Gets the size of the item.
		/// </summary>
		/// <returns>The item size.</returns>
		protected float GetItemSize()
		{
			return (IsHorizontal())
				? itemWidth + LayoutBridge.GetSpacing()
				: itemHeight + LayoutBridge.GetSpacing();
		}

		/// <summary>
		/// Gets the size of the scroll.
		/// </summary>
		/// <returns>The scroll size.</returns>
		protected float GetScrollSize()
		{
			return (IsHorizontal()) ? scrollWidth : scrollHeight;
		}

		/// <summary>
		/// Gets the last index of the visible.
		/// </summary>
		/// <returns>The last visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		protected virtual int GetLastVisibleIndex(bool strict=false)
		{
			var window = GetScrollValue() + GetScrollSize();
			var last_visible_index = (strict)
				? Mathf.FloorToInt(window / GetItemSize())
				: Mathf.CeilToInt(window / GetItemSize());

			return last_visible_index - 1;
		}

		/// <summary>
		/// Gets the first index of the visible.
		/// </summary>
		/// <returns>The first visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		protected virtual int GetFirstVisibleIndex(bool strict=false)
		{
			var first_visible_index = (strict)
				? Mathf.CeilToInt(GetScrollValue() / GetItemSize())
				: Mathf.FloorToInt(GetScrollValue() / GetItemSize());
			first_visible_index = Mathf.Max(0, first_visible_index);
			if (strict)
			{
				return first_visible_index;
			}

			return Mathf.Min(first_visible_index, Mathf.Max(0, DataSource.Count - visibleItems));
		}

		/// <summary>
		/// On ScrollUpdate.
		/// </summary>
		protected virtual void ScrollUpdate()
		{
			var oldTopHiddenItems = topHiddenItems;
			
			topHiddenItems = GetFirstVisibleIndex();
			bottomHiddenItems = Mathf.Max(0, DataSource.Count - visibleItems - topHiddenItems);
			
			if (oldTopHiddenItems==topHiddenItems)
			{
				//do nothing
			}
			// optimization on +-1 item scroll
			else if (oldTopHiddenItems==(topHiddenItems + 1))
			{
				var bottomComponent = components[components.Count - 1];
				components.RemoveAt(components.Count - 1);
				components.Insert(0, bottomComponent);
				bottomComponent.transform.SetAsFirstSibling();
				
				bottomComponent.Index = topHiddenItems;
				SetData(bottomComponent, DataSource[topHiddenItems]);
				Coloring(bottomComponent as ListViewItem);
			}
			else if (oldTopHiddenItems==(topHiddenItems - 1))
			{
				var topComponent = components[0];
				components.RemoveAt(0);
				components.Add(topComponent);
				topComponent.transform.SetAsLastSibling();
				
				topComponent.Index = topHiddenItems + visibleItems - 1;
				SetData(topComponent, DataSource[topHiddenItems + visibleItems - 1]);
				Coloring(topComponent as ListViewItem);
			}
			// all other cases
			else
			{
				var current_visible_range = components.Convert<TComponent,int>(GetComponentIndex);
				var new_visible_range = Enumerable.Range(topHiddenItems, visibleItems).ToArray();

				var new_indicies_to_change = new_visible_range.Except(current_visible_range).ToList();
				var components_to_change = new Stack<TComponent>(components.Where(x => !new_visible_range.Contains(x.Index)));

				new_indicies_to_change.ForEach(index => {
					var component = components_to_change.Pop();

					component.Index = index;
					SetData(component, DataSource[index]);
					Coloring(component as ListViewItem);
				});

				components.Sort(ComponentsComparer);
				components.ForEach(SetComponentAsLastSibling);
			}
			
			if (LayoutBridge!=null)
			{
				LayoutBridge.SetFiller(CalculateTopFillerSize(), CalculateBottomFillerSize());
				LayoutBridge.UpdateLayout();
			}
		}

		/// <summary>
		/// Compare components by component index.
		/// </summary>
		/// <returns>A signed integer that indicates the relative values of x and y.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		protected int ComponentsComparer(TComponent x, TComponent y)
		{
			return x.Index.CompareTo(y.Index);
		}

		/// <summary>
		/// Raises the scroll rect update event.
		/// </summary>
		/// <param name="position">Position.</param>
		protected virtual void OnScrollRectUpdate(Vector2 position)
		{
			StartScrolling();
			ScrollUpdate();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected void UpdateView()
		{
			RemoveCallbacks();

			if ((CanOptimize()) && (DataSource.Count > 0))
			{
				visibleItems = (maxVisibleItems < DataSource.Count) ? maxVisibleItems : DataSource.Count;
			}
			else
			{
				visibleItems = DataSource.Count;
			}

			if (CanOptimize())
			{
				topHiddenItems = GetFirstVisibleIndex();
				if (topHiddenItems > (DataSource.Count - 1))
				{
					topHiddenItems = Mathf.Max(0, DataSource.Count - 2);
				}
				if ((topHiddenItems + visibleItems) > DataSource.Count)
				{
					visibleItems = DataSource.Count - topHiddenItems;
				}
				bottomHiddenItems = Mathf.Max(0, DataSource.Count - visibleItems - topHiddenItems);
			}
			else
			{
				topHiddenItems = 0;
				bottomHiddenItems = DataSource.Count() - visibleItems;
			}

			UpdateComponentsCount();

			var indicies = Enumerable.Range(topHiddenItems, visibleItems).ToArray();
			components.ForEach((x, i) => {
				x.Index = indicies[i];
				SetData(x, DataSource[indicies[i]]);
				Coloring(x as ListViewItem);
			});
			
			AddCallbacks();
			
			if (LayoutBridge!=null)
			{
				LayoutBridge.SetFiller(CalculateTopFillerSize(), CalculateBottomFillerSize());
				LayoutBridge.UpdateLayout();
			}
			
			if (ScrollRect!=null)
			{
				var item_ends = (DataSource.Count==0) ? 0f : Mathf.Max(0f, GetItemPositionBottom(DataSource.Count - 1));
				
				if (GetScrollValue() > item_ends)
				{
					SetScrollValue(item_ends);
				}
			}
		}

		/// <summary>
		/// Keep selected items on items update.
		/// </summary>
		[SerializeField]
		protected bool KeepSelection = true;

		bool IndexNotFound(int index)
		{
			return index==-1;
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		/// <param name="newItems">New items.</param>
		protected virtual void SetNewItems(ObservableList<TItem> newItems)
		{
			DataSource.OnChange -= UpdateItems;
			
			if (Sort && SortFunc!=null)
			{
				newItems.BeginUpdate();
				
				var sorted = SortFunc(newItems).ToArray();
				
				newItems.Clear();
				newItems.AddRange(sorted);
				
				newItems.EndUpdate();
			}

			SilentDeselect(SelectedIndicies);
			var new_selected_indicies = SelectedItemsCache.Convert<TItem,int>(newItems.IndexOf);
			new_selected_indicies.RemoveAll(IndexNotFound);

			dataSource = newItems;

			if (KeepSelection)
			{
				SilentSelect(new_selected_indicies);
			}
			SelectedItemsCache = SelectedItems;

			UpdateView();

			DataSource.OnChange += UpdateItems;
		}

		/// <summary>
		/// Calculates the size of the bottom filler.
		/// </summary>
		/// <returns>The bottom filler size.</returns>
		protected virtual float CalculateBottomFillerSize()
		{
			if (bottomHiddenItems==0)
			{
				return 0f;
			}
			return Mathf.Max(0, bottomHiddenItems * GetItemSize() - LayoutBridge.GetSpacing());
		}

		/// <summary>
		/// Calculates the size of the top filler.
		/// </summary>
		/// <returns>The top filler size.</returns>
		protected virtual float CalculateTopFillerSize()
		{
			if (topHiddenItems==0)
			{
				return 0f;
			}
			return Mathf.Max(0, topHiddenItems * GetItemSize() - LayoutBridge.GetSpacing());
		}

		/// <summary>
		/// Determines if item exists with the specified index.
		/// </summary>
		/// <returns><c>true</c> if item exists with the specified index; otherwise, <c>false</c>.</returns>
		/// <param name="index">Index.</param>
		public override bool IsValid(int index)
		{
			return (index >= 0) && (index < DataSource.Count);
		}

		/// <summary>
		/// Coloring the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void Coloring(ListViewItem component)
		{
			if (component==null)
			{
				return ;
			}
			if (SelectedIndicies.Contains(component.Index))
			{
				SelectColoring(component);
			}
			else
			{
				DefaultColoring(component);
			}
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void HighlightColoring(ListViewItem component)
		{
			if (IsSelected(component.Index))
			{
				return ;
			}
			HighlightColoring(component as TComponent);
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void HighlightColoring(TComponent component)
		{
			component.Background.color = HighlightedBackgroundColor;
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void SelectColoring(ListViewItem component)
		{
			if (component==null)
			{
				return ;
			}

			SelectColoring(component as TComponent);
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void SelectColoring(TComponent component)
		{
			component.Background.color = SelectedBackgroundColor;
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void DefaultColoring(ListViewItem component)
		{
			if (component==null)
			{
				return ;
			}

			DefaultColoring(component as TComponent);
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void DefaultColoring(TComponent component)
		{
			component.Background.color = DefaultBackgroundColor;
		}

		/// <summary>
		/// Updates the colors.
		/// </summary>
		void UpdateColors()
		{
			components.ForEach(x => Coloring(x as ListViewItem));
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{	
			layout = null;
			LayoutBridge = null;

			OnSelect.RemoveListener(OnSelectCallback);
			OnDeselect.RemoveListener(OnDeselectCallback);

			ScrollRect = null;

			RemoveCallbacks();
			
			base.OnDestroy();
		}

		/// <summary>
		/// Calls specified function with each component.
		/// </summary>
		/// <param name="func">Func.</param>
		public override void ForEachComponent(Action<ListViewItem> func)
		{
			base.ForEachComponent(func);
			func(DefaultItem);
			componentsCache.Select(x => x as ListViewItem).ForEach(func);
		}

		/// <summary>
		/// OnStartScrolling event.
		/// </summary>
		public UnityEvent OnStartScrolling = new UnityEvent();

		/// <summary>
		/// OnEndScrolling event.
		/// </summary>
		public UnityEvent OnEndScrolling = new UnityEvent();

		/// <summary>
		/// Time before raise OnEndScrolling event since last OnScrollRectUpdate event raised.
		/// </summary>
		public float EndScrollDelay = 0.3f;

		bool scrolling;
		float lastScrollingTime;

		void Update()
		{
			if (needResize)
			{
				Resize();
			}

			if (IsEndScrolling())
			{
				EndScrolling();
			}
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		public virtual void OnEnable()
		{
			StartCoroutine(ForceRebuild());
		}

		System.Collections.IEnumerator ForceRebuild()
		{
			yield return null;
			ForEachComponent(MarkLayoutForRebuild);
		}

		void MarkLayoutForRebuild(ListViewItem item)
		{
			LayoutRebuilder.MarkLayoutForRebuild(item.transform as RectTransform);
		}

		void StartScrolling()
		{
			lastScrollingTime = Time.time;
			if (scrolling)
			{
				return ;
			}
			scrolling = true;
			OnStartScrolling.Invoke();
		}

		bool IsEndScrolling()
		{
			if (!scrolling)
			{
				return false;
			}
			return (lastScrollingTime + EndScrollDelay) <= Time.time;
		}

		void EndScrolling()
		{
			scrolling = false;
			OnEndScrolling.Invoke();
		}

		bool needResize;
		void SetNeedResize()
		{
			if (!CanOptimize())
			{
				return ;
			}
			needResize = true;
		}
	}
}