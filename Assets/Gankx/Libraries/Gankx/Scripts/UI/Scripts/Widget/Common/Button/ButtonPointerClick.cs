using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// ReSharper disable once CheckNamespace
namespace Gankx.UI
{
    /// <summary>
    ///  监听SlotControl 的OnPointerClick
    /// </summary>
    public class ButtonPointerClick : EventDispatcher
    {
        private PointerClickDispatcher _pointer;
        private Button _button;
        protected override void OnInit()
        {
            _pointer = GetComponent<PointerClickDispatcher>() ?? AddComponent<PointerClickDispatcher>();

            if (null == _pointer)
            {
                Debug.LogError("PointerClickDispatcher is null!!");
                return;
            }

            _button = GetComponent<Button>();

            _pointer.ShouldBlockClick = OnButtonClick;
        }

        public bool OnButtonClick(PointerEventData data)
        {
            if (null != _button && _button.interactable == false)
            {
                SendPanelMessage("OnButtonPointerClick");
            }

            return true;
//            SendPanelMessage("OnPostSound");
        }
    }
}

