using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(UICenterOnChild))]
public class UICenterOnChildChangeAlpha : MonoBehaviour {

    private UICenterOnChild m_UICenterOnChild;

    public float m_MinAlpha = 0.7f;
    public float m_MaxAlpha = 1.0f;

    private float m_MaxSize;
    public Dictionary<RectTransform, CanvasGroup> m_CanvasGroupMap = new Dictionary<RectTransform, CanvasGroup>();

    void Awake() {
        if (m_UICenterOnChild == null) {
            m_UICenterOnChild = GetComponent<UICenterOnChild>();
        }

        m_UICenterOnChild.OnRefreshDataAction += OnRefreshDataAction;
        m_UICenterOnChild.OnSizeChangedAction += OnSizeChangedAction;
        m_UICenterOnChild.OnDoSizeChangedAction += OnDoSizeChangedAction;
    }

    void OnDestroy() {
        m_UICenterOnChild.OnRefreshDataAction -= OnRefreshDataAction;
        m_UICenterOnChild.OnSizeChangedAction -= OnSizeChangedAction;
        m_UICenterOnChild.OnDoSizeChangedAction -= OnDoSizeChangedAction;
    }

    private void OnDoSizeChangedAction(RectTransform target, float scale) {
        if (!m_CanvasGroupMap.ContainsKey(target) || m_CanvasGroupMap[target] == null) return;
        DOTween.To(() => m_CanvasGroupMap[target].alpha, x => m_CanvasGroupMap[target].alpha = x, m_MinAlpha + scale / m_MaxSize * (m_MaxAlpha - m_MinAlpha), m_UICenterOnChild.m_AnimTime);
    }

    private void OnSizeChangedAction(RectTransform target, float scale) {
        if(!m_CanvasGroupMap.ContainsKey(target) || m_CanvasGroupMap[target] == null) return;
        m_CanvasGroupMap[target].alpha = m_MinAlpha + scale / m_MaxSize * (m_MaxAlpha - m_MinAlpha);
    }

    // Use this for initialization
	void OnRefreshDataAction(List<RectTransform> target) {
        if(target == null) return;
	    m_CanvasGroupMap.Clear();
	    for (int i = 0; i < target.Count; i++) {
	        m_CanvasGroupMap.Add(target[i], target[i].GetComponent<CanvasGroup>());
        }
	    m_MaxSize = m_UICenterOnChild.m_Scale - 1;
	}
}
