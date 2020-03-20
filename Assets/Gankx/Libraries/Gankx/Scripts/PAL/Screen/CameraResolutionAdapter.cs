using System.Collections;
using System.Collections.Generic;
using Gankx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CameraResolutionAdapter : MonoBehaviour {
    private Camera cam;
    int _isHighQuality;
    
    public Material m_BlitDepthMat;

    private TransformDirtyTracker dirtyTracker;

    private void Awake() {
        // TODO GameEditorUtility
        if (false/*GameEditorUtility.InEditor */|| GamePortalExport.GetDetailedQualityByKey("QualitySetting_Resolution") == 1)
        {
            Destroy(this);
            return;
        }

        cam = GetComponent<Camera>();

        if (cam == null || cam.targetTexture != null || cam.cullingMask == 1 << LayerMask.NameToLayer("ReflectionPVS") || 
            cam.cullingMask == 1 << LayerMask.NameToLayer("UI") || cam.name == "Light Camera" || cam.cullingMask == 0) {
            Destroy(this);
            return;
        }

        PhysicsRaycaster pr = cam.GetComponent<PhysicsRaycaster>();
        if (pr != null) {
            CustomPhysicsRaycaster cpr = cam.gameObject.AddComponent<CustomPhysicsRaycaster>();
            cpr.eventMask = pr.eventMask;

            Destroy(pr);
        }

        if (cam.CompareTag("MainCamera")) {
            m_BlitDepthMat = Resources.Load<Material>("scene/camera/BlitDepth");
        }
    }

    private bool inited = false;
    // Use this for initialization
    void Start() {
        Refresh(true);
        inited = true;

        dirtyTracker = GetComponent<TransformDirtyTracker>();
    }

    void OnEnable() {
        AddBlitCommand();

        if (inited) {
            if (dirtyTracker != null) {
                dirtyTracker.SetDirty();
            }
        }  
    }

    void Update() {
        if (_isHighQuality != GamePortalExport.GetDetailedQualityByKey("QualitySetting_Resolution")) {
            Refresh();
        }
    }

    public void Refresh(bool isFirstRun = false) {
        _isHighQuality = GamePortalExport.GetDetailedQualityByKey("QualitySetting_Resolution");
//        cam.targetTexture = null;
        if (isFirstRun)
        {
            if (!cam.enabled) return;
        }
        cam.enabled = false;

        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayRender(isFirstRun));
        else {
            SetRenderBuffer();
        }
    }

    IEnumerator DelayRender(bool isFirstRun) {
        if (!isFirstRun)
            yield return new WaitForEndOfFrame();
        SetRenderBuffer();
    }

    void SetRenderBuffer() {
        cam.enabled = true;

        AddBlitCommand();
    }

    bool IsRenderCamera()
    {
        if (cam == null) return false;
        return cam.CompareTag("MainCamera") || cam.CompareTag("QTECamera");
    }
    
    void AddBlitCommand() {
        // TODO GameEditorUtility
        if (false/*GameEditorUtility.InEditor*/)
        {
            return;
        }

        if (GamePortalExport.GetDetailedQualityByKey("QualitySetting_Resolution") == 1) {
            if (saveDepthTexCB != null) {
                cam.RemoveCommandBuffer(CameraEvent.BeforeForwardAlpha, saveDepthTexCB);
                saveDepthTexCB.Dispose();
                saveDepthTexCB = null;
            }

            return;
        }

        if (cam == null) {
            Debug.LogError("当前camera为null");
            return;
        }

        if(ScreenRTMgr.ContainsInstance()) {
            cam.SetTargetBuffers(ScreenRTMgr.instance.GetScreenRT().colorBuffer, ScreenRTMgr.instance.GetDepthRT().depthBuffer);

            ScreenRTMgr.instance.ResetCommandBuffer();

            if (cam.CompareTag("MainCamera")) {
                if (saveDepthTexCB != null) {
                    cam.RemoveCommandBuffer(CameraEvent.BeforeForwardAlpha, saveDepthTexCB);
                    saveDepthTexCB.Dispose();
                    saveDepthTexCB = null;
                }
                    
                {
                    saveDepthTexCB = new CommandBuffer();
                    saveDepthTexCB.name = "Save Depth Texture";
                    saveDepthTexCB.Blit(ScreenRTMgr.instance.GetDepthRT(), ScreenRTMgr.instance.GetLastDepthRT(), m_BlitDepthMat);
                    cam.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, saveDepthTexCB);
                }
            }
        }
    }
    public CommandBuffer saveDepthTexCB;
}
