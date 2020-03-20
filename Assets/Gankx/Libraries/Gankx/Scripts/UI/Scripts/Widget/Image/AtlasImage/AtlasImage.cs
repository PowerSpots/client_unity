using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Gankx.UI
{
    [AddComponentMenu("UI/Image/Atlas Image")]
    public class AtlasImage : Image
    {
        public static readonly Dictionary<int, string> SpriteAtlasDict = new Dictionary<int, string>();
        public static readonly Dictionary<int, string> SpriteBundleNameDict = new Dictionary<int, string>();

        private LruRefAtlas myAtlas;

        [SerializeField]
        [FormerlySerializedAs("_atlasName")]
        private string myAtlasName;

        private BundleSprite myBundleSprite;

        [SerializeField]
        [FormerlySerializedAs("_bundleName")]
        private string myBundleName;

        [SerializeField]
        [FormerlySerializedAs("_spriteName")]
        private string mySpriteName;

        private bool myNativeSize;

        public new Sprite sprite
        {
            get { return base.sprite; }
            set
            {
                if (ResourceService.useAssetBundle == false)
                {
                    base.sprite = value;
                    return;
                }

                if (Application.isPlaying)
                {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || DEVELOPMENT_BUILD
                    var oldSpriteName = mySpriteName;
#endif

                    if (null == value)
                    {
                        if (null != myAtlas)
                        {
                            myAtlas.DecRef();
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || DEVELOPMENT_BUILD
                            myAtlas.RemoveRefSpriteName(mySpriteName);
#endif
                        }

                        mySpriteName = null;
                        myAtlasName = null;
                        myAtlas = null;
                        base.sprite = null;
                        return;
                    }

                    var oldAtlas = myAtlas;

                    BindAtlas(value);
                    base.sprite = value;

                    if (oldAtlas == myAtlas)
                    {
                        return;
                    }

                    if (null != oldAtlas)
                    {
                        oldAtlas.DecRef();
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || DEVELOPMENT_BUILD
                        oldAtlas.RemoveRefSpriteName(oldSpriteName);
#endif
                    }

                    if (null != myAtlas)
                    {
                        myAtlas.IncRef();
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || DEVELOPMENT_BUILD
                        myAtlas.AddRefSpriteName(mySpriteName);
#endif
                    }
                }
                else
                {
                    base.sprite = value;
                }
            }
        }

        public string atlasName
        {
            get { return myAtlasName; }
        }

        public void SetAtlasPath(string atlasPath)
        {
            myAtlasName = null;
            mySpriteName = null;
            if (null != myAtlas)
            {
                myAtlas.DecRef();
                myAtlas = null;
            }

            var indexOfSlash = atlasPath.LastIndexOf('/');
            if (-1 == indexOfSlash)
            {
                return;
            }

            myAtlasName = atlasPath.Substring(0, indexOfSlash).Replace('/', '_');
            mySpriteName = atlasPath.Substring(indexOfSlash + 1);

            if (ResourceService.useAssetBundle == false)
            {
                var atlas = ResourceService.LoadAtlas(myAtlasName);
                if (null == atlas)
                {
                    return;
                }

                base.sprite = atlas.GetSprite(mySpriteName);
                return;
            }

            if (UITools.GetActive(this))
            {
                TryUpdateAtlasAndSprite();
            }
        }

        public override void SetNativeSize()
        {
            if (UITools.GetActive(this))
            {
                myNativeSize = false;
                base.SetNativeSize();
                return;
            }

            myNativeSize = true;
        }

        private void BindAtlas(Sprite atlasSprite)
        {
            myNativeSize = false;
            mySpriteName = null;
            myAtlasName = null;
            myAtlas = null;

            if (null == atlasSprite)
            {
                return;
            }

            mySpriteName = atlasSprite.name;

            if (null == atlasSprite.texture)
            {
                return;
            }

            var texName = atlasSprite.texture.name;
            var index1 = texName.IndexOf('-');
            var index2 = texName.IndexOf('-', index1 + 1);
            if (-1 == index1 || -1 == index2)
            {
                return;
            }

            myAtlasName = "atlas_" + texName.Substring(index1 + 1, index2 - index1 - 1);
            if (UITools.GetActive(this))
            {
                myAtlas = AtlasManager.instance.GetAtlas(myAtlasName);
            }
        }

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();

            if (Application.isPlaying)
            {
                return;
            }

            myAtlasName = null;
            mySpriteName = null;
            myBundleName = null;

            if (null == base.sprite)
            {
                return;
            }

            mySpriteName = base.sprite.name;

            string spriteAtlasName;
            if (SpriteAtlasDict.TryGetValue(base.sprite.GetInstanceID(), out spriteAtlasName))
            {
                myAtlasName = spriteAtlasName;
            }

            string spriteBundleName;
            if (SpriteBundleNameDict.TryGetValue(base.sprite.GetInstanceID(), out spriteBundleName))
            {
                myBundleName = spriteBundleName;
            }
        }

        protected override void OnEnable()
        {
            if (Application.isPlaying && ResourceService.useAssetBundle)
            {
                if (!string.IsNullOrEmpty(myBundleName))
                {
                    myBundleSprite = BundleSpriteManager.instance.GetBundleSprite(myBundleName, mySpriteName);
                    if (null != myBundleSprite)
                    {
                        myBundleSprite.IncRef();
                        if (null == base.sprite)
                        {
                            base.sprite = myBundleSprite.sprite;
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(myAtlasName))
                {
                    TryUpdateAtlasAndSprite();
                }
            }

            base.OnEnable();

            if (myNativeSize)
            {
                myNativeSize = false;
                base.SetNativeSize();
            }
        }

        protected override void OnDisable()
        {
            if (Application.isPlaying && ResourceService.useAssetBundle)
            {
                if (null != myBundleSprite)
                {
                    myBundleSprite.DecRef();
                    myBundleSprite = null;
                    base.sprite = null;
                }

                if (null != myAtlas)
                {
                    myAtlas.DecRef();

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || DEVELOPMENT_BUILD
                    myAtlas.RemoveRefSpriteName(mySpriteName);
#endif

                    myAtlas = null;
                    base.sprite = null;
                }
            }

            base.OnDisable();
        }

        private void TryUpdateAtlasAndSprite()
        {
            if (null != myAtlas)
            {
                return;
            }

            if (string.IsNullOrEmpty(myAtlasName))
            {
                return;
            }

            myAtlas = AtlasManager.instance.GetAtlas(myAtlasName);
            if (null != myAtlas)
            {
                myAtlas.IncRef();

                base.sprite = myAtlas.GetSprite(mySpriteName);

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || DEVELOPMENT_BUILD
                myAtlas.AddRefSpriteName(mySpriteName);
#endif
            }
        }
    }
}