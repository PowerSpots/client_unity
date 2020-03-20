using System;
using Gankx;
using Gankx.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLua;

public static class UIWindowExport
{
    public static uint INVALID_ID = Window.InvalidId;
    public static bool UseCache = true;

    public static void SetObjectName(uint windowId, string name)
    {
        var windowObject = PanelService.GetWindowObject(windowId);
        if (null == windowObject)
        {
            return;
        }

        windowObject.name = name;
    }

    public static string GetObjectName(uint windowId)
    {
        var windowObject = PanelService.GetWindowObject(windowId);
        if (null == windowObject)
        {
            return string.Empty;
        }

        return windowObject.name;
    }

    public static uint GetPanelId(uint windowId)
    {
        var windowObject = PanelService.GetWindowObject(windowId);
        if (null == windowObject)
        {
            return INVALID_ID;
        }

        var window = Window.GetWindow(windowObject);
        if (null == window)
        {
            return INVALID_ID;
        }

        return window.panelId;
    }

    public static void SetVisible(uint windowId, bool visible)
    {
        var windowObject = PanelService.GetWindowObject(windowId);
        if (null == windowObject)
        {
            return;
        }

        windowObject.SetActive(visible);
    }

    public static bool GetVisible(uint windowId)
    {
        var windowObject = PanelService.GetWindowObject(windowId);
        if (null == windowObject)
        {
            return false;
        }

        return windowObject.activeSelf;
    }

    public static bool GetVisibleInHierarchy(uint windowId)
    {
        var windowObject = PanelService.GetWindowObject(windowId);
        if (null == windowObject)
        {
            return false;
        }

        return windowObject.activeInHierarchy;
    }

    public static bool GetVisibleAndRendered(uint windowId, float xScale, float yScale)
    {
        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            return false;
        }

