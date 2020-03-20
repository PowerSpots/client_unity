using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gankx.UI
{
    public class RawImageAssetTextureManager : Singleton<RawImageAssetTextureManager>
    {
        private const int FreeListCount = UIConfig.RawImageAssetTextureFreeListCount;
        private const int CacheCheckCount = UIConfig.RawImageAssetTextureCacheCheckCount;
        private LruRefAllocator<string, LruRefTexture> myAllocator;
        private readonly Dictionary<string, LruRefTexture> myTextureDict = new Dictionary<string, LruRefTexture>();

        protected override void OnInit()
        {
            myAllocator = new LruRefAllocator<string, LruRefTexture>(FreeListCount, CacheCheckCount, true);
            for (var i = 0; i < FreeListCount; i++)
            {
                var lruTexture = new LruRefTexture(myAllocator, null, null);
                myAllocator.Insert(lruTexture);
            }
        }

        public string CacheFeed()
        {
            return myAllocator.CacheFeed();
        }

        public LruRefTexture GetTexture(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }

            var normalizedPath = assetPath.ToLower();
            LruRefTexture lruTexture;

            if (myTextureDict.TryGetValue(normalizedPath, out lruTexture))
            {
                return lruTexture;
            }

            lruTexture = myAllocator.Allocate(normalizedPath);
            if (null == lruTexture)
            {
                Debug.LogError(string.Format(
                    "RawImageAssetTextureManager.GetTexture occured error, Allocator cannot allocate texture, assetPath: {0}, current cached resources: {1}",
                    normalizedPath, myAllocator.CacheFeed()));
                return null;
            }

            if (lruTexture.texture != null)
            {
                return lruTexture;
            }

            var rawTexture = ResourceService.LoadRawImageTexture(normalizedPath);
            if (null == rawTexture)
            {
                myAllocator.Deallocate(normalizedPath);
                Debug.LogError(
                    "RawImageAssetTextureManager.GetTexture occured error, ResourceService cannot load texture, assetPath:" +
                    normalizedPath);
                return null;
            }

            myTextureDict[normalizedPath] = lruTexture;

            lruTexture.texture = rawTexture;
            lruTexture.texturePath = normalizedPath;

            return lruTexture;
        }

        public void UnloadTexture(string assetPath)
        {
            var normalizedPath = assetPath.ToLower();

            if (ResourceService.instance)
            {
                ResourceService.UnloadRawImageTexture(normalizedPath);
            }

            if (myTextureDict.ContainsKey(normalizedPath))
            {
                myTextureDict.Remove(normalizedPath);
            }
        }

        public void UnloadUnusedTextures()
        {
            var keys = myTextureDict.Keys.ToList();
            for (var i = 0; i < keys.Count; i++)
            {
                var lruTexture = myTextureDict[keys[i]];
                if (null == lruTexture || lruTexture.beUsed)
                {
                    continue;
                }

                myAllocator.Deallocate(keys[i]);
            }
        }
    }
}