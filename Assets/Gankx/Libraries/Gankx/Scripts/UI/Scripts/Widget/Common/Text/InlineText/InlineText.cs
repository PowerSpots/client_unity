using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;
using UnityEngine.UI;

/// 已知问题：
/// 1. 下划线支持: 不支持超链接与下划线混排（顶点数据获取方式）, 换行bound计算问题，下划线颜色问题

namespace EmojText
{
    [ExecuteInEditMode]
    public class InlineText : Text, IPointerClickHandler
    {
        /// <summary>
        /// 可通过外部设置避免查找
        /// </summary>
        private InlineSpriteRenderManager mSpriteRenderManager;
        public InlineSpriteRenderManager SpriteRenderManager
        {
            get { return mSpriteRenderManager; }
            set { mSpriteRenderManager = value; }
        }

        //表情信息列表
        private List<SpriteAnimInfo> mAnimSpriteInfoList;
        public List<SpriteAnimInfo> AnimSpriteInfoList
        {
            get { return mAnimSpriteInfoList; }
        }

        // 链接点击反馈
        private HrefClickEvent m_onHrefClick = new HrefClickEvent();
        public HrefClickEvent onHrefClick
        {
            get { return m_onHrefClick; }
            set { m_onHrefClick = value; }
        }

        //文字额外最大宽度/高度 控制
        private InlineTextSizeController mSizeController;
        //表情渲染组件
        private InlineSpriteRender mInlineSpriteRender;
        //文本解析
        private InlineTextParse mInlineTextParse;
        //解析结果
        private string mResultText;

        #region override

        protected override void Awake()
        {
             mSizeController = GetComponent<InlineTextSizeController>();
            // manager
            if (mSpriteRenderManager == null && canvas != null)
            {
                mSpriteRenderManager = GetSpriteManager();

                if (mSpriteRenderManager == null)
                {
                    Debug.LogError("InlineSpriteAnimManager is miss , please add in root");
                }
            }

            if (mSpriteRenderManager != null)
            {
                mInlineSpriteRender = mSpriteRenderManager.GetComponent<InlineSpriteRender>();
            }
        }

        protected override void OnEnable()
        {
            alignByGeometry = true;
            supportRichText = true;

 
            Register();

            base.OnEnable();
        }


        protected override void OnDisable()
        {
            UnRegister();
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            UnRegister();
            base.OnDestroy();
        }

        public override string text
        {
            get { return base.text; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (string.IsNullOrEmpty(m_Text))
                    {
                        return;
                    }

                    m_Text = "";
                    ParseText();
                    SetVerticesDirty();
                }
                else if (m_Text != value)
                {
                    m_Text = value;
                    ParseText();
                    SetVerticesDirty();
                    SetLayoutDirty();
                }
            }
        }

        public override float preferredHeight
        {
            get
            {
                if (mSizeController == null)
                {
                    mSizeController = GetComponent<InlineTextSizeController>();
                }
                var settings = GetGenerationSettings(new Vector2(rectTransform.rect.size.x, 0.0f));
                float height = cachedTextGeneratorForLayout.GetPreferredHeight(mResultText, settings)/pixelsPerUnit;
                if (mSizeController != null && mSizeController.MaxHeight != -1)
                {
                    if (height >= mSizeController.MaxHeight)
                    {
                        height = mSizeController.MaxHeight;
                    }
                }
                return height;
            }
        }

        public override float preferredWidth
        {
            get
            {
                if (mSizeController == null)
                {
                    mSizeController = GetComponent<InlineTextSizeController>();
                }

                var settings = GetGenerationSettings(Vector2.zero);
                float width = cachedTextGeneratorForLayout.GetPreferredWidth(mResultText, settings)/pixelsPerUnit;
                width += 5; //补偿
                if (mSizeController != null && mSizeController.MaxWidth != -1)
                {
                    if (width >= mSizeController.MaxWidth)
                    {
                        width = mSizeController.MaxWidth;
                    }
                }
                return width;
            }
        }

        public override void SetVerticesDirty()
        {
            if (!IsActive())
            {
                return;
            }

#if UNITY_EDITOR
            //处理编辑下文本修改问题，默认TextEditor会绕开override的text实现
//            ParseText();
#endif
            base.SetVerticesDirty();
        }

        #endregion

        #region Data

