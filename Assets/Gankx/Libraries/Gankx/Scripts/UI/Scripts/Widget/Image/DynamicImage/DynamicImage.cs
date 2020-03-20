using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Gankx.UI
{
    [ExecuteInEditMode]
    [AddComponentMenu("UI/Image/Dynamic Image")]
    public class DynamicImage : MonoBehaviour
    {
        public class DynamicImageLoadedEvent : UnityEvent<bool>
        {
        }

        [SerializeField]
        [FormerlySerializedAs("mPath")]
        public string path;

        [SerializeField]
        [FormerlySerializedAs("IsAsync")]
        public bool async;

        public DynamicImageLoadedEvent onLoaded = new DynamicImageLoadedEvent();

        private bool myInitialized;
        private AtlasImage myImage;
        private DynamicSprite myDynamicSprite;
        private ManagedIcon myManagedIcon;

        private void Awake()
        {
            Init();
        }

        private void OnEnable()
        {
            Reload();
        }

        private void OnDisable()
        {
            Unload();
        }

        private void OnDestroy()
        {
            Unload();
        }

        public void Reload()
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (IsIcon(path))
            {
                if (!Application.isPlaying)
                {
                    var isAsync = async;
                    SetPath(path, true);
                    async = isAsync;
                    return;
                }

                if (async)
                {
                    SetPathAsync(path, true);
                }
                else
                {
                    SetPath(path, true);
                }
            }
        }

        private void Unload()
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (IsIcon(path))
            {
                if (null != myManagedIcon)
                {
                    myManagedIcon.DecRef();
                    myManagedIcon = null;
                }

                if (null != myDynamicSprite)
                {
                    myDynamicSprite.DecRef();
                    myDynamicSprite = null;
                }

                if (IconService.ContainsInstance())
                {
                    IconService.instance.RemoveLoadTaskByDelegate(OnIconLoadCallback);
                }
            }
        }

        public void Init()
        {
            if (myInitialized)
            {
                return;
            }

            myImage = GetComponent<AtlasImage>();
            if (null == myImage)
            {
                Debug.LogError("Please AtlasImage Image component before using DynamicImage!", gameObject);
            }

            myInitialized = true;
        }

        public void SetPath(string resourcePath, bool force = false)
        {
            Init();

            if (path == resourcePath && force == false)
            {
                return;
            }

            Unload();

            path = resourcePath;
            async = false;

            if (null == myImage)
            {
                return;
            }

            if (IsIcon(path))
            {
                if (!UITools.GetActive(this))
                {
                    return;
                }

                var cachedSprite = DynamicSpriteService.instance.GetSprite(path);
                if (cachedSprite != null)
                {
                    myDynamicSprite = cachedSprite;
                    myDynamicSprite.IncRef();
                    myImage.sprite = myDynamicSprite.sprite;
                    NotifyLoaded(false);
                    return;
                }

                IconService.instance.Load(path, OnIconLoadCallback);
            }
            else
            {
                SetAtlasPath(path, true);
            }
        }

        public string GetPath()
        {
            return path;
        }

        public void SetPathAsync(string resourcePath, bool force = false)
        {
            Init();

            if (path == resourcePath && force == false)
            {
                return;
            }

            Unload();

            path = resourcePath;
            async = true;

            if (null == myImage)
            {
                return;
            }

            if (IsIcon(path))
            {
                if (!UITools.GetActive(this))
                {
                    return;
                }

                var cachedSprite = DynamicSpriteService.instance.GetSprite(path);
                if (cachedSprite != null)
                {
                    myDynamicSprite = cachedSprite;
                    myDynamicSprite.IncRef();
                    myImage.sprite = myDynamicSprite.sprite;
                    NotifyLoaded(true);
                    return;
                }

                myImage.enabled = false;

                IconService.instance.LoadAsync(path, OnIconLoadCallback);
            }
            else
            {
                SetAtlasPath(path, true);
            }
        }

        private void OnIconLoadCallback(ManagedIcon managedIcon, bool isAsync)
        {
            if (!myImage.enabled && isAsync)
            {
                myImage.enabled = true;
            }

            myManagedIcon = managedIcon;

            if (null == myManagedIcon)
            {
                myImage.sprite = null;
                NotifyLoaded(isAsync);
                return;
            }

            myManagedIcon.IncRef();

            myDynamicSprite = DynamicSpriteService.instance.PackSprite(path, myManagedIcon.resource);
            if (null != myDynamicSprite)
            {
                myDynamicSprite.IncRef();
                myImage.sprite = myDynamicSprite.sprite;

                if (myDynamicSprite.isVirtual)
                {
                    myManagedIcon.DecRef();
                    myManagedIcon = null;
                }
            }
            else
            {
                myImage.sprite = myManagedIcon.resource;
            }

            NotifyLoaded(isAsync);
        }

        public void SetAtlasPath(string spritePath, bool force = false)
        {
            Init();

            if (path == spritePath && force == false)
            {
                return;
            }

            Unload();

            path = spritePath;
            async = false;

            if (null == myImage)
            {
                return;
            }

            if (string.IsNullOrEmpty(spritePath))
            {
                myImage.sprite = null;
            }
            else
            {
                myImage.SetAtlasPath(spritePath);
            }
        }

        private void NotifyLoaded(bool isAsync)
        {
            if (null != onLoaded)
            {
                onLoaded.Invoke(isAsync);
            }
        }

        private static bool IsIcon(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            if (path.StartsWith(UIConfig.IconSimplePath))
            {
                return true;
            }

            return false;
        }

        public void SetNativeSize()
        {
            if (myImage != null)
            {
                myImage.SetNativeSize();
            }
        }
    }
}