using UnityEngine;

namespace Gankx.UI
{
    public class NativeSprite : DynamicSprite
    {
        private readonly LruRefAllocator<string, NativeSprite> myAllocator;

        public NativeSprite(LruRefAllocator<string, NativeSprite> allocator)
        {
            myAllocator = allocator;
            sprite = null;
        }

        public override bool isVirtual
        {
            get { return false; }
        }

        public override void Free()
        {
            sprite = null;

            myAllocator.Insert(this);
        }

        public void SetSprite(Sprite value)
        {
            sprite = value;
        }
    }
}