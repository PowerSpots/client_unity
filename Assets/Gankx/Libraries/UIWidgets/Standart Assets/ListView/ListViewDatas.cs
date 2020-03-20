using UnityEngine;

namespace UIWidgets
{

    //    [System.Serializable]
    //    /// <summary>
    //    /// ListViewIcons item description.
    //    /// </summary>
    //    public class ListViewData : ListViewIconsItemDescription
    //    {
    //        /// <summary>
    //        /// The localized Data.
    //        /// </summary>
    //        [System.NonSerialized]
    //        public object Data;
    //
    //    }

    public interface IListData
    {
        
    }


    /// <summary>
    /// ListViewIcons.
    /// </summary>
    [AddComponentMenu("UI/ListViewDatas", 252)]
    public class ListViewDatas : ListViewCustom<ListViewIconsItemComponent, IListData>
    {
        /// <summary>
        /// Awake this instance.
        /// </summary>
        protected override void Awake()
        {
            Start();
        }

        /// <summary>
        /// Start this instance.
        /// </summary>
        public override void Start()
        {
            base.Start();
        }

        public ListViewItemChanged ItemEvent = new ListViewItemChanged();
        void OnItemChangedEvent(string eventName, GameObject gameObj)
        {
            ItemEvent.Invoke(eventName, gameObj);
        }

        protected override void AddCallback(ListViewItem item, int index)
        {
            base.AddCallback(item, index);
            item.onChanged.AddListener(OnItemChangedEvent);
        }

        protected override void RemoveCallback(ListViewItem item, int index)
        {
            base.RemoveCallback(item, index);
            item.onChanged.RemoveListener(OnItemChangedEvent);
        }

//        /// <summary>
//        /// Sets component data with specified item.
//        /// </summary>
//        /// <param name="component">Component.</param>
//        /// <param name="item">Item.</param>
//        protected override void SetData(ListViewIconsItemComponent component, ListViewIconsItemDescription item)
//        {
//            component.SetData(item);
//        }
        protected override void SetData(ListViewIconsItemComponent component, IListData item)
        {
            if (component == null)
            {
                return;
            }
//            component
            component.SendMessage("SetItemData", item, SendMessageOptions.DontRequireReceiver);
//            component.SetData(item);
        }

        public void SendItemMessage(string msgName, object data)
        {
            int count = components.Count;
            for (int i = 0; i < count; i++)
            {
                components[i].gameObject.SendMessage(msgName, data);
            }
        }

        /// <summary>
        /// Set highlights colors of specified component.
        /// </summary>
        /// <param name="component">Component.</param>
        protected override void HighlightColoring(ListViewIconsItemComponent component)
        {
//            base.HighlightColoring(component);
//            component.Text.color = HighlightedColor;
        }

        /// <summary>
        /// Set select colors of specified component.
        /// </summary>
        /// <param name="component">Component.</param>
        protected override void SelectColoring(ListViewIconsItemComponent component)
        {
//            base.SelectColoring(component);
//            component.Text.color = SelectedColor;
        }
//
//        /// <summary>
//        /// Set default colors of specified component.
//        /// </summary>
//        /// <param name="component">Component.</param>
        protected override void DefaultColoring(ListViewIconsItemComponent component)
        {
//            base.DefaultColoring(component);
//            component.Text.color = DefaultColor;
        }

        protected override void SelectItem(int index)
        {
            var item = GetItem(index);
            if (null == item)
            {
                return;
            }

            item.SendMessage("OnSelectChange", true, SendMessageOptions.DontRequireReceiver);
        }

        protected override void DeselectItem(int index)
        {
            var item = GetItem(index);
            if (null == item)
            {
                return;
            }

            item.SendMessage("OnSelectChange", false, SendMessageOptions.DontRequireReceiver);
        }


#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/UI/New UI Widgets/ListViewDatas", false, 1080)]
        static void CreateObject()
        {
            Utilites.CreateWidgetFromAsset("ListViewIcons");
        }
#endif
    }
}