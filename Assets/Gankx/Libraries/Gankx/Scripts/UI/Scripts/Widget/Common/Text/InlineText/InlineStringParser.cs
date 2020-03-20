using System;
using System.Collections;
using System.Collections.Generic;
using EmojText;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Profiling;

public class HrefParam
{
    public string Content;
    public string Tag;
    public int StartIndex;
    public int EndIndex;

    public HrefParam()
    {
        Reset();
    }

    public bool IsValid()
    {
        return Content != string.Empty && Tag != string.Empty;
    }

    public void Reset()
    {
        Content = string.Empty;
        Tag = string.Empty;
        StartIndex = -1;
        EndIndex = -1;
    }

    public void Print()
    {
//        Debug.Log("HrefParam_______________________________ Content:" + Content);
//        Debug.Log("HrefParam___ Tag:" + Tag);
    }
}

public class EmojParam
{
    public string Name;
    public float Size;
    public float Offset;
    public float Width;
    public int VertextIndex; 

    public EmojParam()
    {
        Reset();
    }

    public bool IsValid()
    {
        return Name != string.Empty && Width != 0; 
    }

    public void Reset()
    {
        Name = string.Empty;
        Offset = 0;
        Width = 0;
        Size = 0;
        VertextIndex = 0;
    }

    public void Print()
    {
//        Debug.Log("EmojParam_______________________________ VertextIndex:" + VertextIndex);
//
//        Debug.Log("EmojParam Name:" + Name);
//        Debug.Log("EmojParam Offset:" + Offset);
//        Debug.Log("EmojParam Width:" + Width);
//        Debug.Log("EmojParam Size:" + Size);
    }
}

///解析字符串
public class InlineStringParser
{
    //最大缓存数量
	private readonly int ConstMaxCacheCount = 16 ;
    
	private readonly int ConstMaxIntCount = 8 ;
	private readonly int ConstMaxDecCount = 2 ;

	private int[] mPosIndexsStartCache ;
    private int[] mPosIndexsEndCache ;
	private EmojParam[] mEmojParams;
	private HrefParam[] mHrefParams;
    private char[] mParserValueCache;
    private char[] mParserValueCacheDec;


    public InlineStringParser()
	{
//		 mPosIndexsStartCache = new int[ConstMaxCacheCount];
//         mPosIndexsEndCache = new int[ConstMaxCacheCount];
//            
//         mEmojParams = new EmojParam[ConstMaxCacheCount];
//         mHrefParams = new HrefParam[ConstMaxCacheCount];
//
//         mParserValueCache = new char[ConstMaxIntCount];
//         mParserValueCacheDec = new char[ConstMaxDecCount];

        ResetCache(ConstMaxCacheCount);
	}

    public void ResetCache(int maxCount)
    {
        maxCount = Math.Max(ConstMaxCacheCount, maxCount);

        mPosIndexsStartCache = new int[maxCount];
        mPosIndexsEndCache = new int[maxCount];

        mEmojParams = new EmojParam[maxCount];
        mHrefParams = new HrefParam[maxCount];

        mParserValueCache = new char[maxCount];
        mParserValueCacheDec = new char[ConstMaxDecCount];
    }


    public bool IsContrainHref(string strText)
    {
        if (string.IsNullOrEmpty(strText))
        {
            return false;
        }

        return strText.IndexOf("<a href=") != -1 && strText.IndexOf("</a>") != -1;
    }

    public bool IsContrainEmoj(string strText)
    {
        if (string.IsNullOrEmpty(strText))
        {
            return false;
        }

        return strText.IndexOf("quad") != -1 && strText.IndexOf("name") != -1;
    }

    public HrefParam[] ParseHref(string str)
    {
        ResetIndexCache();
        ResetHrefCache();

        string startStr = "<a href=";
        string endStr = "</a>";
        string middleStr = ">";

        if (string.IsNullOrEmpty(str))
        {
            return null;
        }

        Split(str, startStr, endStr, ref mPosIndexsStartCache, ref mPosIndexsEndCache);

        for (int i = 0; i < mPosIndexsStartCache.Length; i++)
        {
            if (mPosIndexsStartCache[i] != -1 && mPosIndexsEndCache[i] != -1)
            {
                int startIndex = mPosIndexsStartCache[i] + startStr.Length;
                int endIndex   = mPosIndexsEndCache[i] - 1;

                int middleIndex = str.IndexOf(middleStr , startIndex , endIndex - startIndex + 1);
                if (middleIndex == -1)
                {
                    Debug.LogError("ParseHref Error: href lackl <");
                    continue;
                }

                mHrefParams[i].Content = str.Substring(middleIndex + 1, endIndex - middleIndex);
                mHrefParams[i].Tag     = str.Substring(startIndex , middleIndex - startIndex);
                mHrefParams[i].StartIndex = mPosIndexsStartCache[i];
                mHrefParams[i].EndIndex = mPosIndexsEndCache[i] + endStr.Length -1;
            }
        }

        return mHrefParams;
    }

