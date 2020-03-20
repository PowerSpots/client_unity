using Gankx;
using Gankx.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class BlurBG : MonoBehaviour {
    
    private BlurOptimized m_BlurEffect;
    public BlurOptimized BlurEffect {
        get {
            if (m_BlurEffect != null) return m_BlurEffect;
            if (SceneCamera == null) return null;
            m_BlurEffect = SceneCamera.GetOrAddComponent<BlurOptimized>();
            m_BlurEffect.blurShader = Shader.Find("Hidden/FastBlur");
            m_BlurEffect.downsample = 2;
            return m_BlurEffect;
        }
    }

    private Camera SceneCamera {
        get
        {
            return null;
            // TODO SceneCameraService
//            if(!SceneCameraService.ContainsInstance() || SceneCameraService.Instance.SceneCamera == null) return null;
//            return SceneCameraService.Instance.SceneCamera;
        }
    }

    public bool m_NeedReCapture = true;
    private int m_CameraCullingMask = 0;

    private string[] m_MaskList = new[] {"Player", "WorldUI", "Shadow", "StoryNPC", "Effect", "ShadowCaster"};

    public bool m_IsVisible = false;
    private Camera m_UICamera;

    void OnEnable () {
        if (BlurEffect != null) {
            SetSceneCameraState(true);

            m_NeedReCapture = true;

            if (m_UICamera == null) {
                CanvasScaler scaler = transform.GetComponentInParent<CanvasScaler>();
                if (scaler != null) {
                    Canvas canvas = scaler.GetComponent<Canvas>();
                    if (canvas != null) {
                        m_UICamera = canvas.worldCamera;
                    }
                }
            }
            if (m_UICamera != null) {
                m_IsVisible = m_UICamera.gameObject.activeSelf;
            }
        }
        Messenger.AddListener<PanelLayerType, bool> (PanelLayer.StateChangedMessage, OnPanelGroupVisibleStateChanged);

        rt = ResourceService.GetRendertTexture((int)(ScreenRTMgr.instance.GetWidth() * 0.7f), (int)(ScreenRTMgr.instance.GetHeight() * 0.7f), 16);
    }

    private RenderTexture rt;

    void LateUpdate() {
        if (m_UICamera != null) {
            if (m_IsVisible != m_UICamera.gameObject.activeSelf) {
                m_IsVisible = m_UICamera.gameObject.activeSelf;
                SetSceneCameraState(m_IsVisible);
            }
        }

        if (!m_NeedReCapture || SceneCamera == null || (m_UICamera != null && !m_IsVisible)) return;

        if (SceneCamera != null) {
            CameraResolutionAdapter adapter = SceneCamera.GetComponent<CameraResolutionAdapter>();
            if (adapter != null)
                adapter.Refresh();
            else
                SceneCamera.targetTexture = null;
        }

        if (m_CameraCullingMask > 0) {
            SceneCamera.cullingMask = m_CameraCullingMask;
        }

        RenderTexture currentRT = RenderTexture.active;
        SceneCamera.targetTexture = rt;
        SceneCamera.Render();
        RenderTexture.active = rt;
        RenderTexture.active = currentRT;

        if (ScreenRTMgr.instance.GetScreenRT() == null) {
            SceneCamera.targetTexture = null;
        }

        BlurEffect.enabled = false;

        if (GetComponent<RawImage>() != null) {
            GetComponent<RawImage>().texture = rt;
        }

        m_NeedReCapture = false;
        if (BlurEffect != null) {
            SceneCamera.cullingMask = 0;
        }
    }

    void OnDisable () {
        RenderTexture.ReleaseTemporary(rt);

        Messenger.RemoveListener<PanelLayerType, bool>(PanelLayer.StateChangedMessage, OnPanelGroupVisibleStateChanged);
        if (BlurEffect != null) {
	        SetSceneCameraState(false);
	    }
    }

    void OnPanelGroupVisibleStateChanged(PanelLayerType type, bool state) { 
        if(type != PanelLayerType.Foreground) return;

        SetSceneCameraState(state);
    }

    void SetSceneCameraState(bool hideOtherInfo) {
        if (SceneCamera == null) return;
        if (hideOtherInfo) {
            for (int i = 0; i < m_MaskList.Length; i++) {
                HideMask(m_MaskList[i]);
            }
            BlurEffect.enabled = true;
            if (SceneCamera != null && SceneCamera.cullingMask != 0)
                m_CameraCullingMask = SceneCamera.cullingMask;
        }
        else {
            if (SceneCamera != null) {
                SceneCamera.enabled = false;
                SceneCamera.enabled = true;
                SceneCamera.cullingMask = m_CameraCullingMask;
            }
            BlurEffect.enabled = false;
            for (int i = 0; i < m_MaskList.Length; i++) {
                ShowMask(m_MaskList[i]);
            }
        }
    }

    void HideMask(string mask) {
        if (SceneCamera != null) {
            SceneCamera.cullingMask &= ~(1 << LayerMask.NameToLayer(mask));
        }
    }

    void ShowMask(string mask) {
        if (SceneCamera != null) {
            SceneCamera.cullingMask |= 1 << LayerMask.NameToLayer(mask);
        }
    }
}
