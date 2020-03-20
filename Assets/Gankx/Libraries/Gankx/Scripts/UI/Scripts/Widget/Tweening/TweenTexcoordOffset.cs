using Gankx.UI;
using UnityEngine;

public class TweenTexcoordOffset : Tweener
{
    [Range(-1, 1)]
    public float FromTexcoordS = 0;

    [Range(-1, 1)]
    public float FromTexcoordT = 0;

    [Range(-1, 1)]
    public float ToTexcoordS = 0;

    [Range(-1, 1)]
    public float ToTexcoordT = 0;

    protected override void Start()
    {
        mRenderer = GetComponent<Renderer>();
        base.Start();
    }

    Material GetTargetMaterial()
    {
    #if UNITY_EDITOR
        if (Application.isPlaying == false)
        {
            return GetComponent<Renderer>().sharedMaterial;
        }
    #endif
        return GetComponent<Renderer>().material;
    }

    /// <summary>
    /// Interpolate the position, scale, and rotation.
    /// </summary>
    protected override void OnUpdate(float factor, bool isFinished)
    {
        if (mRenderer != null)
        {
            var s = Mathf.Lerp(FromTexcoordS, ToTexcoordS, factor);
            var t = Mathf.Lerp(FromTexcoordT, ToTexcoordT, factor);

            mRenderer.material.mainTextureOffset = new Vector2(s,t);
        }
    }

    [ContextMenu("Set 'From' to current value")]
    public override void SetStartToCurrentValue()
    {
        var offset = GetTargetMaterial().mainTextureOffset;
        FromTexcoordS = offset.x;
        FromTexcoordT = offset.y;
    }

    [ContextMenu("Set 'To' to current value")]
    public override void SetEndToCurrentValue()
    {
        var offset = GetTargetMaterial().mainTextureOffset;
        ToTexcoordS = offset.x;
        ToTexcoordT = offset.y;
    }

    [ContextMenu("Assume value of 'From'")]
    void SetCurrentValueToStart()
    {
        GetTargetMaterial().mainTextureOffset = new Vector2(FromTexcoordS, FromTexcoordT);
    }

    [ContextMenu("Assume value of 'To'")]
    void SetCurrentValueToEnd()
    {
        GetTargetMaterial().mainTextureOffset = new Vector2(ToTexcoordS, ToTexcoordT);
    }

    private Renderer mRenderer = null;
}
