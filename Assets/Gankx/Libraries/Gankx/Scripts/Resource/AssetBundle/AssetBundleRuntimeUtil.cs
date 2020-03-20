using System.Collections.Generic;
using UnityEngine;

namespace Gankx
{
    public static class AssetBundleRuntimeUtil
    {
        private static readonly Dictionary<string, bool> MyBundleNameMap = new Dictionary<string, bool>();
        private static Dictionary<string, PackStrategy> MyBundleStrategyMap;
        private static List<string> MyBundlePathList;
        private static Dictionary<string, string> MyBundleNameCacheMap;

        public static void Init()
        {
            InitConfig();
            InitBundleList();
        }

        private static void InitBundleList()
        {
            var manifest = AssetBundleManager.instance.GetAssetBundleManifest();
            if (manifest == null)
            {
                return;
            }

            var newAllAssetBundles = manifest.GetAllAssetBundles();
            if (newAllAssetBundles == null || newAllAssetBundles.Length == 0)
            {
                return;
            }

            MyBundleNameMap.Clear();
            for (var i = 0; i < newAllAssetBundles.Length; i++)
            {
                if (!MyBundleNameMap.ContainsKey(newAllAssetBundles[i]))
                {
                    MyBundleNameMap.Add(newAllAssetBundles[i], true);
                }
            }
        }

        private static void InitConfig()
        {
            if (MyBundleStrategyMap != null)
            {
                return;
            }

            MyBundleStrategyMap = new Dictionary<string, PackStrategy>();
            MyBundlePathList = new List<string>();
            MyBundleNameCacheMap = new Dictionary<string, string>();

            var allConfigs = AssetBundleConfig.GetPackTargetConfigs();
            for (var i = 0; i < allConfigs.Length; ++i)
            {
                var targetConfig = allConfigs[i];
                for (var j = 0; j < targetConfig.assetPath.Length; ++j)
                {
                    var path = targetConfig.assetPath[j];
                    if (path.StartsWith("Resources/"))
                    {
                        path = path.Remove(0, "Resources/".Length);
                    }

                    var dotIndex = path.LastIndexOf(".");
                    if (dotIndex >= 0)
                    {
                        path = path.Substring(0, dotIndex);
                    }
                    else
                    {
                        path = path + "/";
                    }

                    if (!MyBundleStrategyMap.ContainsKey(path))
                    {
                        MyBundleStrategyMap.Add(path, targetConfig.packStrategy);
                        MyBundlePathList.Add(path);
                    }
                }
            }
        }

        public static string GetAssetBundleName(string resourcePath)
        {
            return GetAndUpdateBundleName(resourcePath);
        }

        public static bool IsloadUsingBundle(string resourcePath)
        {
            return !string.IsNullOrEmpty(GetAndUpdateBundleName(resourcePath));
        }

        private static string GetAndUpdateBundleName(string resourcePath)
        {
            var bundleName = string.Empty;
            if (string.IsNullOrEmpty(resourcePath))
            {
                return bundleName;
            }

            InitConfig();

            if (MyBundleNameCacheMap.ContainsKey(resourcePath))
            {
                if (!string.IsNullOrEmpty(MyBundleNameCacheMap[resourcePath]))
                {
                    return MyBundleNameCacheMap[resourcePath];
                }
            }

            var usingBundle = false;
            var rootPath = string.Empty;
            if (resourcePath.StartsWith("atlas/"))
            {
                rootPath = "atlas/";
                usingBundle = true;
            }

            if (resourcePath.StartsWith("Scenes/"))
            {
                rootPath = "Scenes/";
                usingBundle = true;
            }

            for (var i = 0; i < MyBundlePathList.Count; ++i)
            {
                if (resourcePath.StartsWith(MyBundlePathList[i]))
                {
                    usingBundle = true;
                    rootPath = MyBundlePathList[i];
                }
            }

            if (usingBundle)
            {
                bundleName = GetBunleNameIntenal(rootPath, resourcePath);
                if (!MyBundleNameCacheMap.ContainsKey(resourcePath))
                {
                    MyBundleNameCacheMap.Add(resourcePath, bundleName);
                }
            }

            return bundleName;
        }

