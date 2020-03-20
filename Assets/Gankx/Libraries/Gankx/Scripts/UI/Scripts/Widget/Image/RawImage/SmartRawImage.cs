using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Gankx.UI
{
    [AddComponentMenu("UI/Image/Smart RawImage")]
    public class SmartRawImage : RawImage, ISerializationCallbackReceiver, ILayoutElement
    {
        public static readonly Dictionary<int, string> TexturePathDict = new Dictionary<int, string>();

        private LruRefTexture myRefTexture;

        [SerializeField]
        [FormerlySerializedAs("_texturePath")]
        private string myTexturePath;

        public string texturePath
        {
            get { return myTexturePath; }
            set
            {
                if (myTexturePath == value)
                {
                    return;
                }

                myTexturePath = value;
                base.texture = null;

                if (Application.isPlaying && ResourceService.useAssetBundle)
                {
                    if (null != myRefTexture)
                    {
                        myRefTexture.DecRef();

                        myRefTexture = null;
                    }

                    if (!UITools.GetActive(gameObject))
                    {
                        return;
                    }

                    if (!string.IsNullOrEmpty(myTexturePath))
                    {
                        myRefTexture = RawImageAssetTextureManager.instance.GetTexture(myTexturePath);
                        if (null != myRefTexture)
                        {
                            myRefTexture.IncRef();

                            base.texture = myRefTexture.texture;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(myTexturePath))
                    {
                        base.texture = ResourceService.Load<Texture>(myTexturePath);
                    }
                }
            }
        }

        public new Texture texture
        {
            get { return base.texture; }
            set
            {
                base.texture = value;

                myTexturePath = null;
            }
        }

        public virtual void CalculateLayoutInputHorizontal()
        {
        }

        public virtual void CalculateLayoutInputVertical()
        {
        }

        public virtual float minWidth
        {
            get { return 0.0f; }
        }

        public virtual float preferredWidth
        {
            get
            {
                if (null == texture)
                {
                    return 0.0f;
                }

                return texture.width;
            }
        }

        public virtual float flexibleWidth
        {
            get { return -1f; }
        }

        public virtual float minHeight
        {
            get { return 0.0f; }
        }

        public virtual float preferredHeight
        {
            get
            {
                if (null == texture)
                {
                    return 0.0f;
                }

                return texture.height;
            }
        }

        public virtual float flexibleHeight
        {
            get { return -1f; }
        }

        public virtual int layoutPriority
        {
            get { return 0; }
        }

        public void OnBeforeSerialize()
        {
            if (Application.isPlaying)
            {
                return;
            }

            myTexturePath = null;

            if (null == base.texture)
            {
                return;
            }

            string texturePathValue;
            if (!TexturePathDict.TryGetValue(base.texture.GetInstanceID(), out texturePathValue))
            {
                return;
            }

            myTexturePath = texturePathValue;
        }

        public void OnAfterDeserialize()
        {
        }

        protected override void OnEnable()
        {
            if (Application.isPlaying && ResourceService.useAssetBundle)
            {
                if (null == myRefTexture && !string.IsNullOrEmpty(myTexturePath))
                {
                    myRefTexture = RawImageAssetTextureManager.instance.GetTexture(myTexturePath);
                }

                if (null != myRefTexture)
                {
                    myRefTexture.IncRef();

                    if (null == base.texture)
                    {
                        base.texture = myRefTexture.texture;
                    }
                }
            }

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            if (Application.isPlaying && ResourceService.useAssetBundle)
            {
                if (null != myRefTexture)
                {
                    myRefTexture.DecRef();

                    myRefTexture = null;

                    base.texture = null;
                }
            }

            base.OnDisable();
        }
    }
}