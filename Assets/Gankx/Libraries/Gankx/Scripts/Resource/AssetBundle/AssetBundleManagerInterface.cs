using System.Collections;
using UnityEngine;

namespace Gankx
{
    internal partial class AssetBundleManager
    {
        public bool IsExist(string resourcePath)
        {
            var isExist = AssetBundleRuntimeUtil.IsExist(resourcePath);
            if (logEnable)
            {
                DebugLog(">> IsExist: ", resourcePath, ", IsExist:", isExist.ToString());
            }

            return isExist;
        }

        public T LoadAsset<T>(string resourcePath) where T : Object
        {
            var bundleName = GetAssetBundleName(resourcePath);
            var assetName = resourcePath.Substring(resourcePath.LastIndexOf("/") + 1);
            return LoadAsset<T>(bundleName, assetName);
        }

        public T LoadAsset<T>(string bundleName, string assetName) where T : Object
        {
            //RemoveFromUnloadList(bundleName);

            T res = null;

            LoadAssetBundleFromFile(bundleName);

            if (myLoadedAssetBundleMap.ContainsKey(bundleName) &&
                myLoadedAssetBundleMap[bundleName].assetBundle != null)
            {
                res = myLoadedAssetBundleMap[bundleName].assetBundle.LoadAsset<T>(assetName);
#if UNITY_EDITOR
                AssetBundlePathUtil.ResetShaderInEditor(res);
#endif
                OnLoadFinish(bundleName);
            }

            return res;
        }

        public LoadAssetRequest LoadAssetAsync<T>(string resourcePath) where T : Object
        {
            var bundleName = GetAssetBundleName(resourcePath);
            var assetName = resourcePath.Substring(resourcePath.LastIndexOf("/") + 1);

            return LoadAssetAsync<T>(bundleName, assetName);
        }

        private LoadAssetRequest LoadAssetAsync<T>(string bundleName, string assetName) where T : Object
        {
            if (string.IsNullOrEmpty(bundleName) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("LoadFromAssetBundleAsync Error: Input is null");
                return null;
            }

            LoadAssetBundleFromFileAsync(bundleName);
            LoadAssetRequest operation = new LoadAssetFromBundleRequest(bundleName, assetName, typeof(T));
            myInProgressOperations.Add(operation);
            return operation;
        }

        public AssetBundle LoadSceneAssetbundle(string scenePath)
        {
            var bundleName = GetAssetBundleName(scenePath);
            LoadAssetBundleFromFile(bundleName);

            var bundle = GetLoadedAssetBundle(bundleName);

            if (bundle != null)
            {
                return bundle.assetBundle;
            }

            return null;
        }

        public void UnloadSceneAssetbundle(string scenePath)
        {
            var bundleName = GetAssetBundleName(scenePath);
            OnLoadFinish(bundleName);
        }

        public bool IsDependentBundle(string assetPath)
        {
            var bundleName = GetAssetBundleName(assetPath);
            bool result;
            myDependencies.TryGetValue(bundleName, out result);
            return result;
        }

        public void AddToUiUnloadList(string assetBundleName, bool unloadRefObjects = false)
        {
            if (string.IsNullOrEmpty(assetBundleName))
            {
                return;
            }

            LoadedAssetBundle bundle;
            myLoadedAssetBundleMap.TryGetValue(assetBundleName, out bundle);
            if (bundle == null)
            {
                return;
            }

            if (bundle.referencedCount > 0)
            {
                return;
            }

            if (unloadRefObjects)
            {
                if (myUiUnloadTrueList.Contains(assetBundleName))
                {
                    return;
                }

                myUiUnloadTrueList.Add(assetBundleName);
            }
            else
            {
                if (myUiUnloadList.Contains(assetBundleName))
                {
                    return;
                }

                myUiUnloadList.Add(assetBundleName);
            }
        }

        public void AddToUnloadFalseList(string assetBundleName)
        {
            if (string.IsNullOrEmpty(assetBundleName))
            {
                return;
            }

            if (myUnloadFalseList.Contains(assetBundleName))
            {
                return;
            }

            StartCoroutine(AddToUnloadFalseListDelay(assetBundleName));
        }

        public IEnumerator AddToUnloadFalseListDelay(string assetBundleName)
        {
            yield return 2;
            myUnloadFalseList.Add(assetBundleName);
        }

        public void RemoveFromUnloadList(string assetBundleName)
        {
            myUiUnloadList.Remove(assetBundleName);
            myUiUnloadTrueList.Remove(assetBundleName);
            myUnloadFalseList.Remove(assetBundleName);
        }
    }
}