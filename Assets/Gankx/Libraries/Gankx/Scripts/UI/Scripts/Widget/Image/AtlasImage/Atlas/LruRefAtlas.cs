using System.Collections.Generic;
using UnityEngine;

namespace Gankx.UI
{
    public class LruRefAtlas : LruRefResource
    {
        private readonly LruRefAllocator<string, LruRefAtlas> myAllocator;
        public AtlasAsset atlasAsset;

        public LruRefAtlas(LruRefAllocator<string, LruRefAtlas> allocator, AtlasAsset atlasAsset)
        {
            myAllocator = allocator;
            this.atlasAsset = atlasAsset;
        }

        public override bool beUsed
        {
            get
            {
                if (base.beUsed)
                {
                    return true;
                }

                if (null == atlasAsset)
                {
                    return false;
                }

                // shared atlas always be used
                if (atlasAsset.atlasName == UIConfig.SharedAtlasName)
                {
                    return true;
                }

                return false;
            }
        }

        public override string ToString()
        {
            if (null == atlasAsset)
            {
                return "";
            }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || DEVELOPMENT_BUILD
            if (refCount <= 3)
            {
                var spriteNames = "";
                foreach (var pair in refSpriteDict)
                {
                    spriteNames = spriteNames + pair.Key + " | ";
                }

                return string.Format("{0}({1})", atlasAsset.atlasName, spriteNames);
            }

            return string.Format("{0}({1})", atlasAsset.atlasName, refCount);
#else
            return string.Format("{0}({1})", atlasAsset.atlasName, refCount);
#endif
        }

        public Sprite GetSprite(string spriteName)
        {
            if (null == atlasAsset)
            {
                return null;
            }

            return atlasAsset.GetSprite(spriteName);
        }

        public override void Free()
        {
            myAllocator.Insert(this);

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || DEVELOPMENT_BUILD
            refSpriteDict.Clear();
#endif

            if (null == atlasAsset)
            {
                return;
            }

            if (AtlasManager.ContainsInstance())
            {
                AtlasManager.instance.UnloadAtlas(atlasAsset.atlasName);
            }

            atlasAsset = null;
        }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || DEVELOPMENT_BUILD
        private Dictionary<string, int> refSpriteDict = new Dictionary<string, int>();

        public void AddRefSpriteName(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName))
            {
                return;
            }

            if (!refSpriteDict.ContainsKey(spriteName))
            {
                refSpriteDict[spriteName] = 1;
            }
            else
            {
                refSpriteDict[spriteName]++;
            }
        }

        public void RemoveRefSpriteName(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName))
            {
                return;
            }

            if (refSpriteDict.ContainsKey(spriteName))
            {
                refSpriteDict[spriteName]--;
                if (refSpriteDict[spriteName] <= 0)
                {
                    refSpriteDict.Remove(spriteName);
                }
            }
        }
#endif
    }
}