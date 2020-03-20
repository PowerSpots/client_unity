using Gankx;
using Gankx.UI;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

/// <summary>
/// 2D UI 跟随3D的物体
/// </summary>
[ExecuteInEditMode]
public class Follow3DObject : MonoBehaviour
{
    private TransformDirtyTracker transformDirtyTracker;
    private Transform prevAttachTrans;
    private bool isDirty = false;

    public Transform AttachTrans = null;
    public string AttachPointName = null;
    //跟随点与跟随目标的偏移量
    public Vector3 Offset3D;
    //在映射到2d之后的偏移量
    public Vector2 Offset2D;

    public bool IsFollowOverlay = false;

    //仅使用一帧来定位
    public bool IsStay = false;

    // 单纯的和目标点位置一致
    [Tooltip("单纯的和目标点位置一致")]
    public bool AttachToTargetOnly = false;
    public UIFollowGameObjectKeepSize keepSize;

    private TransformDirtyTracker cameraDirtyTracker;
    //渲染目标的源相机,默认为主摄像机
    private Camera sourceCamera;
    public Camera SourceCamera
    {
        get
        {
            return sourceCamera;
        }
        set
        {
            sourceCamera = value;
            if (null != sourceCamera)
            {
                cameraDirtyTracker = sourceCamera.GetOrAddComponent<TransformDirtyTracker>();
            }            
            isDirty = true;
        }
    }

    private Camera screenSpaceCamera
    {
        get
        {
            if (null == Canvas)
            {
                return null;
            }

            if (canvas.renderMode != RenderMode.ScreenSpaceCamera)
            {
                return null;
            }

            return canvas.worldCamera;
        }
    }

    private bool isOverlay
    {
        get
        {
            if (null == Canvas)
            {
                return false;
            }

            return Canvas.renderMode == RenderMode.ScreenSpaceOverlay;
        }
    }

    //private CanvasScaler canvasScaler;
    private Vector3 canvasPosition;
    private Vector3 canvasLocalScale;
    private RectTransform rectTransform;
    private Canvas canvas;
    public Canvas Canvas
    {
        get
        {
            return canvas;
        }
        set
        {
            canvas = value;
            isDirty = true;
            //canvasScaler = canvas.GetComponent<CanvasScaler>();
            rectTransform = canvas.GetComponent<RectTransform>();
            canvasPosition = canvas.transform.position;
            canvasLocalScale = canvas.transform.localScale;
        }
    }    

    private Vector3 outScreenWorldPos = new Vector3(100000, 100000);

    private Follow3DLayers follow3DLayers;    
    private void Awake()
    {
        follow3DLayers = GetComponent<Follow3DLayers>();
    }

    void OnScaleVisibleChange(bool visible)
    {
        enabled = visible;
    }

    private void Start()
    {
        UpdateUIPos();

        if (IsStay)
            Destroy(this);
    }

    private void OnEnable()
    {
        isDirty = true;
        Window window = Window.GetWindow(gameObject);
        if (null != window)
        {
            enabled = window.scaleVisible;
        }
    }

    [ContextMenu("LateUpdate")]
    void LateUpdate()
    {            
        if (IsStay)
        {
            UpdateUIPos();
            Destroy(this);
            return;
        }        

        if (transform.hasChanged)
        {
            UpdateUIPos();
            transform.hasChanged = false;
            return;
        }

        if (prevAttachTrans != AttachTrans)
        {
            UpdateUIPos();
            if (null == AttachTrans)
            {
                transformDirtyTracker = null;
            }
            else
            {
                transformDirtyTracker = AttachTrans.GetOrAddComponent<TransformDirtyTracker>();
            }

            prevAttachTrans = AttachTrans;

            return;
        }

        if (SourceCamera == null && !IsFollowOverlay)
        {
            UpdateUIPos();            
            return;
        }

        if (isDirty)
        {
            UpdateUIPos();            
            return;
        }

        // 相机和AttachTrans改变，更新
        if (null != cameraDirtyTracker && cameraDirtyTracker.hasChanged)
        {
            UpdateUIPos();
            return;
        }

        if (null != transformDirtyTracker && transformDirtyTracker.hasChanged)
        {
            UpdateUIPos();
            return;
        }
    }