    public EmojParam[] ParserEmoj(string str , string startStr , string endStr)
	{
        //1 重置参数
		ResetIndexCache();
		ResetEmojParamCache();

		if(string.IsNullOrEmpty(str))
		{
			return null;
		}

		Split(str , startStr, endStr, ref mPosIndexsStartCache , ref mPosIndexsEndCache);

		for(int i = 0 ; i < mPosIndexsStartCache.Length ; i ++)
		{
		    if (mPosIndexsStartCache[i] != -1 && mPosIndexsEndCache[i] != -1)
		    {
                int startIndex = mPosIndexsStartCache[i] + startStr.Length;
                int endIndex = mPosIndexsEndCache[i] -1;
                
                ParseEmojInfo(str, startIndex , endIndex, ref mEmojParams[i]);

                if (mEmojParams[i].IsValid() == true)
		        {
		            mEmojParams[i].VertextIndex = mPosIndexsStartCache[i];
		        }
		    }
		}

        return mEmojParams;
	}

    private void ParseEmojInfo(string subStr , int startIndex, int endIndex,  ref EmojParam emojParam)
	{
        int dstStartIndex = -1;
        int dstLen = -1;

        ParseEmojParam(subStr, startIndex, endIndex, "name=", " ", out dstStartIndex, out dstLen);
        if (dstStartIndex == -1 || dstLen == -1)
        {
            emojParam.Reset();
            return;
        }
        else
        {
            emojParam.Name = subStr.Substring(dstStartIndex, dstLen);
//            Debug.Log("emojParam.Name:" + emojParam.Name);
        }

        ParseEmojParam(subStr , startIndex, endIndex, "size=" , " " , out dstStartIndex , out dstLen);
        if (dstStartIndex == -1 || dstLen == -1)
        {
            emojParam.Reset();
            return;
        }
        else
        {
            emojParam.Size = ParseFloat(subStr , dstStartIndex, dstLen);
//            Debug.Log("emojParam.Size:" + emojParam.Size);
        }

        ParseEmojParam(subStr, startIndex, endIndex, "width=", " ", out dstStartIndex, out dstLen);
        if (dstStartIndex == -1 || dstLen == -1)
        {
            emojParam.Reset();
            return;
        }
        else
        {
            emojParam.Width = ParseFloat(subStr, dstStartIndex, dstLen);
//            Debug.Log("emojParam.Width:" + emojParam.Width);
        }
        
        ParseEmojParam(subStr, startIndex, endIndex, "offset=", " ", out dstStartIndex, out dstLen);
        if (dstStartIndex == -1 || dstLen == -1)
        {
            emojParam.Reset();
            return;
        }
        else
        {
            emojParam.Offset = ParseFloat(subStr, dstStartIndex, dstLen);
//            Debug.Log("emojParam.offset:" + emojParam.Offset);
        }
    }

    /// <summary>
    /// 提取参数，通过前后标记进行区分
    /// </summary>
    /// <param name="str"></param>
    /// <param name="matchStr"></param>
    /// <returns></returns>
    private void  ParseEmojParam(string str, int startIndex, int endIndex, string startMatchStr , string endMatchStr , out int dstStartIndex , out int dstLen)
    {
        dstStartIndex = -1;
        dstLen = -1;

        if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(startMatchStr))
        {
            return ;
        }

        if (string.IsNullOrEmpty(endMatchStr) || string.IsNullOrEmpty(startMatchStr))
        {
            return ;
        }

//        Debug.Log("SUBSTR :" + str.Substring(startIndex , endIndex - startIndex + 1));

        int findIndex = str.IndexOf(startMatchStr, startIndex , endIndex - startIndex + 1);
        if (findIndex == -1)
        {
            return;
        }

        // 需要字符的下一个位置
        int subStrStartIndex = findIndex + startMatchStr.Length ;  

        //结束位置(下一个位置)
        int gapIndex = str.IndexOf(endMatchStr, subStrStartIndex , endIndex - subStrStartIndex + 1);
        if (gapIndex == -1)
        {
            gapIndex = endIndex + 1;
        }
        
        int len = gapIndex - subStrStartIndex ;
        if (len <= 0)
        {
            return;
        }

