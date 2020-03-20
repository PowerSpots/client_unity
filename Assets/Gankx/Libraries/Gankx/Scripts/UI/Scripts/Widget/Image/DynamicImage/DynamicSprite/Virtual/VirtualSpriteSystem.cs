using System;
using UnityEngine;

namespace Gankx.UI
{
    public class VirtualSpriteSystem : ISpriteSystem
    {
        private readonly LruRefAllocator<string, VirtualSprite> myAllocator;
        private readonly Texture2D myAtlasTexture;
        private readonly Vector2 myMatchRange;

        public VirtualSpriteSystem(Texture2D atlasTexture, RectInt atlasRegion, Vector2 matchRange, Sprite[] sprites)
        {
            myMatchRange = matchRange;
            myAtlasTexture = atlasTexture;
            var capacity = atlasRegion.width * atlasRegion.height;
            myAllocator = new LruRefAllocator<string, VirtualSprite>(capacity, capacity);

            for (var i = 0; i < sprites.Length; i++)
            {
                myAllocator.Insert(new VirtualSprite(myAllocator, sprites[i]));
            }
        }

        public DynamicSprite PackSprite(string path, Sprite rawSprite)
        {
            if (null == rawSprite)
            {
                return null;
            }


            var virtualSprite = myAllocator.Allocate(path);
            if (null == virtualSprite)
            {
                return null;
            }

            var packedSprite = virtualSprite.sprite;

            if (virtualSprite.beUsed)
            {
                return virtualSprite;
            }

            try
            {
                var packedRect = packedSprite.rect;

                var colors = rawSprite.texture.GetPixels32();
                myAtlasTexture.SetPixels32((int) packedRect.x, (int) packedRect.y, (int) packedRect.width,
                    (int) packedRect.height, colors);
                DynamicSpriteService.instance.MarkTextureDirty();

#if UNITY_EDITOR
                packedSprite.name = path;
#endif
            }
            catch (Exception)
            {
                myAllocator.Deallocate(path);

                return null;
            }

            return virtualSprite;
        }

        public DynamicSprite GetSprite(string path)
        {
            return myAllocator.Get(path);
        }

        public bool IsPreferred(float width, float height)
        {
            if (!Mathf.Approximately(width, height))
            {
                return false;
            }

            return Mathf.Approximately(myMatchRange.x, width);
        }
    }
}