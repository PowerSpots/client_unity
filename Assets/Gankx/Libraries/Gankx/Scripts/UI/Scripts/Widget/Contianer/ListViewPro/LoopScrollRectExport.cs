using System;
using System.Linq;
using System.Text;
using Gankx.UI;
using UnityEngine;
using UnityEngine.UI;

internal class LoopScrollRectExport
{
    public static void Init(uint windowId)
    {
        LoopScrollRect loopScrollRect = PanelService.GetWindowComponent<LoopScrollRect>(windowId);
        if (loopScrollRect == null)
        {
            return;
        }
        loopScrollRect.prefabSource.InitPool();
    }

    public static void SetListData(uint windowId , int totalCount ,bool isUpdateUI = true , bool topOrBottom = true)
    {
        LoopScrollRect loopScrollRect = PanelService.GetWindowComponent<LoopScrollRect>(windowId);
        if (loopScrollRect == null)
        {
            return;
        }

        loopScrollRect.totalCount = totalCount;

        if (isUpdateUI == true)
        {
            if (topOrBottom == true)
            {
                loopScrollRect.RefillCells();
            }
            else
            {
                loopScrollRect.RefillCellsFromEnd();
            }
        }
    }

    // 刷新显示区间内数据
    public static void RefreshCells(uint windowId)
    {
        LoopScrollRect loopScrollRect = PanelService.GetWindowComponent<LoopScrollRect>(windowId);
        if (loopScrollRect == null)
        {
            return;
        }
        loopScrollRect.RefreshCells();
    }


    public static void RefreshCellsWithEndData(uint windowId)
    {
        LoopScrollRect loopScrollRect = PanelService.GetWindowComponent<LoopScrollRect>(windowId);
        if (loopScrollRect == null)
        {
            return;
        }
        loopScrollRect.RefreshCellsFromEnd();
    }

    public static void RefreshCellsWithEndDataUsingCor(uint windowId)
    {
        LoopScrollRect loopScrollRect = PanelService.GetWindowComponent<LoopScrollRect>(windowId);
        if (loopScrollRect == null)
        {
            return;
        }
        //        loopScrollRect.RefreshCellsFromEnd();
        loopScrollRect.RefreshCellsFromEndFraming();
    }

    public static void RefreshCell(uint windowId, uint dataIndex)
    {
        LoopScrollRect loopScrollRect = PanelService.GetWindowComponent<LoopScrollRect>(windowId);
        if (loopScrollRect == null)
        {
            return;
        }

        
    }

    // 模拟滚动到初始位置
    public static void ScrollToBegin(uint windowId)
    {
        LoopScrollRect loopScrollRect = PanelService.GetWindowComponent<LoopScrollRect>(windowId);
        if (loopScrollRect == null)
        {
            return;
        }

        loopScrollRect.RefillCells();
    }

    // 模拟滚动到最后的位置
    public static void ScrollToEnd(uint windowId)
    {
        LoopScrollRect loopScrollRect = PanelService.GetWindowComponent<LoopScrollRect>(windowId);
        if (loopScrollRect == null)
        {
            return;
        }

        loopScrollRect.RefillCellsFromEnd();
    }

    // 滚动到第N个元素
    public static void ScrollTo(uint windowId,  uint dataIndex)
    {
        LoopScrollRect loopScrollRect = PanelService.GetWindowComponent<LoopScrollRect>(windowId);
        if (loopScrollRect == null)
        {
            return;
        }

        loopScrollRect.RefillCells((int)dataIndex);
    }

    // 当前显示元素的开始区间
    public static int GetCurBeginIndex(uint windowId)
    {
        LoopScrollRect loopScrollRect = PanelService.GetWindowComponent<LoopScrollRect>(windowId);
        if (loopScrollRect == null)
        {
            return -1;
        }

        return loopScrollRect.CurItemBeginIndex;
    }

    // 当前显示元素的结束区间
    public static int GetCurEndIndex(uint windowId)
    {
        LoopScrollRect loopScrollRect = PanelService.GetWindowComponent<LoopScrollRect>(windowId);
        if (loopScrollRect == null)
        {
            return -1;
        }

        return loopScrollRect.CurItemEndIndex;
    }


    public static float GetScrollValueX(uint windowId)
    {
        LoopScrollRect scrollRect = PanelService.GetWindowComponent<LoopScrollRect>(windowId);
        if (null == scrollRect)
        {
            return 0;
        }

        return scrollRect.normalizedPosition.x;
    }

    public static float GetScrollValueY(uint windowId)
    {
        LoopScrollRect scrollRect = PanelService.GetWindowComponent<LoopScrollRect>(windowId);
        if (null == scrollRect)
        {
            return 0;
        }

        return scrollRect.normalizedPosition.y;
    }


    public static void SetNormalizedPositionX(uint windowId , float x)
    {
        LoopScrollRect scrollRect = PanelService.GetWindowComponent<LoopScrollRect>(windowId);
        if (null == scrollRect)
        {
            return;
        }

        scrollRect.SetNormalizedPosition(x,0);
    }

    public static void SetNormalizedPositionY(uint windowId, float value)
    {
        LoopScrollRect scrollRect = PanelService.GetWindowComponent<LoopScrollRect>(windowId);
        if (null == scrollRect)
        {
            return;
        }

        scrollRect.SetNormalizedPosition(value, 1);
    }
}

