using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gankx.UI
{
    public enum PanelLayerType
    {
        Invalid = -999,

        Bottom = -10,
        Background = 0,
        Highlight = 89,
        Foreground = 90,
        Top = 100,
        World,
        Share,
        Foreground720,
        Afk
    }

    public class PanelLayerParam
    {
        public PanelLayerType layerType;
        public bool isOverlay = true;

        // isOverlay == false
        public string camPath;
        public string canvasPath;
        public float xOffset = 0f;

        // isOverlay == true
        public string sortingLayer;
    }

    public class PanelLayer
    {
        public const string StateChangedMessage = "PANEL_LAYER_STATE_CHANGED";
        public Canvas canvas;
        public CanvasGroup canvasGroup;
        public Camera screenSpaceCamera;
        public bool isOverlay = true;

        private Dictionary<int, Canvas> myChildCanvasDic = new Dictionary<int, Canvas>();

        private PanelLayerType myLayerType;

        private int myCullingMask = -1;

        private PanelLayer()
        {
        }

        private static PanelLayer CreateOverlay(GameObject parentObject, PanelLayerParam param, Camera overlayCamera)
        {
            var sortingLayer = param.sortingLayer;
            if (string.IsNullOrEmpty(sortingLayer))
            {
                sortingLayer = "Default";
            }

            var canvasPath = param.canvasPath;
            var layerType = param.layerType;

            var canvasPrefab = ResourceService.Load<GameObject>(canvasPath);
            if (null == canvasPrefab)
            {
                Debug.LogError(string.Format("Can not find canvas template:{0}, please confirm！", canvasPath));
                return null;
            }

            var canvasObject = UITools.AddChild(parentObject, canvasPrefab);
            canvasObject.name = string.Format("Layer_{0}", layerType.ToString());
            var canvas = canvasObject.GetComponent<Canvas>();
            if (null == canvas)
            {
                Debug.LogError(string.Format("Can not instantiate canvas template:{0}!", canvasPath));
                Object.Destroy(canvasObject);
                return null;
            }

            var canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
            canvas.worldCamera = overlayCamera;
            canvas.sortingLayerName = sortingLayer;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;

            UIScreenAdaptor.SetCanvasScaler(canvasObject);
            canvasObject.gameObject.SetActive(true);

            var layer = new PanelLayer();
            layer.myLayerType = layerType;
            layer.canvas = canvas;
            layer.canvasGroup = canvasGroup;

            return layer;
        }

        private static PanelLayer CreateNonOverlay(GameObject parentObject, PanelLayerParam param)
        {
            var camPath = param.camPath;
            var canvasPath = param.canvasPath;
            var layerType = param.layerType;

            var layer = new PanelLayer();
            layer.isOverlay = false;

            var canvasPrefab = ResourceService.Load<GameObject>(canvasPath);
            if (null == canvasPrefab)
            {
                Debug.LogError(string.Format("Can not find canvas template:{0}, please confirm！", canvasPath));

                return null;
            }

            var canvasObject = UITools.AddChild(parentObject, canvasPrefab);
            canvasObject.transform.position = new Vector3(param.xOffset, 0, 0);
            canvasObject.name = string.Format("Layer_{0}", layerType.ToString());
            var canvas = canvasObject.GetComponent<Canvas>();
            if (null == canvas)
            {
                Debug.LogError(string.Format("Can not instantiate canvas template:{0}!", canvasPath));
                Object.Destroy(canvasObject);
                return null;
            }

            canvas.sortingLayerName = "Highlight";
            canvas.sortingOrder = -1;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;

            UIScreenAdaptor.SetCanvasScaler(canvasObject);

            if (!string.IsNullOrEmpty(camPath))
            {
                var camPrefab = ResourceService.Load<GameObject>(camPath);
                if (null == camPrefab)
                {
                    Debug.LogError(string.Format("Can not find camera template:{0}, please confirm！", camPath));

                    Object.Destroy(canvasObject);
                    return null;
                }

                var camObject = UITools.AddChild(canvasObject, camPrefab);
                camObject.name = "UICamera";
                var camera = camObject.GetComponent<Camera>();
                if (null == camera)
                {
                    Debug.LogError(string.Format("Can not instantiate camera template:{0}!", camPath));
                    Object.Destroy(camObject);
                    return null;
                }

                canvas.worldCamera = camera;
                layer.screenSpaceCamera = camera;
            }

            var canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();

            layer.myLayerType = layerType;
            layer.canvas = canvas;
            layer.canvasGroup = canvasGroup;

            return layer;
        }

        public static PanelLayer Create(GameObject parentObject, PanelLayerParam param, Camera overlayCamera = null)
        {
            if (param.isOverlay)
            {
                return CreateOverlay(parentObject, param, overlayCamera);
            }

            return CreateNonOverlay(parentObject, param);
        }

        private GameObject AddPanelNonOverlay(GameObject panelPrefab, int sortOrder, int depth)
        {
            Canvas childCanvas;
            if (!myChildCanvasDic.TryGetValue(sortOrder, out childCanvas))
            {
                var canvasObject = new GameObject(string.Format("Canvas({0})", sortOrder));
                canvasObject.transform.parent = canvas.transform;
                canvasObject.transform.localPosition = Vector3.zero;
                canvasObject.transform.localRotation = Quaternion.identity;
                canvasObject.transform.localScale = Vector3.one;
                childCanvas = canvasObject.AddComponent<Canvas>();
                var rc = childCanvas.GetComponent<RectTransform>();
                rc.anchorMin = PanelService.SafeAreaRect.min;
                rc.anchorMax = PanelService.SafeAreaRect.size;
                rc.anchoredPosition = Vector2.zero;
                rc.sizeDelta = Vector2.zero;
                childCanvas.overrideSorting = true;
                childCanvas.sortingLayerName = "Highlight";
                childCanvas.sortingOrder = sortOrder - 1;

                if (myLayerType == PanelLayerType.World)
                {
                    childCanvas.gameObject.AddComponent<CustomGraphicRaycaster>();
                }
                else
                {
                    childCanvas.gameObject.AddComponent<GraphicRaycaster>();
                }

                if (myLayerType == PanelLayerType.Share)
                {
                    canvasObject.gameObject.layer = LayerMask.NameToLayer("Share");
                }
                else
                {
                    canvasObject.gameObject.layer = LayerMask.NameToLayer("WorldUI");
                }

                myChildCanvasDic[sortOrder] = childCanvas;
            }

            var panelObject = UITools.AddChild(childCanvas.gameObject, panelPrefab);
            if (null == panelObject)
            {
                return null;
            }

            var childCount = childCanvas.transform.childCount;
            var siblingIndex = childCount;
            for (var i = 0; i < childCount; i++)
            {
                var childTrans = childCanvas.transform.GetChild(i);
                var childDepth = 0;
                var panelControl = childTrans.GetComponent<PanelControl>();
                if (null != panelControl)
                {
                    childDepth = panelControl.depth;
                }

                if (depth < childDepth)
                {
                    siblingIndex = i;
                    break;
                }
            }

            panelObject.transform.SetSiblingIndex(siblingIndex);
            var uiWorldCanvas = canvas.GetComponent<UIWorldCanvas>();
            if (uiWorldCanvas != null && uiWorldCanvas.uiMaterial != null)
            {
                var graphicArray = canvas.GetComponentsInChildren<Graphic>(true);
                for (var i = 0; i < graphicArray.Length; i++)
                {
                    if (graphicArray[i].GetComponent<Text>() != null)
                    {
                        continue;
                    }

                    graphicArray[i].material = uiWorldCanvas.uiMaterial;
                }
            }

            return panelObject;
        }

        private GameObject AddPanelOverlay(GameObject panelPrefab, int sortOrder, int depth)
        {
            Canvas childCanvas;
            if (!myChildCanvasDic.TryGetValue(sortOrder, out childCanvas))
            {
                var canvasObject = new GameObject(string.Format("Canvas({0})", sortOrder));
                canvasObject.transform.parent = canvas.transform;
                canvasObject.transform.localPosition = Vector3.zero;
                canvasObject.transform.localRotation = Quaternion.identity;
                canvasObject.transform.localScale = Vector3.one;
                childCanvas = canvasObject.AddComponent<Canvas>();
                var rc = childCanvas.GetComponent<RectTransform>();
                rc.anchorMin = PanelService.SafeAreaRect.min;
                rc.anchorMax = PanelService.SafeAreaRect.size;
                rc.anchoredPosition = Vector2.zero;
                rc.sizeDelta = Vector2.zero;
                childCanvas.overrideSorting = true;
                childCanvas.sortingOrder = sortOrder;
                childCanvas.sortingLayerName = canvas.sortingLayerName;

                childCanvas.gameObject.AddComponent<GraphicRaycaster>();
                canvasObject.gameObject.layer = LayerMask.NameToLayer("UI");
                myChildCanvasDic[sortOrder] = childCanvas;
            }

            var panelObject = UITools.AddChild(childCanvas.gameObject, panelPrefab);
            if (null == panelObject)
            {
                return null;
            }

            var childCount = childCanvas.transform.childCount;
            var siblingIndex = childCount;
            for (var i = 0; i < childCount; i++)
            {
                var childTrans = childCanvas.transform.GetChild(i);
                var childDepth = 0;
                var panelControl = childTrans.GetComponent<PanelControl>();
                if (null != panelControl)
                {
                    childDepth = panelControl.depth;
                }

                if (depth < childDepth)
                {
                    siblingIndex = i;
                    break;
                }
            }

            panelObject.transform.SetSiblingIndex(siblingIndex);

            var canvases = panelObject.GetComponentsInChildren<Canvas>();
            for (var i = 0; i < canvases.Length; i++)
            {
                if (canvases[i].overrideSorting)
                {
                    canvases[i].sortingLayerName = canvas.sortingLayerName;
                }
            }

            return panelObject;
        }

        public GameObject AddPanel(GameObject panelPrefab, int sortOrder, int depth)
        {
            if (isOverlay)
            {
                return AddPanelOverlay(panelPrefab, sortOrder, depth);
            }

            return AddPanelNonOverlay(panelPrefab, sortOrder, depth);
        }

        private GameObject ReaddPanelNonOverlay(GameObject panelObject, int sortOrder, int depth)
        {
            if (null == panelObject)
            {
                return null;
            }

            Canvas childCanvas;
            if (!myChildCanvasDic.TryGetValue(sortOrder, out childCanvas))
            {
                var canvasObject = new GameObject(string.Format("Canvas({0})", sortOrder));
                canvasObject.transform.parent = canvas.transform;
                canvasObject.transform.localPosition = Vector3.zero;
                canvasObject.transform.localRotation = Quaternion.identity;
                canvasObject.transform.localScale = Vector3.one;
                childCanvas = canvasObject.AddComponent<Canvas>();
                var rc = childCanvas.GetComponent<RectTransform>();
                rc.anchorMin = PanelService.SafeAreaRect.min;
                rc.anchorMax = PanelService.SafeAreaRect.size;
                rc.anchoredPosition = Vector2.zero;
                rc.sizeDelta = Vector2.zero;
                childCanvas.overrideSorting = true;
                childCanvas.sortingLayerName = "Highlight";
                childCanvas.sortingOrder = sortOrder - 1;

                if (myLayerType == PanelLayerType.World)
                {
                    childCanvas.gameObject.AddComponent<CustomGraphicRaycaster>();
                }
                else
                {
                    childCanvas.gameObject.AddComponent<GraphicRaycaster>();
                }

                if (myLayerType == PanelLayerType.Share)
                {
                    canvasObject.gameObject.layer = LayerMask.NameToLayer("Share");
                }
                else
                {
                    canvasObject.gameObject.layer = LayerMask.NameToLayer("WorldUI");
                }

                myChildCanvasDic[sortOrder] = childCanvas;
            }

            var t = panelObject.transform;
            t.SetParent(childCanvas.transform, false);
            panelObject.layer = childCanvas.gameObject.layer;

            var childCount = childCanvas.transform.childCount;
            var siblingIndex = childCount;
            for (var i = 0; i < childCount; i++)
            {
                var childTrans = childCanvas.transform.GetChild(i);
                var childDepth = 0;
                var panelControl = childTrans.GetComponent<PanelControl>();
                if (null != panelControl)
                {
                    childDepth = panelControl.depth;
                }

                if (depth < childDepth)
                {
                    siblingIndex = i;
                    break;
                }
            }

            panelObject.transform.SetSiblingIndex(siblingIndex);
            var uiWorldCanvas = canvas.GetComponent<UIWorldCanvas>();
            if (uiWorldCanvas != null && uiWorldCanvas.uiMaterial != null)
            {
                var graphicArray = canvas.GetComponentsInChildren<Graphic>(true);
                for (var i = 0; i < graphicArray.Length; i++)
                {
                    if (graphicArray[i].GetComponent<Text>() != null)
                    {
                        continue;
                    }

                    graphicArray[i].material = uiWorldCanvas.uiMaterial;
                }
            }

            return panelObject;
        }

        private GameObject ReaddPanelOverlay(GameObject panelObject, int sortOrder, int depth)
        {
            if (null == panelObject)
            {
                return null;
            }

            Canvas childCanvas;
            if (!myChildCanvasDic.TryGetValue(sortOrder, out childCanvas))
            {
                var canvasObject = new GameObject(string.Format("Canvas({0})", sortOrder));
                canvasObject.transform.parent = canvas.transform;
                canvasObject.transform.localPosition = Vector3.zero;
                canvasObject.transform.localRotation = Quaternion.identity;
                canvasObject.transform.localScale = Vector3.one;
                childCanvas = canvasObject.AddComponent<Canvas>();
                var rc = childCanvas.GetComponent<RectTransform>();
                rc.anchorMin = PanelService.SafeAreaRect.min;
                rc.anchorMax = PanelService.SafeAreaRect.size;
                rc.anchoredPosition = Vector2.zero;
                rc.sizeDelta = Vector2.zero;
                childCanvas.overrideSorting = true;
                childCanvas.sortingOrder = sortOrder;
                childCanvas.sortingLayerName = canvas.sortingLayerName;

                childCanvas.gameObject.AddComponent<GraphicRaycaster>();
                canvasObject.gameObject.layer = LayerMask.NameToLayer("UI");
                myChildCanvasDic[sortOrder] = childCanvas;
            }

            var t = panelObject.transform;
            t.SetParent(childCanvas.transform, false);
            panelObject.layer = childCanvas.gameObject.layer;

            var childCount = childCanvas.transform.childCount;
            var siblingIndex = childCount;
            for (var i = 0; i < childCount; i++)
            {
                var childTrans = childCanvas.transform.GetChild(i);
                var childDepth = 0;
                var panelControl = childTrans.GetComponent<PanelControl>();
                if (null != panelControl)
                {
                    childDepth = panelControl.depth;
                }

                if (depth < childDepth)
                {
                    siblingIndex = i;
                    break;
                }
            }

            panelObject.transform.SetSiblingIndex(siblingIndex);

            var canvases = panelObject.GetComponentsInChildren<Canvas>();
            for (var i = 0; i < canvases.Length; i++)
            {
                if (canvases[i].overrideSorting)
                {
                    canvases[i].sortingLayerName = canvas.sortingLayerName;
                }
            }

            return panelObject;
        }

        public GameObject ReaddPanel(GameObject panelObject, int sortOrder, int depth)
        {
            if (isOverlay)
            {
                return ReaddPanelOverlay(panelObject, sortOrder, depth);
            }

            return ReaddPanelNonOverlay(panelObject, sortOrder, depth);
        }

        public void Show()
        {
            Messenger.Broadcast(StateChangedMessage, myLayerType, true);
            var raycasters = canvas.GetComponentsInChildren<GraphicRaycaster>(true);

            if (isOverlay)
            {
                for (var i = 0; i < raycasters.Length; i++)
                {
                    raycasters[i].enabled = true;
                }

                canvasGroup.alpha = 1;

                return;
            }

            if (null == screenSpaceCamera)
            {
                return;
            }

            if (myLayerType == PanelLayerType.World)
            {
                if (myCullingMask < 0)
                {
                    myCullingMask = 1 << LayerMask.NameToLayer("WorldUI");
                }

                screenSpaceCamera.cullingMask |= myCullingMask;
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
            }
            else
            {
                screenSpaceCamera.gameObject.SetActive(true);
            }

            for (var i = 0; i < raycasters.Length; i++)
            {
                raycasters[i].enabled = true;
            }
        }

        public void Hide()
        {
            Messenger.Broadcast(StateChangedMessage, myLayerType, false);
            var raycasters = canvas.GetComponentsInChildren<GraphicRaycaster>(true);

            if (isOverlay)
            {
                for (var i = 0; i < raycasters.Length; i++)
                {
                    raycasters[i].enabled = false;
                }

                canvasGroup.alpha = 0;
                return;
            }

            if (null == screenSpaceCamera)
            {
                return;
            }

            if (myLayerType == PanelLayerType.World)
            {
                if (myCullingMask < 0)
                {
                    myCullingMask = 1 << LayerMask.NameToLayer("WorldUI");
                }

                screenSpaceCamera.cullingMask &= ~myCullingMask;
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
            }
            else
            {
                screenSpaceCamera.gameObject.SetActive(false);
            }

            for (var i = 0; i < raycasters.Length; i++)
            {
                raycasters[i].enabled = false;
            }
        }
    }
}