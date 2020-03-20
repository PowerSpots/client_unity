using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Gankx.UI
{
    [RequireComponent(typeof(RawImage))]
    public class RawImageUrlLoader : MonoBehaviour
    {
        public class RawImageUrlLoadedEvent : UnityEvent
        {
        }

        public bool supportDefault = false;

        [FormerlySerializedAs("m_DefaultTexture")]
        public Texture defaultTexture;

        private RawImage myRawImage;
        private RawImageUrlTextureCacheMemory.MemoryEntry myMemoryEntry;

        private string myUrl = "";
        private string myLoadingUrl = "";

        private bool myIsDestroy;
        private bool myApplicationIsQuitting;

        public UnityEvent onLoaded = new RawImageUrlLoadedEvent();

        private Texture texture
        {
            get
            {
                if (null == myRawImage)
                {
                    return null;
                }

                // empty's name is "UnityWhite"
                if ("UnityWhite" == myRawImage.mainTexture.name)
                {
                    return null;
                }

                if (supportDefault && defaultTexture != null && myRawImage.mainTexture != null)
                {
                    if (myRawImage.mainTexture == defaultTexture)
                    {
                        return null;
                    }
                }

                return myRawImage.mainTexture;
            }
            set
            {
                if (null == myRawImage)
                {
                    try
                    {
                        CacheRawImage();
                    }
                    catch (Exception exception)
                    {
                        var ex = "RawImageUrlLoader set texture occurred error: exception =" + exception;
                        Debug.LogError(ex);
                    }
                }

                if (null == myRawImage)
                {
                    return;
                }

                myRawImage.texture = value;
                if (supportDefault)
                {
                    var color = myRawImage.color;
                    if (value == null)
                    {
                        if (defaultTexture == null)
                        {
                            color.a = 0;
                            myRawImage.color = color;
                        }
                        else
                        {
                            myRawImage.texture = defaultTexture;
                        }
                    }
                    else
                    {
                        if (defaultTexture == null)
                        {
                            color.a = 1;
                            myRawImage.color = color;
                        }
                    }
                }
            }
        }

        public string url
        {
            get { return myUrl; }
            set
            {
                if (myUrl == value && texture != null)
                {
                    if (null != onLoaded)
                    {
                        onLoaded.Invoke();
                    }

                    return;
                }

                myUrl = value;

#if UNITY_IPHONE
                if (!myUrl.Contains("https"))
                {
                    myUrl = myUrl.Replace("http", "https");
                }
#endif
                if (string.IsNullOrEmpty(myUrl))
                {
                    if (null != onLoaded)
                    {
                        onLoaded.Invoke();
                    }

                    return;
                }

                if (myLoadingUrl.Equals(myUrl))
                {
                    return;
                }

                myLoadingUrl = myUrl;

                Load();
            }
        }

        private void CacheRawImage()
        {
            if (myRawImage || myIsDestroy)
            {
                return;
            }

            if (gameObject == null || gameObject.transform == null)
            {
                return;
            }

            myRawImage = gameObject.GetComponent<RawImage>();
            if (null == myRawImage)
            {
                Debug.LogError("RawImageUrlLoader require RawImage");
            }
        }

        private void Awake()
        {
            try
            {
                CacheRawImage();
            }
            catch (Exception exception)
            {
                Debug.LogWarning("RawImageUrlLoader Awake occurred error, name=" + gameObject.name + " exception=" +
                                 exception);
            }
        }

        private void Load()
        {
            Unload();

            if (!string.IsNullOrEmpty(myUrl))
            {
                RawImageUrlTextureManager.instance.LoadTextureAsync(myUrl, OnTextureLoaded);
            }
        }

        private void Unload()
        {
            if (myApplicationIsQuitting)
            {
                return;
            }

            texture = null;

            if (myMemoryEntry != null)
            {
                myMemoryEntry.DecRef();
                myMemoryEntry = null;
            }
        }

        private void OnDestroy()
        {
            myIsDestroy = true;

            Unload();
        }

        private void OnEnable()
        {
            if (null != texture)
            {
                return;
            }

            Load();
        }

        private void OnDisable()
        {
            Unload();
        }

        private void OnTextureLoaded(RawImageUrlTextureCacheMemory.MemoryEntry memoryEntry)
        {
            if (memoryEntry == null)
            {
                return;
            }

            myLoadingUrl = "";

            if (memoryEntry.url != null && !memoryEntry.url.Equals(myUrl))
            {
                Debug.LogWarning("OnRawImageUrlLoaded async loaded callback override issue!");
                return;
            }

            myMemoryEntry = memoryEntry;
            myMemoryEntry.IncRef();

            texture = myMemoryEntry.texture;

            if (null != onLoaded)
            {
                onLoaded.Invoke();
            }
        }

        private void OnDeepClone()
        {
            OnTextureLoaded(myMemoryEntry);
        }

        private void OnApplicationQuit()
        {
            myApplicationIsQuitting = true;
        }
    }
}