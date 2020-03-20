using Gankx;
using Gankx.UI;
using UnityEngine;
using UnityEngine.UI;


public interface IUISoftNewObjectHandler
{
    void OnNewObjectLoaded(GameObject obj);
}

/// <inheritdoc>
///     <cref></cref>
/// </inheritdoc>
//[ExecuteInEditMode]
// ReSharper disable once CheckNamespace
// ReSharper disable once InconsistentNaming
[RequireComponent(typeof(RectMask2D))]
public class UISoftClip : MonoBehaviour, IUISoftNewObjectHandler
{
    public Vector4 SoftClip;

    // Use this for initialization
    protected void Start ()
    {
        if (Application.isPlaying)
            UpdateGraphic(gameObject);
    }

    protected void UpdateGraphic(GameObject obj)
    {
        Init();
        var graphics = obj.GetComponentsInChildren<MaskableGraphic>(true);
        foreach (var graphic in graphics) {
            if (graphic.material == null) continue;

            string matName = graphic.material.name;
            if (matName == "UIGrayEffect") {
                graphic.material = _matGray;
            }
            else if (matName == "Default UI Material") {
                graphic.material = _matBase;
            }
            else {
                graphic.material.EnableKeyword("UI_SOFT_CLIP");
                graphic.material.SetVector("_ClipSoft", SoftClip);
            }
        }
        UpdateMat();
    }

    public void OnNewObjectLoaded(GameObject obj)
    {
        if (Application.isPlaying)
        {
            UpdateGraphic(obj);
        }        
    }

    protected void UpdateMat()
    {
        if(_matBase != null)
            _matBase.SetVector("_ClipSoft", SoftClip);
        if (_matGray != null)
            _matGray.SetVector("_ClipSoft", SoftClip);

    }

#if UNITY_EDITOR
    protected void Update()
    {
        UpdateMat();
    }
#endif

    //    private readonly List<IClippable> _clipTargets = new List<IClippable>();
    private Material _matBase;
    private Material _matGray;

    public Material MatBase {
        get
        {
            if (null == _matBase)
            {
                Init();
            }
            return _matBase;
        }
    }

    public Material MatGray
    {
        get
        {
            if (null == _matGray)
            {
                Init();
            }
            return _matGray;
        }
    }

    protected  void OnDestroy()
    {
        Destroy(_matBase);
        Destroy(_matGray);

    }

    protected  void Awake()
    {
        Init();
    }


    private bool _init = false;
    protected void Init()
    {
        if (_init)
        {
            return;
        }
        _matBase = Instantiate(ResourceService.Load<Material>("common/mat/UISoftClip"));
        if (_matBase == null)
        {
            Debug.LogError("common/mat/UISoftClip is null", gameObject);
        }
        if (_matBase != null) _matBase.name = "UISoftClip";
        //        if (_matBase != null) _matBase.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
        _matGray = Instantiate(ResourceService.Load<Material>("common/mat/UISoftClipGrayEffect"));
        //        if (_matGray != null) _matGray.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
        if (_matGray == null)
        {
            Debug.LogError("common/mat/UISoftClipGrayEffect", gameObject);
        }
        if (_matGray != null) _matGray.name = "UISoftClipGrayEffect";

        _init = true;
    }
    //    protected override void OnEnable()
    //    {
    //        base.OnEnable();
    ////        m_ShouldRecalculateClipRects = true;
    //        ClipperRegistry.Register(this);
    //        Notify2DMaskStateChanged(this);
    //    }
    //
    //    protected override void OnDisable()
    //    {
    //        // we call base OnDisable first here
    //        // as we need to have the IsActive return the
    //        // correct value when we notify the children
    //        // that the mask state has changed.
    //        base.OnDisable();
    //        //        m_ClipTargets.Clear();
    //        //        m_Clippers.Clear();
    //        _clipTargets.Clear();
    //        ClipperRegistry.Unregister(this);
    //        Notify2DMaskStateChanged(this);
    //    }
    //
    //    public static void Notify2DMaskStateChanged(Component mask)
    //    {
    //        var components = ListPool<Component>.Get();
    //        mask.GetComponentsInChildren(components);
    //        foreach (var t in components)
    //        {
    //            if (t == null || t.gameObject == mask.gameObject)
    //                continue;
    //
    //            var toNotify = t as IClippable;
    //            if (toNotify != null)
    //                toNotify.RecalculateClipping();
    //        }
    //        ListPool<Component>.Release(components);
    //    }
    //
    //    public new void AddClippable(IClippable clippable)
    //    {
    //        if (clippable == null)
    //            return;
    //
    //        if (!_clipTargets.Contains(clippable))
    //            _clipTargets.Add(clippable);
    //
    ////        clippable.SetClipRect(m_LastClipRectCanvasSpace, m_LastClipRectValid);
    ////        clippable.Cull(m_LastClipRectCanvasSpace, m_LastClipRectValid);
    //    }
    //
    //    public new void RemoveClippable(IClippable clippable)
    //    {
    //        if (clippable == null)
    //            return;
    //
    //        //        clippable.SetClipRect(new Rect(), false);
    //        _clipTargets.Remove(clippable);
    //    }
    //
    //    /// <inheritdoc />
    //    public override void PerformClipping()
    //    {
    ////        // if the parents are changed
    ////        // or something similar we
    ////        // do a recalculate here
    ////        if (m_ShouldRecalculateClipRects)
    ////        {
    ////            MaskUtilities.GetRectMasksForClip(this, m_Clippers);
    ////            m_ShouldRecalculateClipRects = false;
    ////        }
    ////
    ////        // get the compound rects from
    ////        // the clippers that are valid
    ////        bool validRect = true;
    ////        Rect clipRect = Clipping.FindCullAndClipWorldRect(m_Clippers, out validRect);
    ////        if (clipRect != m_LastClipRectCanvasSpace)
    ////        {
    ////            for (int i = 0; i < m_ClipTargets.Count; ++i)
    ////                m_ClipTargets[i].SetClipRect(clipRect, validRect);
    ////
    ////            m_LastClipRectCanvasSpace = clipRect;
    ////            m_LastClipRectValid = validRect;
    ////        }
    ////
    ////        for (int i = 0; i < m_ClipTargets.Count; ++i)
    ////            m_ClipTargets[i].Cull(m_LastClipRectCanvasSpace, m_LastClipRectValid);
    //    }

}
