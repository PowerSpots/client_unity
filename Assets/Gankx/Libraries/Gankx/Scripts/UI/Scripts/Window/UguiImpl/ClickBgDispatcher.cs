using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gankx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Gankx.UI
{
    public class ClickBgDispatcher : EventDispatcher
    {
        PointerEventData pe;
        private List<RaycastResult> hits;
       
        protected uint otherPanelId = Window.InvalidId;
        private uint otherWindowId = Window.InvalidId;
        void Awake()
        {
            pe = new PointerEventData(EventSystem.current);
            hits = new List<RaycastResult>();
        }

        void Start()
        {
            window = Window.GetWindowInParent(gameObject);
        }
        void Update()
        {

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        if (Input.touchCount == 1)
#endif
            {
                if (Input.GetMouseButtonDown(0))
                {
                    pe.position = Input.mousePosition;
                    EventSystem.current.RaycastAll(pe, hits);
                    CheckSendPanelMessage();
                }
                   
            }
        }

        void SendOtherPanelMessage(string message, object param1 = null, object param2 = null)
        {
            if (LuaService.instance != null)
            {
                LuaService.instance.FireEvent(
                    "OnPanelMessage", otherPanelId, otherWindowId, message, param1, param2);
            }
        }

        private void CheckSendPanelMessage()
        {
            bool sendMsg = false;
            bool close = true;
            if (hits.Count > 0)
            {
                if (hits[0].gameObject == gameObject)
                {
                    if(hits.Count > 1)
                    {
                        sendMsg = CheckHit(1);
                    }
                }
                else
                {
                    close = false;
                }
            }

            if (close)
            {
                SendPanelMessage("OnRaycastTipClose");
            }
            
            if (sendMsg)
            {
                SendOtherPanelMessage("OnClick");
            }
        }

        private bool CheckHit(int index )
        {
            if (hits[index].gameObject.GetComponent<ButtonClickDispatcher>() != null)
            { 
                Window window = Window.GetWindowInParent(hits[index].gameObject);
                if (window != null)
                {
                    otherPanelId = window.panelId;
                    otherWindowId = window.id;
                    return true;
                }
            }
            return false;

        }
    }
}