        private static string GetBunleNameIntenal(string rootPath, string resourcePath)
        {
            if (string.IsNullOrEmpty(rootPath) || string.IsNullOrEmpty(resourcePath))
            {
                return string.Empty;
            }

            InitConfig();

            if (resourcePath.StartsWith("atlas/") || resourcePath.StartsWith("Scenes/"))
            {
                return AssetBundlePathUtil.GetBundleNameInRuntime(resourcePath);
            }

            PackStrategy strategy;
            if (MyBundleStrategyMap.TryGetValue(rootPath, out strategy))
            {
                if (strategy == PackStrategy.One)
                {
                    return AssetBundlePathUtil.GetBundleNameInRuntime(resourcePath.Substring(0,
                        resourcePath.LastIndexOf("/")));
                }

                return AssetBundlePathUtil.GetBundleNameInRuntime(resourcePath);
            }

            return string.Empty;
        }

        public static bool IsExist(string resourcePath)
        {
            if (MyBundleNameMap == null || MyBundleNameMap.Count == 0)
            {
                return false;
            }

            var bundleName = AssetBundlePathUtil.GetBundleName(resourcePath);
            return MyBundleNameMap.ContainsKey(bundleName);
        }

        public static Dictionary<string, List<string>> GetParentsDictionary(AssetBundleManifest manifest)
        {
            var dicParents = new Dictionary<string, List<string>>();
            if (manifest == null)
            {
                return dicParents;
            }

            foreach (var item in MyBundleNameMap)
            {
                var childs = manifest.GetAllDependencies(item.Key);
                if (childs.Length == 0)
                {
                    continue;
                }

                for (var i = 0; i < childs.Length; ++i)
                {
                    if (!dicParents.ContainsKey(childs[i]))
                    {
                        var parent = new List<string>();
                        parent.Add(item.Key);
                        dicParents.Add(childs[i], parent);
                    }
                    else
                    {
                        dicParents[childs[i]].Add(item.Key);
                    }
                }
            }

            return dicParents;
        }

        public static Dictionary<string, List<string>> GetRootParentsDictionary(AssetBundleManifest manifest)
        {
            var dicRootParents = new Dictionary<string, List<string>>();
            var dicParents = GetParentsDictionary(manifest);
            if (dicParents.Count <= 0)
            {
                return dicRootParents;
            }

            foreach (var item in MyBundleNameMap)
            {
                dicRootParents.Add(item.Key, GetRootParent(item.Key, dicParents));
            }

            return dicRootParents;
        }

        public static List<string> GetRootParent(string child, Dictionary<string, List<string>> dicParents)
        {
            var rootParents = new List<string>();
            if (dicParents.Count <= 0)
            {
                return rootParents;
            }

            if (!dicParents.ContainsKey(child))
            {
                return rootParents;
            }

            foreach (var parent in dicParents[child])
            {
                if (!dicParents.ContainsKey(parent) || dicParents[parent].Count == 0)
                {
                    rootParents.Add(parent);
                }
                else
                {
                    rootParents.AddRange(GetRootParent(parent, dicParents));
                }
            }

            return rootParents;
        }

        public static void InitBundleListEditor(AssetBundleManifest manifest)
        {
            if (manifest == null)
            {
                return;
            }

            var newAllAssetBundles = manifest.GetAllAssetBundles();
            if (newAllAssetBundles == null || newAllAssetBundles.Length == 0)
            {
                return;
            }

            MyBundleNameMap.Clear();
            for (var i = 0; i < newAllAssetBundles.Length; i++)
            {
                if (!MyBundleNameMap.ContainsKey(newAllAssetBundles[i]))
                {
                    MyBundleNameMap.Add(newAllAssetBundles[i], true);
                }
            }
        }
    }
}