    public void UpdateUIPos()
    {
        isDirty = false;        

        if (SourceCamera == null && !IsFollowOverlay)
        {
            SourceCamera = Camera.main;
        }

        if (null != AttachTrans && (null != sourceCamera || IsFollowOverlay))
        {
            Vector3 sourceOffsetPosition = AttachTrans.position + Offset3D;
            // 非world canvas
            if (null != screenSpaceCamera || isOverlay)
            {                
                Vector3 viewportPosition;

                if (!IsFollowOverlay && !IsTargetInCameraForward(AttachTrans, sourceCamera))
                {
                    transform.position = outScreenWorldPos;
                    return;
                }
                else
                {                    
                    if (null != follow3DLayers && null != follow3DLayers.followLayers && follow3DLayers.followLayers.Length > 0)
                    {
                        for (int i = 0; i < follow3DLayers.followLayers.Length; i++)
                        {
                            if (follow3DLayers.followLayers[i].value == (1 << AttachTrans.gameObject.layer))
                            {
                                break;
                            }
                        }

                        // 不是follow的layer，那么移出屏幕
                        transform.position = outScreenWorldPos;
                        return;
                    }

                    if (null != follow3DLayers && null != follow3DLayers.dontFollowLayers && follow3DLayers.dontFollowLayers.Length > 0)
                    {
                        for (int i = 0; i < follow3DLayers.dontFollowLayers.Length; i++)
                        {
                            if (follow3DLayers.dontFollowLayers[i].value == (1 << AttachTrans.gameObject.layer))
                            {
                                // 不是follow的layer，那么移出屏幕
                                transform.position = outScreenWorldPos;
                                return;
                            }
                        }
                    }

                    viewportPosition = ScreenRTMgr.WorldToViewportPoint(sourceCamera, sourceOffsetPosition);
                }

                //                Vector2 pos;
                float posX = (viewportPosition.x - 0.5f) * rectTransform.GetWidth();
                float posY = (viewportPosition.y - 0.5f) * rectTransform.GetHeight();
                //                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, screenSpaceCamera, out pos)) {
                transform.position = canvasPosition +
                                        new Vector3((posX + Offset2D.x) * canvasLocalScale.x,
                                            (posY + Offset2D.y) * canvasLocalScale.y, 0);
//                }
            }
            else if (AttachToTargetOnly) {
                transform.position = sourceOffsetPosition;
                if (keepSize != null) {
                    keepSize.UpdateLoacalScale(AttachTrans);
                }
            }
            else
            {
                if (!Util.CheckInCameraView(sourceCamera, sourceOffsetPosition)) {
                    transform.position = canvas.worldCamera.transform.position;
                    return;
                }
                // 如果是非屏幕空间相机渲染，那么直接复制3D坐标
                
                Matrix4x4 V1 = sourceCamera.worldToCameraMatrix;
                Matrix4x4 P1 = sourceCamera.projectionMatrix;
                Matrix4x4 V2 = canvas.worldCamera.worldToCameraMatrix;
                Matrix4x4 P2 = canvas.worldCamera.projectionMatrix;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                // Invert Y for rendering to a render texture
                for (int i = 0; i < 4; i++)
                {
                    P1[1, i] = -P1[1, i];
                    P2[1, i] = -P2[1, i];
                }
                // Scale and bias from OpenGL -> D3D depth range
                for (int i = 0; i < 4; i++)
                {
                    P1[2, i] = P1[2, i] * 0.5f + P1[3, i] * 0.5f;
                    P2[2, i] = P2[2, i] * 0.5f + P2[3, i] * 0.5f;
                }
#endif
                //Matrix4x4 MVP = P * V * M;
                Matrix4x4 VP1 = P1 * V1;
                Matrix4x4 VP2 = P2 * V2;

                Vector4 pos = VP1 * new Vector4(sourceOffsetPosition.x, sourceOffsetPosition.y, sourceOffsetPosition.z, 1f);
                pos /= pos.w;

                Vector4 newWorldPos = VP2.inverse * new Vector4(pos.x, pos.y, pos.z, 1f);
                newWorldPos = newWorldPos / newWorldPos.w;

                transform.position = newWorldPos;
                transform.forward = canvas.worldCamera.transform.forward;
            }
        }
    }

    bool IsTargetInCameraForward(Transform target,Camera camera)
    {
        Vector3 offset = target.position - camera.transform.position;
        return Vector3.Dot(camera.transform.forward, offset) > 0;
    }
}
