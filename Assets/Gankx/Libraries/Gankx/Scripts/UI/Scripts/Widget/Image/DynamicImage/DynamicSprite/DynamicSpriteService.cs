using UnityEngine;

namespace Gankx.UI
{
    public class DynamicSpriteService : Singleton<DynamicSpriteService>
    {
        public struct SpriteBlock
        {
            public SpriteBlock(RectInt rect, Vector2 matchRange, string holderPath)
            {
                this.rect = rect;
                this.holderPath = holderPath;
                this.matchRange = matchRange;
            }

            public RectInt rect;
            public string holderPath;
            public Vector2 matchRange;
        }

        public const int TextureWidth = UIConfig.DynamicSpriteTextureWidth;
        public const int TextureHeight = UIConfig.DynamicSpriteTextureHeight;
        public const int TexturePadding = UIConfig.DynamicSpriteTexturePadding;
        public const string TexturePath = UIConfig.DynamicSpriteTexturePath;

        public static readonly SpriteBlock[] SpriteBlocks = UIConfig.DynamicSpriteBlocks;

        public Texture2D atlasTexture;

        private bool myTextureDirty;
        private DynamicSpriteSystem myDynamicSpriteSystem;

        protected override void OnInit()
        {
            Init();
        }

        private void Init()
        {
            atlasTexture = Resources.Load<Texture2D>(TexturePath);
            atlasTexture.wrapMode = TextureWrapMode.Clamp;
            atlasTexture.filterMode = FilterMode.Bilinear;

            myDynamicSpriteSystem = new DynamicSpriteSystem(SpriteBlocks.Length);
            for (var i = 0; i < SpriteBlocks.Length; i++)
            {
                var spriteHolder = Resources.Load<VirtualSpriteHolder>(SpriteBlocks[i].holderPath);
                var system = new VirtualSpriteSystem(atlasTexture, SpriteBlocks[i].rect, SpriteBlocks[i].matchRange,
                    spriteHolder.sprites);
                myDynamicSpriteSystem.Mount(i, system);
            }
        }

        public DynamicSprite PackSprite(string path, Sprite rawSprite)
        {
            if (!Application.isPlaying)
            {
                return null;
            }

            if (null == rawSprite)
            {
                return null;
            }

            return myDynamicSpriteSystem.PackSprite(path, rawSprite);
        }

        public DynamicSprite GetSprite(string path)
        {
            if (!Application.isPlaying)
            {
                return null;
            }

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return myDynamicSpriteSystem.GetSprite(path);
        }

        public void MarkTextureDirty()
        {
            myTextureDirty = true;
        }

        private void LateUpdate()
        {
            if (myTextureDirty)
            {
                myTextureDirty = false;
                if (atlasTexture != null)
                {
                    atlasTexture.Apply();
                }
            }
        }
    }
}