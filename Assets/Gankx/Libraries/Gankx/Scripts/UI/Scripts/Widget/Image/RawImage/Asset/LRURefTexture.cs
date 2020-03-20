using UnityEngine;

namespace Gankx.UI
{
    public class LruRefTexture : LruRefResource
    {
        private readonly LruRefAllocator<string, LruRefTexture> myAllocator;
        public Texture2D texture;
        public string texturePath;

        public LruRefTexture(LruRefAllocator<string, LruRefTexture> allocator, Texture2D texture, string texturePath)
        {
            myAllocator = allocator;
            this.texture = texture;
            this.texturePath = texturePath;
        }

        public override string ToString()
        {
            if (null == texturePath)
            {
                return "";
            }

            return string.Format("{0}({1})", texturePath, refCount);
        }

        public override void Free()
        {
            myAllocator.Insert(this);
            if (null == texture)
            {
                return;
            }

            if (RawImageAssetTextureManager.ContainsInstance())
            {
                RawImageAssetTextureManager.instance.UnloadTexture(texturePath);
            }

            texture = null;
            texturePath = null;
        }
    }
}