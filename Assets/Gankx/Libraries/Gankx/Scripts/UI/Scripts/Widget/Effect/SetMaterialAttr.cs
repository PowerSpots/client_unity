using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetMaterialAttr : MonoBehaviour {


    private IEnumerable<MeshRenderer> GetMateralMeshRenderers()
    {
        return GetComponentsInChildren<MeshRenderer>(true);
    }

    public void SetMaterialAlpha(float a, float duration)
    {
        StopAllCoroutines();
        if (duration <= 0)
        {
            _SetMaterialAlpha(a);
        }
        else
        {
            StartCoroutine(Mathf.Approximately(0, a)
                ? __SetMaterialAlpha(1, 0, duration)
                : __SetMaterialAlpha(0, 1, duration));
        }
    }

    private IEnumerator __SetMaterialAlpha(float start, float end, float dura)
    {
        _SetMaterialAlpha(start);
        var time = 0f;
        while (time < dura)
        {
            yield return null;
//            Debug.Log(time);
            time += Time.deltaTime;
            _SetMaterialAlpha(Mathf.Lerp(start,end,time/dura));
        }
    }

    private void _SetMaterialAlpha(float a)
    {
        var meshRenderers = GetMateralMeshRenderers();
        foreach (var ren in meshRenderers)
        {
            foreach (var mat in ren.materials)
            {
                if (!mat.HasProperty("_Color")) continue;
                var c = mat.color;
                c.a = a;
                mat.color = c;
            }
        }
    }

#if UNITY_EDITOR

    public float MaterialAlpha = 1f;

    [ContextMenu("SetMaterialAlpha")]
    [UsedImplicitly]
    public void Test()
    {
        SetMaterialAlpha(MaterialAlpha,2);
    }

#endif


}
