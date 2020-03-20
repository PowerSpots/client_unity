using Gankx;
using Gankx.UI;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EasyListView : ListViewBase {
    private int dataSourceCount;

    public void SetDataSource(int sourceCount)
    {
        // already invoke init start
        if (isStartedListViewCustom)
        {
            dataSourceCount = sourceCount;
            if (CanOptimize()) {
                Resize();
            }
            else {
                UpdateView();
            }

            
            //if (dataSourceCount != sourceCount)
            //{
            //    SetNeedResize();
            //    dataSourceCount = sourceCount;
            //}
            //else
            //{
            //    dataSourceCount = sourceCount;
            //    UpdateView();
            //}
        }
        else
        {
            dataSourceCount = sourceCount;

            InitStart();
            UpdateView();
        }
    }

    private void RefreshView()
    {
        if (LuaService.instance != null)
        {
            Window window = Window.GetWindow(gameObject);
            if (window != null)
            {
                LuaService.instance.FireEvent(
                    "OnPanelMessage", window.panelId, window.id, "OnListViewUpdate");
            }
        }
    }

    private void RefreshItem(ListViewItem item)
    {
        if (LuaService.instance != null)
        {
            Window window = Window.GetWindow(gameObject);
            if (window != null)
            {
                Window itemWindow = Window.GetWindow(item.gameObject);

                if (itemWindow != null)
                {
                    LuaService.instance.FireEvent(
                        "OnPanelMessage", window.panelId, window.id, "OnListItemUpdate", itemWindow.id, item.Index);
                }
            }
        }
    }

    private void ScrollUpdateOver()
    {
        if (LuaService.instance != null)
        {
            Window window = Window.GetWindow(gameObject);
            if (window != null)
            {
                LuaService.instance.FireEvent(
                    "OnPanelMessage", window.panelId, window.id, "OnScrollUpdateOver");
            }
        }
    }

    /// <summary>
    /// The default item.
    /// </summary>
    [SerializeField]
    public ListViewItem DefaultItem;

    /// <summary>
    /// The components list.
    /// </summary>
    protected List<ListViewItem> components = new List<ListViewItem>();

    /// <summary>
    /// The components cache list.
    /// </summary>
    protected List<ListViewItem> componentsCache = new List<ListViewItem>();

    Dictionary<int, UnityAction<PointerEventData>> callbacksEnter = new Dictionary<int, UnityAction<PointerEventData>>();

    Dictionary<int, UnityAction<PointerEventData>> callbacksExit = new Dictionary<int, UnityAction<PointerEventData>>();

    /// <summary>
    /// The ScrollRect.
    /// </summary>
    [SerializeField]
    protected ScrollRect scrollRect;

    /// <summary>
    /// Gets or sets the ScrollRect.
    /// </summary>
    /// <value>The ScrollRect.</value>
    public ScrollRect ScrollRect
    {
        get { return scrollRect; }
        set
        {
            if (scrollRect != null)
            {
                var r = scrollRect.GetComponent<ResizeListener>();
                if (r != null)
                {
                    r.OnResize.RemoveListener(SetNeedResize);
                }
                scrollRect.onValueChanged.RemoveListener(OnScrollRectUpdate);
            }
            scrollRect = value;
            if (scrollRect != null)
            {
                var r = scrollRect.GetComponent<ResizeListener>() ??
                        scrollRect.gameObject.AddComponent<ResizeListener>();
                r.OnResize.AddListener(SetNeedResize);

                scrollRect.onValueChanged.AddListener(OnScrollRectUpdate);
            }
        }
    }

    /// <summary>
    /// The height of the DefaultItem.
    /// </summary>
    protected float itemHeight;

    /// <summary>
    /// The width of the DefaultItem.
    /// </summary>
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
    /// The height of the content.
    /// </summary>
    protected float contentHeight;

    /// <summary>
    /// The width of the content.
    /// </summary>
    protected float contentWidth;

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
    public ListViewDirection Direction
    {
        get { return direction; }
        set
        {
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

    bool isStartedListViewCustom = false;

    /// <summary>
    /// The layout.
    /// </summary>
    protected LayoutGroup layout;

    /// <summary>
    /// LayoutBridge.
    /// </summary>
    protected ILayoutBridge LayoutBridge;

    /// <summary>
    /// Start this instance.
    /// </summary>
    private void InitStart()
    {
        if (isStartedListViewCustom)
        {
            return;
        }

        base.Start();
        base.Items = new List<ListViewItem>();

        SetItemIndicies = false;

        DestroyGameObjects = false;

        if (DefaultItem == null)
        {
            throw new NullReferenceException(
                String.Format("DefaultItem is null. Set component of type {0} to DefaultItem.",
                    typeof(ListViewItem).FullName));
        }
        DefaultItem.gameObject.SetActive(true);


        if (CanOptimize())
        {
            ScrollRect = scrollRect;

            var scroll_rect_transform = scrollRect.transform as RectTransform;
            scrollHeight = scroll_rect_transform.rect.height;
            scrollWidth = scroll_rect_transform.rect.width;

            var content_rect_transform = Container.transform as RectTransform;
            contentHeight = content_rect_transform.rect.height;
            contentWidth = content_rect_transform.rect.width;

            layout = Container.GetComponent<LayoutGroup>();

            if (layout is EasyLayout.EasyLayout)
            {
                LayoutBridge = new EasyLayoutBridge(layout as EasyLayout.EasyLayout,
                    DefaultItem.transform as RectTransform);
                LayoutBridge.IsHorizontal = IsHorizontal();
            }
            else if (layout is HorizontalOrVerticalLayoutGroup)
            {
                LayoutBridge = new StandardLayoutBridge(layout as HorizontalOrVerticalLayoutGroup,
                    DefaultItem.transform as RectTransform);
            }

            CalculateItemSize();
            CalculateMaxVisibleItems();

            var r = scrollRect.gameObject.GetOrAddComponent<ResizeListener>();
            r.OnResize.AddListener(SetNeedResize);
        }

        DefaultItem.gameObject.SetActive(false);

        Direction = direction;

        UpdateItems();

        isStartedListViewCustom = true;
    }

    /// <summary>
    /// Calculates the size of the item.
    /// </summary>
    protected virtual void CalculateItemSize()
    {
        if (LayoutBridge == null)
        {
            return;
        }
        var size = LayoutBridge.GetItemSize();
        if (itemHeight == 0f)
        {
            itemHeight = size.y;
        }
        if (itemWidth == 0f)
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
        return direction == ListViewDirection.Horizontal;
    }

    /// <summary>
    /// Resize this instance.
    /// </summary>
    public virtual void Resize()
    {
        needResize = false;
        if (!CanOptimize()) return;

        var scroll_rect_transform = scrollRect.transform as RectTransform;
        scrollHeight = scroll_rect_transform.rect.height;
        scrollWidth = scroll_rect_transform.rect.width;

        var content_rect_transform = Container.transform as RectTransform;
        contentHeight = content_rect_transform.rect.height;
        contentWidth = content_rect_transform.rect.width;

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
        var scrollRectSpecified = scrollRect != null;
        var containerSpecified = Container != null;
        var currentLayout = containerSpecified ? (layout ?? Container.GetComponent<LayoutGroup>()) : null;
        var validLayout = currentLayout is EasyLayout.EasyLayout;

        return scrollRectSpecified && validLayout;
    }

    /// <summary>
    /// Clear items of this instance.
    /// </summary>
    public override void Clear()
    {
        SetScrollValue(0f);
    }

    /// <summary>
    /// Sets the scroll value.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <param name="callScrollUpdate">Call ScrollUpdate() if position changed.</param>
    public void SetScrollValue(float value, bool callScrollUpdate = true)
    {
        if (scrollRect == null || scrollRect.content == null)
        {
            return;
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

        return IsHorizontal() ? Mathf.Clamp(-pos.x,0f,scrollRect.content.GetWidth() - scrollRect.GetComponent<RectTransform>().GetWidth()) : Mathf.Clamp(pos.y,0f, scrollRect.content.GetHeight() - scrollRect.GetComponent<RectTransform>().GetHeight());
    }

    /// <summary>
    /// Scrolls to item with specifid index.
    /// </summary>
    /// <param name="index">Index.</param>
    protected override void ScrollTo(int index)
    {
        if (!CanOptimize())
        {
            return;
        }

        var first_visible = GetFirstVisibleIndex(true);
        var last_visible = GetLastVisibleIndex(true);

        var block_index = Mathf.FloorToInt((float)index / (float)ItemsPerBlock());
        if (first_visible > block_index)
        {
            SetScrollValue(GetItemPosition(index));
        }
        else if (last_visible < block_index)
        {
            SetScrollValue(GetItemPositionBottom(index));
        }
    }

    public void LazyScrollTo(int index) {
        ScrollTo(index);
    }

    public void CenterScrollTo(int index, float scrollDelay = -1) {
        if (!CanOptimize()) {
            return;
        }

        if (IsHorizontal()) {
            Debug.LogError("暂不支持！");
            ScrollTo(index);
            return;
        }
        
        float height = scrollHeight + LayoutBridge.GetSpacing() - LayoutBridge.GetFullMargin();
        float centerPos = -height / 2 + GetItemSize() / 2 + GetItemSize() * index;
        if (scrollDelay < 0) {
            scrollDelay = EndScrollDelay;
            if (Mathf.Approximately(scrollRect.content.anchoredPosition.y, centerPos)) return;
            DOTween.To(() => scrollRect.content.anchoredPosition.y, value => SetScrollValue(value), centerPos, scrollDelay);
        }
        else {
            scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x, centerPos);

            RectTransform content_rect_transform = Container.transform as RectTransform;
            contentHeight = content_rect_transform.rect.height;
            if (contentHeight < 1.0f) {
                if (LayoutBridge != null)
                {
                    LayoutBridge.SetFiller(CalculateTopFillerSize(), CalculateBottomFillerSize());
                    LayoutBridge.UpdateLayout();
                    if (layout is EasyLayout.EasyLayout) {
                        content_rect_transform.SetHeight((layout as EasyLayout.EasyLayout).BlockSize.y);
                    }
                }
            }

            ScrollUpdate();
        }
    }
    
    // [ContextMenu("GetCenterIndex")]
    public int GetCenterIndex() {
        if (IsHorizontal()) {
            Debug.LogError("暂不支持！");
            return 0;
        }
        
        float currentHeight = scrollRect.content.anchoredPosition.y;
        float height = scrollHeight + LayoutBridge.GetSpacing() - LayoutBridge.GetFullMargin();
        float index = (currentHeight - (-height / 2 + GetItemSize() / 2)) / GetItemSize();
        
        return Mathf.Clamp(Convert.ToInt32(index), 0, dataSourceCount - 1);
    }

    private bool enableCenterOnChild = false;

    [ContextMenu("EnableCenterOnChild")]
    public void EnableCenterOnChild() {
        enableCenterOnChild = true;
        DragListener dagListener = scrollRect.GetOrAddComponent<DragListener>();
        dagListener.BeginDragEvent = () => DOTween.KillAll();
        dagListener.EndDragEvent = () => CenterScrollTo(GetCenterIndex());
    }

    int itemsPerRow;
    int itemsPerColumn;
    int ItemsPerBlock()
    {
        return IsHorizontal() ? itemsPerColumn : itemsPerRow;
    }

    /// <summary>
    /// Gets the item position.
    /// </summary>
    /// <returns>The item position.</returns>
    /// <param name="index">Index.</param>
    protected float GetItemPosition(int index)
    {
        var block_index = Mathf.FloorToInt((float)index / (float)ItemsPerBlock());
        return block_index * GetItemSize();
    }

    /// <summary>
    /// Calculates the max count of visible items.
    /// </summary>
    protected void CalculateMaxVisibleItems()
    {
        var spacing = LayoutBridge.GetSpacing();
        var fullMargin = LayoutBridge.GetFullMargin();

        EasyLayoutBridge elb = LayoutBridge as EasyLayoutBridge;
        if (null != elb)
        {
            spacing = IsHorizontal() ? elb.GetSpacingY():elb.
            GetSpacingX();

            fullMargin = IsHorizontal() ? elb.GetFullMarginY() : elb.
            GetFullMarginX();
        }

        if (IsHorizontal())
        {
            var height = contentHeight + spacing - fullMargin;

            itemsPerRow = Mathf.CeilToInt(scrollWidth / itemWidth) + 1;
            itemsPerRow = Mathf.Max(2, itemsPerRow);

            itemsPerColumn = Mathf.FloorToInt(height / (itemHeight + spacing));
            itemsPerColumn = Mathf.Max(1, itemsPerColumn);
        }
        else
        {
            var width = contentWidth + spacing - fullMargin;

            itemsPerRow = Mathf.FloorToInt(width / (itemWidth + spacing));
            itemsPerRow = Mathf.Max(1, itemsPerRow);

            itemsPerColumn = Mathf.CeilToInt(scrollHeight / itemHeight) + 1;
            itemsPerColumn = Mathf.Max(2, itemsPerColumn);
        }

        maxVisibleItems = itemsPerRow * itemsPerColumn;
    }

    /// <summary>
    /// Gets the item position bottom.
    /// </summary>
    /// <returns>The item position bottom.</returns>
    /// <param name="index">Index.</param>
    protected virtual float GetItemPositionBottom(int index)
    {
        return GetItemPosition(index) + GetItemSize() - LayoutBridge.GetSpacing() + LayoutBridge.GetMargin() -
               GetScrollSize();
    }

    /// <summary>
    /// Removes the callbacks.
    /// </summary>
    protected void RemoveCallbacks()
    {
        base.Items.ForEach(RemoveCallback);
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
            if (item != null)
            {
                item.onPointerEnter.RemoveListener(callbacksEnter[index]);
            }
            callbacksEnter.Remove(index);
        }
        if (callbacksExit.ContainsKey(index))
        {
            if (item != null)
            {
                item.onPointerExit.RemoveListener(callbacksExit[index]);
            }
            callbacksExit.Remove(index);
        }
    }

    /// <summary>
    /// Updates the components count.
    /// </summary>
    protected void UpdateComponentsCount()
    {
        components.RemoveAll(IsNullComponent);

        if (components.Count != visibleItems)
        {
            if (components.Count < visibleItems)
            {
                componentsCache.RemoveAll(IsNullComponent);

                Enumerable.Range(0, visibleItems - components.Count).ForEach(AddComponent);
            }
            else
            {
                var to_cache =
                    components.GetRange(visibleItems, components.Count - visibleItems)
                        .OrderByDescending<ListViewItem, int>(GetComponentIndex);

                to_cache.ForEach(DeactivateComponent);
                componentsCache.AddRange(to_cache);
                components.RemoveRange(visibleItems, components.Count - visibleItems);
            }

            base.Items = components.Convert(x => x as ListViewItem);
        }

        var indicies = Enumerable.Range(topHiddenItems, visibleItems).ToArray();

        // 将设置数据的部分移到Lua端
        components.ForEach((x, i) =>
        {
            x.Index = indicies[i];
        });

        // 通知Lua端更新
        RefreshView();
    }

    bool IsNullComponent(ListViewItem component)
    {
        return component == null;
    }

    void AddComponent(int index)
    {
        ListViewItem component;
        if (componentsCache.Count > 0)
        {
            component = componentsCache[componentsCache.Count - 1];
            componentsCache.RemoveAt(componentsCache.Count - 1);
        }
        else
        {
            component = Instantiate(DefaultItem) as ListViewItem;
            Utilites.FixInstantiated(DefaultItem, component);
        }
        component.Index = -1;
        component.transform.SetAsLastSibling();
        component.gameObject.SetActive(true);
        components.Add(component);
    }

    void DeactivateComponent(ListViewItem component)
    {
        RemoveCallback(component, component.Index);
        if (component != null)
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
    protected virtual int BaseGetLastVisibleIndex(bool strict = false)
    {
        var window = GetScrollValue() + GetScrollSize();
        var last_visible_index = (strict)
            ? Mathf.FloorToInt(window / GetItemSize())
            : Mathf.CeilToInt(window / GetItemSize());

        return last_visible_index - 1;
    }

    protected int GetLastVisibleIndex(bool strict = false)
    {
        return (BaseGetLastVisibleIndex(strict) + 1) * ItemsPerBlock() - 1;
    }

    /// <summary>
    /// Gets the first index of the visible.
    /// </summary>
    /// <returns>The first visible index.</returns>
    /// <param name="strict">If set to <c>true</c> strict.</param>
    protected virtual int BaseGetFirstVisibleIndex(bool strict = false)
    {
        var first_visible_index = (strict)
            ? Mathf.CeilToInt(GetScrollValue() / GetItemSize())
            : Mathf.FloorToInt(GetScrollValue() / GetItemSize());
        first_visible_index = Mathf.Max(0, first_visible_index);
        if (strict)
        {
            return first_visible_index;
        }

        return Mathf.Min(first_visible_index, Mathf.Max(0, dataSourceCount - visibleItems));
    }

    protected int GetFirstVisibleIndex(bool strict = false)
    {
        return Mathf.Max(0, BaseGetFirstVisibleIndex(strict) * ItemsPerBlock());
    }

    [ContextMenu("ScrollUpdate")]
    protected void ScrollUpdate()
    {
        var oldTopHiddenItems = topHiddenItems;

        topHiddenItems = GetFirstVisibleIndex();
        if (topHiddenItems > (dataSourceCount - 1))
        {
            topHiddenItems = Mathf.Max(0, dataSourceCount - 2);
        }

        if (oldTopHiddenItems == topHiddenItems)
        {
            return;
        }

        if ((CanOptimize()) && (dataSourceCount > 0))
        {
            visibleItems = (maxVisibleItems < dataSourceCount) ? maxVisibleItems : dataSourceCount;
        }
        else
        {
            visibleItems = dataSourceCount;
        }
        if ((topHiddenItems + visibleItems) > dataSourceCount)
        {
            visibleItems = dataSourceCount - topHiddenItems;
            if (visibleItems < ItemsPerBlock())
            {
                visibleItems = Mathf.Min(dataSourceCount, visibleItems + ItemsPerBlock());
                topHiddenItems = dataSourceCount - visibleItems;
            }
        }
//        Debug.Log(string.Format("GetFirstVisibleIndex :{2} GetScrollValue:{3} topHiddenItems: {0} \t visibleItems:{1}", topHiddenItems, visibleItems,
//            GetFirstVisibleIndex(), GetScrollValue()));
        RemoveCallbacks();

        UpdateComponentsCount();

        bottomHiddenItems = Mathf.Max(0, dataSourceCount - visibleItems - topHiddenItems);

        var new_visible_range = Enumerable.Range(topHiddenItems, visibleItems).ToList();
        var current_visible_range = components.Convert<ListViewItem, int>(GetComponentIndex);

        var new_indicies_to_change = new_visible_range.Except(current_visible_range).ToList();
        var components_to_change = new Stack<ListViewItem>(components.Where(x => !new_visible_range.Contains(x.Index)));

        new_indicies_to_change.ForEach(index => {
            var component = components_to_change.Pop();

            component.Index = index;
            RefreshItem(component);
        });

        components.Sort(ComponentsComparer);
        components.ForEach(SetComponentAsLastSibling);

        if (LayoutBridge != null)
        {
            LayoutBridge.SetFiller(CalculateTopFillerSize(), CalculateBottomFillerSize());
            LayoutBridge.UpdateLayout();
        }

        ScrollUpdateOver();
    }

    int GetBlocksCount(int items)
    {
        return Mathf.CeilToInt((float)items / (float)ItemsPerBlock());
    }

    /// <summary>
    /// Compare components by component index.
    /// </summary>
    /// <returns>A signed integer that indicates the relative values of x and y.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    protected int ComponentsComparer(ListViewItem x, ListViewItem y)
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

        if ((CanOptimize()) && (dataSourceCount > 0))
        {
            visibleItems = (maxVisibleItems < dataSourceCount) ? maxVisibleItems : dataSourceCount;
        }
        else
        {
            visibleItems = dataSourceCount;
        }

        if (CanOptimize())
        {
            topHiddenItems = GetFirstVisibleIndex();
            if (topHiddenItems > (dataSourceCount - 1))
            {
                topHiddenItems = Mathf.Max(0, dataSourceCount - 2);
            }
            if ((topHiddenItems + visibleItems) > dataSourceCount)
            {
                visibleItems = dataSourceCount - topHiddenItems;
            }
            bottomHiddenItems = Mathf.Max(0, dataSourceCount - visibleItems - topHiddenItems);
        }
        else
        {
            topHiddenItems = 0;
            bottomHiddenItems = dataSourceCount - visibleItems;
        }

        UpdateComponentsCount();

        if (LayoutBridge != null)
        {
            LayoutBridge.SetFiller(CalculateTopFillerSize(), CalculateBottomFillerSize());
            LayoutBridge.UpdateLayout();
        }

        if (ScrollRect != null)
        {
            var item_ends = (dataSourceCount == 0) ? 0f : Mathf.Max(0f, GetItemPositionBottom(dataSourceCount - 1));

            if (GetScrollValue() > item_ends)
            {
                SetScrollValue(item_ends);
            }
        }
    }

    /// <summary>
    /// Calculates the size of the bottom filler.
    /// </summary>
    /// <returns>The bottom filler size.</returns>
    protected virtual float CalculateBottomFillerSize()
    {
        var blocks = GetBlocksCount(bottomHiddenItems);
        if (blocks == 0)
        {
            return 0f;
        }
        return Mathf.Max(0, blocks * GetItemSize() - LayoutBridge.GetSpacing());
    }

    /// <summary>
    /// Calculates the size of the top filler.
    /// </summary>
    /// <returns>The top filler size.</returns>
    protected float CalculateTopFillerSize()
    {
        var blocks = GetBlocksCount(topHiddenItems);
        if (blocks == 0)
        {
            return 0f;
        }
        return Mathf.Max(0, GetBlocksCount(topHiddenItems) * GetItemSize() - LayoutBridge.GetSpacing());
    }

    /// <summary>
    /// Determines if item exists with the specified index.
    /// </summary>
    /// <returns><c>true</c> if item exists with the specified index; otherwise, <c>false</c>.</returns>
    /// <param name="index">Index.</param>
    public override bool IsValid(int index)
    {
        return (index >= 0) && (index < dataSourceCount);
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    protected override void OnDestroy()
    {
        layout = null;
        LayoutBridge = null;

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

    void LateUpdate()
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
        if (item != null)
        {
            LayoutRebuilder.MarkLayoutForRebuild(item.transform as RectTransform);
        }
    }

    void StartScrolling()
    {
        lastScrollingTime = Time.time;
        if (scrolling)
        {
            return;
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
            return;
        }

        needResize = true;
    }
}