        return uiWindow.GetVisibleAndRendered(xScale, yScale);
    }

    public static void SetSelected(uint windowId, bool selected)
    {
        var windowObject = PanelService.GetWindowObject(windowId);
        if (null == windowObject)
        {
            return;
        }

        if (selected)
        {
            EventSystem.current.SetSelectedGameObject(windowObject);
        }
        else
        {
            if (EventSystem.current.currentSelectedGameObject == windowObject)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }

    public static bool GetSelected(uint windowId)
    {
        var windowObject = PanelService.GetWindowObject(windowId);
        if (null == windowObject)
        {
            return false;
        }

        return EventSystem.current.currentSelectedGameObject == windowObject;
    }

    public static void SetWidth(uint windowId, float width)
    {
        var rectTrans = PanelService.GetWindowComponent<RectTransform>(windowId, true);
        if (null == rectTrans)
        {
            return;
        }

        rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    public static float GetWidth(uint windowId)
    {
        var rectTrans = PanelService.GetWindowComponent<RectTransform>(windowId, true);
        if (null == rectTrans)
        {
            return 0;
        }


        return rectTrans.rect.width;
    }

    public static void SetAnchorPosition(uint windowId, float x, float y)
    {
        var rectTrans = PanelService.GetWindowComponent<RectTransform>(windowId, true);
        if (null == rectTrans)
        {
            return;
        }

        rectTrans.anchoredPosition = new Vector2(x, y);
    }

    public static void SetAnchorPositionX(uint windowId, float x)
    {
        var rectTrans = PanelService.GetWindowComponent<RectTransform>(windowId, true);
        if (null == rectTrans)
        {
            return;
        }

        rectTrans.anchoredPosition = new Vector2(x, rectTrans.anchoredPosition.y);
    }

    public static void SetAnchorPositionY(uint windowId, float y)
    {
        var rectTrans = PanelService.GetWindowComponent<RectTransform>(windowId, true);
        if (null == rectTrans)
        {
            return;
        }

        rectTrans.anchoredPosition = new Vector2(rectTrans.anchoredPosition.x, y);
    }

    public static float GetAnchorPositionX(uint windowId)
    {
        var rectTrans = PanelService.GetWindowComponent<RectTransform>(windowId, true);
        if (null == rectTrans)
        {
            return 0;
        }

        return rectTrans.anchoredPosition.x;
    }

    public static float GetAnchorPositionY(uint windowId)
    {
        var rectTrans = PanelService.GetWindowComponent<RectTransform>(windowId, true);
        if (null == rectTrans)
        {
            return 0;
        }

        return rectTrans.anchoredPosition.y;
    }

    public static float GetAnchorPosition(uint windowId, out float y)
    {
        var rectTrans = PanelService.GetWindowComponent<RectTransform>(windowId, true);

        if (null == rectTrans)
        {
            y = 0f;
            return 0;
        }

        y = rectTrans.anchoredPosition.y;
        return rectTrans.anchoredPosition.x;
    }

    public static void SetHeight(uint windowId, float height)
    {
        var uiRect = PanelService.GetWindowComponent<RectTransform>(windowId, true);
        if (null == uiRect)
        {
            return;
        }

        uiRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public static float GetHeight(uint windowId)
    {
        var rectTrans = PanelService.GetWindowComponent<RectTransform>(windowId, true);
        if (null == rectTrans)
        {
            return 0;
        }

        return rectTrans.rect.height;
    }

    public static float GetColorR(uint windowId)
    {
        var maskableGraphic = PanelService.GetWindowComponent<MaskableGraphic>(windowId);
        if (null == maskableGraphic)
        {
            return 0;
        }

        return maskableGraphic.color.r;
    }

    public static float GetColorG(uint windowId)
    {
        var maskableGraphic = PanelService.GetWindowComponent<MaskableGraphic>(windowId);
        if (null == maskableGraphic)
        {
            return 0;
        }

        return maskableGraphic.color.g;
    }

    public static float GetColorB(uint windowId)
    {
        var maskableGraphic = PanelService.GetWindowComponent<MaskableGraphic>(windowId);
        if (null == maskableGraphic)
        {
            return 0;
        }

        return maskableGraphic.color.b;
    }

    public static void SetAlpha(uint windowId, float alpha)
    {
        var maskableGraphic = PanelService.GetWindowComponent<MaskableGraphic>(windowId);
        if (null == maskableGraphic)
        {
            return;
        }

        var color = maskableGraphic.color;
        color.a = alpha;
        maskableGraphic.color = color;
    }

    public static float GetAlpha(uint windowId)
    {
        var maskableGraphic = PanelService.GetWindowComponent<MaskableGraphic>(windowId);
        if (null == maskableGraphic)
        {
            return 0;
        }

        return maskableGraphic.color.a;
    }

    public static void SetColor(uint windowId, float red, float green, float blue, float alpha)
    {
        var maskableGraphic = PanelService.GetWindowComponent<MaskableGraphic>(windowId);
        if (null == maskableGraphic)
        {
            return;
        }

        var color = maskableGraphic.color;
        color.r = red;
        color.g = green;
        color.b = blue;
        color.a = alpha;
        maskableGraphic.color = color;
    }

    public static void SetAnchor(uint windowId, float minX, float minY, float maxX, float maxY)
    {
        var rectTrans = PanelService.GetWindowComponent<RectTransform>(windowId, true);
        if (null == rectTrans)
        {
            return;
        }

        var anchorMin = new Vector2(minX, minY);
        var anchorMax = new Vector2(maxX, maxY);

        rectTrans.anchorMin = anchorMin;
        rectTrans.anchorMax = anchorMax;
    }

    public static void SetPivot(uint windowId, float pivotX, float pivotY)
    {
        var rectTrans = PanelService.GetWindowComponent<RectTransform>(windowId, true);
        if (null == rectTrans)
        {
            return;
        }

        rectTrans.pivot = new Vector2(pivotX, pivotY);
    }

    public static void SetPivotX(uint windowId, float pivotX)
    {
        var rectTrans = PanelService.GetWindowComponent<RectTransform>(windowId, true);
        if (null == rectTrans)
        {
            return;
        }

        var oldPivot = rectTrans.pivot;
        oldPivot.x = pivotX;
        rectTrans.pivot = oldPivot;
    }

    public static float GetPivotX(uint windowId)
    {
        var rectTrans = PanelService.GetWindowComponent<RectTransform>(windowId, true);
        if (null == rectTrans)
        {
            return 0f;
        }

        return rectTrans.pivot.x;
    }

    public static void SetPivotY(uint windowId, float pivotY)
    {
        var rectTrans = PanelService.GetWindowComponent<RectTransform>(windowId, true);
        if (null == rectTrans)
        {
            return;
        }

        var oldPivot = rectTrans.pivot;
        oldPivot.y = pivotY;
        rectTrans.pivot = oldPivot;
    }

    public static float GetPivotY(uint windowId)
    {
        var rectTrans = PanelService.GetWindowComponent<RectTransform>(windowId, true);
        if (null == rectTrans)
        {
            return 0f;
        }

        return rectTrans.pivot.y;
    }

    public static void SetWorldPosX(uint windowId, float x)
    {
        var windowObject = PanelService.GetWindowObject(windowId);
        if (null == windowObject)
        {
            return;
        }

        var position = windowObject.transform.position;
        position.x = x;
        windowObject.transform.position = position;
    }

    public static float GetWorldPosX(uint windowId)
    {
        var windowObject = PanelService.GetWindowObject(windowId);
        if (null == windowObject)
        {
            return 0;
        }

        return windowObject.transform.position.x;
    }

    public static void SetWorldPosY(uint windowId, float y)
    {
        var windowObject = PanelService.GetWindowObject(windowId);
        if (null == windowObject)
        {
            return;
        }

        var position = windowObject.transform.position;
        position.y = y;
        windowObject.transform.position = position;
    }

    public static float GetWorldPosY(uint windowId)
    {
        var windowObject = PanelService.GetWindowObject(windowId);
        if (null == windowObject)
        {
            return 0;
        }

        return windowObject.transform.position.y;
    }

    public static void SetWorldPosZ(uint windowId, float z)
    {
        var windowObject = PanelService.GetWindowObject(windowId);
        if (null == windowObject)
        {
            return;
        }

        var position = windowObject.transform.position;
        position.z = z;
        windowObject.transform.position = position;
    }

    public static void SetLocalPosX(uint windowId, float x)
    {
        var windowObject = PanelService.GetWindowObject(windowId);
        if (null == windowObject)
        {
            return;
        }

        var position = windowObject.transform.localPosition;
        position.x = x;
        windowObject.transform.localPosition = position;
    }

    public static float GetLocalPosX(uint windowId)
    {
        var windowObject = PanelService.GetWindowObject(windowId);
        if (null == windowObject)
        {
            return 0;
        }

        return windowObject.transform.localPosition.x;
    }

    public static void SetLocalPosY(uint windowId, float y)
    {
        var windowObject = PanelService.GetWindowObject(windowId);
        if (null == windowObject)
        {
            return;
        }

        var position = windowObject.transform.localPosition;
        position.y = y;
        windowObject.transform.localPosition = position;
    }

    public static float GetLocalPosY(uint windowId)
    {
        var windowObject = PanelService.GetWindowObject(windowId);
        if (null == windowObject)
        {
            return 0;
        }

        return windowObject.transform.localPosition.y;
    }

    public static void SetLocalPosZ(uint windowId, float z)
    {
        var windowObject = PanelService.GetWindowObject(windowId);
        if (null == windowObject)
        {
            return;
        }

        var position = windowObject.transform.localPosition;
        position.z = z;
        windowObject.transform.localPosition = position;
    }

    public static float GetLocalPosZ(uint windowId)
    {
        var windowObject = PanelService.GetWindowObject(windowId);
        if (null == windowObject)
        {
            return 0;
        }

        return windowObject.transform.localPosition.z;
    }

    public static float GetScreenPosX(uint windowId)
    {
        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            return 0;
        }

        var canvasT = uiWindow.cachedTransform.GetComponentInParent<Canvas>();
        if (canvasT)
        {
            var cam = canvasT.rootCanvas.worldCamera;
            if (cam != null)
            {
                return cam.WorldToScreenPoint(uiWindow.cachedTransform.position).x;
            }
        }

        return RectTransformUtility.WorldToScreenPoint(null, uiWindow.cachedTransform.position).x;
    }

    public static float GetScreenPosY(uint windowId)
    {
        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            return 0;
        }

        var canvasT = uiWindow.cachedTransform.GetComponentInParent<Canvas>();
        if (canvasT)
        {
            var cam = canvasT.rootCanvas.worldCamera;
            if (cam != null)
            {
                return cam.WorldToScreenPoint(uiWindow.cachedTransform.position).y;
            }
        }

        return RectTransformUtility.WorldToScreenPoint(null, uiWindow.cachedTransform.position).y;
    }

    public static bool GetCanvasPos(uint windowId, out float x, out float y)
    {
        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            x = 0;
            y = 0;
            return false;
        }

        var canvasT = uiWindow.cachedTransform.GetComponentInParent<Canvas>();

        if (null == canvasT)
        {
            x = 0;
            y = 0;
            return false;
        }

        Vector2 localPoint;
        var screenP = RectTransformUtility.WorldToScreenPoint(canvasT.worldCamera, uiWindow.cachedTransform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasT.GetComponent<RectTransform>(), screenP,
            canvasT.worldCamera, out localPoint);

        x = localPoint.x;
        y = localPoint.y;
        return true;
    }

    public static void SetLocalRotation(uint windowId, int angleX, int angleY, int angleZ)
    {
        var windowObject = PanelService.GetWindowObject(windowId);
        if (null == windowObject)
        {
            return;
        }

        var rotation = Quaternion.Euler(angleX, angleY, angleZ);
        windowObject.transform.localRotation = rotation;
    }

    public static void LoadSlotAsync(uint windowId)
    {
        if (null == PanelService.instance)
        {
            return;
        }

        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            return;
        }

        uiWindow.LoadSlotAsync();
    }

    public static void LoadSlot(uint windowId)
    {
        if (null == PanelService.instance)
        {
            return;
        }

        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            return;
        }

        uiWindow.LoadSlot();
    }

    public static int GetSlotLoadStatus(uint windowId)
    {
        var rectTrans = PanelService.GetWindowComponent<RectTransform>(windowId, true);
        if (null == rectTrans)
        {
            return 0;
        }

        var userControl = rectTrans.GetComponent<SlotControl>();
        if (null == userControl)
        {
            return 0;
        }

        return (int) userControl.loadStatus;
    }

    public static void SetSlotPath(uint windowId, string path)
    {
        if (null == PanelService.instance)
        {
            return;
        }

        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            return;
        }

        uiWindow.SetSlotPath(path);
    }

    public static string GetSlotPath(uint windowId)
    {
        if (null == PanelService.instance)
        {
            return "";
        }

        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            return "";
        }

        var slotControl = uiWindow.GetComponent<SlotControl>();
        if (null == slotControl)
        {
            return "";
        }

        return slotControl.prefabPath;
    }

    public static int GetChildCount(uint windowId)
    {
        if (null == PanelService.instance)
        {
            return 0;
        }

        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            return 0;
        }

        return uiWindow.GetChildCount();
    }

    public static uint GetChildByName(uint windowId, string childName)
    {
        if (null == PanelService.instance)
        {
            return INVALID_ID;
        }

        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            return INVALID_ID;
        }

        var childTrans = uiWindow.FindChild(childName);
        if (null == childTrans)
        {
            return INVALID_ID;
        }

        return PanelService.instance.AddWindow(childTrans.gameObject, uiWindow.panelId);
    }

    public static uint GetChildAt(uint windowId, int index)
    {
        if (null == PanelService.instance)
        {
            return INVALID_ID;
        }

        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            return INVALID_ID;
        }

        var childTrans = uiWindow.GetChildAt(index);
        if (null == childTrans)
        {
            return INVALID_ID;
        }

        return PanelService.instance.AddWindow(childTrans.gameObject, uiWindow.panelId);
    }

    public static void ReaddChild(uint parentWindowId, uint childWindowId)
    {
        if (null == PanelService.instance)
        {
            return;
        }

        PanelService.instance.ReaddWindowChild(parentWindowId, childWindowId);
    }

    public static void InsertChild(uint parentWindowId, uint childWindowId, uint refChildWindowId)
    {
        if (null == PanelService.instance)
        {
            return;
        }

        PanelService.instance.InsertWindowChild(parentWindowId, childWindowId, refChildWindowId);
    }

    public static int GetSiblingIndex(uint windowId)
    {
        if (null == PanelService.instance)
        {
            return -1;
        }

        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            return -1;
        }

        return uiWindow.transform.GetSiblingIndex();
    }

    public static uint Clone(uint windowId)
    {
        if (null == PanelService.instance)
        {
            return INVALID_ID;
        }

        var cloneId = PanelService.instance.CloneWindow(windowId);
        return cloneId;
    }

    public static uint CloneTo(uint windowId, uint parentWindowId, int siblingIndex = 1)
    {
        if (null == PanelService.instance)
        {
            return INVALID_ID;
        }

        var cloneId = PanelService.instance.CloneWindowTo(windowId, parentWindowId, siblingIndex);
        return cloneId;
    }

    public static void BindEvent(uint windowId, string eventName)
    {
        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            return;
        }

        uiWindow.BindEvent(eventName);
    }

    public static LuaTable GetPredefinedEvents()
    {
        var events = LuaService.instance.NewTable();

        var eventNames = EventDispatcherService.instance.GetPredefinedEvents();
        for (var i = 0; i < eventNames.Count; i++)
        {
            events.Set(i + 1, eventNames[i]);
        }

        return events;
    }

    public static void SetLocalScale(uint windowId, float x, float y, float z)
    {
        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            return;
        }

        uiWindow.localScale = new Vector3(x, y, z);
    }

    public static float GetLossyScaleX(uint windowId)
    {
        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            return 0;
        }

        return uiWindow.transform.lossyScale.x;
    }

    public static float GetLossyScaleY(uint windowId)
    {
        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            return 0;
        }

        return uiWindow.transform.lossyScale.y;
    }

    public static float GetLossyWidth(uint windowId)
    {
        return GetLossyScaleX(windowId) * GetWidth(windowId);
    }

    public static float GetLossyHeight(uint windowId)
    {
        return GetLossyScaleY(windowId) * GetHeight(windowId);
    }

    public static void SetScaleVisible(uint windowId, bool scaleVisible)
    {
        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            return;
        }

        uiWindow.scaleVisible = scaleVisible;
    }

    public static bool GetScaleVisible(uint windowId)
    {
        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            return false;
        }

        return uiWindow.scaleVisible;
    }

    public static Transform GetTransform(uint windowId)
    {
        var uiWindow = PanelService.instance.GetWindow(windowId);
        return null == uiWindow ? null : uiWindow.transform;
    }

    public static Vector3 GetChildWindowScreenPos(uint cameraWindowId, GameObject root, string name)
    {
        var uiWindow = PanelService.instance.GetWindow(cameraWindowId);
        if (null == uiWindow)
        {
            return Vector3.zero;
        }

        var cam = uiWindow.GetComponent<Camera>();
        if (cam == null)
        {
            return Vector3.zero;
        }

        var t = root.transform.FindDeepChild(name);
        if (t == null)
        {
            return Vector3.zero;
        }

        return cam.WorldToScreenPoint(t.position);
    }

    public static void ForceRebuildLayoutImmediate(uint windowId)
    {
        var rect = PanelService.GetWindowComponent<RectTransform>(windowId);
        if (null == rect)
        {
            return;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    public static uint GetWindowIdByAbsPath(uint windowId, string absPath)
    {
        if (null == PanelService.instance)
        {
            return INVALID_ID;
        }

        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            return INVALID_ID;
        }

        var childTrans = uiWindow.transform.Find(absPath);
        if (null == childTrans)
        {
            return INVALID_ID;
        }

        var window = Window.GetWindow(childTrans.gameObject);
        if (null == childTrans.parent)
        {
            if (null == window)
            {
                return INVALID_ID;
            }

            return window.id;
        }

        var eventDispatcher = childTrans.GetComponent<EventDispatcher>();
        if (null != eventDispatcher)
        {
            return window.id;
        }

        var slotControl = childTrans.parent.GetComponent<SlotControl>();
        if (null == slotControl)
        {
            if (null == window)
            {
                return INVALID_ID;
            }

            return window.id;
        }

        var pWindow = Window.GetWindow(slotControl.gameObject);
        if (null == pWindow)
        {
            return INVALID_ID;
        }

        return pWindow.id;
    }

    public static bool CopyTransform(uint srcWindowId, uint destWindowId, float xScale, float yScale)
    {
        if (null == PanelService.instance)
        {
            return false;
        }

        var srcWindow = PanelService.instance.GetWindow(srcWindowId);
        if (null == srcWindow)
        {
            return false;
        }

        var destWindow = PanelService.instance.GetWindow(destWindowId);
        if (null == destWindow)
        {
            return false;
        }

        var newRt = srcWindow.transform as RectTransform;
        var rt = destWindow.transform as RectTransform;

        var srcCanvas = srcWindow.GetComponentInParent<Canvas>();
        if (null == srcCanvas)
        {
            return false;
        }

        var destCanvas = destWindow.GetComponentInParent<Canvas>();
        if (null == destCanvas)
        {
            return false;
        }

        if (newRt != null && rt != null)
        {
            newRt.pivot = rt.pivot;
            float radioX = 0;
            float radioY = 0;
            if (Math.Abs(newRt.transform.lossyScale.x) > float.Epsilon &&
                Math.Abs(rt.transform.lossyScale.x) > float.Epsilon)
            {
                radioX = rt.transform.lossyScale.x / newRt.transform.lossyScale.x;
            }

            if (Math.Abs(newRt.transform.lossyScale.y) > float.Epsilon &&
                Math.Abs(rt.transform.lossyScale.y) > float.Epsilon)
            {
                radioY = rt.transform.lossyScale.y / newRt.transform.lossyScale.y;
            }

            newRt.SetWidth(Mathf.Abs(rt.GetWidth() * radioX) * xScale);
            newRt.SetHeight(Mathf.Abs(rt.GetHeight() * radioY) * yScale);
        }

        var follow3DObject = srcWindow.transform.GetOrAddComponent<Follow3DObject>();

        follow3DObject.AttachTrans = destWindow.transform;
        follow3DObject.IsStay = false;
        follow3DObject.Canvas = srcCanvas.rootCanvas;
        follow3DObject.SourceCamera = destCanvas.worldCamera;
        follow3DObject.UpdateUIPos();

        return true;
    }

    public static void AttachPointer(uint aWindowId, PointerEventData eventData)
    {
        if (null == PanelService.instance)
        {
            return;
        }

        var aUiWindow = PanelService.instance.GetWindow(aWindowId);
        if (null == aUiWindow)
        {
            return;
        }

        var canvas = aUiWindow.GetComponentInParent<Canvas>();
        Vector2 localPointerPosition;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(aUiWindow.transform.parent as RectTransform,
            eventData.position, canvas.worldCamera, out localPointerPosition))
        {
            return;
        }

        aUiWindow.transform.localPosition = localPointerPosition;
    }

    public static void SetInScreen(uint aWindowId)
    {
        if (null == PanelService.instance)
        {
            return;
        }

        var aUiWindow = PanelService.instance.GetWindow(aWindowId);
        if (null == aUiWindow)
        {
            return;
        }

        aUiWindow.SetInScreen();
    }

    public static void SetGray(uint windowId, bool state)
    {
        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            return;
        }

        var grayEffect = uiWindow.GetOrAddComponent<UIImageGrayEffect>();
        grayEffect.Gray = state;
    }

    public static void SetGrayRecursively(uint windowId, bool state)
    {
        var uiWindow = PanelService.instance.GetWindow(windowId);
        if (null == uiWindow)
        {
            return;
        }

        var targets = uiWindow.GetComponentsInChildren<MaskableGraphic>();
        foreach (var maskableGraphic in targets)
        {
            var grayEffect = maskableGraphic.GetOrAddComponent<UIImageGrayEffect>();
            grayEffect.Gray = state;
        }
    }

    public static void UnloadUnusedSlotControls()
    {
        SlotControlService.instance.UnloadUnusedAssets();
    }

    public static void SetRayCastTargetRecursively(uint windowId, bool state)
    {
        var window = PanelService.GetWindowComponent<Window>(windowId, true);
        if (null == window)
        {
            return;
        }

        foreach (var graphic in window.GetComponentsInChildren<Graphic>())
        {
            graphic.raycastTarget = state;
        }
    }
}