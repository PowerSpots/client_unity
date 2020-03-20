using UnityEngine;

namespace Gankx.UI
{
    public class VirtualSprite : DynamicSprite
    {
        private readonly LruRefAllocator<string, VirtualSprite> myAllocator;

        public VirtualSprite(LruRefAllocator<string, VirtualSprite> allocator, Sprite sprite)
        {
            myAllocator = allocator;
            this.sprite = sprite;
        }

        public override bool isVirtual
        {
            get { return true; }
        }

        public override void Free()
        {
            myAllocator.Insert(this);
        }
    }
}