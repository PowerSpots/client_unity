using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gankx
{
    public abstract class LoadAssetRequest : CustomAsyncOperation
    {
        private AsyncOperation myLoadRequest;

        private bool myAllowSceneActivation = true;

        private int myPriority;

        public override bool keepWaiting
        {
            get
            {
                return !Execute();
            }
        }

        protected AsyncOperation loadRequest
        {
            get { return myLoadRequest; }
            set
            {
                if (null == value)
                {
                    return;
                }

                myLoadRequest = value;
                myLoadRequest.allowSceneActivation = myAllowSceneActivation;
                myLoadRequest.priority = myPriority;
            }
        }

        public override float progress
        {
            get
            {
                if (null == loadRequest)
                {
                    return 0.0f;
                }

                return loadRequest.progress;
            }
        }

        public override bool allowSceneActivation
        {
            get { return myAllowSceneActivation; }

            set
            {
                myAllowSceneActivation = value;

                if (loadRequest != null)
                {
                    loadRequest.allowSceneActivation = value;
                }
            }
        }

        public override int priority
        {
            get { return myPriority; }

            set
            {
                myPriority = value;

                if (loadRequest != null)
                {
                    loadRequest.priority = value;
                }
            }
        }

        public abstract Object asset { get; }
    }

    public class LoadAssetFromBundleRequest : LoadAssetRequest
    {
        private readonly string myAssetBundleName;

        private readonly string myAssetName;

        private readonly Type myType;

        private AssetBundleRequest myAssetBundleRequest;

        public LoadAssetFromBundleRequest(string bundleName, string assetName, Type type)
        {
            myAssetBundleName = bundleName;
            myAssetName = assetName;
            myType = type;
        }

        public override Object asset
        {
            get
            {
                if (myAssetBundleRequest != null && myAssetBundleRequest.isDone)
                {
                    return myAssetBundleRequest.asset;
                }

                return null;
            }
        }

        protected override bool OnExecute()
        {
            if (string.IsNullOrEmpty(myAssetBundleName) || string.IsNullOrEmpty(myAssetName))
            {
                Debug.LogError("LoadAssetFromBundleRequest error: invalid parameter");
                return true;
            }

            if (myAssetBundleRequest == null)
            {
                var loadError = AssetBundleManager.instance.GetAsyncLoadError(myAssetBundleName);
                if (loadError != null)
                {
                    return true;
                }

                var bundle = AssetBundleManager.instance.GetLoadedAssetBundle(myAssetBundleName);
                if (bundle != null && bundle.assetBundle != null)
                {
                    try
                    {
                        myAssetBundleRequest = bundle.assetBundle.LoadAssetAsync(myAssetName, myType);
                        loadRequest = myAssetBundleRequest;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("LoadAssetFromBundleRequest Error in load bundle:" + myAssetBundleName +
                                       ",ERROR:" + e);
                        return true;
                    }
                }
            }
            else
            {
                if (myAssetBundleRequest.isDone)
                {
#if UNITY_EDITOR
                    AssetBundlePathUtil.ResetShaderInEditor(myAssetBundleRequest.asset as GameObject);
#endif
                    AssetBundleManager.instance.OnLoadFinish(myAssetBundleName);
                    return true;
                }
            }

            return false;
        }
    }

    public class LoadAssetFromResourceRequest : LoadAssetRequest
    {
        private readonly ResourceRequest myResourceRequest;

        public LoadAssetFromResourceRequest(ResourceRequest request)
        {
            myResourceRequest = request;
            if (myResourceRequest == null)
            {
                Debug.LogError("LoadAssetFromResourceRequest error: invalid parameter");
            }
        }

        public override Object asset
        {
            get
            {
                if (myResourceRequest != null && myResourceRequest.isDone)
                {
                    return myResourceRequest.asset;
                }

                return null;
            }
        }

        protected override bool OnExecute()
        {
            return myResourceRequest == null || myResourceRequest.isDone;
        }
    }
}