        private void Register()
        {
            // manager
            if (mSpriteRenderManager == null && canvas != null)
            {
                mSpriteRenderManager = GetSpriteManager();

                if (mSpriteRenderManager == null)
                {
                    Debug.LogError("InlineSpriteAnimManager is miss , please add in root");
                }
            }

            //inline
            if (mSpriteRenderManager != null)
            {
                mInlineSpriteRender = mSpriteRenderManager.GetComponent<InlineSpriteRender>();
            }

            if (mSpriteRenderManager != null && mInlineSpriteRender != null)
            {
                mInlineSpriteRender.SetAllDirty();
                ParseText();
                SetVerticesDirty();
                mSpriteRenderManager.Register(this);
            }
        }

        private void UnRegister()
        {
            if (mSpriteRenderManager != null)
            {
                mSpriteRenderManager.UnRegister(this);
            }
        }

        private void ParseText()
        {
            InlineTextParse textParse = GetInlineTextParse();
            if (textParse != null)
            {
                mResultText = textParse.ParseText(m_Text);
                ResetSpriteInfoList();
            }
        }

        private List<SpriteTagInfo> GetSpriteTagInfos()
        {
            InlineTextParse textParse = GetInlineTextParse();
            if (textParse != null)
            {
                return textParse.GetSpriteTagInfos();
            }

            return null;
        }
        private List<UnderlineTagInfo> GetUnderlineTagInfos()
        {
            InlineTextParse textParse = GetInlineTextParse();
            if (textParse != null)
            {
                return textParse.GetUnderlineTagInfos();
            }

            return null;
        }

        private List<HrefTagInfo> GetHrefTagInfos()
        {
            InlineTextParse textParse = GetInlineTextParse();
            if (textParse != null)
            {
                return textParse.GetHrefTagInfos();
            }
            return null;
        }

        private InlineTextParse GetInlineTextParse()
        {
            if (mInlineTextParse == null )
            {
                if (gameObject != null && mInlineSpriteRender!=null)
                {
                    mInlineTextParse = new InlineTextParse(gameObject, mInlineSpriteRender);
                }
            }

            return mInlineTextParse;
        }

        /// <summary>
        /// 从自身向上查找，表情图片单独渲染，解决层级问题可以通过增加多个管理器解决（不是很好的解决方案）
        /// </summary>
        /// <returns></returns>
        private InlineSpriteRenderManager GetSpriteManager()
        {
            Transform current = transform.parent;
            while (null != current)
            {
                InlineSpriteRenderManager temp = current.GetComponentInChildren<InlineSpriteRenderManager>();
                if (temp != null)
                {
                    return temp;
                }

                current = current.parent;
            }
            return null;
        }

        private void ResetSpriteInfoList()
        {
            List<SpriteTagInfo> animSpiteTagList = GetSpriteTagInfos();

            if (animSpiteTagList == null || animSpiteTagList.Count == 0)
            {
                mAnimSpriteInfoList = null;
                return;
            }

            if (mAnimSpriteInfoList == null)
            {
                mAnimSpriteInfoList = new List<SpriteAnimInfo>(2);
            }

            int validCount = 0;
            for (int i = 0; i < animSpiteTagList.Count; ++i)
            {
                if (animSpiteTagList[i].IsValid())
                {
                    validCount++;
                }
            }

            if (validCount > mAnimSpriteInfoList.Count)
            {
                int needCount = validCount - mAnimSpriteInfoList.Count;
                for (int i = 0; i <= needCount; ++i)
                {
                    SpriteAnimInfo infos = new SpriteAnimInfo();
                    mAnimSpriteInfoList.Add(infos);
                }
            }
            else
            {
                for (int i = validCount; i < mAnimSpriteInfoList.Count; ++i)
                {
                    mAnimSpriteInfoList[i].Reset();
                }
            }
        }

        #endregion

        #region update

        private void Update()
        {
            if (mSpriteRenderManager == null)
            {
                Register();
            }

            if (rectTransform.hasChanged)
            {
                rectTransform.hasChanged = false;
                UpdateSpritePos();
            }
        }

        private readonly UIVertex[] m_TempVerts = new UIVertex[4];
        private VertexHelper mVertexHelperRef;

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (font == null)
            {
                return;
            }
            UnityEngine.Profiling.Profiler.BeginSample("inlineText OnPopulateMesh");

            // We don't care if we the font Texture changes while we are doing our Update.
            // The end result of cachedTextGenerator will be valid for this instance.
            // Otherwise we can get issues like Case 619238.
            m_DisableFontTextureRebuiltCallback = true;

            Vector2 extents = rectTransform.rect.size;
            var textGenerationSettings = GetGenerationSettings(extents);
            cachedTextGenerator.Populate(mResultText, textGenerationSettings);

