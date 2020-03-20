using Gankx.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Tweener = DG.Tweening.Tweener;

public static class UIScrollViewExport
{
    public static float GetViewHeight(uint windowId)
    {
        ScrollRect scrollRect = PanelService.GetWindowComponent<ScrollRect>(windowId);
        if (null == scrollRect)
        {
            return 0;
        }

        RectTransform viewport = scrollRect.viewport;
        if (null == viewport)
        {
            viewport = scrollRect.GetComponent<RectTransform>();
        }

        if (null == viewport)
        {
            return 0;
        }

        return viewport.rect.height;
    }

    public static float GetViewWidth(uint windowId)
    {
        ScrollRect scrollRect = PanelService.GetWindowComponent<ScrollRect>(windowId);
        if (null == scrollRect)
        {
            return 0;
        }

        RectTransform viewport = scrollRect.viewport;
        if (null == viewport)
        {
            viewport = scrollRect.GetComponent<RectTransform>();
        }

        if (null == viewport)
        {
            return 0;
        }

        return viewport.rect.width;
    }

    public static void ResetPosition(uint windowId)
    {
        ScrollRect scrollRect = PanelService.GetWindowComponent<ScrollRect>(windowId);
        if (null == scrollRect)
        {
            return;
        }

        scrollRect.horizontalNormalizedPosition = 0;
        scrollRect.verticalNormalizedPosition = 0;
    }

    /// <summary>
    /// 设置ScrollView的显示位置，可以配合ScrollBar使用
    /// </summary>
    public static void SetPosition(uint windowId, float x, float y)
    {
        ScrollRect scrollRect = PanelService.GetWindowComponent<ScrollRect>(windowId);
        if (null == scrollRect)
        {
            return;
        }

        scrollRect.normalizedPosition = new Vector2(x, y);
    }

    public static float GetPositionX(uint windowId)
    {
        ScrollRect scrollRect = PanelService.GetWindowComponent<ScrollRect>(windowId);
        if (null == scrollRect)
        {
            return 0;
        }

        return scrollRect.normalizedPosition.x;
    }


    public static float GetPositionY(uint windowId)
    {
        ScrollRect scrollRect = PanelService.GetWindowComponent<ScrollRect>(windowId);
        if (null == scrollRect)
        {
            return 0;
        }

        return scrollRect.normalizedPosition.y;
    }
    /// <summary>
    /// 设置滚动速度
    /// </summary>
    public static void SetMomentumAmount(uint windowId, float momentumAmount)
    {
        ScrollRect scrollRect = PanelService.GetWindowComponent<ScrollRect>(windowId);
        if (null == scrollRect)
        {
            return;
        }

        scrollRect.scrollSensitivity = momentumAmount;
    }

    public static void SetLayoutVertical(uint windowId)
    {
        ScrollRect scrollRect = PanelService.GetWindowComponent<ScrollRect>(windowId);
        if (null == scrollRect)
        {
            return;
        }

        scrollRect.SetLayoutVertical();
    }

    public static void SetLayoutHorizontal(uint windowId)
    {
        ScrollRect scrollRect = PanelService.GetWindowComponent<ScrollRect>(windowId);
        if (null == scrollRect)
        {
            return;
        }

        scrollRect.SetLayoutHorizontal();
    }

    public static void StopMovement(uint windowId)
    {
        ScrollRect scrollRect = PanelService.GetWindowComponent<ScrollRect>(windowId);
        if (null == scrollRect)
        {
            return;
        }

        scrollRect.StopMovement();
    }

    public static Tweener MoveToPositionX(uint windowId, float x, float dur)
    {
        var scrollRect = PanelService.GetWindowComponent<ScrollRect>(windowId);
        return null == scrollRect ? null : scrollRect.DOHorizontalNormalizedPos(x, dur);
    }

    public static Tweener MoveToPositionY(uint windowId, float y, float dur)
    {
        var scrollRect = PanelService.GetWindowComponent<ScrollRect>(windowId);
        return null == scrollRect ? null : scrollRect.DOVerticalNormalizedPos(y, dur);
    }

    public static void SetEnable(uint windowId, bool state)
    {
        ScrollRect scrollRect = PanelService.GetWindowComponent<ScrollRect>(windowId);
        if (null == scrollRect)
        {
            return;
        }

        scrollRect.enabled = state; 
    }
}