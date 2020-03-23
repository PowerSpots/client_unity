using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Gankx.UI
{
    public class PanelService : Singleton<PanelService>
    {
        public const string PanelResourcePath = UIConfig.PanelSimplePath;
        public const string OverlayCameraPath = UIConfig.OverlayCameraPath;

        private Dictionary<uint, Window> myWindowMap;
        private Dictionary<string, Window> myPanelMap;
        private Dictionary<string, string> myPanelPathMap;
        private Dictionary<string, GameObject> myPanelTemplateMap;
        private Dictionary<string, Coroutine> myPanelLoadingMap;

        private Camera myOverlayCamera;
        private Camera myAfkCamera;

        private readonly Dictionary<PanelLayerType, PanelLayer> myPanelLayerMap =
            new Dictionary<PanelLayerType, PanelLayer>();

        private readonly Dictionary<PanelLayerType, PanelLayerParam> myPanelLayerParamMap =
            new Dictionary<PanelLayerType, PanelLayerParam>();

        // TODO PAL.Screen
#if UNITY_IOS
		[DllImport("__Internal")]
		private extern static void GetSafeAreaImpl(out float x, out float y, out float w, out float h, out float screenWidth, out float screenHeight);
#endif
        /// iPhoneX 的分辨率为 2436:1125 其安全区为 2172:1062 (132, 63, 2304, 1125) 实际刘海占88像素
        private static Rect m_SafeAreaRect = new Rect(-1, -1, -1, -1);

        public static Rect SafeAreaRect
        {
            get
            {
                if (m_SafeAreaRect.x < 0)
                {
                    float x = 0,
                        y = 0,
                        w = Screen.width,
                        h = Screen.height,
                        screenWidth = Screen.width,
                        screenHeight = Screen.height;

                    float edge = 0; // (1 - w * 1.0f / screenWidth) / 2.0f;
#if UNITY_IOS && !UNITY_EDITOR
                    GetSafeAreaImpl(out x, out y, out w, out h, out screenWidth, out screenHeight);
                    if(x > 44) {
                        edge = 88.0f / 2436;
                    }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
                    edge = FullScreenSupport.NotchEdge;
#endif
                    if (PlayerPrefs.GetString("Simulate_iPhoneX", "0") == "1")
                    {
                        edge = 88.0f / 2436;
                    }

                    m_SafeAreaRect = new Rect(edge, 0, 1 - edge, 1);

                    //Debug.Log("m_SafeAreaRect is " + m_SafeAreaRect);
                }

                return m_SafeAreaRect;
            }
        }

        private float m_SafeWidthEdge = -1;

        public float SafeWidthEdge
        {
            get
            {
                if (m_SafeWidthEdge < 0)
                {
                    float x = 0,
                        y = 0,
                        w = Screen.width,
                        h = Screen.height,
                        screenWidth = Screen.width,
                        screenHeight = Screen.height;
                    m_SafeWidthEdge = 0;
#if UNITY_IOS && !UNITY_EDITOR
                    GetSafeAreaImpl(out x, out y, out w, out h, out screenWidth, out screenHeight);
                    if(x > 44) {
                        m_SafeWidthEdge = 88.0f; //screenWidth * 88.0f / 2436;
                    }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
                    m_SafeWidthEdge = FullScreenSupport.NotchEdge * UIScreenAdaptor.GetCanvasWidth();
#endif
                    if (PlayerPrefs.GetString("Simulate_iPhoneX", "0") == "1")
                    {
                        m_SafeWidthEdge = 88.0f;
                    }
                }

                return m_SafeWidthEdge;
            }
        }

        protected override void OnInit()
        {
            myWindowMap = new Dictionary<uint, Window>();
            myPanelMap = new Dictionary<string, Window>();
            myPanelPathMap = new Dictionary<string, string>();
            myPanelTemplateMap = new Dictionary<string, GameObject>();
            myPanelLoadingMap = new Dictionary<string, Coroutine>();

            var panelLayerConfigList = UIConfig.PanelLayerList;
            for (var i = 0; i < panelLayerConfigList.Count; i++)
            {
                var panelLayerConfig = panelLayerConfigList[i];
                myPanelLayerParamMap.Add(panelLayerConfig.layerType, panelLayerConfig);
            }

            var camPrefab = ResourceService.Load<GameObject>(OverlayCameraPath);
            if (null == camPrefab)
            {
                Debug.LogError(string.Format("Can not find camera template:{0}, please confirm！", OverlayCameraPath));
                return;
            }

            var camObject = UITools.AddChild(gameObject, camPrefab);
            myOverlayCamera = camObject.GetComponent<Camera>();

            camObject = UITools.AddChild(gameObject, camPrefab);
            myAfkCamera = camObject.GetComponent<Camera>();
            myAfkCamera.enabled = false;
            myAfkCamera.depth = 100;
        }

        private void OnDestroy()
        {
            if (myWindowMap != null)
            {
                myWindowMap.Clear();
            }

            myPanelMap.Clear();
            myPanelPathMap.Clear();
            myPanelTemplateMap.Clear();
            myPanelLoadingMap.Clear();
        }

        public PanelLayer GetOrAddPanelLayer(PanelLayerType layerType)
        {
            PanelLayer layer;
            if (!myPanelLayerMap.TryGetValue(layerType, out layer))
            {
                PanelLayerParam param;
                myPanelLayerParamMap.TryGetValue(layerType, out param);
                if (null == param)
                {
                    Debug.LogError("Cannot find panel layer param: " + layerType);
                    return null;
                }

                if (param.layerType == PanelLayerType.Afk)
                {
                    layer = PanelLayer.Create(gameObject, param, myAfkCamera);
                }
                else
                {
                    layer = PanelLayer.Create(gameObject, param, myOverlayCamera);
                }

                myPanelLayerMap[layerType] = layer;
            }

            return layer;
        }

        public PanelLayer GetLayer(PanelLayerType layerType)
        {
            PanelLayer layer;
            myPanelLayerMap.TryGetValue(layerType, out layer);
            return layer;
        }

        public void HideLayer(PanelLayerType layerType)
        {
            PanelLayer layer;
            if (!myPanelLayerMap.TryGetValue(layerType, out layer))
            {
                return;
            }

            layer.Hide();
        }

        public void ShowLayer(PanelLayerType layerType)
        {
            PanelLayer layer;
            if (!myPanelLayerMap.TryGetValue(layerType, out layer))
            {
                return;
            }

            layer.Show();
        }

        public void SetAfkState(bool state)
        {
            if (state)
            {
                if (Camera.main != null && Camera.main.cullingMask != 0)
                {
                    myOverlayCamera.enabled = false;
                }
                else
                {
                    myOverlayCamera.enabled = true;
                }

                myAfkCamera.enabled = true;
            }
            else
            {
                myOverlayCamera.enabled = true;
                myAfkCamera.enabled = false;
            }
        }

        public void CachePanel(string panelName, string panelPath)
        {
            if (string.IsNullOrEmpty(panelName))
            {
                Debug.LogError("CachePanel参数错误！");
                return;
            }

            Window window;
            if (myPanelMap.TryGetValue(panelName, out window) && !myPanelTemplateMap.ContainsKey(panelName))
            {
                var template = Instantiate(window.gameObject, transform);
                myPanelTemplateMap[panelName] = template;
                template.gameObject.name = panelPath;
            }
        }

        public uint LoadPanel(string panelName, string panelPath)
        {
            if (string.IsNullOrEmpty(panelName) || string.IsNullOrEmpty(panelPath))
            {
                Debug.LogError("LoadPanel: invalid parameter!");
                return Window.InvalidId;
            }

            // 如果Panel已经加载那么直接返回
            Window panelWindow;
            if (myPanelMap.TryGetValue(panelName, out panelWindow))
            {
                return panelWindow.id;
            }

            var filepath = PanelResourcePath + panelPath;

            Profiler.BeginSample("LoadPanel: " + panelPath);

            GameObject panelPrefab;
            if (!myPanelTemplateMap.TryGetValue(panelName, out panelPrefab))
            {
                panelPrefab = ResourceService.Load<GameObject>(filepath);
            }

            Profiler.EndSample();

            if (null == panelPrefab)
            {
                Debug.LogError(string.Format("Can not load panel({0}) from Resources path:{1}!", panelPath,
                    PanelResourcePath));
                return Window.InvalidId;
            }

            if (panelPrefab.activeSelf)
            {
                panelPrefab.SetActive(false);
            }

            var sortOrder = 0;
            var depth = 0;
            var layerType = PanelLayerType.Foreground;
            var panelControl = panelPrefab.GetComponent<PanelControl>();
            if (null != panelControl)
            {
                sortOrder = panelControl.sortOrder;
                depth = panelControl.depth;
                layerType = panelControl.layer;
            }

            var layer = GetOrAddPanelLayer(layerType);
            if (null == layer)
            {
                return Window.InvalidId;
            }

            var panelObject = layer.AddPanel(panelPrefab, sortOrder, depth);
            if (null == panelObject)
            {
                Debug.LogError(string.Format("PanelLayer.AddPanel return null:{0}", panelPath));
                return Window.InvalidId;
            }

//            panelObject.SetActive(false);

            panelWindow = AddWindow(panelObject);
            panelWindow.panelId = panelWindow.id;

            myPanelMap[panelName] = panelWindow;
            myPanelPathMap[panelName] = filepath;

            return panelWindow.id;
        }

        private void CancelLoadPanelAsyncCouroutine(string panelName)
        {
            if (string.IsNullOrEmpty(panelName))
            {
                return;
            }

            Coroutine panelCoroutine;
            if (!myPanelLoadingMap.TryGetValue(panelName, out panelCoroutine))
            {
                return;
            }

            myPanelLoadingMap.Remove(panelName);
            LuaService.instance.FireEvent("OnLoadPanelAsync", Window.InvalidId, panelName);
        }

        private IEnumerator LoadPanelAsyncCouroutine(string panelName, string panelPath)
        {
            Window panelWindow;
            if (!myPanelMap.TryGetValue(panelName, out panelWindow))
            {
                var filepath = PanelResourcePath + panelPath;

                GameObject panelPrefab;
                if (!myPanelTemplateMap.TryGetValue(panelName, out panelPrefab))
                {
                    var panelResourceRequest = ResourceService.LoadAsync<GameObject>(filepath);

                    yield return panelResourceRequest;

                    if (null == panelResourceRequest)
                    {
                        Debug.LogError(string.Format("LoadAsync panel prefab error: {0}！", filepath));
                        yield break;
                    }

                    panelPrefab = panelResourceRequest.asset as GameObject;
                }
                else if (null == panelPrefab)
                {
                    Debug.LogError(string.Format("Cache null panel prefab: {0}！", filepath));
                }

                if (null == panelPrefab)
                {
                    Debug.LogError(string.Format("Can not load panel({0}) from Resources path:{1}!", panelPath,
                        PanelResourcePath));
                    CancelLoadPanelAsyncCouroutine(panelName);
                    yield break;
                }

                if (panelPrefab.activeSelf)
                {
                    panelPrefab.SetActive(false);
                }

                var sortOrder = 0;
                var depth = 0;
                var layerType = PanelLayerType.Foreground;
                var panelControl = panelPrefab.GetComponent<PanelControl>();
                if (null != panelControl)
                {
                    sortOrder = panelControl.sortOrder;
                    depth = panelControl.depth;
                    layerType = panelControl.layer;
                }

                var layer = GetOrAddPanelLayer(layerType);
                if (null == layer)
                {
                    Debug.LogError(string.Format("GetOrAddPanelLayer return null:{0}", filepath));
                    CancelLoadPanelAsyncCouroutine(panelName);
                    yield break;
                }

                var panelObject = layer.AddPanel(panelPrefab, sortOrder, depth);
                if (null == panelObject)
                {
                    Debug.LogError(string.Format("PanelLayer.AddPanel return null:{0}", filepath));
                    CancelLoadPanelAsyncCouroutine(panelName);
                    yield break;
                }

                panelWindow = AddWindow(panelObject);
                panelWindow.panelId = panelWindow.id;

                myPanelMap[panelName] = panelWindow;
                myPanelPathMap[panelName] = filepath;
            }

            myPanelLoadingMap.Remove(panelName);

            LuaService.instance.FireEvent("OnLoadPanelAsync", panelWindow.id, panelName);
        }

        public bool LoadPanelAsync(string panelName, string panelPath)
        {
            if (string.IsNullOrEmpty(panelName) || string.IsNullOrEmpty(panelPath))
            {
                Debug.LogError("LoadPanelAsync: invalid parameter!");
                return false;
            }

            Coroutine panelCoroutine;
            if (myPanelLoadingMap.TryGetValue(panelName, out panelCoroutine))
            {
                return true;
            }

            var loadingCoroutine = StartCoroutine(LoadPanelAsyncCouroutine(panelName, panelPath));
            if (null != loadingCoroutine)
            {
                myPanelLoadingMap[panelName] = loadingCoroutine;
            }

            return true;
        }

        public void CancelLoadPanelAsync(string panelName)
        {
            if (string.IsNullOrEmpty(panelName))
            {
                return;
            }

            Coroutine panelCoroutine;
            if (!myPanelLoadingMap.TryGetValue(panelName, out panelCoroutine))
            {
                return;
            }

            myPanelLoadingMap.Remove(panelName);

            StopCoroutine(panelCoroutine);
        }

        private readonly List<uint> myUnloadWindowList = new List<uint>();

        public bool UnloadPanel(string panelName)
        {
            Window panelWindow;
            if (!myPanelMap.TryGetValue(panelName, out panelWindow))
            {
                return false;
            }

            if (!myPanelTemplateMap.ContainsKey(panelName))
            {
                var controls = panelWindow.gameObject.GetComponentsInChildren<SlotControl>(true);
                for (var i = 0; i < controls.Length; i++)
                {
                    if (controls[i].enabled)
                    {
                        controls[i].OnUnload();
                    }
                }
            }

            myUnloadWindowList.Clear();
            foreach (var pair in myWindowMap)
            {
                if (pair.Value.panelId == panelWindow.id)
                {
                    myUnloadWindowList.Add(pair.Value.id);
                }
            }

            for (var i = 0; i < myUnloadWindowList.Count; i++)
            {
                myWindowMap.Remove(myUnloadWindowList[i]);
            }

            myPanelMap.Remove(panelName);
            Destroy(panelWindow.gameObject);

            string filePath;
            if (myPanelPathMap.TryGetValue(panelName, out filePath) && !myPanelTemplateMap.ContainsKey(panelName))
            {
                ResourceService.UnloadPanel(filePath, true);
                myPanelPathMap.Remove(panelName);
            }

            return true;
        }

        public uint AddWindow(GameObject go, uint panelId)
        {
            var uiWindow = Window.AddWindow(go);
            if (null == uiWindow)
            {
                return Window.InvalidId;
            }

            myWindowMap[uiWindow.id] = uiWindow;

            uiWindow.panelId = panelId;
            return uiWindow.id;
        }

        public Window AddWindow(GameObject go)
        {
            var uiWindow = Window.AddWindow(go);

            myWindowMap[uiWindow.id] = uiWindow;
            return uiWindow;
        }

        public uint CloneWindow(uint windowId)
        {
            var uiWindow = GetWindow(windowId);
            if (null == uiWindow)
            {
                return Window.InvalidId;
            }

            var newWindow = uiWindow.Clone();
            if (null == newWindow)
            {
                return Window.InvalidId;
            }

            var newWindowObject = newWindow.gameObject;
            if (null == newWindowObject)
            {
                return Window.InvalidId;
            }

            var t = newWindowObject.transform;
            t.SetParent(uiWindow.transform.parent);
            t.localPosition = uiWindow.transform.localPosition;
            t.localRotation = uiWindow.transform.localRotation;
            t.localScale = uiWindow.transform.localScale;
            newWindow.CopyLocalScale(uiWindow);

            var newRt = newWindowObject.transform as RectTransform;
            if (newRt != null)
            {
                var rt = uiWindow.transform as RectTransform;
                if (rt != null)
                {
                    newRt.anchorMax = rt.anchorMax;
                    newRt.anchorMin = rt.anchorMin;
                    newRt.anchoredPosition = rt.anchoredPosition;
                    newRt.sizeDelta = rt.sizeDelta;
                }
            }

            newWindowObject.layer = uiWindow.gameObject.layer;
            newWindow.panelId = uiWindow.panelId;

            myWindowMap[newWindow.id] = newWindow;
            return newWindow.id;
        }

        public uint CloneWindowTo(uint windowId, uint parentWindowId, int siblingIndex)
        {
            var uiWindow = GetWindow(windowId);
            if (null == uiWindow)
            {
                return Window.InvalidId;
            }

            var newWindow = uiWindow.Clone();
            if (null == newWindow)
            {
                return Window.InvalidId;
            }

            var newWindowObject = newWindow.gameObject;
            if (null == newWindowObject)
            {
                return Window.InvalidId;
            }

            var parentWindow = GetWindow(parentWindowId);
            if (null == parentWindow)
            {
                return newWindow.id;
            }

            var t = newWindowObject.transform;
            t.SetParent(uiWindow.transform.parent);
            t.localPosition = uiWindow.transform.localPosition;
            t.localRotation = uiWindow.transform.localRotation;
            t.localScale = uiWindow.transform.localScale;

            t.SetParent(parentWindow.transform);
            t.SetSiblingIndex(siblingIndex);

            var list = ListPool<Canvas>.Get();
            uiWindow.GetComponentsInParent(false, list);
            if (list.Count == 0)
            {
                return newWindow.id;
            }

            var rootCanvas = list[0];
            ListPool<Canvas>.Release(list);

            var newRt = newWindowObject.transform as RectTransform;
            if (newRt != null)
            {
                var rt = uiWindow.transform as RectTransform;
                newRt.SetWidth(rt.GetWidth());
                newRt.SetHeight(rt.GetHeight());
            }

            t.localPosition = rootCanvas.transform.InverseTransformPoint(uiWindow.transform.position);
            t.localRotation.SetFromToRotation(rootCanvas.transform.rotation.eulerAngles,
                uiWindow.transform.rotation.eulerAngles);

            newWindowObject.layer = uiWindow.gameObject.layer;
            newWindow.panelId = uiWindow.panelId;

            myWindowMap[newWindow.id] = newWindow;
            return newWindow.id;
        }

        public bool RemoveWindow(uint windowId)
        {
            return myWindowMap.Remove(windowId);
        }

        public Window GetWindow(uint windowId)
        {
            Window uiWindow;
            myWindowMap.TryGetValue(windowId, out uiWindow);
            return uiWindow;
        }

        private static T DoGetWindowComponent<T>(uint windowId, bool forceParentSlotControl = false) where T : Component
        {
            if (null == instance)
            {
                return null;
            }

            var uiWindow = instance.GetWindow(windowId);
            if (null == uiWindow)
            {
                return null;
            }

            return uiWindow.GetCachedComponent<T>(forceParentSlotControl);
        }

        public static T GetWindowComponent<T>(uint windowId, bool forceParentSlotControl = false) where T : Component
        {
            Profiler.BeginSample("GetWindowComponent");
            var t = DoGetWindowComponent<T>(windowId, forceParentSlotControl);
            Profiler.EndSample();
            return t;
        }

        public static T[] GetWindowComponentsInChildren<T>(uint windowId) where T : Component
        {
            if (null == instance)
            {
                return null;
            }

            var uiWindow = instance.GetWindow(windowId);
            if (null == uiWindow)
            {
                return null;
            }

            var slotControl = uiWindow.GetComponent<SlotControl>();
            if (null != slotControl && null != slotControl.cachedControlTransform)
            {
                return slotControl.cachedControlTransform.GetComponentsInChildren<T>();
            }

            return uiWindow.GetComponentsInChildren<T>();
        }

        public static GameObject GetWindowObject(uint windowId)
        {
            if (null == instance)
            {
                return null;
            }

            var uiWindow = instance.GetWindow(windowId);
            if (null == uiWindow)
            {
                return null;
            }

            return uiWindow.gameObject;
        }

        public static string GetWindowPath(uint windowId)
        {
            var windowObject = GetWindowObject(windowId);
            if (null == windowObject)
            {
                return string.Empty;
            }

            return GameObjectUtility.GetGameObjectPath(windowObject);
        }

        public void ReaddWindowChild(uint parentWindowId, uint childWindowId)
        {
            var childWindow = GetWindow(childWindowId);
            if (null == childWindow)
            {
                return;
            }

            var parentWindow = GetWindow(parentWindowId);
            if (null == parentWindow)
            {
                return;
            }

            if (parentWindow.transform != childWindow.transform.parent)
            {
                return;
            }

            childWindow.transform.SetAsLastSibling();
        }

        public void InsertWindowChild(uint parentWindowId, uint childWindowId, uint refChildWindowId)
        {
            var childWindow = GetWindow(childWindowId);
            if (null == childWindow)
            {
                return;
            }

            var parentWindow = GetWindow(parentWindowId);
            if (null == parentWindow)
            {
                return;
            }

            var refChildWindow = GetWindow(refChildWindowId);
            if (null == refChildWindow)
            {
                return;
            }

            if (parentWindow.transform != childWindow.transform.parent ||
                parentWindow.transform != refChildWindow.transform.parent)
            {
                return;
            }

            if (childWindowId == refChildWindowId)
            {
                return;
            }

            var refIndex = refChildWindow.transform.GetSiblingIndex();
            var childIndex = childWindow.transform.GetSiblingIndex();
            if (refIndex > childIndex)
            {
                refIndex--;
            }

            childWindow.transform.SetSiblingIndex(refIndex);
        }

        public Window FindPanel(string panelName)
        {
            Window window;
            if (!myPanelMap.TryGetValue(panelName, out window))
            {
                return null;
            }

            return window;
        }

        public Camera GetOverlayCamera()
        {
            return myOverlayCamera;
        }
    }
}
