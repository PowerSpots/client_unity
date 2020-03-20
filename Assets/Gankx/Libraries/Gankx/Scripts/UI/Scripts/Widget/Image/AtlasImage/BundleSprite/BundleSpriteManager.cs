using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gankx.UI
{
    public class BundleSpriteManager : Singleton<BundleSpriteManager>
    {
        private const int FreeListCount = UIConfig.BundleSpriteFreeListCount;
        private const int CacheCheckCount = UIConfig.BundleCacheCheckCount;
        private LruRefAllocator<string, BundleSprite> myAllocator;

        private readonly Dictionary<string, BundleSprite> myBundleSpriteDict = new Dictionary<string, BundleSprite>();

        protected override void OnInit()
        {
            myAllocator = new LruRefAllocator<string, BundleSprite>(FreeListCount, CacheCheckCount, true);
            for (var i = 0; i < FreeListCount; i++)
            {
                var bundleSprite = new BundleSprite(myAllocator);
                myAllocator.Insert(bundleSprite);
            }
        }

        public string CacheFeed()
        {
            return myAllocator.CacheFeed();
        }

        public BundleSprite GetBundleSprite(string bundleName, string spriteName)
        {
            if (string.IsNullOrEmpty(bundleName))
            {
                return null;
            }

            BundleSprite bundleSprite;
            var normalizedPath = bundleName.ToLower();

            if (myBundleSpriteDict.TryGetValue(normalizedPath, out bundleSprite))
            {
                return bundleSprite;
            }

            bundleSprite = myAllocator.Allocate(normalizedPath);
            if (null == bundleSprite)
            {
                Debug.LogError(string.Format(
                    "BundleSpriteManager.GetBundleSprite occured error, Allocator cannot allocate sprite, bundleName: {0}, {1}",
                    normalizedPath, myAllocator.CacheFeed()));
                return null;
            }

            if (bundleSprite.sprite != null)
            {
                return bundleSprite;
            }

            bundleSprite.bundleName = bundleName;

            var sprite = ResourceService.LoadBundleSprite(normalizedPath, spriteName);
            if (null == sprite)
            {
                myAllocator.Deallocate(normalizedPath);
                Debug.LogError(
                    "BundleSpriteManager.GetBundleSprite occured error, ResourceService cannot load sprite, bundleName:" +
                    normalizedPath);
                return null;
            }

            bundleSprite.sprite = sprite;
            myBundleSpriteDict[normalizedPath] = bundleSprite;

            return bundleSprite;
        }

        public void UnloadBundleSprite(string bundleName)
        {
            if (string.IsNullOrEmpty(bundleName))
            {
                Debug.LogError("BundleSpriteManager.UnloadBundleSprite occured error, bundle name is null or empty");
                return;
            }

            var normalizedPath = bundleName.ToLower();
            if (ResourceService.instance)
            {
                ResourceService.UnloadBundleSprite(normalizedPath);
            }

            if (myBundleSpriteDict.ContainsKey(normalizedPath))
            {
                myBundleSpriteDict.Remove(normalizedPath);
            }
        }

        public void UnloadUnusedBundleSprites()
        {
            var keys = myBundleSpriteDict.Keys.ToList();
            for (var i = 0; i < keys.Count; i++)
            {
                var bundleSprite = myBundleSpriteDict[keys[i]];
                if (null == bundleSprite || bundleSprite.beUsed)
                {
                    continue;
                }

                myAllocator.Deallocate(keys[i]);
            }
        }
    }
}