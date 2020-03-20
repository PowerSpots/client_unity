using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

///已知问题：
/// 1.下划线解析和超链接解析都是基于字符位置对应实际字符顶点位置，同时存在时位置计算会有偏差
/// 2.字符串使用正则表达式，会有少量GC (1)减少不必要的表情顶点数据更新; 
/// 3.位置更新流程

namespace EmojText
{
    [ExecuteInEditMode]
    [RequireComponent(typeof (InlineSpriteRender))]
    public class InlineSpriteRenderManager : MonoBehaviour
    {
        /// <summary>
        /// 所有动画数据，使用前检查Key是否有效
        /// </summary>
        private Dictionary<string, SpriteAnimInfo> mTotalSpriteAnimDic = new Dictionary<string, SpriteAnimInfo>();

        /// <summary>
        /// Text对应的表情动画Key值
        /// </summary>
        private Dictionary<int, List<string>> mTextSpriteAnimKeysDic = new Dictionary<int, List<string>>();

        /// <summary>
        /// 当前激活中的Text
        /// </summary>
        private Dictionary<int, InlineText> mActicveTextDic = new Dictionary<int, InlineText>();

        private InlineSpriteRender mInlineSprite;

        private bool mRenderDirty = false;

        public void Register(InlineText inlineText)
        {
            if (null == inlineText)
            {
                return;
            }

            int id = inlineText.GetInstanceID();
            if (mActicveTextDic.ContainsKey(id))
            {
                return;
            }

//             Debug.Log("___________________________Register Name:" + inlineText.name);
            mActicveTextDic[id] = inlineText;
        }

        public void UnRegister(InlineText inlineText)
        {
            if (null == inlineText)
            {
                return;
            }

            RemoveSpriteAnimInfos(inlineText);

            int id = inlineText.GetInstanceID();
            if (mActicveTextDic.ContainsKey(id))
            {
                mActicveTextDic.Remove(id);
            }
//             Debug.Log("___________________________UnRegister Name:" + inlineText.name);
        }

        private void OnEnable()
        {
            mTotalSpriteAnimDic.Clear();
            mTextSpriteAnimKeysDic.Clear();
        }

        private void OnDisable()
        {
            mTextSpriteAnimKeysDic.Clear();
            mActicveTextDic.Clear();
            mTotalSpriteAnimDic.Clear();
        }

        private void OnDestroy()
        {
            mVertexHelper.Clear();
        }

        public void RemoveSpriteAnimInfos(InlineText inlineText)
        {
            if (inlineText == null)
            {
                return;
            }

            int id = inlineText.GetInstanceID();
            if (!mTextSpriteAnimKeysDic.ContainsKey(id))
            {
                return;
            }

            //delete 
            int count = mTotalSpriteAnimDic.Count;
            List<string> spriteAnimKeys = mTextSpriteAnimKeysDic[id];
            for (int i = 0; i < spriteAnimKeys.Count; ++i)
            {
                if (mTotalSpriteAnimDic.ContainsKey(spriteAnimKeys[i]))
                {
                    mTotalSpriteAnimDic.Remove(spriteAnimKeys[i]);
                }
            }
            mTextSpriteAnimKeysDic.Remove(id);
        }

