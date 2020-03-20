using System;
using UIExtention;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Gankx.UI
{
    [ExecuteInEditMode]
    public class SlotControl : MonoBehaviour
    {
        public enum LoadStatus
        {
            None = 0,
            Loading = 1,
            Loaded = 2
        }

        public const string LookAndFeelLayerName = "LookAndFeel";

        [NonSerialized]
        private GameObject myControlObject;
        [NonSerialized]
        private ManagedSlotControl myManagedControl;

        [SerializeField]
        [HideInInspector]
        [FormerlySerializedAs("mPrefabPath")]
        private string myPrefabPath;

        [SerializeField]
        [HideInInspector]
        [FormerlySerializedAs("_lookAndFeel")]
        private int myLookAndFeel = -1;

        [NonSerialized]
        private Transform myControlTrans;
        [NonSerialized]
        private LoadStatus myLoadStatus = LoadStatus.None;
        [NonSerialized]
        private Window myWindow;

        [FormerlySerializedAs("m_OverrideAction")]
        public Action overrideAction;
        [FormerlySerializedAs("ParentPivotOverride")]
        public bool overridePivot = false;
        [FormerlySerializedAs("SetAsLastSibling")]
        public bool setAsLastSibling = false;
        [FormerlySerializedAs("m_SupportSoftMask")]
        public bool supportSoftMask = false;

        public string prefabPath
        {
            get { return myPrefabPath; }
            set { myPrefabPath = value; }
        }

        public int lookAndFeel
        {
            get { return myLookAndFeel; }
            set
            {
                if (myLookAndFeel == value)
                {
                    return;
                }

                myLookAndFeel = value;

                ApplyLookAndFeel();
            }
        }

        public LoadStatus loadStatus
        {
            get { return myLoadStatus; }
        }

        public Transform cachedControlTransform
        {
            get
            {
                LoadControl();

                if (null == myControlObject)
                {
                    return null;
                }

                if (myControlTrans == null)
                {
                    myControlTrans = myControlObject.transform;
                }

                return myControlTrans;
            }
        }

        private Window window
        {
            get
            {
                if (null == myWindow)
                {
                    myWindow = Window.GetWindow(gameObject);
                }

                return myWindow;
            }
        }

        private void OnResourceLoaded(bool isAsync = false)
        {
            if (myControlObject == null)
            {
                if (null == myManagedControl)
                {
                    Debug.LogError(
                        string.Format("SlotControl.Load on [{0}] occurred error: cannot load control at path<{1}>!",
                            gameObject.name, myPrefabPath), gameObject);
                    OnLoaded(isAsync);
                    return;
                }

                var loadedPrefab = myManagedControl.resource;
                if (null == loadedPrefab)
                {
                    Debug.LogError(
                        string.Format("SlotControl.Load on [{0}] occurred error: cannot load control at path<{1}>!",
                            gameObject.name, myPrefabPath), gameObject);
                    OnLoaded(isAsync);
                    return;
                }

                if (loadedPrefab.activeSelf)
                {
                    loadedPrefab.SetActive(false);
                }

                try
                {
                    myControlObject = UITools.AddChild(gameObject, loadedPrefab);
                }
                catch (Exception e)
                {
                    Debug.LogError(
                        string.Format(
                            "SlotControl.Load on [{0}] occurred error: cannot add child, path:<{1}>! Exception: {2}",
                            gameObject.name, myPrefabPath, e.Message));
                }

                if (null == myControlObject)
                {
                    OnLoaded(isAsync);
                    return;
                }

                var t = myControlObject.transform;
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;

                //remove (Clone) postfix
                t.name = t.name.Substring(0, t.name.Length - 7);

                myControlObject.AddComponent<SlotControlObject>();

                if (setAsLastSibling)
                {
                    myControlObject.transform.SetAsLastSibling();
                }
                else
                {
                    myControlObject.transform.SetAsFirstSibling();
                }


#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    controlPrefab = loadedPrefab;

                    for (var i = 0; i < transform.childCount; i++)
                    {
                        var childTrans = transform.GetChild(i);
                        if (childTrans != null)
                        {
                            var components = childTrans.GetComponents<Component>();
                            for (var j = 0; j < components.Length; j++)
                            {
                                if (components[j] == null)
                                {
                                    DestroyImmediate(childTrans.gameObject);
                                    break;
                                }
                            }
                        }
                    }

                    myControlObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
                }
#endif
            }

            if (null == myControlObject)
            {
                OnLoaded(isAsync);
                return;
            }

            myControlObject.SetActive(true);

            var controlRect = myControlObject.GetComponent<RectTransform>();
            var rootRect = GetComponent<RectTransform>();
            if (null != controlRect)
            {
                controlRect.anchorMax = Vector2.one;
                controlRect.anchorMin = Vector2.zero;
                if (null != rootRect)
                {
                    controlRect.pivot = overridePivot ? rootRect.pivot : new Vector2(0.5f, 0.5f);
                }

                controlRect.anchoredPosition = Vector2.zero;
                controlRect.sizeDelta = Vector2.zero;
            }

            ApplyLookAndFeel();

            if (Application.isPlaying)
            {
                SupportSoftClip();
            }

            OnLoaded(isAsync);
        }

        private void OnResourceLoadCallback(ManagedSlotControl managedControl, bool isAsync)
        {
            myManagedControl = managedControl;
            if (myManagedControl != null)
            {
                myManagedControl.IncRef();
            }

            OnResourceLoaded(isAsync);
        }

        private void OnLoaded(bool isAsync)
        {
            myLoadStatus = LoadStatus.Loaded;

            if (!isAsync)
            {
                return;
            }

            if (null == window)
            {
                return;
            }

            LuaService.instance.FireEvent(
                "OnPanelMessage", window.panelId, window.id, "OnLoadAsync");
        }

        private bool CheckControlObject()
        {
            var childCount = transform.childCount;
            for (var i = 0; i < childCount; ++i)
            {
                var slotControlObject = transform.GetChild(i).GetComponent<SlotControlObject>();
                if (slotControlObject != null)
                {
                    myControlObject = slotControlObject.gameObject;
                    return true;
                }
            }

            return false;
        }

        public void LoadControlAsync()
        {
            if (myLoadStatus == LoadStatus.Loading)
            {
                return;
            }

            if (myLoadStatus == LoadStatus.Loaded)
            {
                OnLoaded(true);
                return;
            }

            myLoadStatus = LoadStatus.Loading;

            if (null != myManagedControl)
            {
                myManagedControl.DecRef();
                myManagedControl = null;
            }

            if (CheckControlObject())
            {
                OnResourceLoaded(true);
                return;
            }

            SlotControlService.instance.LoadAsync(PanelService.PanelResourcePath + myPrefabPath, OnResourceLoadCallback);
        }

        public void LoadControl()
        {
#if UNITY_EDITOR
            if (PrefabUtility.GetPrefabParent(gameObject) == null && PrefabUtility.GetPrefabObject(gameObject) != null)
            {
                return;
            }

            if (!Application.isPlaying && !gameObject.activeInHierarchy)
            {
                return;
            }
#endif
            if (myLoadStatus != LoadStatus.None)
            {
                return;
            }

            if (null != myManagedControl)
            {
                myManagedControl.DecRef();
                myManagedControl = null;
            }

            myLoadStatus = LoadStatus.Loading;
            if (CheckControlObject())
            {
                OnResourceLoaded();
                return;
            }

            SlotControlService.instance.Load(PanelService.PanelResourcePath + myPrefabPath, OnResourceLoadCallback);
        }

        public void ClearControl()
        {
            myControlObject = null;
            myControlTrans = null;
        }

        private void SupportSoftClip()
        {
            var icomponents = GetComponentsInParent<IUISoftNewObjectHandler>(true);
            foreach (var c in icomponents)
            {
                c.OnNewObjectLoaded(myControlObject);
            }
        }

        public Transform FindChild(string childName)
        {
            if (null == cachedControlTransform)
            {
                return null;
            }

            return myControlTrans.Find(childName);
        }

        public int GetChildCount()
        {
            if (null == cachedControlTransform)
            {
                return 0;
            }

            return myControlTrans.childCount;
        }

        public Transform GetChildAt(int index)
        {
            if (null == cachedControlTransform)
            {
                return null;
            }

            return myControlTrans.GetChild(index);
        }

        public void BindEvent(string eventName)
        {
            if (null == cachedControlTransform)
            {
                return;
            }

            EventDispatcherService.instance.BindEvent(myControlTrans.gameObject, eventName);
        }

        private void ApplyLookAndFeel()
        {
            if (overrideAction != null)
            {
                overrideAction();
            }

            if (null == myControlObject)
            {
                return;
            }

            if (!isActiveAndEnabled)
            {
                return;
            }

            var animator = myControlObject.GetComponent<Animator>();
            if (null == animator)
            {
                return;
            }

            var layerIndex = animator.GetLayerIndex(LookAndFeelLayerName);
            if (layerIndex < 0)
            {
                return;
            }

            if (!animator.HasState(layerIndex, lookAndFeel))
            {
                return;
            }

            animator.enabled = true;
            animator.Play(lookAndFeel, layerIndex, 1f);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                try
                {
                    if (animator.recorderMode == AnimatorRecorderMode.Playback)
                    {
                        animator.StopPlayback();
                    }

                    animator.StartRecording(-1);

                    var success = animator.recorderMode == AnimatorRecorderMode.Record;
                    if (success)
                    {
                        animator.Update(0f);
                        animator.StopRecording();

                        animator.StartPlayback();
                        animator.playbackTime = 0f;
                        animator.Update(0f);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
#endif
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                LoadControl();
                return;
            }
#endif
            ApplyLookAndFeel();
        }

        public void OnUnload()
        {
            if (null != myManagedControl)
            {
                myManagedControl.DecRef();
                myManagedControl = null;
            }

            if (SlotControlService.ContainsInstance())
            {
                SlotControlService.instance.RemoveLoadTaskByDelegate(OnResourceLoadCallback);
            }
        }

        private void OnTransformChildrenChanged()
        {
            if (!supportSoftMask)
            {
                return;
            }

            SupportMaskEffect();
        }

        private void SupportMaskEffect()
        {
            var graphics = gameObject.GetComponentsInChildren<Graphic>();
            for (var i = 0; i < graphics.Length; i++)
            {
                if (graphics[i].enabled)
                {
                    graphics[i].gameObject.AddComponent<SoftMaskMeshEffect>();
                }
            }
        }

#if UNITY_EDITOR
        public GameObject controlPrefab { get; set; }

        public void DeletePreview()
        {
            myLoadStatus = LoadStatus.None;
            if (myControlObject != null)
            {
                DestroyImmediate(myControlObject);
            }
        }

        private void OnDisable()
        {
            if (!Application.isPlaying)
            {
                DeletePreview();
            }
        }

        public void SetNativeSize()
        {
            if (null == controlPrefab)
            {
                return;
            }

            var current = transform.GetComponent<RectTransform>();
            var prefabRect = controlPrefab.GetComponent<RectTransform>();
            if (prefabRect != null && current != null)
            {
                current.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, prefabRect.rect.width);
                current.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, prefabRect.rect.height);
            }
        }

        public void SavePrefab()
        {
            if (null == myControlObject || null == controlPrefab)
            {
                return;
            }

            myControlObject.hideFlags = HideFlags.None;
            DestroyImmediate(myControlObject.GetComponent<SlotControlObject>());
            DestroyImmediate(myControlObject.GetComponent<SlotControlObject>());
            var controlRect = myControlObject.GetComponent<RectTransform>();
            if (null != controlRect)
            {
                var size = controlRect.rect.size;
                controlRect.anchorMax = new Vector2(0.5f, 0.5f);
                controlRect.anchorMin = new Vector2(0.5f, 0.5f);
                controlRect.sizeDelta = size;
            }

            PrefabUtility.ReplacePrefab(myControlObject, controlPrefab, ReplacePrefabOptions.ReplaceNameBased);

            myControlObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
            myControlObject.AddComponent<SlotControlObject>();

            if (null != controlRect)
            {
                controlRect.anchorMax = Vector2.one;
                controlRect.anchorMin = Vector2.zero;
                controlRect.pivot = new Vector2(0.5f, 0.5f);
                controlRect.anchoredPosition = Vector2.zero;
                controlRect.sizeDelta = Vector2.zero;
            }
        }
#endif
    }
}