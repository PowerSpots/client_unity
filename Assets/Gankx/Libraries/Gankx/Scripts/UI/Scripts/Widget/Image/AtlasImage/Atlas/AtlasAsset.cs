using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gankx.UI
{
    public class AtlasAsset : MonoBehaviour
    {        
        public string atlasName;

        [SerializeField]
        [FormerlySerializedAs("sprites")]
        private List<Sprite> mySprites = new List<Sprite>();

        private readonly Dictionary<string,Sprite> mySpritesDic = new Dictionary<string, Sprite>();               

        public void Init()
        {
            mySpritesDic.Clear();
            for (int i = 0; i < mySprites.Count; i++)
            {
                if (mySprites[i] == null)
                {
                    continue;
                }

                mySpritesDic[mySprites[i].name] = mySprites[i];
            }
        }

        public void AddSprite(Sprite sprite)
        {
            if (null == sprite)
            {
                return;
            }

            mySprites.Add(sprite);
        }

        public void RemoveAll()
        {
            mySprites.Clear();
        }

        public Sprite GetSprite(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName))
            {
                Debug.LogError("AddSprite Error. Sprite name invalid: " + spriteName);
                return null;
            }

            Sprite sprite;
            if (!mySpritesDic.TryGetValue(spriteName, out sprite))
            {
                Debug.LogError(string.Format("UIAtlas.GetSprite occured error. Cannot find sprite({0}) on atlas({1})",
                    spriteName, atlasName));
                return null;
            }

            return sprite;
        }

        public bool ContainsSprite(Sprite sprite)
        {
            for (int i = 0; i < mySprites.Count; i++)
            {
                if (mySprites[i] == sprite)
                {
                    return true;
                }
            }

            return false;
        }

        public Sprite GetSprite(int index)
        {
            if (index < 0 || index >= mySprites.Count)
            {
                return null;
            }
            return mySprites[index];
        }

        public List<Sprite> GetSpriteList()
        {
            return mySprites;
        }

        public int GetCount()
        {
            return mySprites.Count;
        }
    }

}