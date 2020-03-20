using System;
using System.Collections;
using Gankx.UI;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gankx
{
    public class ResourceService : Singleton<ResourceService>
    {
        public static bool useAssetBundle = true;

#if UNITY_EDITOR
        private bool myUseAssetBundleInEditor;
#endif

        private bool myInitialized;
        private bool myCollecting;
        private bool myShouldCollectAll;

        protected override void OnInit()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (myInitialized)
            {
                return;
            }

#if UNITY_EDITOR
            myUseAssetBundleInEditor = ResourceLoadModeInEditor.instance.UsingBundle;
            useAssetBundle = myUseAssetBundleInEditor;
#endif

            Debug.Log(useAssetBundle ? "ResourceService Load Mode：AssetBundle" : "ResourceService Load Mode：Resources");

            if (useAssetBundle)
            {
                AssetBundleManager.instance.Initialize();
            }

            SceneBuildSetting.instance.Initialize();

            RawImageUrlTextureManager.instance.Init();

            myInitialized = true;
        }

        protected override void OnRelease()
        {
            StopAllCoroutines();
            UnloadUnusedAssets();
        }

        public static RenderTexture GetRendertTexture(int width, int height, int depthBuffer,
            RenderTextureFormat format = RenderTextureFormat.Default,
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default, int antiAliasing = 1)
        {
            var rt = RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing);
            return rt;
        }

        public static RenderTexture CreateRendertTexture(int width, int height, int depthBuffer,
            RenderTextureFormat format = RenderTextureFormat.Default,
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default)
        {
            var rt = new RenderTexture(width, height, depthBuffer, format, readWrite);
            return rt;
        }

        private static bool IsUsingAssetBundle(string resourcePath)
        {
            if (!Application.isPlaying)
            {
                return false;
            }

            if (useAssetBundle)
            {
                if (AssetBundleRuntimeUtil.IsloadUsingBundle(resourcePath))
                {
                    return true;
                }
            }

            return false;
        }

        public static T Load<T>(string resourcesPath) where T : Object
        {
            if (string.IsNullOrEmpty(resourcesPath))
            {
                Debug.LogError("ResourceService.Load Error:resourcesPath" + resourcesPath + " is empty ");
                return null;
            }

            T refabObj;
            if (IsUsingAssetBundle(resourcesPath))
            {
                refabObj = LoadFromBundle<T>(resourcesPath);
            }
            else
            {
                refabObj = LoadFromResources<T>(resourcesPath);
            }

            return refabObj;
        }

        public static LoadAssetRequest LoadAsync<T>(string resourcesPath) where T : Object
        {
            if (string.IsNullOrEmpty(resourcesPath))
            {
                Debug.LogError("ResourceService.LoadAsync Error:resourcesPath" + resourcesPath + " is empty ");
                return null;
            }

            if (IsUsingAssetBundle(resourcesPath))
            {
                return LoadFromBundleAsync<T>(resourcesPath);
            }

            var request = LoadFromResourcesAsync<T>(resourcesPath);
            if (request != null)
            {
                return new LoadAssetFromResourceRequest(request);
            }

            return null;
        }

        public static void Unload()
        {
            // dummy
        }

        public static Texture2D LoadRawImageTexture(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }

            Texture2D texture;

            if (IsUsingAssetBundle(assetPath))
            {
                texture = AssetBundleManager.instance.LoadAsset<Texture2D>(assetPath);
                if (null == texture)
                {
                    Debug.LogError("ResouceService.LoadRawImageTexture occured error, load failed, assetPath: " +
                                   assetPath);
                }

                return texture;
            }

            texture = Resources.Load<Texture2D>(assetPath);

            return texture;
        }

        public static void UnloadRawImageTexture(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            if (IsUsingAssetBundle(assetPath))
            {
                var assetBundleName = AssetBundleManager.instance.GetAssetBundleName(assetPath);
                AssetBundleManager.instance.DecAssetBundleRef(assetBundleName);
                AssetBundleManager.instance.AddToUiUnloadList(assetBundleName, true);
            }
        }

        public static Sprite LoadBundleSprite(string bundleName, string spriteName)
        {
            if (string.IsNullOrEmpty(bundleName))
            {
                return null;
            }

            bundleName = bundleName.ToLower();
            var sprite = AssetBundleManager.instance.LoadAsset<Sprite>(bundleName, spriteName);
            if (null == sprite)
            {
                Debug.LogError("ResouceService.LoadBundleSprite occured error, load failed, bundleName: " + bundleName);
                return null;
            }

            return sprite;
        }

        public static void UnloadBundleSprite(string bundleName)
        {
            AssetBundleManager.instance.DecAssetBundleRef(bundleName);
            AssetBundleManager.instance.AddToUiUnloadList(bundleName, true);
        }

        [UsedImplicitly]
        public static GameObject LoadPanel(string assetPath)
        {
            return Load<GameObject>(assetPath);
        }

        public static void UnloadPanel(string resourcePath, bool unloadRefObjects = false)
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                return;
            }

            if (IsUsingAssetBundle(resourcePath))
            {
                var assetBundleName = AssetBundleManager.instance.GetAssetBundleName(resourcePath);
                AssetBundleManager.instance.DecAssetBundleRef(assetBundleName);
                AssetBundleManager.instance.AddToUiUnloadList(assetBundleName, unloadRefObjects);
            }
        }

        public static GameObject LoadControl(string assetPath)
        {
            return Load<GameObject>(assetPath);
        }

        public static void UnloadControl(string resourcePath, bool unloadRefObjects = false)
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                return;
            }

            if (IsUsingAssetBundle(resourcePath))
            {
                var assetBundleName = AssetBundleManager.instance.GetAssetBundleName(resourcePath);
                AssetBundleManager.instance.DecAssetBundleRef(assetBundleName);
                AssetBundleManager.instance.AddToUiUnloadList(assetBundleName, unloadRefObjects);
            }
        }

        public static Sprite LoadIcon(string resourcePath)
        {
            return Load<Sprite>(resourcePath);
        }

        public static void UnloadIcon(string resourcePath, bool unloadRefObjects = false)
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                return;
            }

            if (IsUsingAssetBundle(resourcePath))
            {
                var assetBundleName = AssetBundleManager.instance.GetAssetBundleName(resourcePath);
                AssetBundleManager.instance.DecAssetBundleRef(assetBundleName);
                AssetBundleManager.instance.AddToUiUnloadList(assetBundleName, unloadRefObjects);
            }
        }

        public static AtlasAsset LoadAtlas(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }

            AtlasAsset atlasAsset;

            if (IsUsingAssetBundle("atlas/" + assetPath))
            {
                assetPath = "atlas/" + assetPath.Replace('/', '_');
                var atlasObject = AssetBundleManager.instance.LoadAsset<GameObject>(assetPath);
                if (null == atlasObject)
                {
                    Debug.LogError("ResouceService.LoadAtlas occured error, load failed, assetPath: " + assetPath);
                    return null;
                }

                atlasAsset = atlasObject.GetComponent<AtlasAsset>();
                if (null == atlasAsset)
                {
                    Debug.LogError(
                        "ResouceService.LoadAtlas occured error, no UIAtlas Component is found, assetPath: " +
                        assetPath);
                    return null;
                }
            }
            else
            {
                assetPath = "atlas/" + assetPath.Replace('/', '_');
                atlasAsset = Resources.Load<AtlasAsset>(assetPath);
            }

            if (null != atlasAsset)
            {
                atlasAsset.Init();
            }

            return atlasAsset;
        }

        public static void UnloadAtlas(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            if (IsUsingAssetBundle("atlas/" + assetPath))
            {
                assetPath = assetPath.ToLower();
                assetPath = "atlas/" + assetPath.Replace('/', '_');
                var assetBundleName = AssetBundleManager.instance.GetAssetBundleName(assetPath);
                AssetBundleManager.instance.DecAssetBundleRef(assetBundleName);
                AssetBundleManager.instance.AddToUiUnloadList(assetBundleName, true);
            }
        }

        /**
         * LOad dragon bone anim res
         */
        public static GameObject loadDBRes(string dbAssetPath)
        {
            return Load<GameObject>(dbAssetPath);
        }

        private static T LoadFromBundle<T>(string resourcePath) where T : Object
        {
            var obj = AssetBundleManager.instance.LoadAsset<T>(resourcePath);
            return obj;
        }

        private static LoadAssetRequest LoadFromBundleAsync<T>(string resourcePath) where T : Object
        {
            return AssetBundleManager.instance.LoadAssetAsync<T>(resourcePath);
        }

        private static ResourceRequest LoadFromResourcesAsync<T>(string resourcePath) where T : Object
        {
            return Resources.LoadAsync<T>(resourcePath);
        }

        public static T LoadFromResources<T>(string resourcePath) where T : Object
        {
            try
            {
                var go = Resources.Load<T>(resourcePath);
                return go;
            }
            catch (Exception ex)
            {
                Debug.LogError("ResourceService.LoadFromResources<T> Error:" + ex);
            }

            return null;
        }

        public static void TryReleaseUnuseBundle()
        {
            if (useAssetBundle)
            {
                AssetBundleManager.instance.TryReleaseUnuseBundle();
            }
        }

        public static void UnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
        }


        public static void Collect()
        {
            GC.Collect();
        }

        private void LateUpdate()
        {
            if (myShouldCollectAll)
            {
                CollectAll();
                myShouldCollectAll = false;
            }
        }

        public void MarkForCollectAll()
        {
            myShouldCollectAll = true;
        }

        public void CollectAll()
        {
            myShouldCollectAll = false;
            if (myCollecting)
            {
                GC.Collect();
            }
            else
            {
                myCollecting = true;
                StartCoroutine(CollectAllCoroutine());
            }
        }

        private IEnumerator CollectAllCoroutine()
        {
            GC.Collect();
            yield return null;

            UnloadUnusedAssets();
            myCollecting = false;
        }
    }
}