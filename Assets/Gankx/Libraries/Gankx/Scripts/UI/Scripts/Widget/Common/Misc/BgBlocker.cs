using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <inheritdoc>
///     <cref />
/// </inheritdoc>
// ReSharper disable once CheckNamespace
public class BgBlocker : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
    IPointerUpHandler, IPointerClickHandler, IBeginDragHandler, IInitializePotentialDragHandler, IDragHandler,
    IEndDragHandler, IDropHandler, IScrollHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler,
    IMoveHandler, ISubmitHandler, ICancelHandler
{

#region Interface
    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UpdateTargetGameObject(eventData);
        ExecuteEvents.ExecuteHierarchy(TargetGo, eventData, ExecuteEvents.pointerEnterHandler);
        ExecuteEvents.ExecuteHierarchy(TargetGo, eventData, ExecuteEvents.pointerDownHandler);

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ExecuteEvents.ExecuteHierarchy(TargetGo, eventData, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.ExecuteHierarchy(TargetGo, eventData, ExecuteEvents.pointerExitHandler);
        //        ClearTargetGameObject(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ExecuteEvents.ExecuteHierarchy(TargetGo, eventData, ExecuteEvents.pointerClickHandler);

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ExecuteEvents.ExecuteHierarchy(TargetGo, eventData, ExecuteEvents.beginDragHandler);
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        ExecuteEvents.ExecuteHierarchy(TargetGo, eventData, ExecuteEvents.initializePotentialDrag);
    }

    public void OnDrag(PointerEventData eventData)
    {
        ExecuteEvents.ExecuteHierarchy(TargetGo, eventData, ExecuteEvents.dragHandler);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ExecuteEvents.ExecuteHierarchy(TargetGo, eventData, ExecuteEvents.endDragHandler);
    }

    public void OnDrop(PointerEventData eventData)
    {
    }

    public void OnScroll(PointerEventData eventData)
    {
        ExecuteEvents.ExecuteHierarchy(TargetGo, eventData, ExecuteEvents.scrollHandler);
    }

    public void OnUpdateSelected(BaseEventData eventData)
    {
    }

    public void OnSelect(BaseEventData eventData)
    {
        ExecuteEvents.Execute(TargetGo, eventData, ExecuteEvents.selectHandler);
        
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ExecuteEvents.Execute(TargetGo, eventData, ExecuteEvents.deselectHandler);
    }

    public void OnMove(AxisEventData eventData)
    {
        ExecuteEvents.Execute(TargetGo, eventData, ExecuteEvents.moveHandler);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        ExecuteEvents.ExecuteHierarchy(TargetGo, eventData, ExecuteEvents.submitHandler);
    }

    public void OnCancel(BaseEventData eventData)
    {
        ExecuteEvents.ExecuteHierarchy(TargetGo, eventData, ExecuteEvents.cancelHandler);
    }
#endregion

    private BaseEventData _temp;
    protected GameObject TargetGo;
    private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();

    protected void UpdateTargetGameObject(BaseEventData eventData)
    {
        _temp = eventData;
        EventSystem.current.RaycastAll(eventData as PointerEventData, _raycastResults);

        if (_raycastResults.Count > 1 && _raycastResults[0].gameObject == gameObject)
        {
            TargetGo = _raycastResults[1].gameObject;
        }
        _raycastResults.Clear();
    }

    protected void ClearTargetGameObject(BaseEventData eventData)
    {
        TargetGo = null;
    }

#if UNITY_EDITOR
    public void Test()
    {

        EventSystem.current.RaycastAll(_temp as PointerEventData, _raycastResults);

        foreach (var raycastResult in _raycastResults)
        {
            Debug.Log(raycastResult.gameObject, raycastResult.gameObject);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Test();
        }
    }
#endif

}
