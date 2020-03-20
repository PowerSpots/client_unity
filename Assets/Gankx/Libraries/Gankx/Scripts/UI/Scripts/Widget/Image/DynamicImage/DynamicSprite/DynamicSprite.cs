using UnityEngine;

namespace Gankx.UI
{
    public abstract class DynamicSprite : LruRefResource
    {
        public abstract bool isVirtual { get; }
        public Sprite sprite { get; protected set; }
    }
}