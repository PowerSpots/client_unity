﻿using UnityEngine.UI;

public class ScrollCircle : ScrollRect
{
    protected float mRadius = 0f;

    protected override void Start()
    {
        base.Start();
        //计算摇杆块的半径
        mRadius = viewport.sizeDelta.x * 0.5f;
    }

    public override void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        base.OnDrag(eventData);
        var contentPostion = this.content.anchoredPosition;
        if (contentPostion.magnitude > mRadius)
        {
            contentPostion = contentPostion.normalized * mRadius;
            SetContentAnchoredPosition(contentPostion);
        }
    }
}