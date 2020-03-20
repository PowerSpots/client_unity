using System.Collections;
using System.Collections.Generic;
using Gankx.UI;
using UnityEngine;
using UnityEngine.UI;

public static class GridLayoutGroupExport {
    public static void SetCellSizeX(uint windowId, float width) {
        GridLayoutGroup layoutGroup = PanelService.GetWindowComponent<GridLayoutGroup>(windowId);
        if (null == layoutGroup) {
            return;
        }

        layoutGroup.cellSize = new Vector2(width, layoutGroup.cellSize.y);
    }

    public static void SetCellSizeY(uint windowId, float height) {
        GridLayoutGroup layoutGroup = PanelService.GetWindowComponent<GridLayoutGroup>(windowId);
        if (null == layoutGroup) {
            return;
        }

        layoutGroup.cellSize = new Vector2(layoutGroup.cellSize.x, height);
    }

    public static void SetCellSize(uint windowId, float width, float height) {
        GridLayoutGroup layoutGroup = PanelService.GetWindowComponent<GridLayoutGroup>(windowId);
        if (null == layoutGroup) {
            return;
        }

        layoutGroup.cellSize = new Vector2(width, height);
    }
}