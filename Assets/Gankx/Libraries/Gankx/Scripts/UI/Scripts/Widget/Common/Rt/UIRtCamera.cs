using System.Collections;
using System.Collections.Generic;
using Gankx;
using Gankx.UI;
using UnityEngine;
using UnityEngine.Profiling;

public class UIRtCamera : MonoBehaviour
{
    public PanelLayerType panelLayerType = PanelLayerType.Foreground;
    public int rtPanelSortOrder = -1;
    public string rtPanelPath = "ui/panel/rtPanel";

    private GameObject rtPanelObject;
    private Camera camera;

    private bool isOn;

    private void CacheCamera()
    {
        if (null == camera)
        {
            camera = GetComponent<Camera>();
        }        
    }

    void CheckCamera()
    {
        // TODO GameEditorUtility
        if (false/*GameEditorUtility.InEditor*/)
        {
            return;
        }

        CacheCamera();
        if (null == camera)
        {
            return;
        }

        if (!isOn && camera.enabled && gameObject.activeInHierarchy)
        {
            isOn = true;
            PanelLayer panelLayer = PanelService.instance.GetOrAddPanelLayer(panelLayerType);
            if (null == panelLayer)
            {
                return;
            }

            if (null == rtPanelObject)
            {
                // 从Prefab中加载Panel
                GameObject panelPrefab = ResourceService.Load<GameObject>(rtPanelPath);
                if (null == panelPrefab)
                {
                    Debug.LogError(string.Format("Resource目录{0}中不存在Panel，请确认！", rtPanelPath));
                    return;
                }

                // 向panel组里实例化并加入该panel
                rtPanelObject = panelLayer.AddPanel(panelPrefab, rtPanelSortOrder, 0);
                if (null == rtPanelObject)
                {
                    Debug.LogError(string.Format("PanelGroup.AddPanel失败！{0}", rtPanelPath));
                    return;
                }
            }

            IUIRtHandler[] handlers = rtPanelObject.GetComponentsInChildren<IUIRtHandler>(true);
            for (int i = 0; i < handlers.Length; i++)
            {
                handlers[i].InitRt(camera);
            }

            rtPanelObject.SetActive(true);
        }
        else if (isOn && (!camera.enabled || !gameObject.activeInHierarchy))
        {
            isOn = false;
            if (null == rtPanelObject)
            {
                return;
            }

            IUIRtHandler[] handlers = rtPanelObject.GetComponentsInChildren<IUIRtHandler>(true);
            for (int i = 0; i < handlers.Length; i++)
            {
                handlers[i].UninitRt(camera);
            }

            rtPanelObject.SetActive(false);
        }
    }

    private void Update()
    {
        CheckCamera();
    }

    private void OnEnable()
    {
        CheckCamera();
    }

    private void OnDisable()
    {
        CheckCamera();
    }

    private void OnDestroy()
    {
        // TODO GameEditorUtility
        if (false/*GameEditorUtility.InEditor*/)
        {
            return;
        }

        if (null == rtPanelObject)
        {
            return;
        }

        GameObject.Destroy(rtPanelObject);
    }
}
