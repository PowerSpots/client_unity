using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gankx
{
    internal partial class AssetBundleManager
    {
        private readonly string[] myConstStaticAssetPrefix =
        {
            "ui_v2_fonts",
            "ui_v2_bitmap_font",
            "common_shader_asset"
        };

        private readonly string[] myConstManualUnloadAssetPrefix =
        {
            "ui_",
            "atlas_",
            "ui_v2_"
        };

        private readonly string[] myConstWaitUnloadAssetPrefix =
        {
            "camera_camera_",
            "common_mat_"
        };

        private void AddToLoadedBundleList(string assetBundleName, LoadedAssetBundle bundle)
        {
            if (string.IsNullOrEmpty(assetBundleName) || bundle == null)
            {
                return;
            }

            if (myLoadedAssetBundleMap.ContainsKey(assetBundleName))
            {
                return;
            }

            myLoadedAssetBundleMap.Add(assetBundleName, bundle);

            if (logEnable)
            {
                DebugLog(">> AddToLoadedBundleList: ", assetBundleName);
            }
        }

        private void RemoveFromLoadedBundleListAndUnload(string assetBundleName, bool allowUnloadRefObjects = false)
        {
            if (string.IsNullOrEmpty(assetBundleName))
            {
                return;
            }

            if (IsStaticAsset(assetBundleName))
            {
                return;
            }

            LoadedAssetBundle bundle;
            if (myLoadedAssetBundleMap.TryGetValue(assetBundleName, out bundle))
            {
                bundle.Unload(allowUnloadRefObjects);
                myLoadedAssetBundleMap.Remove(assetBundleName);
                myDependencieMap.Remove(assetBundleName);

                if (logEnable)
                {
                    DebugLog(">> RemoveFromLoadedBundleListAndUnload: ", assetBundleName);
                }
            }
        }

        public void TryReleaseUnuseBundle()
        {
            if (myLoadedAssetBundleMap == null || myLoadedAssetBundleMap.Count == 0)
            {
                return;
            }

            var keys = myLoadedAssetBundleMap.Keys.ToList();
            for (var i = 0; i < keys.Count; i++)
            {
                if (!IsManualUnloadAsset(keys[i]))
                {
                    RemoveFromLoadedBundleListAndUnload(keys[i]);
                }
            }
        }

        public void DecAssetBundleRef(string assetBundleName, bool checkDependencies = true)
        {
            DecAssetBundleRefInternal(assetBundleName);
            if (checkDependencies)
            {
                string[] dependencies;
                if (!myDependencieMap.TryGetValue(assetBundleName, out dependencies))
                {
                    return;
                }

                for (var i = 0; i < dependencies.Length; i++)
                {
                    var dependency = dependencies[i];
                    DecAssetBundleRefInternal(dependency);
                }
            }
        }

        private void DecAssetBundleRefInternal(string assetBundleName)
        {
            LoadedAssetBundle bundle;
            if (myLoadedAssetBundleMap.TryGetValue(assetBundleName, out bundle))
            {
                bundle.DecRef();
            }
        }

        public void UnloadAssetBundle(string assetBundleName)
        {
            UnloadAssetBundleInternal(assetBundleName);

            string[] dependencies;
            if (myDependencieMap.TryGetValue(assetBundleName, out dependencies))
            {
                for (var i = 0; i < dependencies.Length; i++)
                {
                    var dependency = dependencies[i];
                    UnloadAssetBundleInternal(dependency);
                }
            }
        }

        private void UnloadAssetBundleInternal(string assetBundleName)
        {
            if (IsStaticAsset(assetBundleName))
            {
                return;
            }

            if (IsManualUnloadAsset(assetBundleName))
            {
                return;
            }

            if (IsWaitUnloadAsset(assetBundleName))
            {
                return;
            }

            LoadedAssetBundle bundle;
            myLoadedAssetBundleMap.TryGetValue(assetBundleName, out bundle);
            if (bundle == null)
            {
                return;
            }

            if (bundle.referencedCount <= 0)
            {
                AddToUnloadFalseList(assetBundleName);
            }
        }

        private bool IsManualUnloadAsset(string assetBundleName)
        {
            if (string.IsNullOrEmpty(assetBundleName))
            {
                return false;
            }

            for (var i = 0; i < myConstManualUnloadAssetPrefix.Length; i++)
            {
                if (assetBundleName.StartsWith(myConstManualUnloadAssetPrefix[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsStaticAsset(string assetBundleName)
        {
            if (string.IsNullOrEmpty(assetBundleName))
            {
                return false;
            }

            for (var i = 0; i < myConstStaticAssetPrefix.Length; i++)
            {
                if (assetBundleName.StartsWith(myConstStaticAssetPrefix[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsWaitUnloadAsset(string assetBundleName)
        {
            if (string.IsNullOrEmpty(assetBundleName))
            {
                return false;
            }

            if (assetBundleName.Contains("_wholetexture")
                || assetBundleName.Contains("_wholeanim")
                || assetBundleName.Contains("shaders_"))
            {
                return true;
            }

            for (var i = 0; i < myConstWaitUnloadAssetPrefix.Length; i++)
            {
                if (assetBundleName.StartsWith(myConstWaitUnloadAssetPrefix[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public void Printloadedab()
        {
            var etor = myLoadedAssetBundleMap.GetEnumerator();
            var c = 0;
            var sb = new StringBuilder();
            while (etor.MoveNext())
            {
                var abName = etor.Current.Key;
                abName = Path.GetFileName(abName);
                sb.AppendLine(string.Format("------{0}, ref {1}", abName, etor.Current.Value.referencedCount));

                if (etor.Current.Value.assetBundle == null)
                {
                    c++;
                }
            }

            etor.Dispose();
            Debug.LogError("=====================================================\n" + sb);
            Debug.LogError(string.Format("LoadedAssetBundles, asset bundle count {0}, null ab count {1}",
                myLoadedAssetBundleMap.Count, c));
        }


        protected override void OnRelease()
        {
            foreach (var ab in myLoadedAssetBundleMap)
            {
                ab.Value.Unload();
            }

            myLoadedAssetBundleMap.Clear();
        }
    }
}