        dstStartIndex = subStrStartIndex;
        dstLen = len;
    }


    /// <summary>
    /// 只支持4位
    /// </summary>
    /// <param name="str"></param>
    /// <param name="startIndex"></param>
    /// <param name="len"></param>
    private int  ParseInt(string str , int startIndex , int len)
    {
        int count = 0;
        int temp = 0;
        char emptyChar = ' ';
        
        for (int i = 0; i < mParserValueCache.Length; i++)
        {
            mParserValueCache[i] = emptyChar;
        }

        for (int i = 0; i < len; i++)
        {
            mParserValueCache[i] = str[startIndex + i];
            if (mParserValueCache[i] >= '0' && mParserValueCache[i] <= '9')
            {
                count ++;
            }
        }

        for(int i = count -1 ;  i >= 0 ; i --)
        {
            int baseNum = mParserValueCache[i] - '0';
            if (baseNum > 9)
            {
                return 0;
            }
            int num = baseNum*  (int)Math.Pow(10.0f , (float)(count - 1 - i));
            temp += num;
        }
        return temp;
    }

    private float ParseFloat(string str, int startIndex, int len)
    {
        if (string.IsNullOrEmpty(str))
        {
            return 0;
        }

        if (len <= 0)
        {
            return 0;
        }

        int count = 0;
        int countDec = 0;
        float temp = 0;
        char emptyChar = '0';


        for (int i = 0; i < mParserValueCache.Length; i++)
        {
            mParserValueCache[i] = emptyChar;
        }

        for (int i = 0; i < mParserValueCacheDec.Length; i++)
        {
            mParserValueCacheDec[i] = emptyChar;
        }

        bool isDec = false;
        int decStartIndex = 0;
        for (int i = 0; i < len; i++)
        {
            if (str[startIndex + i] == '.')
            {
                isDec = true;
                decStartIndex = i ;
                continue;
            }

            if (false == isDec)
            {
                if (i >= ConstMaxIntCount)
                {
                    Debug.LogError("Inline ParseFloat 超过8位整数");
                    break;
                }

                if (str[startIndex + i] >= '0' && str[startIndex + i] <= '9')
                {
                    mParserValueCache[i] = str[startIndex + i];
                    count++;
                }
                else
                {
                    Debug.LogError("Inline ParseFloat 带有不数字字符");
                    break;
                }
            }
            else
            {
                int decIndex = i - decStartIndex - 1;
                
                if (str[startIndex + i] >= '0' && str[startIndex + i] <= '9')
                {
                    mParserValueCacheDec[decIndex] = str[startIndex + i];
                    countDec++;
                }
                else
                {
                    Debug.LogError("Inline ParseFloat 带有不数字字符");
                    break;
                }

                // 小数位置控制
                if (countDec == ConstMaxDecCount)
                {
                    break;
                }
            }
        }

        for (int i = count - 1; i >= 0; i--)
        {
            int baseNum = mParserValueCache[i] - '0';
            if (baseNum > 9)
            {
                return 0;
            }
            int num = baseNum * (int)Math.Pow(10.0f, (float)(count - 1 - i));
            temp += num;
        }

        temp += (mParserValueCacheDec[0] - '0') * 0.1f;
        temp += (mParserValueCacheDec[1] - '0') * 0.01f;

        return temp;
    }


    private void Split(string str , string startStr , string endStr  , ref int[] startIndexs, ref int[] endIndex)
	{
		if(string.IsNullOrEmpty(str))
		{
			return ;
		}

		if(string.IsNullOrEmpty(startStr) || string.IsNullOrEmpty(endStr))
		{
			return ;
		}

		FindIndexs(str , startStr , ref startIndexs ) ;
		FindIndexs(str , endStr , ref endIndex ) ;
	}


    /// <summary>
    /// 只记录查询起始位置
    /// </summary>
    /// <param name="str"></param>
    /// <param name="matchStr"></param>
    /// <param name="indexs"></param>
    private void FindIndexs(string str , string matchStr,ref int[] indexs )
	{
		int index = -1 ;
		int startIndex = 0;
	    int len = str.Length;
	    int count = 0;
        while (startIndex <= len)
		{
			index = str.IndexOf(matchStr, startIndex) ;
			if(index == -1)
			{
				break ;
			}
			else
			{
			    if (count >= indexs.Length)
			    {
			        break;
			    }

                indexs[count] = index;
                startIndex = index + 1;
                count++;
            }
		}
	}

    private void ResetIndexCache()
	{
        for (int i = 0; i < mPosIndexsStartCache.Length; ++i)
        {
            mPosIndexsStartCache[i] = -1;
        }
        for (int i = 0; i < mPosIndexsEndCache.Length; ++i)
        {
            mPosIndexsEndCache[i] = -1;
        }
    }

    private void ResetEmojParamCache()
    {
        for (int i = 0; i < mEmojParams.Length; ++i)
        {
            if (mEmojParams[i] == null)
            {
                mEmojParams[i] = new EmojParam();
            }
            mEmojParams[i].Reset();
        }
    }

    private void ResetHrefCache()
    {
        for (int i = 0; i < mHrefParams.Length; ++i)
        {
            if (mHrefParams[i] == null)
            {
                mHrefParams[i] = new HrefParam();
            }
            mHrefParams[i].Reset();
        }
    }
}
