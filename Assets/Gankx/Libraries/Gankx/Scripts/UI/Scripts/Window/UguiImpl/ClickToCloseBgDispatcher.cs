using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gankx.UI
{
    public class ClickToCloseBgDispatcher : EventDispatcher
    {
        public GameObject[] m_targetPanelObj;
        public GameObject[] m_targetParentPanelObj;

        public bool bIgnoreClick = false;
        public string[] mIgnoreLayers;

        PointerEventData pe;
        private List<RaycastResult> hits;

        protected override void OnInit()
        {
            pe = new PointerEventData(EventSystem.current);
            hits = new List<RaycastResult>();
        }

        private void Start()
        {
            window = Window.GetWindowInParent(gameObject);
        }

        void LateUpdate()
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
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            if (Input.touchCount > 1)
            {
                SendPanelMessage("OnTipClose");
            }
#endif
        }

        private void CheckSendPanelMessage()
        {
            for (int i = 0; i < hits.Count; i++)
            {                                           
                if (hits[i].gameObject.CompareTag("ClickToCloseBgIgnore"))
                {
                    return;
                }
                else
                {
                    for (int j = 0; j < m_targetPanelObj.Length; j++)
                    {
                        if ((m_targetPanelObj[j] != null && hits[i].gameObject == m_targetPanelObj[j]))
                        {
                            return;
                        }
                    }

                    for (int j = 0; j < mIgnoreLayers.Length; j++)
                    {
                        if (LayerMask.LayerToName(hits[i].gameObject.layer) == mIgnoreLayers[j])
                        {
                            return;
                        }
                    }

                    for (int j = 0; j < m_targetParentPanelObj.Length; j++)
                    {
                        if (m_targetParentPanelObj[j] != null && hits[i].gameObject.transform.IsChildOf(m_targetParentPanelObj[j].transform))
                        {
                            return;
                        }
                    }
                }
            }

            if (bIgnoreClick)
            {
                for (int i = 0; i < hits.Count; i++)
                {
                    if (hits[i].gameObject.GetComponentInParent(typeof(PointerClickDispatcher)))
                    {
                        return;
                    }

                    if (hits[i].gameObject.GetComponentInParent(typeof(ButtonClickDispatcher)))
                    {
                        return;
                    }
                }
            }

            SendPanelMessage("OnTipClose");
        }

    }
}

