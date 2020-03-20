using UnityEngine;

namespace Gankx.UI
{
    public class BundleSprite : LruRefResource
    {
        private readonly LruRefAllocator<string, BundleSprite> myAllocator;
        public Sprite sprite;
        public string bundleName;

        public BundleSprite(LruRefAllocator<string, BundleSprite> allocator)
        {
            myAllocator = allocator;
            sprite = null;
            bundleName = null;
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", bundleName, refCount);
        }

        public override void Free()
        {
            myAllocator.Insert(this);
            BundleSpriteManager.instance.UnloadBundleSprite(bundleName);

            sprite = null;
        }
    }
}