using UnityEngine;
using Gankx;
using  System.Collections.Generic;
using System.Linq;
using System.Text;
using Gankx.UI;
using UnityEngine.Sprites;


namespace EmojText
{
    /// <summary>
    /// 管理表情資源列表數據
    /// </summary>
    public class InlineSpriteAssetManager : Singleton<InlineSpriteAssetManager>
    {
        //已加載的Atlas
        [SerializeField] private Dictionary<string, AtlasAsset> mAtlaseMap = new Dictionary<string, AtlasAsset>();

        //Atlas中 uv信息
        [SerializeField] private Dictionary<string, Dictionary<string, SpriteAssetInfo>> mSpriteUVMap =
            new Dictionary<string, Dictionary<string, SpriteAssetInfo>>();
        
        public AtlasAsset GetAtlas(string assetPath)
        {
            if (true == mAtlaseMap.ContainsKey(assetPath))
            {
                if (mAtlaseMap[assetPath] != null && mAtlaseMap[assetPath].GetCount() > 0)
                {
                    return mAtlaseMap[assetPath];
                }
                else
                {
                    mAtlaseMap.Remove(assetPath);
                    mSpriteUVMap.Remove(assetPath);
                }
            }

            AtlasAsset atlasAsset = ResourceService.LoadAtlas(assetPath);
            if (atlasAsset == null)
            {
                Debug.LogError("Load Atlas Failed , Path:" + assetPath);
                return null;
            }

            mAtlaseMap[assetPath] = atlasAsset;


            Dictionary<string, SpriteAssetInfo> uvs = GetSpriteUVInfoMap(atlasAsset);
            mSpriteUVMap[assetPath] = uvs;

            return atlasAsset;
        }

        private Dictionary<string, SpriteAssetInfo> GetSpriteUVInfoMap(AtlasAsset atlasAsset)
        {
            if (atlasAsset == null)
            {
                return null;
            }

            Dictionary<string, SpriteAssetInfo> assetInfoMap = new Dictionary<string, SpriteAssetInfo>();
            List<Sprite> sprites = atlasAsset.GetSpriteList();
            foreach (Sprite sprite in sprites)
            {
                SpriteAssetInfo assetInfo = GetUv(sprite);
                assetInfoMap.Add(sprite.name, assetInfo);
            }

            return assetInfoMap;
        }


        /// <summary>
        /// 获得表情名字
        /// </summary>
        /// <param name="atlasAssetPath"></param>
        /// <returns></returns>
        public List<string> GetEmojNameList(string atlasAssetPath)
        {
            if (string.IsNullOrEmpty(atlasAssetPath))
            {
                return new List<string>();
            }

            if (false == mSpriteUVMap.ContainsKey(atlasAssetPath))
            {
                GetAtlas(atlasAssetPath);
            }

            if (false == mSpriteUVMap.ContainsKey(atlasAssetPath))
            {
                Debug.LogError("GetEmojNameList atlasAssetPath:" + atlasAssetPath);
                return new List<string>(); ;
            }

            Dictionary<string, string> nameDic = new Dictionary<string, string>();
            Dictionary<string, SpriteAssetInfo> spriteDic = mSpriteUVMap[atlasAssetPath];

            List<string> keys = spriteDic.Keys.ToList();

            for (int i = 0; i < keys.Count; ++i)
            {
                string fileName = keys[i];
                int index = fileName.LastIndexOf('_'); //Emoji_QQ_AiXin_00
                if (index == -1)
                {
                    Debug.Log("GetAllEmojiNames Split Fail");
                    continue; 
                }
                
                string name = fileName.Substring(0, index);
                if (!nameDic.ContainsKey(name))
                {
                    nameDic.Add(name, name);
                }
            }

            return nameDic.Keys.ToList();
        }

        public int GetSpriteNamesFromPrefix(string atlasAssetPath, string namePrefix , ref string[] names)
        {
            int count = 0;


            for (int nameIdx = 0; nameIdx < names.Length; nameIdx++)
            {
                names[nameIdx] = string.Empty;
            }

            SpriteAssetInfo[] temps = GetSpriteInfosFromPrefix(atlasAssetPath, namePrefix);
            if (temps == null)
            {
                Debug.LogError("____________________GetSpriteNamesFromPrefix ERROR: 获取图片序列失败， name:" + namePrefix);
                return count;
            }

            for (int i = 0; i < temps.Length; ++i)
            {
                if (string.IsNullOrEmpty(temps[i].name))
                {
                    continue;
                }

                names[i] = temps[i].name;
                count++;
            }

            return count;
        }

