using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Text;

namespace EmojText
{
    /// <summary>
    /// 对输入字符进行解析，返回解析后字符
    /// </summary>
    class InlineTextParse
    {
//        private static readonly Regex mConstSpriteTagRegex =
        //new Regex(@"<quad name=(.+?) size=(\d*\.?\d+%?) width=(\d*\.?\d+%?)/>", RegexOptions.Singleline);
        //增加Y方向偏移，避免小表情位置偏移
//        new Regex(@"<quad name=(.+?) size=(\d*\.?\d+%?) width=(\d*\.?\d+%?) offset=(\d*\.?\d+%?)/>", RegexOptions.Singleline);

//        private static readonly Regex mConstSimpleSpriteTagRegex2 = new Regex(@"\[(.+?)\]", RegexOptions.Singleline);
//        private static readonly Regex mHrefRegex = new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);
        private static readonly Regex mUnderlineRegex = new Regex(@"<u>(.+?)</u>", RegexOptions.Singleline);

        //链接信息
        private readonly List<HrefTagInfo> mHrefTagInfos = new List<HrefTagInfo>();
        //下划线信息
        private readonly List<UnderlineTagInfo> mUnderlineTagInfos = new List<UnderlineTagInfo>();
        //表情信息
        private List<SpriteTagInfo> mAnimSpiteTagList;

        private static readonly StringBuilder mTextBuilder = new StringBuilder();

        private InlineSpriteRender _mInlineSpriteRender;
        private GameObject   mInlineTextGameObject;

        private InlineStringParser _mInlineStringParser;

        private string[] mEmojNameArray; 

        public InlineTextParse(GameObject textObj, InlineSpriteRender inlineSpriteRender)
        {
            if (null == inlineSpriteRender || null == textObj)
            {
                Debug.LogError("ParseText Error: InlineSprite or textObj is miss");
            }

            _mInlineSpriteRender = inlineSpriteRender;
            mInlineTextGameObject = textObj;
        }

        /// <summary>
        /// 输入解析，注意解析顺序
        /// </summary>
        /// <param name="text"></param>
        public string ParseText(string text)
        {
            UnityEngine.Profiling.Profiler.BeginSample("InlineText ParseText");
            if (_mInlineStringParser == null)
            {
                _mInlineStringParser = new InlineStringParser();
            }

            string parseOutputText = text;

            //        if (mParseOutputText.IndexOf('[') >= 0 && mParseOutputText.IndexOf(']') > 0)
            //        {
            //            mParseOutputText = ReplaceSimpleSpriteTags(mParseOutputText);
            //        }

            //提取下划线 todo 临时无需求
            //            parseOutputText = ParseUnderlineTag(parseOutputText);

            //提取超链接
            UnityEngine.Profiling.Profiler.BeginSample("InlineText Parse ParseHrefTags");
            parseOutputText = ParseHrefTags(parseOutputText);
            UnityEngine.Profiling.Profiler.EndSample();

            //解析小表情符号
            UnityEngine.Profiling.Profiler.BeginSample("InlineText Parse ParseSpriteTags");
            ParseSpriteTags(parseOutputText);
            UnityEngine.Profiling.Profiler.EndSample();

            UnityEngine.Profiling.Profiler.EndSample();

            return parseOutputText;
        }

        public List<SpriteTagInfo> GetSpriteTagInfos()
        {
            return mAnimSpiteTagList;
        }
        public List<UnderlineTagInfo> GetUnderlineTagInfos()
        {
            return mUnderlineTagInfos;
        }

        public List<HrefTagInfo> GetHrefTagInfos()
        {
            return mHrefTagInfos;
        }

        //计算在顶点中起始和结束位置，考虑<u></u>的影响，其他标签暂且不考虑
        //归根到底是计算文字在顶点数据中位置方式不太靠谱
        protected string ParseHrefTags(string strText)
        {

            mTextBuilder.Length = 0;

            if (_mInlineStringParser.IsContrainHref(strText) == false)
            {
                for (int i = 0; i < mHrefTagInfos.Count; ++i)
                {
                    mHrefTagInfos[i].Reset();
                }
                return strText;
            }

            var indexText = 0;
            int index = 0;
            UnityEngine.Profiling.Profiler.BeginSample("ParseHrefTags 11");

            HrefParam[] mHrefParams = _mInlineStringParser.ParseHref(strText);
            for (int i = 0; i < mHrefParams.Length; ++i)
            {
                if (false == mHrefParams[i].IsValid())
                {
                    continue;
                }

                HrefParam param = mHrefParams[i];
                mHrefParams[i].Print();

                mTextBuilder.Append(strText.Substring(indexText, param.StartIndex - indexText ));

                if (index + 1 > mHrefTagInfos.Count)
                {
                    var temp = new HrefTagInfo();
                    mHrefTagInfos.Add(temp);
                }

                HrefTagInfo hrefInfo = mHrefTagInfos[index];
                
                hrefInfo.StartIndex = mTextBuilder.Length;
                hrefInfo.EndIndex = mTextBuilder.Length + param.Content.Length;
                hrefInfo.Name = param.Tag;
                
                mTextBuilder.Append(param.Content);
                indexText = param.EndIndex + 1;
                index++;
            }

            mTextBuilder.Append(strText.Substring(indexText, strText.Length - indexText));

            if (index < mHrefTagInfos.Count)
            {
                int count = mHrefTagInfos.Count;
                for (int i = index; i < count; ++i)
                {
                    mHrefTagInfos[i].Reset();
                }
            }

            UnityEngine.Profiling.Profiler.EndSample();

            return mTextBuilder.ToString();
        }

