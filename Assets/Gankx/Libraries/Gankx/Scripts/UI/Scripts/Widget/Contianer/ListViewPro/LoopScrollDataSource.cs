using UnityEngine;
using System.Collections;
using Gankx;
using Gankx.UI;

namespace UnityEngine.UI
{
    public class LoopScrollSendLuaSource 
    {
        public static readonly LoopScrollSendLuaSource Instance = new LoopScrollSendLuaSource();

        LoopScrollSendLuaSource() { }

        /// <summary>
        /// 增加或者更新Item 数据
        /// </summary>
        /// <param name="rootTransform"></param>
        /// <param name="itemTransform"></param>
        /// <param name="itemSiblingIndex"></param>
        /// <param name="dataIndex"></param>
        public void SetItemData(Transform rootTransform , Transform itemTransform, int itemSiblingIndex, int dataIndex )
        {
            if (LuaService.instance != null)
            {
                Window window = Window.GetWindow(rootTransform.gameObject);
                if (window != null)
                {
                    Window itemWindow = Window.GetWindow(itemTransform.gameObject);
                    if (itemWindow != null)
                    {
                        Profiling.Profiler.BeginSample(" LoopScrollSendLuaSource SetItemData");

                        LuaService.instance.FireEvent("OnPanelMessage", window.panelId, window.id, "OnListItemUpdate", itemWindow.id, itemSiblingIndex , dataIndex);

                        Profiling.Profiler.EndSample();
                    }
                }
            }
        }

        /// <summary>
        /// 元素返回到pool 之后，需要通知lua 更新 数据列表
        /// </summary>
        /// <param name="rootTransform"></param>
        /// <param name="itemTransform"></param>
        /// <param name="itemSiblingIndex"></param>
        /// <param name="dataIndex"></param>
        public void ReturnItemToPool(Transform rootTransform, Transform itemTransform)
        {
            if (LuaService.instance != null)
            {
                Window window = Window.GetWindow(rootTransform.gameObject);
                if (window != null)
                {
                    Window itemWindow = Window.GetWindow(itemTransform.gameObject);
                    if (itemWindow != null)
                    {
                        LuaService.instance.FireEvent("OnPanelMessage", window.panelId, window.id, "OnReturnItemToPool", itemWindow.id);
                    }
                }
            }
        }
    }
}