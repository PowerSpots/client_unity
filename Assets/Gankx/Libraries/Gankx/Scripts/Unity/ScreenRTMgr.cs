using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ScreenRTMgr : Gankx.Singleton<ScreenRTMgr>
{
    public RenderTexture screenRT;
    public RenderTexture depthRT;
    public RenderTexture _LastDepthRT;
    /// <summary>
    /// Todo：当前版本不在更改屏幕大小，临时改动 960/1080
    /// </summary>
    public float scale = 0.889f;
    private float _CurrentScale = 1;

    public float depthScale = 1;

    HashSet<Camera> resoluationAdaptCams;

    public static int RT_Index = 1;

    public Camera[] m_AllCameras;
    public SortByCameraDepth m_Campare;
    public CommandBuffer CmdBuffer;
    public Camera m_LastRenderTargetCamera;

    public float GetWidth() {
        if (GamePortalExport.GetDetailedQuality("Resolution") == 1) {
            return Screen.width;
        }

        if (GetScreenRT() != null) {
            return GetScreenRT().width;
        }
        return Screen.width;
    }

    public float GetHeight() {
        if (GamePortalExport.GetDetailedQuality("Resolution") == 1) {
            return Screen.height;
        }

        if (GetScreenRT() != null) {
            return GetScreenRT().height;
        }
        return Screen.height;
    }

    public RenderTexture GetScreenRT()
    {
        if (GamePortalExport.GetDetailedQuality("Resolution") == 1) {
            return null;
        }

        if (screenRT == null)
        {
            float baseScale = 1125.0f / Screen.height; // iPhoneX 的分辨率
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
            if (baseScale > 1)
#endif
            baseScale = 1;
#if UNITY_EDITOR
            //            baseScale = 1.3f;
#endif
            _CurrentScale = GamePortalExport.GetDetailedQuality("Resolution") >= 3 ? baseScale : baseScale * scale;
            int width = Mathf.RoundToInt((float)m_InitScreenWidth * _CurrentScale);
            int height = Mathf.RoundToInt((float)m_InitScreenHeight * _CurrentScale);

            screenRT = new RenderTexture(width, height, 0);
            screenRT.name = "Gankx Screen RT " + RT_Index + " (" + width + "x" + height + ")";

            InitDepthRTByScreenRT();

            RT_Index++;
        }

        return screenRT;
    }

    void InitDepthRTByScreenRT() {
        if(screenRT == null) return;
        if (depthRT != null) {
            Destroy(depthRT);
        }
        depthRT = new RenderTexture(screenRT.width, screenRT.height, 24, RenderTextureFormat.Depth);
        depthRT.name = "Gankx Depth RT " + RT_Index;
        depthRT.filterMode = FilterMode.Point;

        if (_LastDepthRT != null) {
            Destroy(_LastDepthRT);
        }

        _LastDepthRT = new RenderTexture((int)(screenRT.width * depthScale), (int)(screenRT.height * depthScale), 0);
        _LastDepthRT.name = "Gankx Last Depth RT " + RT_Index;
        _LastDepthRT.filterMode = FilterMode.Point;

        Shader.SetGlobalTexture("_DepthTexture", depthRT);
        Shader.SetGlobalTexture("_LastDepthTexture", _LastDepthRT);
    }

    public RenderTexture GetDepthRT() {
        if (GamePortalExport.GetDetailedQuality("Resolution") == 1) {
            return null;
        }

        if (depthRT == null) {
            GetScreenRT();
        }

        return depthRT;
    }

    public RenderTexture GetLastDepthRT() {
        if (GamePortalExport.GetDetailedQuality("Resolution") == 1) {
            return null;
        }

        if (_LastDepthRT == null) {
            GetScreenRT();
        }

        return _LastDepthRT;
    }

    public float CurrentScale {
        get { return _CurrentScale; }
    }

    public void Release()
    {
        if (CmdBuffer != null) {
            if (m_LastRenderTargetCamera != null) {
                m_LastRenderTargetCamera.RemoveCommandBuffer(CameraEvent.AfterEverything, CmdBuffer);
            }
            CmdBuffer.Clear();
            CmdBuffer.Dispose();
            CmdBuffer = null;

            RefreshCameraResolutionAdapter();

            if (GamePortalExport.GetDetailedQuality("Resolution") == 1) {
                m_LastRenderTargetCamera.targetTexture = null;
            }

            m_LastRenderTargetCamera = null;
        }

        if (screenRT != null) {
            Destroy(screenRT);
            screenRT = null;
        }
        if (depthRT != null) {
            Destroy(depthRT);
            depthRT = null;
        }
        if (_LastDepthRT != null) {
            Destroy(_LastDepthRT);
            _LastDepthRT = null;
        }
    }
        
    protected override void OnRelease()
    {
        Release();
    }

    public int m_InitScreenWidth, m_InitScreenHeight;
    protected override void OnInit() {
        m_InitScreenWidth = Screen.width;
        m_InitScreenHeight = Screen.height;
        resoluationAdaptCams = new HashSet<Camera>();
    }

    void LateUpdate() {
#if UNITY_EDITOR
        // TODO GameEditorUtility
        if (false/*GameEditorUtility.InEditor*/) {
            return;
        }
#endif
        if (GamePortalExport.GetDetailedQualityByKey("QualitySetting_Resolution") == 1) {
            return;
        }

        if (m_AllCameras == null) {
            m_AllCameras = new Camera[15];
        }

        if (m_Campare == null) {
            m_Campare = new SortByCameraDepth();
        }

        int count = Camera.GetAllCameras(m_AllCameras);
        PerformInsertionSort(m_AllCameras, count, m_Campare);
        Camera firstDepthG0Camera = null;

        for (int i = 0; i < m_AllCameras.Length && i < count; i++) {
            Camera cam = m_AllCameras[i];
            if(cam == null) continue;

            if (cam.depth <= 0 && cam.targetTexture == null) {
                if (!CompareCameraName(cam, "Light Camera") && !(cam.cullingMask == 1 << LayerMask.NameToLayer("ReflectionPVS") || 
                cam.cullingMask == 1 << LayerMask.NameToLayer("UI")|| cam.cullingMask == 0)) 
                {
                    firstDepthG0Camera = cam;
                }
            }

            if (resoluationAdaptCams.Contains(cam)
                || cam.depth > 0) {
                continue;
            }
            cam.gameObject.GetOrAddComponent<CameraResolutionAdapter>();
            resoluationAdaptCams.Add(cam);
        }

        if (firstDepthG0Camera != m_LastRenderTargetCamera) {
            if (CmdBuffer == null) {
                CmdBuffer = new CommandBuffer();
                CmdBuffer.name = "Blit target to CurrentActive";
                CmdBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                CmdBuffer.Blit(ScreenRTMgr.instance.GetScreenRT(), BuiltinRenderTextureType.CurrentActive);
            }

            if (m_LastRenderTargetCamera != null) {
                m_LastRenderTargetCamera.RemoveCommandBuffer(CameraEvent.AfterEverything, CmdBuffer);
            }
            m_LastRenderTargetCamera = firstDepthG0Camera;
            if (m_LastRenderTargetCamera != null) {
                m_LastRenderTargetCamera.RemoveCommandBuffer(CameraEvent.AfterEverything, CmdBuffer);
                m_LastRenderTargetCamera.AddCommandBuffer(CameraEvent.AfterEverything, CmdBuffer);
            }
        }
    }

    private Dictionary<Camera, string> m_CachedCameraName;

    bool CompareCameraName(Camera cam, string name) {
        if (cam == null) return false;

        if (m_CachedCameraName == null) {
            m_CachedCameraName = new Dictionary<Camera, string>();
        }

        string camName = string.Empty;
        if (!m_CachedCameraName.TryGetValue(cam, out camName)) {
            camName = cam.name;
            m_CachedCameraName.Add(cam, camName);

            List<Camera> buffer = new List<Camera>(m_CachedCameraName.Keys);
            for (int i = 0; i < buffer.Count; i++) {
                Camera item = buffer[i];
                if (item == null || item.name.Equals("null")) {
                    m_CachedCameraName.Remove(item);
                }
            }
        }

        return camName == name;
    }

    public class SortByCameraDepth : Comparer<Camera> {
        public override int Compare(Camera x, Camera y) {
            return x.depth.CompareTo(y.depth);
        }
    }

    public static void PerformInsertionSort<T>(T[] inputarray, int Count = -1, Comparer<T> comparer = null) {
        Comparer<T> equalityComparer = comparer;
        if (equalityComparer == null) equalityComparer = Comparer<T>.Default;
        if (Count < 0) Count = inputarray.Length;
        for (int counter = 0; counter < Count - 1; counter++) {
            int index = counter + 1;
            while (index > 0) {
                if (equalityComparer.Compare(inputarray[index - 1], inputarray[index]) > 0) {
                    T temp = inputarray[index - 1];
                    inputarray[index - 1] = inputarray[index];
                    inputarray[index] = temp;
                }
                index--;
            }
        }
    }

#if UNITY_EDITOR
    private int m_LastScreenHeight = -1;
    void Update() {
        if (GamePortalExport.GetDetailedQualityByKey("QualitySetting_Resolution") == 1) {
            return;
        }

        if (m_LastScreenHeight < 0) {
            m_LastScreenHeight = Screen.height;
        }
        if (m_LastScreenHeight != Screen.height) {
            Release();
            m_LastScreenHeight = Screen.height;
            RefreshCameraResolutionAdapter();
            ResetCommandBuffer();
        }
    }
#endif

    public void ResetCommandBuffer() {
        m_LastRenderTargetCamera = null;
    }

    public void RefreshCameraResolutionAdapter() {
        foreach (Camera resoluationAdaptCam in resoluationAdaptCams) {
            if(resoluationAdaptCam == null) continue;
            CameraResolutionAdapter adapter = resoluationAdaptCam.GetComponent<CameraResolutionAdapter>();
            if (adapter == null) continue;
            adapter.Refresh();
        }
    }

    /// 消除缩放屏幕带来的影响
    /// 放大
    public static Vector3 ScaleToOriSize(Vector3 screenPos) {
        if (instance.screenRT == null) return screenPos;
        return screenPos / instance.CurrentScale;
    }

    // 缩小
    public static Vector3 OriToScaleSize(Vector3 screenPos) {
        if (instance.screenRT == null) return screenPos;
        return screenPos * instance.CurrentScale;
    }

    public static Vector3 WorldToScreenPoint(Camera cam, Vector3 position) {
        if (cam == null || Vector2.zero.Equals(cam.rect.min)) return position;
        Vector3 screenPos = cam.WorldToScreenPoint(position);
        if (cam.depth <= 0)
            screenPos = ScaleToOriSize(screenPos);
        return screenPos;
    }

    public static Vector3 ScreenToWorldPoint(Camera cam, Vector3 screenPoint) {
        if (cam == null || Vector2.zero.Equals(cam.rect.min)) return screenPoint;
        if (cam.depth <= 0) screenPoint = OriToScaleSize(screenPoint);
        return cam.ScreenToWorldPoint(screenPoint);
    }

    public static Ray ScreenPointToRay(Camera cam, Vector3 screenPoint) {
        // to avoid Screen position out of view frustum error : Camera rect 0 0 2220 1080
        if (cam == null) return new Ray();
        bool frustumError = false;
        int zeroVectors = 0;
        if (Mathf.Approximately(cam.transform.rotation.x, 0)) zeroVectors++;
        if (Mathf.Approximately(cam.transform.rotation.y, 0)) zeroVectors++;
        if (Mathf.Approximately(cam.transform.rotation.z, 0)) zeroVectors++;
        if (zeroVectors > 1) {
            frustumError = true;
        }
        if(frustumError) return new Ray();

        if (cam.depth <= 0) screenPoint = OriToScaleSize(screenPoint);

        if(screenPoint.x < 0 || screenPoint.x > cam.pixelWidth || screenPoint.y < 0 || screenPoint.y > cam.pixelHeight)
            return new Ray();

        Ray res;
        try {
            res = cam.ScreenPointToRay(screenPoint);
        }
        catch (Exception) {
            return new Ray();
        }
        return res;
    }

    public static Vector3 WorldToViewportPoint(Camera cam, Vector3 position)
    {
        if (null == cam)
        {
            return new Vector3(position.x / Screen.width, position.y / Screen.height, 0f);
        }

        return cam.WorldToViewportPoint(position);
    }
}