        protected string ParseUnderlineTag(string strText)
        {
            UnityEngine.Profiling.Profiler.BeginSample("InlineText Parse ParseUnderlineTag");

            mTextBuilder.Length = 0;

            if (string.IsNullOrEmpty(strText) || strText.IndexOf("<u>") == -1 || strText.IndexOf("</u>") == -1)
            {
                for (int i = 0; i < mUnderlineTagInfos.Count; ++i)
                {
                    mUnderlineTagInfos[i].Reset();
                }
                return strText;
            }

            var indexText = 0;
            int index = 0;
            foreach (Match match in mUnderlineRegex.Matches(strText))
            {
                mTextBuilder.Append(strText.Substring(indexText, match.Index - indexText));

                if (index + 1 > mUnderlineTagInfos.Count)
                {
                    //TODO 尽量走缓存
                    var temp = new UnderlineTagInfo();
                    mUnderlineTagInfos.Add(temp);
                }

                //文本起始顶点索引 TODO 位置计算容易出问题，万一中间带有其他解释符号。所以下滑先只能放在次里面（仅次于超链接）
                mUnderlineTagInfos[index].StartIndex = mTextBuilder.Length;
                mUnderlineTagInfos[index].EndIndex = mTextBuilder.Length + match.Groups[1].Length;

                mTextBuilder.Append(match.Groups[1].Value);
                indexText = match.Index + match.Length;

                index++;
            }

            if (index < mUnderlineTagInfos.Count)
            {
                int count = mUnderlineTagInfos.Count;
                for (int i = index; i < count; ++i)
                {
                    mUnderlineTagInfos[i].Reset();
                }
            }

            mTextBuilder.Append(strText.Substring(indexText, strText.Length - indexText));

            UnityEngine.Profiling.Profiler.EndSample();

            return mTextBuilder.ToString();
        }

//        private string ReplaceSimpleSpriteTags(string strText)
//        {
//            UnityEngine.Profiling.Profiler.BeginSample("InlineText Parse ReplaceSimpleSpriteTags");
//
//            mTextBuilder.Length = 0;
//            var indexText = 0;
//            foreach (Match match in mConstSimpleSpriteTagRegex2.Matches(strText))
//            {
//                mTextBuilder.Append(strText.Substring(indexText, match.Index - indexText));
//                string strSprite = "<quad name=" + match.Groups[1].ToString().Trim() + " size=" + fontSize + " width=1.2/>";
//                mTextBuilder.Append(strSprite);
//                indexText = match.Index + match.Length;
//            }
//            mTextBuilder.Append(strText.Substring(indexText, strText.Length - indexText));
//            UnityEngine.Profiling.Profiler.EndSample();
//            return mTextBuilder.ToString();
//        }
        
        private void ParseSpriteTags(string strText)
        {
            if (_mInlineSpriteRender == null)
            {
                return;
            }

            if (mAnimSpiteTagList == null)
            {
                mAnimSpiteTagList = new List<SpriteTagInfo>();
            }

            if (_mInlineStringParser.IsContrainEmoj(strText) == false)
            {
                for (int i = 0; i < mAnimSpiteTagList.Count; ++i)
                {
                    mAnimSpiteTagList[i].Reset();
                }
                return;
            }

            int index = 0;

            UnityEngine.Profiling.Profiler.BeginSample("InlineText Parse ParserEmoj");

            EmojParam[] emojParams = _mInlineStringParser.ParserEmoj(strText , "<quad " , "/>");

            UnityEngine.Profiling.Profiler.EndSample();

            UnityEngine.Profiling.Profiler.BeginSample("InlineText Parse Tag");

            for (int i = 0; i < emojParams.Length; ++ i)
            {
                EmojParam emojParam = emojParams[i];
                if (emojParam.IsValid() == false)
                {
                    continue;
                }

                if(mEmojNameArray == null)
                {
                    mEmojNameArray = new string[InlineDefine.MaxAnimSpriteNum];
                }
                else
                {
                    for (int nameIdx = 0; nameIdx < mEmojNameArray.Length; nameIdx++)
                    {
                        mEmojNameArray[nameIdx] = string.Empty;
                    }
                }

                int nameCount = _mInlineSpriteRender.GetSpriteNamesFromPrefix(emojParam.Name , ref mEmojNameArray);
                if (nameCount == 0 )
                {
                    continue;
                }

                if (index + 1 > mAnimSpiteTagList.Count)
                {
                    SpriteTagInfo tempNew = new SpriteTagInfo();
                    mAnimSpiteTagList.Add(tempNew);
                }

                SpriteTagInfo tempArrayTag = mAnimSpiteTagList[index];
                tempArrayTag.Key = GenerateKey(emojParam.Name, index);
                tempArrayTag.SetNames(mEmojNameArray, nameCount);
                tempArrayTag.VertextIndex = emojParam.VertextIndex;
                
                float offset = 0.0f;
                if (emojParam.Width > 1.0f)
                {
                    offset = (emojParam.Width - 1.0f) / 2.0f;
                }
                tempArrayTag.Size = new Vector2(emojParam.Size, emojParam.Size);
                tempArrayTag.OffsetX = offset;
                tempArrayTag.OffsetY = emojParam.Offset;

                index++;
            }
            UnityEngine.Profiling.Profiler.EndSample();
            
            if (index < mAnimSpiteTagList.Count)
            {
                int count = mAnimSpiteTagList.Count;
                for (int i = index; i < count; ++i)
                {
                    mAnimSpiteTagList[i].Reset();
                }
            }
        }

        private string GenerateKey(string name, int pos)
        {
            if (null == mInlineTextGameObject)
            {
                Debug.LogError("GenerateKey Error: GameObject is miss");
                return "";
            }

            return name + "_" + mInlineTextGameObject.gameObject.GetInstanceID() + "_" + pos.ToString();
        }
    }

}
