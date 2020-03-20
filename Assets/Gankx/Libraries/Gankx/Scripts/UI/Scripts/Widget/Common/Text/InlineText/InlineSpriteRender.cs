using UnityEngine;
using System.Collections.Generic;
using Gankx;
using Gankx.UI;
using UnityEngine.UI;
using UnityEngine.Serialization;


namespace EmojText
{
    public class InlineSpriteRender : MaskableGraphic
    {
        private string AtlasAssetPath = "atlas/emoj";

        private bool mDirty = false;
        public override Texture mainTexture
        {
            get { return GetMainTextture(); }
        }

        public Texture GetMainTextture()
        {
            AtlasAsset atlasAsset = InlineSpriteAssetManager.instance.GetAtlas(AtlasAssetPath);
            if (atlasAsset == null)
            {
                return s_WhiteTexture;
            }

            Sprite sprite = atlasAsset.GetSprite(0);
            if (sprite != null)
            {
                return sprite.texture;
            }

            return s_WhiteTexture;
        }

        protected override void Awake()
        {
            //todo 临时修改
            AtlasAssetPath = "atlas/emoj";
            transform.localPosition = new Vector3(0, 0, 0);
            rectTransform.sizeDelta = new Vector2(0, 0);
            rectTransform.localScale = new Vector3(1,1,1);
            UpdateMaterial();
        }
        
        private Mesh mTempMesh ;

        public void UpdateMesh(VertexHelper vertexHelper)
        {
            mDirty = true;
            cacheVertexHelper = vertexHelper;
            if (cacheVertexHelper == null)
            {
                return;
            }

            if (mTempMesh == null)
            {
                mTempMesh = new Mesh();
            }

            cacheVertexHelper.FillMesh(mTempMesh);
            GetComponent<CanvasRenderer>().SetMesh(mTempMesh);
        }

        private VertexHelper cacheVertexHelper;

        private void Update()
        {
            if (mDirty)
            {
                mDirty = false;
            }
        }

        //----------------------------------------------------资源数据

        public SpriteAssetInfo GetSpriteInfo(int index)
        {
            return InlineSpriteAssetManager.instance.GetSpriteUvInfoByIndex(AtlasAssetPath, index);
        }

        public SpriteAssetInfo GetSpriteInfo(string name)
        {
            return InlineSpriteAssetManager.instance.GetSpriteUVInfo(AtlasAssetPath, name);
        }
        
        public int  GetSpriteNamesFromPrefix(string namePrefix , ref string[] names)
        {
            return InlineSpriteAssetManager.instance.GetSpriteNamesFromPrefix(AtlasAssetPath, namePrefix , ref  names);
        }

    }
}