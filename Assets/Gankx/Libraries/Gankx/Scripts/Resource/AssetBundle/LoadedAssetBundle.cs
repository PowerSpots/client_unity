using System;
using UnityEngine;

namespace Gankx
{
    public class LoadedAssetBundle : IComparable<LoadedAssetBundle>
    {
        public string bundleName;
        public AssetBundle assetBundle;
        public int referencedCount;
        public long loadTimeStamp;

        public LoadedAssetBundle(string bundleName, AssetBundle assetBundle = null)
        {
            this.bundleName = bundleName;
            this.assetBundle = assetBundle;
            IncRef();
        }

        public int CompareTo(LoadedAssetBundle other)
        {
            if (referencedCount != other.referencedCount)
            {
                return other.referencedCount.CompareTo(referencedCount);
            }

            return other.loadTimeStamp.CompareTo(loadTimeStamp);
        }

        public int IncRef()
        {
            ++referencedCount;
            loadTimeStamp = GetNowTimeStamp();
            return referencedCount;
        }

        public int DecRef()
        {
            if (referencedCount > 0)
            {
                --referencedCount;
            }

            return referencedCount;
        }

        public void SetBundle(AssetBundle bundle)
        {
            assetBundle = bundle;
        }

        private long GetNowTimeStamp()
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }

        internal void Unload(bool unloadAllLoadedObjects = false)
        {
            if (assetBundle != null)
            {
                assetBundle.Unload(unloadAllLoadedObjects);
            }
        }
    }
}