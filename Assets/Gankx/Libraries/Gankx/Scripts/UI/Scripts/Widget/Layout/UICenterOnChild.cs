using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Gankx;
using Gankx.UI;
using EasyLayout;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class UICenterOnChild : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {

    public float m_StartDelay = -1f;
    public float m_AnimTime = 0.5f;
    public bool _Snap = false;

    public RectTransform m_DefaultItem;
    public bool m_DefaultChangeItemSize = true;


    [Space(10)]
    [Range(1, 2)]
    public float m_Scale = 1.5f;

    public bool changeOffsetOnly = false;
    public float m_Offset_Y = 0;

    [HeaderAttribute("Change Scale require 'EasyLayout' as layout component.")]
    public bool m_ChangeScaleInsteadCellSize = false;

    private Dictionary<RectTransform, LayoutElement> m_LayoutDict = new Dictionary<RectTransform, LayoutElement>();

    private ScrollRect m_ScrollRect;
    private RectTransform m_MaskTransform;
    private RectTransform m_Content;
    private Vector3 m_MaskCenterPos;

    private List<RectTransform> m_AllChild = new List<RectTransform>();
    private Vector2 m_DefaultCellSize = new Vector2(160, 200);
    private float m_ChangeSizeDis = 0;
    private Vector2 m_SpaceSize = Vector2.zero;
    private float m_FixedLength = 0;

    private bool isHorizontal = true;

    public Action<List<RectTransform>> OnRefreshDataAction;
    public Action<RectTransform, float> OnSizeChangedAction;
    public Action<RectTransform, float> OnDoSizeChangedAction;

    private int curIndex = 0;

    private Vector3 curItemPosition;
    public float slideRatio = 0.5f;
    void OnEnable() {
        if(!started) return;
        StartCoroutine(DelayRefreshData());
    }

    IEnumerator DelayRefreshData() {
        if (m_Content == null) yield break;;
        if (m_StartDelay > 0.2f) {
            yield return new WaitForSeconds(m_StartDelay - 0.2f);
            ScrollTo(curIndex);
            yield return null;
            ScrollTo(curIndex);
        }
    }

    private void Awake() {
        m_ScrollRect = GetComponent<ScrollRect>();
        m_ScrollRect.movementType = ScrollRect.MovementType.Unrestricted;

        m_MaskTransform = m_ScrollRect.viewport;
        m_Content = m_ScrollRect.content;

        isHorizontal = m_ScrollRect.horizontal;
       
        if (m_DefaultItem == null && m_Content != null && m_Content.childCount > 0) {
            m_DefaultItem = m_Content.GetChild(0).GetComponent<RectTransform>();
        }

        if (m_DefaultItem != null) {
            m_DefaultCellSize = m_DefaultItem.sizeDelta;
        }

        if (m_Content != null) {
            if (m_Content.GetComponent<EasyLayout.EasyLayout>() != null) {
                EasyLayout.EasyLayout easyLayout = m_Content.GetComponent<EasyLayout.EasyLayout>();
                m_SpaceSize = easyLayout.Spacing;
                if (easyLayout.LayoutType == LayoutTypes.Compact) {
                    m_FixedLength = isHorizontal ? (m_SpaceSize.x / 2) : (m_SpaceSize.y / 2);
                }
                
            }
            else if (m_Content.GetComponent<HorizontalLayoutGroup>() != null) {
                m_DefaultCellSize.x = m_Content.GetComponent<HorizontalLayoutGroup>().spacing;
            } else if (m_Content.GetComponent<VerticalLayoutGroup>() != null) {
                m_DefaultCellSize.y = m_Content.GetComponent<VerticalLayoutGroup>().spacing;
            }
        }
    }

    private bool started = false;
    IEnumerator Start() {
        started = true;
        if (m_StartDelay < 0)
            // Wait for Rect Init
            yield return new WaitForEndOfFrame();
        else {
            yield return new WaitForSeconds(m_StartDelay);
        }

        Vector2 center = m_MaskTransform.rect.center;
        Vector3 canvasScale = GetComponentInParent<CanvasScaler>().transform.localScale;
        m_MaskCenterPos = m_MaskTransform.position + new Vector3(center.x * canvasScale.x, center.y * canvasScale.y, 0);

        if (isHorizontal)
            m_ChangeSizeDis = m_DefaultCellSize.x * canvasScale.x / 2;
        else {
            m_ChangeSizeDis = m_DefaultCellSize.y * canvasScale.y / 2;
        }

        RefreshData();
        ScrollTo(curIndex);
    }

    [ContextMenu("RefreshData")]
    public void RefreshData() {
        if(m_Content == null) return;
        m_AllChild.Clear();
        m_LayoutDict.Clear();
        for (int i = 0; i < m_Content.transform.childCount; i++) {
            if (!m_Content.GetChild(i).gameObject.activeSelf) continue;
            RectTransform child = m_Content.GetChild(i).GetComponent<RectTransform>();
            m_AllChild.Add(child);

            if (m_ChangeScaleInsteadCellSize && !changeOffsetOnly) {
                LayoutElement layout = m_Content.GetChild(i).GetOrAddComponent<LayoutElement>();
                m_LayoutDict.Add(child, layout);
            }
        }

        if (changeOffsetOnly) {
            EasyLayout.EasyLayout easyLayout = m_Content.GetComponent<EasyLayout.EasyLayout>();
            if (easyLayout != null) {
                Dictionary<Transform, float> offsetMap = new Dictionary<Transform, float>();
                for (int i = 0; i < m_Content.transform.childCount; i++) {
                    Transform child = m_Content.transform.GetChild(i);
                    offsetMap.Add(child, child.localPosition.y);
                }
                easyLayout.UpdateLayout();
                for (int i = 0; i < m_Content.transform.childCount; i++) {
                    Transform child = m_Content.transform.GetChild(i);
                    SetLocalPosY(child, offsetMap[child]);
                }
                easyLayout.enabled = false;
            }
        }

        if (OnRefreshDataAction != null) {
            OnRefreshDataAction(m_AllChild);
        }
    }

    void SetLocalPosY(Transform trans, float y) {
        Vector3 position = trans.localPosition;
        position.y = y;
        trans.localPosition = position;
    }

    [ContextMenu("ScrollTo")]
    public void ScrollTo() {
        ScrollTo(0);
    }

    public void DelayScrollTo(int index) {
        // ReSharper disable ConditionIsAlwaysTrueOrFalse
        if (this == null || !gameObject.activeSelf) return;
        // ReSharper restore ConditionIsAlwaysTrueOrFalse
        StartCoroutine(OnDelayScrollTo(index));
    }

    IEnumerator OnDelayScrollTo(int index) {
        yield return Yielders.EndOfFrame;
        ScrollTo(index);
    }

    
    public void ScrollTo(int index) {
        if (index < 0 || index >= m_AllChild.Count) {
            Debug.LogWarning("Invalid Index [" + index + "] try to Scroll to!");
            return;
        }

        curIndex = index;
        CenterOnItem(m_AllChild[index]);
        ResizeOnTarget(index, true);
    }

    public void ForceScrollTo(int index) {
        if (index < 0 || index >= m_AllChild.Count) {
            Debug.LogWarning("Invalid Index [" + index + "] try to Scroll to!");
            return;
        }

        curIndex = index;
        CenterOnItem(m_AllChild[index], true);
        ResizeOnTarget(index, true);
    }

    public void CenterOnItem(RectTransform target, bool forceSnap = false,int tarIndex = -1) {
        Vector3 newPos = GetCenterPosOnItem(target);
        
        if (_Snap || forceSnap) {
            m_Content.position = newPos;
            ResizeOnTarget(target, m_Scale, m_Offset_Y);
        } else {
            DOTween.To(() => m_Content.position, x => m_Content.position = x, newPos, m_AnimTime)
                .OnComplete(
                    () =>
                    {
                        if(tarIndex != -1)
                        {
                            curIndex = tarIndex;
                        }
                    });
        }
        m_ScrollRect.StopMovement();
    }

    public Vector3 GetCenterPosOnItem(RectTransform target) {
        int index = m_AllChild.IndexOf(target);
        if (index == -1) return m_Content.position;

        Vector3 targetLocalPos = m_Content.localPosition;
        if (isHorizontal)
            targetLocalPos.x = ((m_AllChild.Count - 1) / 2.0f - index) * (m_DefaultCellSize.x + m_SpaceSize.x) + (m_AllChild.Count > 1 ? m_FixedLength : 0);
        else
            targetLocalPos.y = (0.5f + index) * (m_DefaultCellSize.y + m_SpaceSize.y);
        return m_Content.transform.parent.TransformPoint(targetLocalPos);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        DOTween.KillAll();
        curIndex = FindClosestFrom(m_MaskCenterPos, m_AllChild);
        curItemPosition = m_AllChild[curIndex].position;
    }

    public void OnEndDrag(PointerEventData eventData) {
        int centerIndex = FindClosestFrom(m_MaskCenterPos, m_AllChild);
//        Debug.Log("Center On " + centerIndex);
        CenterOnItem(m_AllChild[centerIndex],false, centerIndex);
        ResizeOnTarget(centerIndex);

        m_AllChild[centerIndex].SendMessage("OnButtonClick", SendMessageOptions.DontRequireReceiver);


        if (LuaService.instance != null) {
            Window window = Window.GetWindow(gameObject);
            if (window != null)
            {
                LuaService.instance.FireEvent(
                    "OnPanelMessage", window.panelId, window.id, "OnCenterIndexUpdated", centerIndex);
            }
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (!m_DefaultChangeItemSize) return;

        for (int i = 0; i < m_AllChild.Count; i++) {
            RectTransform rect = m_AllChild[i];
            float dis = GetDistance(m_MaskCenterPos, rect.position);
            float checkDis = m_ChangeSizeDis * (m_Scale + 1);
            if (dis > checkDis) {
                ResizeOnTarget(rect , 1, 0);
            } else {
                float delta = dis / checkDis;
                ResizeOnTarget(rect, 1 + (m_Scale - 1) * (1 - delta), m_Offset_Y * (1 - delta));
            }
        }
    }

    int FindClosestFrom(Vector3 checkPos, List<RectTransform> rects) {
//                int index = -1;
//                float distance = Mathf.Infinity;
//        
//                for (int i = 0; i < rects.Count; i++) {
//                    RectTransform rect = rects[i];
//                    if (GetDistance(checkPos, rect.position) < distance) {
//                        distance = GetDistance(checkPos, rect.position);
//                        index = i;
//                    }
//                }

        int index = curIndex;
        float moveDistance = m_AllChild[curIndex].position.x - checkPos.x;
  
        Vector3 canvasScale = GetComponentInParent<CanvasScaler>().transform.localScale;
        
        // 滑的比较快的时候可能滑动多个
        int _addIndex = (int)(Mathf.Abs(moveDistance)/ (m_AllChild[curIndex].rect.width*canvasScale.x));

        if (moveDistance > slideRatio * m_AllChild[curIndex].rect.width * canvasScale.x)
        {
            index = index - _addIndex;
            if (index <= 0 || index - 1 <=0)
            {
                return 0;
            }
            return index - 1;

        }
        if (moveDistance < -slideRatio * m_AllChild[curIndex].rect.width * canvasScale.x)
        {
            index = index + _addIndex;
            if (index >= m_AllChild.Count-1 || index + 1 >= m_AllChild.Count - 1)
            {
                return m_AllChild.Count - 1;
            }
            return index + 1 ;
        }
        return index;
    }

    float GetDistance(Vector3 start, Vector3 end) {
        if (isHorizontal) return Mathf.Abs((start - end).x);
        return Mathf.Abs((start - end).y);
    }

    LayoutElement GetLayoutElemnt(RectTransform rect) {
        if (m_LayoutDict.ContainsKey(rect))
            return m_LayoutDict[rect];
        return null;
    }

    void ResizeOnTarget(RectTransform target, float scale, float offsetY) {
        if (!m_DefaultChangeItemSize) return;

        if (changeOffsetOnly) {
            target.anchoredPosition = new Vector2(target.anchoredPosition.x, offsetY);
            return;
        }
        if (m_ChangeScaleInsteadCellSize) {
            target.localScale = Vector3.one * scale;
            if (GetLayoutElemnt(target) != null) {
                if(isHorizontal)
                    GetLayoutElemnt(target).flexibleWidth = (m_DefaultCellSize * scale).x;
                else {
                    GetLayoutElemnt(target).flexibleHeight = (m_DefaultCellSize * scale).y;
                }
            }
        }
        else {
            target.sizeDelta = m_DefaultCellSize * scale;
        }

        if (OnSizeChangedAction != null) {
            OnSizeChangedAction(target, scale - 1);
        }
    }

    void DoResizeOnTarget(RectTransform target, float scale, float offsetY) {
        if (!m_DefaultChangeItemSize) return;

        if (changeOffsetOnly)
        {
//            Debug.Log("set" + target + " to offsety " + offsetY, target);
            target.DOAnchorPosY(offsetY, m_AnimTime);
            return;
        }

        if (m_ChangeScaleInsteadCellSize) {
            DOTween.To(() => target.localScale, x => target.localScale = x, Vector3.one * scale, m_AnimTime);

            if (GetLayoutElemnt(target) != null) {
                LayoutElement element = GetLayoutElemnt(target);
                if (isHorizontal)
                    DOTween.To(() => element.flexibleWidth, x => element.flexibleWidth = x, (m_DefaultCellSize * scale).x, m_AnimTime);
                else {
                    DOTween.To(() => element.flexibleHeight, x => element.flexibleHeight = x, (m_DefaultCellSize * scale).y, m_AnimTime);
                }
            }
        }
        else
            DOTween.To(() => target.sizeDelta, x => target.sizeDelta = x, m_DefaultCellSize * scale, m_AnimTime);

        if (OnDoSizeChangedAction != null) {
            OnDoSizeChangedAction(target, scale - 1);
        }
    }

    public void ResizeOnTarget(int currentIndex, bool checkAll = false, bool forceSnap = false) {
        if(!m_DefaultChangeItemSize) return;

        if (checkAll) {
            for (int i = 0; i < m_AllChild.Count; i++) {
                float size = i == currentIndex ? m_Scale : 1;
                float offsetY = i == currentIndex ? m_Offset_Y : 0;
                if (_Snap || forceSnap) {
                    ResizeOnTarget(m_AllChild[i], size, offsetY);
                } else {
                    DoResizeOnTarget(m_AllChild[i], size, offsetY);
                }
            }
            return;
        }

        if (_Snap || forceSnap) {
            if (currentIndex > 0) ResizeOnTarget(m_AllChild[currentIndex - 1], 1, 0);
            if (currentIndex < m_AllChild.Count - 1) ResizeOnTarget(m_AllChild[currentIndex + 1], 1, 0);

            ResizeOnTarget(m_AllChild[currentIndex], m_Scale, m_Offset_Y);
        } else {
            if (currentIndex > 0) DoResizeOnTarget(m_AllChild[currentIndex - 1], 1, 0);
            if (currentIndex < m_AllChild.Count - 1) DoResizeOnTarget(m_AllChild[currentIndex + 1], 1, 0);

            DoResizeOnTarget(m_AllChild[currentIndex], m_Scale, m_Offset_Y);
        }
    }
}