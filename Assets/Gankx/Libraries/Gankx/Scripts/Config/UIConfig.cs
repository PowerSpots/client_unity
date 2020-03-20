using System.Collections.Generic;
using Gankx.UI;

namespace Gankx
{
    public static class UIConfig
    {
        public const string AssetPath = "Assets/UI/";

        #region Panel

        public const string PanelEditorCanvas = "GamePortal/GMCanvas";

        // Base folder "Resources"
        public const string PanelSimplePath = "ui/panel/";
        public const string OverlayCameraPath = "ui/template/UICamera_Overlay";


        public static readonly List<PanelLayerParam> PanelLayerList =
            new List<PanelLayerParam>
            {
                new PanelLayerParam
                {
                    layerType = PanelLayerType.Bottom,
                    canvasPath = "ui/template/OverlayCanvas",
                    sortingLayer = "Bottom"
                },
                new PanelLayerParam
                {
                    layerType = PanelLayerType.Background,
                    canvasPath = "ui/template/OverlayCanvas",
                    sortingLayer = "Background"
                },
                new PanelLayerParam
                {
                    layerType = PanelLayerType.Foreground,
                    canvasPath = "ui/template/OverlayCanvas",
                    sortingLayer = "Default"
                },
                new PanelLayerParam
                {
                    layerType = PanelLayerType.Highlight,
                    canvasPath = "ui/template/OverlayCanvas",
                    sortingLayer = "Highlight"
                },
                new PanelLayerParam
                {
                    layerType = PanelLayerType.Top,
                    canvasPath = "ui/template/OverlayCanvas",
                    sortingLayer = "Top"
                },
                new PanelLayerParam
                {
                    layerType = PanelLayerType.World,
                    canvasPath = "ui/template/WorldCanvas",
                    isOverlay = false,
                    xOffset = 250f
                },
                new PanelLayerParam
                {
                    layerType = PanelLayerType.Share,
                    canvasPath = "ui/template/ShareCanvas",
                    camPath = "ui/template/UICamera_Share",
                    isOverlay = false
                },
                new PanelLayerParam
                {
                    layerType = PanelLayerType.Foreground720,
                    canvasPath = "ui/template/720Canvas",
                    sortingLayer = "Default"
                },
                new PanelLayerParam
                {
                    layerType = PanelLayerType.Afk,
                    canvasPath = "ui/template/OverlayCanvas",
                    sortingLayer = "Default"
                }
            };

        #endregion

        #region AtlasImage

        // Base folder "Assets"
        public const string AtlasBasePath = "UI";

        // Base folder ""
        public const string AtlasAssetPath = "Assets/UI/atlas";

        // Base folder "Assets"
        public const string AtlasResourcePath = "Resources/atlas";

        // Base folder "Assets/Resources"
        public const string AtlasSimplePath = "atlas";

        public const char AtlasNameSeparator = '_';

        public const int AtlasFreeListCount = 20;
        public const int AtlasCacheCheckCount = 7;

        public const int BundleSpriteFreeListCount = 30;
        public const int BundleCacheCheckCount = 4;

        // Base folder "Assets"
        public static readonly string[] BundleSpritePaths =
        {
            "UI/noatlas_sprites",
            "UI/noref"
        };

        public const string SharedAtlasName = "atlas_shared";
        public const string SharedAtlasWhiteSprite = "shared_whitesquare";

        #endregion

        #region DynamicImage

        // Base folder ""
        public const string IconAssetResourcePath = "Assets/Resources/ui/icon";

        // Base folder "Assets/Resources"
        public const string IconSimplePath = "ui/icon";


        public const int DynamicSpriteTextureWidth = 1024;

        public const int DynamicSpriteTextureHeight = 1024;

        public const int DynamicSpriteTexturePadding = 4;

        // Base folder "Assets/Resources"
        public const string DynamicSpriteTexturePath = "ui/dynamicatlas/virtual_sprite_atlas";

        // Base folder "Assets/Resources"
        public static readonly DynamicSpriteService.SpriteBlock[] DynamicSpriteBlocks =
        {
            new DynamicSpriteService.SpriteBlock
            {
                rect = {xMin = 0, yMin = 0, width = 7, height = 7},
                matchRange = {x = 128, y = 128},
                holderPath = "ui/dynamicatlas/block128"
            },
            new DynamicSpriteService.SpriteBlock
            {
                rect = {xMin = 924, yMin = 0, width = 1, height = 12},
                matchRange = {x = 80, y = 80},
                holderPath = "ui/dynamicatlas/block80"
            },
            new DynamicSpriteService.SpriteBlock
            {
                rect = {xMin = 0, yMin = 924, width = 11, height = 1},
                matchRange = {x = 80, y = 80},
                holderPath = "ui/dynamicatlas/block80two"
            }
        };

        #endregion

        #region RawImage

        // Base folder "Assets"
        public const string RawImageResourcePath = "Resources/ui/rawimage";

        // Base folder "Assets/Resources"
        public const string RawImageSimplePath = "ui/rawimage";

        // Base folder "Data"
        public const string LocalJpgPath = "localimage/";

        public const int RawImageAssetTextureFreeListCount = 25;

        public const int RawImageAssetTextureCacheCheckCount = 2;

        #region Url

        public const int RawImageUrlTextureMaxLoadCountInFrame = 5;

        public const float RawImageUrlTextureDiskCacheTimeElapsed = 3600;

        public const float RawImageUrlTextureDiskCacheTimeMin = 0;

        public const float RawImageUrlTextureDiskCacheTimeMax = 1200;

        public const int RawImageUrlTextureMaxDiskCount = 200;

        public const string RawImageUrlTextureDiskExtension = ".urlimg";

        public const string RawImageUrlTextureDiskSearchPattern = "*.urlimg";

        public const string RawImageUrlTextureDiskSimplePath = "/urlimg/";

        #endregion

        #endregion
    }
}