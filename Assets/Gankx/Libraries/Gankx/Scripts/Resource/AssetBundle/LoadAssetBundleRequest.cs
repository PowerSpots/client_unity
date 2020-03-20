using Microsoft.Win32;
using UnityEngine;

namespace Gankx
{
    public abstract class LoadAssetBundleRequest : CustomAsyncOperation
    {
        protected LoadAssetBundleRequest(string assetBundleName)
        {
            this.assetBundleName = assetBundleName;
        }

        public override float progress
        {
            get { return 0.0f; }
        }

        public override int priority
        {
            get { return 0;} 
            set { }
        }
        public override bool allowSceneActivation
        {
            get { return false;}
            set { }
        }

        public string assetBundleName { get; private set; }
        public LoadedAssetBundle assetBundle { get; protected set; }
        public string error { get; protected set; }
    }

    public class LoadAssetBundleFromFileRequest : LoadAssetBundleRequest
    {
        private AssetBundleCreateRequest myRequest;

        public LoadAssetBundleFromFileRequest(string assetBundleName, AssetBundleCreateRequest request) : base(
            assetBundleName)
        {
            myRequest = request;
        }

        protected override bool OnExecute()
        {
            // TODO AssetBundle Refactor: it's a smelly dependency
            var loadedAssetBundle = AssetBundleManager.instance.GetLoadedAssetBundle(assetBundleName);
            if (loadedAssetBundle != null)
            {
                assetBundle = loadedAssetBundle;
                return true;
            }

            if (null == myRequest)
            {
                return true;
            }

            if (!myRequest.isDone)
            {
                return false;
            }

            var bundle = myRequest.assetBundle;

            if (null == bundle)
            {
                error = string.Format("{0} is not a valid asset bundle from file!", assetBundleName);
            }
            else
            {
                // TODO AssetBundle Refactor: it's a smelly dependency
                loadedAssetBundle = AssetBundleManager.instance.GetDownloadingAssetBundle(assetBundleName);
                if (loadedAssetBundle != null)
                {
                    loadedAssetBundle.SetBundle(bundle);
                    assetBundle = loadedAssetBundle;
                }
                else
                {
                    assetBundle = new LoadedAssetBundle(assetBundleName, bundle);
                }
            }

            return true;
        }
    }

    public class LoadAssetBundleFromWebRequest : LoadAssetBundleRequest
    {
        private WWW myWWW;

        public LoadAssetBundleFromWebRequest(string assetBundleName, WWW www)
            : base(assetBundleName)
        {
            myWWW = www;
        }

        protected override bool OnExecute()
        {
            // TODO AssetBundle Refactor: it's a smelly dependency
            var loadedAssetBundle = AssetBundleManager.instance.GetLoadedAssetBundle(assetBundleName);
            if (loadedAssetBundle != null)
            {
                assetBundle = loadedAssetBundle;
                return true;
            }

            if (null == myWWW)
            {
                return true;
            }

            if (myWWW.isDone)
            {
                return false;
            }

            error = myWWW.error;
            if (!string.IsNullOrEmpty(error))
            {
                return true;
            }

            var bundle = myWWW.assetBundle;
            if (null == bundle)
            {
                error = string.Format("{0} is not a valid asset bundle from web url:{1}!", assetBundleName, myWWW.url);
            }
            else
            {
                // TODO AssetBundle Refactor: it's a smelly dependency
                loadedAssetBundle = AssetBundleManager.instance.GetDownloadingAssetBundle(assetBundleName);
                if (loadedAssetBundle != null)
                {
                    loadedAssetBundle.SetBundle(bundle);
                    assetBundle = loadedAssetBundle;
                }
                else
                {
                    assetBundle = new LoadedAssetBundle(assetBundleName, bundle);
                }
            }

            myWWW.Dispose();
            myWWW = null;

            return true;
        }
    }
}