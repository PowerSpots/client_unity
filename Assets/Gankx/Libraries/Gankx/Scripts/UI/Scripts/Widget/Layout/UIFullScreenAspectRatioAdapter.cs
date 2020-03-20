using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[AddComponentMenu("Layout/UI Full Screen Aspect Ratio Adapter", 200)]
[ExecuteInEditMode]
[RequireComponent(typeof (RectTransform))]
[DisallowMultipleComponent]
// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedMember.Global
// ReSharper disable once CheckNamespace
public class UIFullScreenAspectRatioAdapter : UIBehaviour, ILayoutSelfController
{
    // ReSharper disable once InconsistentNaming
    public enum UIFullScreenAspectMode
    {
        None,
        PerfferWidth,
        PerfferHeight
    }


    [SerializeField] private UIFullScreenAspectMode _aspectMode = UIFullScreenAspectMode.None;

    [SerializeField] private float _aspectRatio = 1f;

    private bool _delayedSetDirty;

    [NonSerialized] private RectTransform _rect;

#pragma warning disable 649
    private DrivenRectTransformTracker _tracker;
#pragma warning restore 649

    public UIFullScreenAspectMode AspectMode
    {
        get { return _aspectMode; }
        set
        {
            _aspectMode = value;
            SetDirty();
        }
    }

    public float AspectRatio
    {
        get { return _aspectRatio; }
        set
        {
            _aspectRatio = value;
            SetDirty();
        }
    }

    private RectTransform RectTransform
    {
        get
        {
            if (_rect == null)
                _rect = GetComponent<RectTransform>();
            return _rect;
        }
    }

    public virtual void SetLayoutHorizontal()
    {
    }

    public virtual void SetLayoutVertical()
    {
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _delayedSetDirty = true;
    }

    protected override void OnDisable()
    {
        _tracker.Clear();
        LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
        base.OnDisable();
    }

    protected virtual void Update()
    {
        if (!_delayedSetDirty)
            return;
        _delayedSetDirty = false;
        SetDirty();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        UpdateRect();
    }

    protected void UpdateRect()
    {
        if (!IsActive())
            return;
        _tracker.Clear();
        var pSize = GetParentSize();
        switch (_aspectMode)
        {
            case UIFullScreenAspectMode.None:
                if (Application.isPlaying)
                    break;
               _aspectRatio = Mathf.Clamp(RectTransform.rect.width / RectTransform.rect.height, 1f / 1000f, 1000f);
                break;
            case UIFullScreenAspectMode.PerfferWidth:
                _tracker.Add(this, RectTransform, DrivenTransformProperties.SizeDeltaY);
                if (pSize.x/pSize.y <= _aspectRatio)
                {
                    FitWidth(pSize);
                }
                else
                {
                    FitHeight(pSize);
                }
                break;
            case UIFullScreenAspectMode.PerfferHeight:
                _tracker.Add(this, RectTransform, DrivenTransformProperties.SizeDeltaX);
                if (pSize.x/pSize.y <= _aspectRatio)
                {
                    FitHeight(pSize);
                }
                else
                {
                    FitWidth(pSize);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected void FitWidth(Vector2 pSize)
    {
        RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pSize.x);
        RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, RectTransform.rect.width / _aspectRatio);
    }

    protected void FitHeight(Vector2 pSize)
    {
        RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pSize.y);
        RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, RectTransform.rect.height * _aspectRatio);
    }
    protected void SetDirty()
    {
        UpdateRect();
    }
#if UNITY_EDITOR
    protected override void OnValidate()
    {
        _aspectRatio = Mathf.Clamp(_aspectRatio, 1f / 1000f, 1000f);
        SetDirty();
    }
#endif


    private Vector2 GetParentSize()
    {
        var parent = RectTransform.parent as RectTransform;            
        return parent != null ? parent.rect.size : Vector2.zero;
    }
}