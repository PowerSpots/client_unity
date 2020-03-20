using Gankx.UI;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectDragDispatcher : EventDispatcher, IEndDragHandler
{
    private ScrollRect scrollRect;
    private float threshold = 0.01f;

    protected override void OnInit()
    {
        scrollRect = GetComponent<ScrollRect>();
        if (null == scrollRect)
        {
            Debug.LogError("No ScrollRect Component is found!!!");
            return;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (null == scrollRect)
        {
            return;
        }

        if(CheckReachBottom())
            SendPanelMessage("OnDragToBottom");
        else if(CheckReachTop())
            SendPanelMessage("OnDragToTop");
    }

    private bool CheckReachBottom()
    {
        bool isVertBottom = true;
        bool isHoriBottom = true;

        if (scrollRect.vertical)
        {
            isVertBottom = (Math.Max(0, scrollRect.normalizedPosition.y) - 0) <= threshold;
        }
        if (scrollRect.horizontal)
        {
            isHoriBottom = (Math.Max(0, scrollRect.normalizedPosition.x) - 0) <= threshold;
        }

        return isVertBottom && isHoriBottom;
    }

    private bool CheckReachTop()
    {
        bool isVertTop = true;
        bool isHoriTop = true;

        if (scrollRect.vertical)
        {
            isVertTop = (1 - Math.Min(1, scrollRect.normalizedPosition.y)) <= threshold;
        }
        if (scrollRect.horizontal)
        {
            isHoriTop = (1 - Math.Min(1, scrollRect.normalizedPosition.x)) <= threshold;
        }

        return isVertTop && isHoriTop;
    }
}
