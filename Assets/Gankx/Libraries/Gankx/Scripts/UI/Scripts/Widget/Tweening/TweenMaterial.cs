// #undef UNITY_EDITOR

using Gankx.UI;
using UnityEngine;
using UnityEngine.UI;

public abstract class TweenMaterial : Tweener {

    public string Keyword = string.Empty;

    private int mid = 0;
    public Material mMaterial;

    public Material cachedMaterial {
        get {
            if (mMaterial == null) {
                Renderer renderer = GetComponent<Renderer>();
                if (renderer != null) {
#if UNITY_EDITOR
                    if (!Application.isPlaying) {
                        mMaterial = renderer.sharedMaterial;
                    }
                    else
#endif
                        mMaterial = renderer.material;
                }
            }
            return mMaterial;
        }
    }

    protected int id {
        get {
            if (mid == 0) {
                mid = Shader.PropertyToID(Keyword);
            }
            return mid;
        }
    }
}