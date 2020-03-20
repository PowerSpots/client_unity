using UnityEngine;

namespace Gankx.UI
{
    public interface ISpriteSystem
    {
        DynamicSprite PackSprite(string path, Sprite rawSprite);

        DynamicSprite GetSprite(string path);

        bool IsPreferred(float width, float height);
    }
}