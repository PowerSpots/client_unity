using UnityEngine;

namespace Gankx.UI
{
    public class NativeSpriteSystem : ISpriteSystem
    {
        private readonly LruRefAllocator<string, NativeSprite> myAllocator;

        public NativeSpriteSystem(int maxCapacity, int cacheCapacity)
        {
            myAllocator = new LruRefAllocator<string, NativeSprite>(maxCapacity, cacheCapacity);

            for (var i = 0; i < maxCapacity; i++)
            {
                myAllocator.Insert(new NativeSprite(myAllocator));
            }
        }

        public DynamicSprite PackSprite(string path, Sprite rawSprite)
        {
            if (null == rawSprite)
            {
                return null;
            }

            var nativeSprite = myAllocator.Allocate(path);
            if (null == nativeSprite)
            {
                return null;
            }

            nativeSprite.SetSprite(rawSprite);

            return nativeSprite;
        }

        public DynamicSprite GetSprite(string path)
        {
            return myAllocator.Get(path);
        }

        public bool IsPreferred(float width, float height)
        {
            return true;
        }
    }
}