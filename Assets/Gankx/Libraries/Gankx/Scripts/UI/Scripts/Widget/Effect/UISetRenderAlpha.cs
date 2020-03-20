using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UISetRenderAlpha : MonoBehaviour {
    public CanvasRenderer m_Render;

    public float m_InitAlpha = 0;
    public float _TargetAlpha;

    void OnEnable() {
        if (m_Render == null)
            m_Render = GetComponent<CanvasRenderer>();
        if (m_Render == null) {
            enabled = false;
        }

        _TargetAlpha = m_InitAlpha;
        SetAlpha();
    }

    void Update() {
        SetAlpha();
    }

    void SetAlpha() {
        if (!Mathf.Approximately(m_Render.GetAlpha(), _TargetAlpha)) {
            m_Render.SetAlpha(_TargetAlpha);
        }
    }
}