            Rect inputRect = rectTransform.rect;

            // get the text alignment anchor point for the text in local space
            Vector2 textAnchorPivot = GetTextAnchorPivot(alignment);
            Vector2 refPoint = Vector2.zero;
            refPoint.x = (textAnchorPivot.x == 1 ? inputRect.xMax : inputRect.xMin);
            refPoint.y = (textAnchorPivot.y == 0 ? inputRect.yMin : inputRect.yMax);

            // Determine fraction of pixel to offset text mesh.
            Vector2 roundingOffset = PixelAdjustPoint(refPoint) - refPoint;

            // Apply the offset to the vertices
            IList<UIVertex> verts = cachedTextGenerator.verts;
            float unitsPerPixel = 1/pixelsPerUnit;
            //Last 4 verts are always a new line...
            int vertCount = verts.Count - 4;

            toFill.Clear();

            ClearQuadUv(verts);

            if (roundingOffset != Vector2.zero)
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                    if (tempVertsIndex == 3)
                    {
                        toFill.AddUIVertexQuad(m_TempVerts);
                    }
                }
            }
            else
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    if (tempVertsIndex == 3)
                    {
                        toFill.AddUIVertexQuad(m_TempVerts);
                    }
                }
            }
            mVertexHelperRef = toFill;

            HerfTagHandler();
            UnderlineTagsHandler();
            SpriteTagHandler();

            m_DisableFontTextureRebuiltCallback = false;

            UnityEngine.Profiling.Profiler.EndSample();
        }

        #endregion

        #region sprite

        private List<Vector3> mSpriteVertPosList;

        private void UpdateSpritePos()
        {
            Profiler.BeginSample("inlineText UpdateSpritePos");

            if (mSpriteVertPosList != null)
            {
                CalcQuadTag(mSpriteVertPosList, true);
                if (mSpriteRenderManager != null)
                {
                    mSpriteRenderManager.UpdatePositon(this, mAnimSpriteInfoList);
                }
            }

            Profiler.EndSample();
        }

        private void SpriteTagHandler()
        {
            if (mSpriteVertPosList == null)
            {
                mSpriteVertPosList = new List<Vector3>();
            }

            //保证与Tag数量一致
            mSpriteVertPosList.Clear();

            List<SpriteTagInfo> animSpiteTagList = GetSpriteTagInfos();
            if (animSpiteTagList == null || animSpiteTagList.Count == 0)
            {
                return;
            }


            UIVertex tempVer = new UIVertex();
            for (int i = 0; i < animSpiteTagList.Count; i++)
            {
                SpriteTagInfo tempTagInfo = animSpiteTagList[i];
                if (!tempTagInfo.IsValid())
                {
                    continue;
                }

                int vertexIndex = ((tempTagInfo.VertextIndex + 1) * 4) - 1;
                if (vertexIndex >= mVertexHelperRef.currentVertCount || vertexIndex < 0)
                {
                    mSpriteVertPosList.Add(new Vector3(-1000000000 , -1000000000, -1000000000));
//                    Debug.LogError("CalcQuadTag Vertex Index is out of range:" + vertexIndex);
                    continue;
                }
                
                mVertexHelperRef.PopulateUIVertex(ref tempVer, vertexIndex);
                mSpriteVertPosList.Add(tempVer.position);
            }


            CalcQuadTag(mSpriteVertPosList);

            if (mSpriteRenderManager != null)
            {
                mSpriteRenderManager.UpdateSpriteAnimInfos(this, mAnimSpriteInfoList);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteVexPos">图片位置信息</param>
        /// <param name="onlyUpdatePositon">是否只更新位置</param>
        private void CalcQuadTag(List<Vector3> spriteVexPos, bool onlyUpdatePositon = false)
        {
            List<SpriteTagInfo> animSpiteTagList = GetSpriteTagInfos();

            if (animSpiteTagList == null || animSpiteTagList.Count == 0)
            {
                return;
            }
            
            //canvas scale 
            Vector3 relativePostion = Vector3.zero;
            if (mSpriteRenderManager != null)
            {
                relativePostion = mSpriteRenderManager.transform.InverseTransformPoint(transform.position);
            }

            UnityEngine.Profiling.Profiler.BeginSample("inlineText CalcQuadTag Cal");

            for (int i = 0; i < animSpiteTagList.Count; i++)
            {
                SpriteTagInfo tempTagInfo = animSpiteTagList[i];
                if (!tempTagInfo.IsValid())
                {
                    continue;
                }

                SpriteAnimInfo tempSpriteAnimInfos = mAnimSpriteInfoList[i];

                tempSpriteAnimInfos.Key = tempTagInfo.Key;
                tempSpriteAnimInfos.SetName(tempTagInfo.GetNames(), tempTagInfo.GetValidCount());   //Refrence

                if (i >= spriteVexPos.Count || spriteVexPos[i].x == -1000000000)
                {
                    //数据无效
                    tempSpriteAnimInfos.Reset();
                    continue;
                }

                Vector3 textpos = relativePostion + spriteVexPos[i];

                float sizeX = tempTagInfo.Size.x *mSpriteRenderManager.transform.lossyScale.x / transform.lossyScale.x;
                float sizey = tempTagInfo.Size.y * mSpriteRenderManager.transform.lossyScale.y / transform.lossyScale.y;

                float xOffset = tempTagInfo.OffsetX * sizeX;
                float yOffset = tempTagInfo.OffsetY * sizey;

                tempSpriteAnimInfos.Vertices[0] = new Vector3(xOffset, -yOffset, 0) + textpos;
                tempSpriteAnimInfos.Vertices[1] = new Vector3(xOffset + sizeX, -yOffset + sizey, 0) + textpos;
                tempSpriteAnimInfos.Vertices[2] = new Vector3(xOffset + sizeX, -yOffset, 0) + textpos;
                tempSpriteAnimInfos.Vertices[3] = new Vector3(xOffset, -yOffset + sizey, 0) + textpos;

                // 位置超出
                if (onlyUpdatePositon == true)
                {
                    continue;
                }
                
                for (int j = 0; j < tempTagInfo.GetValidCount(); j++)
                {
                    Rect newSpriteRect;
                    if (string.IsNullOrEmpty(tempTagInfo.GetNames()[j]))
                    {
                        continue;
                    }

                    SpriteAssetInfo tempSpriteAsset = mInlineSpriteRender.GetSpriteInfo(tempTagInfo.GetNames()[j]);
                    if (tempSpriteAsset != null)
                    {
                        newSpriteRect = tempSpriteAsset.rect;
                    }
                    else
                    {
                        newSpriteRect = mInlineSpriteRender.GetSpriteInfo(0).rect;
                        Debug.LogError("CalcQuadTag Can Find Sprite(name=" + tempTagInfo.Key + ")");
                    }
                    tempSpriteAnimInfos.SetUv(j, newSpriteRect);
                }
            }

            UnityEngine.Profiling.Profiler.EndSample();
        }

        //UGUIText不支持<quad/>标签，表现为乱码, 将uv全设置为0
        private void ClearQuadUv(IList<UIVertex> verts)
        {
            List<SpriteTagInfo> animSpiteTagList = GetSpriteTagInfos();

            if (animSpiteTagList == null || animSpiteTagList.Count == 0)
            {
                return;
            }

            UIVertex tempVertex;

            for (int i = 0; i < animSpiteTagList.Count; i++)
            {
                SpriteTagInfo temp = animSpiteTagList[i];
                if (!temp.IsValid())
                {
                    continue;
                }

                int startIndex = temp.VertextIndex * 4;
                int endIndex = startIndex + 4;

                for (int m = startIndex; m < endIndex; m++)
                {
                    if (m >= verts.Count)
                    {
                        continue;
                    }

                    tempVertex = verts[m];
                    tempVertex.uv0 = Vector2.zero;
                    verts[m] = tempVertex;
                }
            }
        }
        #endregion

        #region underline
        private void UnderlineTagsHandler()
        {
            List<UnderlineTagInfo> underlineTagInfos = GetUnderlineTagInfos();

            if (underlineTagInfos == null || underlineTagInfos.Count == 0)
            {
                return;
            }

            for (int i = 0; i < underlineTagInfos.Count; ++i)
            {
                UnderlineTagInfo temp = underlineTagInfos[i];
                if (!temp.IsValid())
                {
                    continue;
                }

                int vertexStart = underlineTagInfos[i].StartIndex*4;
                int vertexEnd = (underlineTagInfos[i].EndIndex - 1)*4 + 3;
                underlineTagInfos[i].Boxes = GetBounds(vertexStart, vertexEnd);
            }

            TextGenerator textGenerator = new TextGenerator();
            var settings = GetGenerationSettings(Vector2.zero);
            textGenerator.Populate("_", settings);
            IList<UIVertex> underlineVerts = textGenerator.verts;

            for (int m = 0; m < underlineTagInfos.Count; ++m)
            {
                var underlineInfo = underlineTagInfos[m];
                if (!underlineInfo.IsValid())
                {
                    continue;
                }
                if (underlineInfo.StartIndex >= mVertexHelperRef.currentVertCount)
                {
                    continue;
                }

                for (int i = 0; i < underlineInfo.Boxes.Count; i++)
                {
                    Vector3 startBoxPos = new Vector3(underlineInfo.Boxes[i].x, underlineInfo.Boxes[i].y, 0.0f);
                    Vector3 endBoxPos = startBoxPos + new Vector3(underlineInfo.Boxes[i].width, 0.0f, 0.0f);
                    AddUnderlineQuad(underlineVerts, startBoxPos, endBoxPos);
                }
            }
        }

        //根据起始位置获得包围盒
        private List<Rect> GetBounds(int vertexStartIndex, int vertexEndIndex)
        {
            List<Rect> boxs = new List<Rect>();
            if (null == mVertexHelperRef)
            {
                return boxs;
            }

            if (vertexStartIndex < 0 || vertexStartIndex >= mVertexHelperRef.currentVertCount)
            {
                return boxs;
            }

            if (vertexEndIndex < 0 || vertexEndIndex >= mVertexHelperRef.currentVertCount)
            {
                return boxs;
            }

            UIVertex vert = new UIVertex();
            mVertexHelperRef.PopulateUIVertex(ref vert, vertexStartIndex);
            var pos = vert.position;
            var bounds = new Bounds(pos, Vector3.zero);
            for (int i = vertexStartIndex, m = vertexEndIndex; i < m; i++)
            {
                if (i >= mVertexHelperRef.currentVertCount)
                {
                    break;
                }

                mVertexHelperRef.PopulateUIVertex(ref vert, i);
                pos = vert.position;
                if (pos.x < bounds.min.x) // 换行重新添加包围框     todo
                {
                    boxs.Add(new Rect(bounds.min, bounds.size));
                    bounds = new Bounds(pos, Vector3.zero);
                }
                else //扩展包围盒
                {
                    bounds.Encapsulate(pos);
                }
            }

            boxs.Add(new Rect(bounds.min, bounds.size));
            return boxs;
        }

        //添加下划线
        private void AddUnderlineQuad(IList<UIVertex> underlineVerts, Vector3 startBoxPos, Vector3 endBoxPos)
        {
            Vector3[] underlinePos = new Vector3[4];
            underlinePos[0] = startBoxPos + new Vector3(0, fontSize*-0.1f, 0);
            underlinePos[1] = endBoxPos + new Vector3(0, fontSize*-0.1f, 0);
            ;
            underlinePos[2] = endBoxPos + new Vector3(0, fontSize*0f, 0);
            underlinePos[3] = startBoxPos + new Vector3(0, fontSize*0f, 0);

            for (int i = 0; i < 4; ++i)
            {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = underlineVerts[i%4];
                m_TempVerts[tempVertsIndex].color = Color.blue;
                m_TempVerts[tempVertsIndex].position = underlinePos[i];
                if (tempVertsIndex == 3)
                {
                    mVertexHelperRef.AddUIVertexQuad(m_TempVerts);
                }
            }
        }

        #endregion

        #region href

        private void HerfTagHandler()
        {
            List<HrefTagInfo> hrefTagInfos = GetHrefTagInfos();
            if (hrefTagInfos == null || hrefTagInfos.Count == 0)
            {
                return;
            }

            for (int i = 0; i < hrefTagInfos.Count; ++i)
            {
                HrefTagInfo temp = hrefTagInfos[i];
                if (!temp.IsValid())
                {
                    continue;
                }

                int vertexStart = temp.StartIndex * 4;
                int vertexEnd = (temp.EndIndex - 1) * 4 + 3;

                hrefTagInfos[i].Boxes = GetBounds(vertexStart, vertexEnd);
            }
        }

        /// 点击事件检测是否点击到超链接文本
        public void OnPointerClick(PointerEventData eventData)
        {
            List<HrefTagInfo> hrefTagInfos = GetHrefTagInfos();

            Vector2 lp;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out lp);
            foreach (var hrefInfo in hrefTagInfos)
            {
                if (!hrefInfo.IsValid())
                {
                    continue;
                }

                var boxes = hrefInfo.Boxes;
                for (var i = 0; i < boxes.Count; ++i)
                {
                    if (boxes[i].Contains(lp))
                    {
                        m_onHrefClick.Invoke(hrefInfo.Name);
                        return;
                    }
                }
            }
        }

        #endregion

        private void DebugLog(string str)
        {
            // Debug.Log("_______________________" + str);
        }
    }

}