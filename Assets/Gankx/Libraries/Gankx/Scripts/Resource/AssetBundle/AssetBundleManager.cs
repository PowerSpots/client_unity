using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gankx
{
    internal partial class AssetBundleManager : Singleton<AssetBundleManager>
    {
        private AssetBundleManifest myManifest;

        private readonly Dictionary<string, LoadedAssetBundle> myLoadedAssetBundleMap =  new Dictionary<string, LoadedAssetBundle>();
        private readonly Dictionary<string, string[]> myDependencieMap = new Dictionary<string, string[]>();
        // ReSharper disable once InconsistentNaming
        private readonly Dictionary<string, WWW> myLoadingWWWMap = new Dictionary<string, WWW>();

        private readonly Dictionary<string, LoadedAssetBundle> myDownloadingBundleMap = new Dictionary<string, LoadedAssetBundle>();
        private readonly Dictionary<string, AssetBundleCreateRequest> myDownloadingBundleRequestMap = new Dictionary<string, AssetBundleCreateRequest>();
        private readonly List<CustomAsyncOperation> myInProgressOperations = new List<CustomAsyncOperation>();
        private readonly Dictionary<string, string> myDownloadingErrors = new Dictionary<string, string>();
        private readonly Dictionary<string, bool> myDependencies = new Dictionary<string, bool>();

        private List<string> myUiUnloadList = new List<string>();
        private List<string> myUiUnloadTrueList = new List<string>();
        private List<string> myUnloadFalseList = new List<string>();

        private bool myInitialized;
        private bool myLogEnable;

        public bool logEnable
        {
            get { return myLogEnable; }
            set { myLogEnable = value; }
        }

        private void Update()
        {
            UpdateAsyncProcessOperation();
            PrcoessUiUnloadList();
            PrcoessUnloadList();
        }

        public void Initialize()
        {
            if (false == myInitialized || null == myManifest)
            {
                LoadManifest();

                AssetBundleRuntimeUtil.Init();

                myInitialized = true;
            }
        }

        public AssetBundleManifest GetAssetBundleManifest()
        {
            if (myManifest == null)
            {
                Initialize();
            }

            return myManifest;
        }

        public LoadedAssetBundle GetLoadedAssetBundle(string assetBundleName)
        {
            LoadedAssetBundle bundle;
            myLoadedAssetBundleMap.TryGetValue(assetBundleName, out bundle);
            if (bundle == null)
            {
                return null;
            }

            string[] dependencies;
            if (!myDependencieMap.TryGetValue(assetBundleName, out dependencies))
            {
                return bundle;
            }

            for (int i = 0; i < dependencies.Length; i++)
            {
                string dependency = dependencies[i];

                LoadedAssetBundle dependentBundle;
                myLoadedAssetBundleMap.TryGetValue(dependency, out dependentBundle);
                if (dependentBundle == null)
                {
                    return null;
                }
            }

            return bundle;
        }

        public LoadedAssetBundle GetDownloadingAssetBundle(string assetBundleName)
        {
            LoadedAssetBundle bundle;
            myDownloadingBundleMap.TryGetValue(assetBundleName, out bundle);
            return bundle;
        }

        private void LoadManifest()
        {
            string manifestName = AssetBundleConfig.GetManifestName();
            LoadAssetBundleFromFile(manifestName, false);

            if (myLoadedAssetBundleMap.ContainsKey(manifestName) && myLoadedAssetBundleMap[manifestName].assetBundle != null)
            {
                myManifest = myLoadedAssetBundleMap[manifestName].assetBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
            }
            LoadedAssetBundle bundle;
            myLoadedAssetBundleMap.TryGetValue(manifestName, out bundle);
            if (bundle != null)
            {
                RemoveFromLoadedBundleListAndUnload(manifestName);
            }

            if (myManifest == null)
            {
                Debug.LogError("LoadManifest failed!");
            }
        }

		public string GetAssetBundleName(string assetPath)
		{
			if (string.IsNullOrEmpty(assetPath))
			{
				return string.Empty;
			}

			assetPath = AssetBundlePathUtil.ToAssetPath(assetPath);

			// Effect LOD
			if (assetPath.Contains("effect/"))
			{
				int posIndex = assetPath.IndexOf("_lod1");
				if (posIndex > 0)
				{
					assetPath = assetPath.Substring(0 , posIndex);
				}
				else
				{
					posIndex = assetPath.IndexOf("_lod2");
					if (posIndex > 0)
					{
						assetPath = assetPath.Substring(0, posIndex);
					}
				}
			}

			return AssetBundleRuntimeUtil.GetAssetBundleName(assetPath);
		}

        private string[] GetDependenciesList(string assetBundleName)
        {
            string[] fileList = {};
            if (string.IsNullOrEmpty(assetBundleName))
            {
                return fileList;
            }

            if (null == myManifest)
            {
                Debug.LogError("GetDependenciesList Error: Manifest is missing");
                return fileList;
            }

            string[] dependencies = GetAssetBundleManifest().GetAllDependencies(assetBundleName);
            List<string> validList = dependencies.ToList();
            validList.RemoveAll(DontNeedDependency);
            dependencies = validList.ToArray();

            return dependencies;
        }

        private static bool DontNeedDependency(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return true;
            }

            if (name.Contains("atlas_atlas_")
                || name.Contains("ui_rawimage_")
                || name.Contains("ui_v2_noatlas_sprites_")
                || name.Contains("ui_icon_"))
            {
                return true;
            }

            return false;
        }

        private void LoadAssetBundleFromFile(string assetBundleName, bool checkDependencies = true)
        {
            if (logEnable)
            {
                DebugLog(">> <color=yellow> LoadAssetBundleFromFile </color>" , assetBundleName);
            }

            if (checkDependencies)
            {
                LoadDependenciesFromFile(assetBundleName);
            }

            LoadAssetBundleFromFileInternal(assetBundleName);
        }

        private void LoadDependenciesFromFile(string assetBundleName)
        {
            if (GetAssetBundleManifest() == null)
            {
                Debug.LogError("Please call Initialize() to initialize GetAssetBundleManifest");
                return;
            }

            string[] dependencies = GetDependenciesList(assetBundleName);
            if (dependencies.Length == 0)
            {
                return;
            }

            if (!myDependencieMap.ContainsKey(assetBundleName))
            {
                myDependencieMap.Add(assetBundleName, dependencies);
            }

            for (int i = 0; i < dependencies.Length; i++)
            {
                myDependencies[dependencies[i]] = true;
                LoadAssetBundleFromFileInternal(dependencies[i]);
            }
        }

        private void LoadAssetBundleFromFileInternal(string assetBundleName)
        {
            if (myLogEnable)
            {
                DebugLog("      >>>> LoadAssetBundleFromFileInternal", assetBundleName);
            }

            if (string.IsNullOrEmpty(assetBundleName))
            {
                return;
            }

            RemoveFromUnloadList(assetBundleName);

            LoadedAssetBundle bundle;
            AssetBundle assetBundle = null;

            myLoadedAssetBundleMap.TryGetValue(assetBundleName, out bundle);
            if (bundle != null)
            {
                bundle.IncRef();
                return;
            }

            bool bInAsync = false;
            bundle = GetDownloadingAssetBundle(assetBundleName);
            if (bundle != null)
            {
                bInAsync = true;
                myDownloadingBundleMap.Remove(assetBundleName);

                AssetBundleCreateRequest request;
                if (myDownloadingBundleRequestMap.TryGetValue(assetBundleName, out request))
                {
                    myDownloadingBundleRequestMap.Remove(assetBundleName);

                    if (request != null && request.isDone)
                    {
                        if (myLogEnable)
                        {
                            DebugLog("      >>>> <color=red> LoadAssetBundleFromFileInternal request is done but not in the loaded list   </color>, BundleName:", assetBundleName);
                        }

                        bundle.IncRef();
                        bundle.SetBundle(request.assetBundle);
                       
                        AddToLoadedBundleList(assetBundleName, bundle);

                        return;
                    }

                    assetBundle = request.assetBundle;
                }
            }
            
            if (assetBundle == null)
            {
                string url = AssetBundleConfig.GetAssetBundlePath(assetBundleName);

                try
                {
                    assetBundle = AssetBundle.LoadFromFile(url);
                }
                catch (Exception e)
                {
                    Debug.LogError("Load AssetBundle @ [" + url + "], Error [" + e + "]");
                }
            }           
       
            if (bInAsync && bundle != null)
            {
                bundle.IncRef();
                bundle.SetBundle(assetBundle);
            }
            else
            {
                bundle = new LoadedAssetBundle(assetBundleName, assetBundle);
            }
            
            AddToLoadedBundleList(assetBundleName, bundle);
        }

        private void LoadAssetBundleFromFileAsync(string assetBundleName)
        {
            if (logEnable)
            {
                DebugLog(">> <color=yellow>LoadAssetBundleFromFileAsync</color>, BundleName:", assetBundleName);
            }

            LoadAssetBundleFromFileAsyncInternal(assetBundleName);
            LoadDependenciesFromFileAsync(assetBundleName);
        }

        private void LoadDependenciesFromFileAsync(string assetBundleName)
        {
            string[] dependencies = GetDependenciesList(assetBundleName);
            if (dependencies.Length == 0)
            {
                return;
            }

            if (!myDependencieMap.ContainsKey(assetBundleName))
            {
                myDependencieMap.Add(assetBundleName, dependencies);
            }

            for (int i = 0; i < dependencies.Length; i++)
            {
                myDependencies[dependencies[i]] = true;
                LoadAssetBundleFromFileAsyncInternal(dependencies[i]);
            }
        }

        private void LoadAssetBundleFromFileAsyncInternal(string assetBundleName)
        {
            if (logEnable)
            {
                DebugLog("      >>>> LoadAssetBundleFromFileAsyncInternal BundleName:", assetBundleName);
            }

            RemoveFromUnloadList(assetBundleName);

            LoadedAssetBundle bundle;
            myLoadedAssetBundleMap.TryGetValue(assetBundleName, out bundle);

            if (bundle != null)
            {
                bundle.IncRef();
                return;
            }

            bundle = GetDownloadingAssetBundle(assetBundleName); 
            if (bundle != null)
            {
                bundle.IncRef();
                return;
            }

            string url = AssetBundleConfig.GetAssetBundlePath(assetBundleName);
            
            var operation = AssetBundle.LoadFromFileAsync(url);
            myInProgressOperations.Add(new LoadAssetBundleFromFileRequest(assetBundleName, operation));
            bundle = new LoadedAssetBundle(assetBundleName);
            myDownloadingBundleMap.Add(assetBundleName, bundle);
            myDownloadingBundleRequestMap.Add(assetBundleName, operation);
        }

        private void PrcoessUiUnloadList()
        {
            for (int i = 0; i < myUiUnloadTrueList.Count; i++)
            {
                RemoveFromLoadedBundleListAndUnload(myUiUnloadTrueList[i], true);
            }

            for (int i = 0; i < myUiUnloadList.Count; i++)
            {
                RemoveFromLoadedBundleListAndUnload(myUiUnloadList[i]);
            }

            myUiUnloadTrueList.Clear();
            myUiUnloadList.Clear();    
        }

        private void PrcoessUnloadList()
        {
            if (myUnloadFalseList.Count <= 0 )
            {
                return;
            }

            if (myInProgressOperations.Count > 0)
            {
                return;
            }

            if (myDownloadingBundleMap.Count > 0)
            {
                Debug.LogError("AssetBundleManager.PrcoessUnloadList, mInProgressOperations is empty but mDownloadingBundleMap is not empty");
            }

            for (int i = 0; i < myUnloadFalseList.Count; i++)
            {
                RemoveFromLoadedBundleListAndUnload(myUnloadFalseList[i]);
            }

            myUnloadFalseList.Clear();
        }

        private void UpdateAsyncProcessOperation()
        {
            for (int i = 0; i < myInProgressOperations.Count;)
            {
                var operation = myInProgressOperations[i];
                if (operation.Execute())
                {                    
                    if (operation is LoadAssetBundleRequest)
                    {
                        ProcessFinishedOperation(operation);
                    }

                    myInProgressOperations.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        private void ProcessFinishedOperation(CustomAsyncOperation operation)
        {
            LoadAssetBundleRequest download = operation as LoadAssetBundleRequest;
            if (download == null)
            {
                return;
            }

            if (download.error == null)
            {
                if (logEnable)
                {
                    DebugLog(">> <color=yellow>ProcessFinishedOperation Load Complete</color>, BundleName:", download.assetBundleName);
                }

                AddToLoadedBundleList(download.assetBundleName, download.assetBundle);                
            }
            else
            {
                if (!myDownloadingErrors.ContainsKey(download.assetBundleName))
                {
                    string msg = string.Format("Bundle download failed, please check {0} : {1}", download.assetBundleName, download.error);
                    myDownloadingErrors.Add(download.assetBundleName, msg);
                    Debug.LogError(msg);
                }
            }

            myDownloadingBundleMap.Remove(download.assetBundleName);
            myDownloadingBundleRequestMap.Remove(download.assetBundleName);
        }

        public string GetAsyncLoadError(string bundlename)
        {
            if (myDownloadingErrors.ContainsKey(bundlename))
            {
                return myDownloadingErrors[bundlename];
            }
            return null;
        }


        public void OnLoadFinish(string assetBundleName)
        {
            if (IsManualUnloadAsset(assetBundleName))
                return;

            DecAssetBundleRef(assetBundleName);

            if (!IsWaitUnloadAsset(assetBundleName))
            {
                UnloadAssetBundle(assetBundleName);
            }
        }

        private void DebugLog(string title1 , string content1 = "", string title2 = "", string content2 = "")
        {
            if (myLogEnable)
            {
               Debug.Log("---------------- AssetBundleManager Log:" + title1 + "," +  content1 + "," + title2 + "," + content2);
            }
        }
    }
}