        public void UpdateSpriteAnimInfos(InlineText inlineText, List<SpriteAnimInfo> inputSpriteAnimInfos)
        {
//            UnityEngine.Profiling.Profiler.BeginSample("inlineSpriteManager UpdateSpriteAnimInfos ");

            if (inlineText == null)
            {
                return;
            }

            bool isUpdateMeshData = false;

            //emoj name list in text
            int id = inlineText.GetInstanceID();
            List<string> oldSpriteKeys = null;

            if (mTextSpriteAnimKeysDic.ContainsKey(id))
            {
                oldSpriteKeys = mTextSpriteAnimKeysDic[id];
            }

            //input is null
            if (inputSpriteAnimInfos == null)
            {
                // delete old emoj 
                if (oldSpriteKeys != null)
                {
                    for (int i = 0; i < oldSpriteKeys.Count; ++i)
                    {
                        mTotalSpriteAnimDic.Remove(oldSpriteKeys[i]);
                    }
                    mTextSpriteAnimKeysDic.Remove(id);
                    isUpdateMeshData = true;
                }
            }
            else
            {
                int oldCount = mTotalSpriteAnimDic.Count;

                // delete old emoj 
                if (oldSpriteKeys != null)
                {
                    for (int i = 0; i < oldSpriteKeys.Count; ++i)
                    {
                        mTotalSpriteAnimDic.Remove(oldSpriteKeys[i]);
                    }
                }

                // add new emoj name list and emoj sprite assets
                List<string> keys = new List<string>();
                for (int i = 0; i < inputSpriteAnimInfos.Count; ++i)
                {
                    SpriteAnimInfo temp = inputSpriteAnimInfos[i];
                    if (temp != null && temp.IsValid())
                    {
                        mTotalSpriteAnimDic[temp.Key] = temp;
                        keys.Add(temp.Key);
                    }
                }

                // key handler
                if (keys.Count > 0)
                {
                    mTextSpriteAnimKeysDic[id] = keys;
                }
                else
                {
                    if (oldSpriteKeys != null)
                    {
                        mTextSpriteAnimKeysDic.Remove(id);
                    }
                }

                if (oldCount != mTotalSpriteAnimDic.Count)
                {
                    isUpdateMeshData = true;
                }
            }

            // update mesh
            if (isUpdateMeshData)
            {
                //Debug.LogWarning("mInlineSpriteAnimInfoDic Count:" + mSpriteAnimInfoDic.Count);
//                UpdateMeshCapacity();
            }

//            UnityEngine.Profiling.Profiler.EndSample();
        }


        /// <summary>
        /// 只更新位置信息
        /// </summary>
        /// <param name="inlineText"></param>
        /// <param name="inputSpriteAnimInfos"></param>
        public void UpdatePositon(InlineText inlineText, List<SpriteAnimInfo> inputSpriteAnimInfos)
        {
            UpdateSpriteAnimInfos(inlineText, inputSpriteAnimInfos);
            mRenderDirty = true;
        }

        private void LateUpdate()
        {
            if (mTotalSpriteAnimDic == null)
            {
                return;
            }

            bool isNeedUpdate = false;
            foreach(KeyValuePair<string,SpriteAnimInfo> spritanim in mTotalSpriteAnimDic)
            {
                SpriteAnimInfo temp = spritanim.Value;
                if (!temp.IsValid())
                {
                    continue;
                }

                temp.RuningTime += Time.deltaTime;
                if (temp.RuningTime >= InlineDefine.SpriteAnimTimeGap)
                {
                    temp.RuningTime = 0;
                    temp.Currnt++;

                    if (temp.Currnt >= temp.GetValidCount())
                    {
                        temp.Currnt = 0;
                    }
                }

                isNeedUpdate = true;
            }
            
            if (isNeedUpdate || mTotalSpriteAnimDic.Count == 0)
            {
                mRenderDirty = true;
            }

            //if (mRenderDirty)
            {
                DrawSprite();
                mRenderDirty = false;
            }
        }

        private VertexHelper mVertexHelper = new VertexHelper();

        public void DrawSprite()
        {
            
            mVertexHelper.Clear();

            int index = 0;
            foreach (KeyValuePair<string, SpriteAnimInfo> spritanim in mTotalSpriteAnimDic)
            {
                SpriteAnimInfo spriteAnimInfo = spritanim.Value;
                if (spriteAnimInfo == null || spriteAnimInfo.Vertices == null || !spriteAnimInfo.IsValid())
                {
                    continue;
                }

                Vector2[] uvs = spriteAnimInfo.GetUvs();
                Vector3[] vertexs = spriteAnimInfo.Vertices;
                mVertexHelper.AddVert(vertexs[0], Color.white, uvs[0]);
                mVertexHelper.AddVert(vertexs[1], Color.white, uvs[1]);
                mVertexHelper.AddVert(vertexs[2], Color.white, uvs[2]);
                mVertexHelper.AddVert(vertexs[3], Color.white, uvs[3]);

                mVertexHelper.AddTriangle(index * 4 + 0, index * 4 + 1, index * 4 + 2);
                mVertexHelper.AddTriangle(index * 4 + 1, index * 4 + 0, index * 4 + 3);
                index ++;
            }


            UpdateMesh();

        }

        private void UpdateMesh()
        {

            if (mInlineSprite == null)
            {
                mInlineSprite = GetComponent<InlineSpriteRender>();
                if (mInlineSprite.enabled == false)
                {
                    mInlineSprite.enabled = true;
                }
            }

            if (mInlineSprite != null)
            {
                mInlineSprite.UpdateMesh(mVertexHelper);
            }

        }
    }
}