using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gankx.UI
{
    public class EventDispatcherService : Singleton<EventDispatcherService>
    {
        private readonly Dictionary<string, Type> myEventDispatcherDict = new Dictionary<string, Type>();

        protected override void OnInit()
        {
            myEventDispatcherDict.Add("OnClick", typeof(ButtonClickDispatcher));
            myEventDispatcherDict.Add("OnButtonPointerClick", typeof(ButtonPointerClick));
            myEventDispatcherDict.Add("OnPointerClick", typeof(PointerClickDispatcher));
            myEventDispatcherDict.Add("OnPointerDown", typeof(PointerDownDispatcher));
            myEventDispatcherDict.Add("OnPointerUp", typeof(PointerUpDispatcher));
            myEventDispatcherDict.Add("OnPointerEnter", typeof(PointerEnterDispatcher));
            myEventDispatcherDict.Add("OnPointerExit", typeof(PointerExitDispatcher));

            myEventDispatcherDict.Add("OnScrollbarValueChanged", typeof(ScrollbarChangeDispatcher));
            myEventDispatcherDict.Add("OnScrollrectValueChanged", typeof(ScrollRectChangeDispatcher));
            myEventDispatcherDict.Add("OnDropdownValueChanged", typeof(DropdownChangeDispatcher));
            myEventDispatcherDict.Add("OnSliderValueChanged", typeof(SliderChangeDispatcher));
            myEventDispatcherDict.Add("OnToggleValueChanged", typeof(ToggleChangeDispatcher));
            myEventDispatcherDict.Add("OnInputTextValueChanged", typeof(InputTextChangeDispatcher));
            myEventDispatcherDict.Add("OnInputTextEndEdit", typeof(InputTextEndEditDispatcher));


            myEventDispatcherDict.Add("OnLongPress", typeof(LongPressDispatcher));
            myEventDispatcherDict.Add("OnLongPressStay", typeof(LongPressDispatcher));
            myEventDispatcherDict.Add("OnLongRelease", typeof(LongPressDispatcher));

            myEventDispatcherDict.Add("OnDragToBottom", typeof(ScrollRectDragDispatcher));
            myEventDispatcherDict.Add("OnDragToTop", typeof(ScrollRectDragDispatcher));

            myEventDispatcherDict.Add("OnPlayTweenFinish", typeof(PlayTweenDispatcher));
            myEventDispatcherDict.Add("OnTweenerFinish", typeof(TweenerDispatcher));

            myEventDispatcherDict.Add("OnHrefClick", typeof(TextHrefLinkClickDispatcher));
            myEventDispatcherDict.Add("OnSelect", typeof(OnSelectDispatcher));
            myEventDispatcherDict.Add("OnDeselect", typeof(OnDeselectDispatcher));

            myEventDispatcherDict.Add("OnShake", typeof(ShakeDispatcher));

            myEventDispatcherDict.Add("OnTipClose", typeof(ClickToCloseBgDispatcher));
            myEventDispatcherDict.Add("OnRaycastTipClose", typeof(ClickBgDispatcher));

            myEventDispatcherDict.Add("OnSpriteLoaded", typeof(DynamicImageSpriteLoadedDispatcher));
            myEventDispatcherDict.Add("OnUrlTextureLoaded", typeof(RawImageUrlTextureLoadedDispatcher));

            myEventDispatcherDict.Add("OnDrag", typeof(DragDispatcher));
            myEventDispatcherDict.Add("OnBeginDrag", typeof(DragDispatcher));
            myEventDispatcherDict.Add("OnEndDrag", typeof(DragDispatcher));
        }

        public List<string> GetPredefinedEvents()
        {
            var eventNames = new List<string>();
            foreach (var pair in myEventDispatcherDict)
            {
                eventNames.Add(pair.Key);
            }

            return eventNames;
        }

        public void BindEvent(GameObject windowObject, string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return;
            }

            Type type;
            if (myEventDispatcherDict.TryGetValue(eventName, out type) && null == windowObject.GetComponent(type))
            {
                windowObject.AddComponent(type);
            }
        }
    }
}