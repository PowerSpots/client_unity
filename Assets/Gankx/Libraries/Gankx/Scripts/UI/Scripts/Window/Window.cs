using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gankx.UI
{
    public class Window : MonoBehaviour
    {
        public const uint InvalidId = 0xffffffff;

        private static uint BaseId;

        private static readonly PointerEventData MyEventData = new PointerEventData(EventSystem.current);
        private static readonly List<RaycastResult> MyHits = new List<RaycastResult>();

        private SlotControl mySlotControl;

        private Transform myTransform;

        private Dictionary<Type, Component> myCachedSlotComponentDic;
        private Dictionary<Type, Component> myCachedParentComponentDic;
        private Dictionary<Type, Component> myCachedComponentDic;
        private bool myParentIsChecked;
        private bool myParentHasSlot;

        [SerializeField]
        private Vector3 myLocalScale;

        [SerializeField]
        private bool myLocalScaleInitialized;

        [SerializeField]
        private bool myScaleVisible = true;

        public Window()
        {
            id = ++BaseId;
            panelId = InvalidId;
        }

        public uint id { get; private set; }

        public uint panelId { get; set; }

        public Transform cachedTransform
        {
            get
            {
                if (null == myTransform)
                {
                    myTransform = transform;
                }

                return myTransform;
            }
        }

        private bool parentHasSlot
        {
            get
            {
                if (myParentIsChecked)
                {
                    return myParentHasSlot;
                }

                myParentHasSlot = GetComponent<SlotControlObject>() != null &&
                                  transform.parent != null &&
                                  transform.parent.GetComponent<SlotControl>() != null;
                myParentIsChecked = true;

                return myParentHasSlot;
            }
        }

        public Vector3 localScale
        {
            get
            {
                if (!myLocalScaleInitialized)
                {
                    return transform.localScale;
                }

                return myLocalScale;
            }

            set
            {
                myLocalScaleInitialized = true;
                myLocalScale = value;

                ApplyLocalScale();
            }
        }

        public bool scaleVisible
        {
            get { return myScaleVisible; }
            set
            {
                if (myScaleVisible == value)
                {
                    return;
                }

                myScaleVisible = scaleVisible;

                ApplyLocalScale();

                gameObject.SendMessage("OnScaleVisibleChange", scaleVisible, SendMessageOptions.DontRequireReceiver);
            }
        }

        private void ApplyLocalScale()
        {
            if (!myLocalScaleInitialized)
            {
                myLocalScaleInitialized = true;
                myLocalScale = transform.localScale;
            }

            if (myScaleVisible)
            {
                transform.localScale = myLocalScale;
                return;
            }

            transform.localScale = new Vector3(0, transform.localScale.y, transform.localScale.z);
        }

        public void CopyLocalScale(Window another)
        {
            myLocalScaleInitialized = another.myLocalScaleInitialized;
            myLocalScale = another.myLocalScale;
            myScaleVisible = another.myScaleVisible;

            ApplyLocalScale();
        }

        public T GetCachedComponent<T>(bool forceParentSlotControl = false) where T : Component
        {
            if (forceParentSlotControl)
            {
                if (parentHasSlot)
                {
                    return GetParentComponent<T>();
                }
            }
            else
            {
                if (mySlotControl != null)
                {
                    return GetSlotComponent<T>();
                }
            }

            return GetMyComponent<T>();
        }

        private T GetMyComponent<T>() where T : Component
        {
            if (UIWindowExport.UseCache)
            {
                if (myCachedComponentDic == null)
                {
                    myCachedComponentDic = new Dictionary<Type, Component>();
                }

                return DoGetCachedComponent<T>(transform, myCachedComponentDic);
            }

            return DoGetComponent<T>(transform);
        }

        private T GetParentComponent<T>() where T : Component
        {
            if (UIWindowExport.UseCache)
            {
                if (myCachedParentComponentDic == null)
                {
                    myCachedParentComponentDic = new Dictionary<Type, Component>();
                }

                return DoGetCachedComponent<T>(transform.parent, myCachedParentComponentDic);
            }

            return DoGetComponent<T>(transform.parent);
        }

        private T GetSlotComponent<T>() where T : Component
        {
            if (UIWindowExport.UseCache)
            {
                if (myCachedSlotComponentDic == null)
                {
                    myCachedSlotComponentDic = new Dictionary<Type, Component>();
                }

                return DoGetCachedComponent<T>(mySlotControl.cachedControlTransform, myCachedSlotComponentDic);
            }

            return DoGetComponent<T>(mySlotControl.cachedControlTransform);
        }

        private static T DoGetComponent<T>(Component t)
        {
            return t.GetComponent<T>();
        }

        private static T DoGetCachedComponent<T>(Component t, IDictionary<Type, Component> cache) where T : Component
        {
            var type = typeof(T);
            if (cache.ContainsKey(type))
            {
                return cache[type] as T;
            }

            var cacheComponent = t.GetComponent<T>();
            cache[type] = cacheComponent;
            return cacheComponent;
        }

        public Window Clone()
        {
            var go = Instantiate(gameObject);
            if (!go)
            {
                return null;
            }

            var uiWindow = go.GetComponent<Window>();
            return uiWindow;
        }

        public static Window AddWindow(GameObject go)
        {
            var uiWindow = go.GetComponent<Window>();

            if (null == uiWindow)
            {
                uiWindow = go.AddComponent<Window>();
            }

            return uiWindow;
        }

        public static Window GetWindow(GameObject go)
        {
            var uiWindow = go.GetComponent<Window>();
            return uiWindow;
        }

        public static Window GetWindowInParent(GameObject go)
        {
            var uiWindow = go.GetComponentInParent<Window>();
            return uiWindow;
        }

        private void OnDestroy()
        {
            if (PanelService.ContainsInstance())
            {
                PanelService.instance.RemoveWindow(id);
            }

            id = InvalidId;
            panelId = InvalidId;

            if (myCachedComponentDic != null)
            {
                myCachedComponentDic.Clear();
            }

            if (myCachedParentComponentDic != null)
            {
                myCachedParentComponentDic.Clear();
            }

            if (myCachedSlotComponentDic != null)
            {
                myCachedSlotComponentDic.Clear();
            }

            myParentIsChecked = false;
            myParentHasSlot = false;
        }

        public void LoadSlotAsync()
        {
            if (null == mySlotControl)
            {
                return;
            }

            mySlotControl.LoadControlAsync();
        }

        public void LoadSlot()
        {
            if (null == mySlotControl)
            {
                return;
            }

            mySlotControl.LoadControl();
        }

        public void SetSlotPath(string path)
        {
            mySlotControl = gameObject.GetComponent<SlotControl>();

            if (!string.IsNullOrEmpty(path))
            {
                if (null == mySlotControl)
                {
                    mySlotControl = gameObject.AddComponent<SlotControl>();
                }

                mySlotControl.prefabPath = path;
            }
        }

        public Transform FindChild(string childName)
        {
            if (null == mySlotControl)
            {
                return cachedTransform.Find(childName);
            }

            return mySlotControl.FindChild(childName);
        }

        public int GetChildCount()
        {
            if (null == mySlotControl)
            {
                return cachedTransform.childCount;
            }

            return mySlotControl.GetChildCount();
        }

        public Transform GetChildAt(int index)
        {
            if (null == mySlotControl)
            {
                return cachedTransform.GetChild(index);
            }

            return mySlotControl.GetChildAt(index);
        }

        public void BindEvent(string eventName)
        {
            EventDispatcherService.instance.BindEvent(gameObject, eventName);
        }

        public bool GetVisibleAndRendered(float xScale, float yScale)
        {
            if (!gameObject.activeInHierarchy)
            {
                return false;
            }

            if (!scaleVisible)
            {
                return false;
            }

            var graphic = gameObject.GetComponentInChildren<Graphic>();
            if (null == graphic)
            {
                return gameObject.activeInHierarchy;
            }


            var renderCamera = graphic.canvas.rootCanvas.worldCamera;
            if (null != renderCamera && !renderCamera.gameObject.activeInHierarchy)
            {
                return false;
            }

            var rt = cachedTransform as RectTransform;
            if (null == rt)
            {
                return false;
            }

            var centerPos = Vector3.zero;

            centerPos.x = (float) (rt.position.x + (0.5 - rt.pivot.x) * rt.GetWidth() * xScale * rt.lossyScale.x);
            centerPos.y = (float) (rt.position.y + (0.5 - rt.pivot.y) * rt.GetHeight() * yScale * rt.lossyScale.y);
            centerPos.z = rt.position.z;

            MyEventData.position = RectTransformUtility.WorldToScreenPoint(renderCamera, centerPos);

            var oldRaycastTarget = graphic.raycastTarget;

            graphic.raycastTarget = true;

            EventSystem.current.RaycastAll(MyEventData, MyHits);

            graphic.raycastTarget = oldRaycastTarget;

            if (MyHits.Count <= 0)
            {
                return false;
            }

            GameObject hitGo = null;
            for (var i = 0; i < MyHits.Count; ++i)
            {
                var hit = MyHits[i].gameObject;
                if (hit.name != "ImageHint" && hit.name != "MaskedImage")
                {
                    hitGo = hit;
                    break;
                }
            }

            if (null == hitGo)
            {
                return false;
            }

            return CheckGoInChildren(cachedTransform, hitGo);
        }

        private static bool CheckGoInChildren(Transform trans, GameObject go)
        {
            if (trans.gameObject == go)
            {
                return true;
            }

            for (var i = 0; i < trans.childCount; ++i)
            {
                var tr = trans.GetChild(i);
                if (CheckGoInChildren(tr, go))
                {
                    return true;
                }
            }

            return false;
        }

        public void SetInScreen()
        {
            StartCoroutine(DelaySetInScreen());
        }

        private IEnumerator DelaySetInScreen()
        {
            yield return new WaitForEndOfFrame();

            var canvas = transform.GetComponentInParent<Canvas>();
            if (null == canvas)
            {
                yield return null;
            }

            var target = transform.GetComponent<RectTransform>();
            var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(canvas.transform, target);

            // ReSharper disable once PossibleLossOfFraction
            var area = new Rect(-Screen.width / 2, -Screen.height / 2, Screen.width, Screen.height);

            var delta = default(Vector2);
            if (bounds.center.x - bounds.extents.x < area.x)
            {
                delta.x += Mathf.Abs(bounds.center.x - bounds.extents.x - area.x);
            }
            else if (bounds.center.x + bounds.extents.x > area.width / 2)
            {
                delta.x -= Mathf.Abs(bounds.center.x + bounds.extents.x - area.width / 2);
            }

            if (bounds.center.y - bounds.extents.y < area.y)
            {
                delta.y += Mathf.Abs(bounds.center.y - bounds.extents.y - area.y);
            }
            else if (bounds.center.y + bounds.extents.y > area.height / 2)
            {
                delta.y -= Mathf.Abs(bounds.center.y + bounds.extents.y - area.height / 2);
            }

            target.anchoredPosition += delta;
        }
    }
}