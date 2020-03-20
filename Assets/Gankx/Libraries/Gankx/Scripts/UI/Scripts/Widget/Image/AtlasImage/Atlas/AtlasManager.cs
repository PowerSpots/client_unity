using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gankx.UI
{
    public class AtlasManager : Singleton<AtlasManager>
    {
        private const int FreeListCount = UIConfig.AtlasFreeListCount;
        private const int CacheCheckCount = UIConfig.AtlasCacheCheckCount;
        private LruRefAllocator<string, LruRefAtlas> myAllocator;

        private readonly Dictionary<string, LruRefAtlas> myAtlasDict = new Dictionary<string, LruRefAtlas>();

        protected override void OnInit()
        {
            myAllocator = new LruRefAllocator<string, LruRefAtlas>(FreeListCount, CacheCheckCount);
            for (var i = 0; i < FreeListCount; i++)
            {
                var lruAtlas = new LruRefAtlas(myAllocator, null);
                myAllocator.Insert(lruAtlas);
            }
        }

        public string CacheFeed()
        {
            return myAllocator.CacheFeed();
        }

        public LruRefAtlas GetAtlas(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }

            var normalizedPath = assetPath.ToLower();

            LruRefAtlas lruAtlas;
            if (myAtlasDict.TryGetValue(normalizedPath, out lruAtlas))
            {
                return lruAtlas;
            }

            lruAtlas = myAllocator.Allocate(normalizedPath);
            if (null == lruAtlas)
            {
                Debug.LogError(string.Format(
                    "AtlasManager.GetAtlas occured error, Allocator cannot load atlas, assetPath: {0}. Current cache resources: {1}",
                    normalizedPath, myAllocator.CacheFeed()));
                return null;
            }

            if (lruAtlas.atlasAsset != null)
            {
                return lruAtlas;
            }

            var atlas = ResourceService.LoadAtlas(normalizedPath);
            if (null == atlas)
            {
                myAllocator.Deallocate(normalizedPath);
                Debug.LogError("AtlasManager.GetAtlas occured error, ResourceService cannot load atlas, assetPath:" +
                               normalizedPath);
                return null;
            }

            myAtlasDict[normalizedPath] = lruAtlas;

            lruAtlas.atlasAsset = atlas;

            return lruAtlas;
        }

        public void UnloadAtlas(string assetPath)
        {
            var normalizedPath = assetPath.ToLower();
            if (ResourceService.instance)
            {
                ResourceService.UnloadAtlas(normalizedPath);
            }

            if (myAtlasDict.ContainsKey(normalizedPath))
            {
                myAtlasDict.Remove(normalizedPath);
            }
        }

        public void UnloadUnusedAtlases()
        {
            var keys = myAtlasDict.Keys.ToList();
            for (var i = 0; i < keys.Count; i++)
            {
                var lruRefAtlas = myAtlasDict[keys[i]];
                if (null == lruRefAtlas || lruRefAtlas.beUsed)
                {
                    continue;
                }

                myAllocator.Deallocate(keys[i]);
            }
        }
    }
}