        string[] mTargetNameCache ;
        SpriteAssetInfo[] mTargetSprioteInfoCache;
        private StringBuilder mStringBuilder;  
        /// <summary>
        /// 返回数组会被别的地方修改，不建议直接使用引用
        /// </summary>
        public SpriteAssetInfo[] GetSpriteInfosFromPrefix(string atlasAssetPath, string namePrefix)
        {
            if (mTargetNameCache == null || mTargetNameCache.Length  != InlineDefine.MaxAnimSpriteNum)
            {
                mTargetNameCache = new string[InlineDefine.MaxAnimSpriteNum];
            }
            else
            {
                for (int nameIdx = 0; nameIdx < mTargetNameCache.Length; nameIdx++)
                {
                    mTargetNameCache[nameIdx] = string.Empty;
                }
            }
            
            if (mTargetSprioteInfoCache == null || mTargetSprioteInfoCache.Length != InlineDefine.MaxAnimSpriteNum)
            {
                mTargetSprioteInfoCache = new SpriteAssetInfo[InlineDefine.MaxAnimSpriteNum];
                for (int i = 0; i < InlineDefine.MaxAnimSpriteNum; i++)
                {
                    mTargetSprioteInfoCache[i] = new SpriteAssetInfo();
                }
            }
            else
            {
                for (int i = 0; i < mTargetSprioteInfoCache.Length; i++)
                {
                    mTargetSprioteInfoCache[i].name = string.Empty;
                }
            }
            
            for (int i = 0; i < InlineDefine.MaxAnimSpriteNum; ++i)
            {
                //string name = string.Format("{0}_{1:00}", namePrefix, i);  480k
                // 48K
                if (mStringBuilder == null)
                {
                    mStringBuilder = new StringBuilder();
                }
                mStringBuilder.Length = 0;
                mStringBuilder.Append(namePrefix);
                mStringBuilder.Append("_");
                mStringBuilder.Append("0");
                mStringBuilder.Append(i.ToString());
                
                mTargetNameCache[i] = mStringBuilder.ToString();
            }

            for (int i = 0; i < InlineDefine.MaxAnimSpriteNum; ++i)
            {
                SpriteAssetInfo t = GetSpriteUVInfo(atlasAssetPath, mTargetNameCache[i]);
                if (t != null)
                {
                    mTargetSprioteInfoCache[i].name = t.name;
                    mTargetSprioteInfoCache[i].rect = t.rect;
                }
            }

            return mTargetSprioteInfoCache;
        }
        
        public SpriteAssetInfo GetSpriteUVInfo(string atlasAssetPath, string spriteName)
        {
            if (false == mSpriteUVMap.ContainsKey(atlasAssetPath))
            {
                GetAtlas(atlasAssetPath);
            }

            if (false == mSpriteUVMap.ContainsKey(atlasAssetPath))
            {
                Debug.LogError("GetSpriteUVInfo atlasAssetPath:" + atlasAssetPath + ", spriteName:" + spriteName);
                return null;
            }

            Dictionary<string, SpriteAssetInfo> uvs = mSpriteUVMap[atlasAssetPath];
            if (false == uvs.ContainsKey(spriteName))
            {
                return null;
            }

            return uvs[spriteName];
        }


        public SpriteAssetInfo GetSpriteUvInfoByIndex(string atlasAssetPath, int index)
        {
            if (false == mAtlaseMap.ContainsKey(atlasAssetPath))
            {
                return null;
            }

            Sprite sprite = mAtlaseMap[atlasAssetPath].GetSprite(index);

            return GetUv(sprite);
        }

        private SpriteAssetInfo GetUv(Sprite sprite)
        {
            if (sprite == null)
            {
                return null;
            }

            SpriteAssetInfo assetInfo = new SpriteAssetInfo();
            Vector4 outer = DataUtility.GetOuterUV(sprite);
            assetInfo.name = sprite.name;
            assetInfo.rect = GetUvRect(sprite);
            return assetInfo;
        }

        private Rect GetUvRect(Sprite sprite)
        {
            Vector4 outer = DataUtility.GetOuterUV(sprite);
            Rect rect = new Rect();
            rect.x = outer.x;
            rect.y = outer.y;
            rect.width = outer.z - outer.x;
            rect.height = outer.w - outer.y;
            return rect;
        }
    }
}