using UnityEngine;

namespace Gankx.UI
{
    public class DynamicSpriteSystem
    {
        private readonly ISpriteSystem[] myAtlases;

        public DynamicSpriteSystem(int slotCount)
        {
            myAtlases = new ISpriteSystem[slotCount];
        }

        public void Mount(int slotIndex, ISpriteSystem system)
        {
            myAtlases[slotIndex] = system;
        }

        public DynamicSprite PackSprite(string path, Sprite rawSprite)
        {
            var spriteRect = rawSprite.rect;

            var preferredWidth = spriteRect.width;
            var preferredHeight = spriteRect.height;

            var atlasCount = myAtlases.Length;
            for (var i = 0; i < atlasCount; ++i)
            {
                var system = myAtlases[i];
                if (system.IsPreferred(preferredWidth, preferredHeight))
                {
                    var virtualSprite = system.PackSprite(path, rawSprite);
                    if (virtualSprite != null)
                    {
                        return virtualSprite;
                    }
                }
            }

            return null;
        }

        public DynamicSprite GetSprite(string path)
        {
            var atlasCount = myAtlases.Length;
            for (var i = 0; i < atlasCount; ++i)
            {
                var sprite = myAtlases[i].GetSprite(path);
                if (sprite != null)
                {
                    return sprite;
                }
            }

            return null;
        }
    }
}