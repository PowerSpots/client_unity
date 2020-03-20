using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;


namespace EmojText
{
    class  InlineDefine
    {
        public static readonly int MaxAnimSpriteNum = 8;
        public static readonly float SpriteAnimTimeGap   = 0.2f;
    }

    public class HrefClickEvent : UnityEvent<string> { }

    public class SpriteAssetInfo
    {
        public string name;
        public Rect rect;
    }

    public class SpriteTagInfo
    {
        public string Key;
        private string[] Names;
        private int ValidCount;

        public int VertextIndex;
        public Vector2 Size;
        public float OffsetX;
        public float OffsetY;
        
        public void Reset()
        {
            Key = "";
        }

        public SpriteTagInfo()
        {
        }

        public void SetNames(string[] names , int validCount)
        {
            if (Names == null)
            {
                Names = new string[InlineDefine.MaxAnimSpriteNum];
            }

            ValidCount = validCount;
            for (int nameIdx = 0; nameIdx < names.Length; nameIdx++)
            {
                Names[nameIdx] = names[nameIdx];
            }
        }

        public string[] GetNames()
        {
            return Names;
        }

        public int GetValidCount()
        {
            return ValidCount;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Key);
        }
    }

    /// <summary>
    /// 超链接信息类
    /// </summary>
    public class HrefTagInfo
    {
        public int StartIndex;

        public int EndIndex;

        public string Name;

        public void Reset()
        {
            StartIndex = -1;
            EndIndex = -1;
            Boxes.Clear();
        }

        public bool IsValid()
        {
            return StartIndex != -1 && EndIndex != -1;
        }

        public List<Rect> Boxes = new List<Rect>();

        public void Print()
        {
            if (Boxes.Count > 0)
            {
                Debug.Log("HrefTagInfoItem Name:" + Name + ", Boxes:" + Boxes[0]);
            }
        }
    }

    public class UnderlineTagInfo
    {
        public int StartIndex;

        public int EndIndex;

        public List<Rect> Boxes = new List<Rect>();

        public UnderlineTagInfo()
        {
            Reset();
        }

        public void Reset()
        {
            StartIndex = -1;
            EndIndex = -1;
        }

        public bool IsValid()
        {
            return StartIndex != -1 && EndIndex != -1;
        }
    }

    public class SpriteAnimInfo
    {
        public string Key;
        public string[] Names;
        public int ValidCount;

        public Vector3[] Vertices;
        public Rect[] Uvs;

        private int mCurrnt = 0;
        public float RuningTime = 0;

        private Vector2[] mCurUvList; 

        public SpriteAnimInfo()
        {
            Key = "";
            Vertices = new Vector3[4];
            Names = null;
        }

        public void Reset()
        {
            Key = "";
        }

        /// <summary>
        /// 引用关系
        /// </summary>
        public void SetName(string[] names , int validCount)
        {
            Names = names;
            ValidCount = validCount;
        }

        public int Currnt
        {
            get
            {
                return mCurrnt;
            }
            set
            {
                mCurrnt = value;
            }
        }

        public void SetUv(int index, Rect rect)
        {
            if (Uvs == null)
            {
                Uvs = new Rect[InlineDefine.MaxAnimSpriteNum];
            }

            if (index >= Uvs.Length)
            {
                Debug.LogError("SetUv Error: index is out of range");
                return;
            }

            Uvs[index] = rect;
        }

        public int GetValidCount()
        {
            return ValidCount;
        }
        
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Key);
        }

        public Vector2[] GetUvs()
        {
            if (Uvs == null)
            {
                Uvs = new Rect[InlineDefine.MaxAnimSpriteNum];
            }

            if (Currnt >= Uvs.Length)
            {
                Debug.LogError("SpriteAnimInfo Error: index out of range");
                return  null;
            }

            if (mCurUvList == null)
            {
                mCurUvList = new Vector2[4];
            }

            Rect cur = Uvs[Currnt];

            mCurUvList[0].x = cur.x;
            mCurUvList[0].y = cur.y;

            mCurUvList[1].x = cur.x + cur.width;
            mCurUvList[1].y = cur.y + cur.height;

            mCurUvList[2].x = cur.x + cur.width;
            mCurUvList[2].y = cur.y;

            mCurUvList[3].x = cur.x;
            mCurUvList[3].y = cur.y + cur.height;

            return mCurUvList;
        }
    }
}