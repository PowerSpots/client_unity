using Gankx.PAL;

namespace Gankx
{
    public enum BundleType
    {
        BuildIn,
        Other,
        UIAtlas,
        NoAtlasSprites,
        Font,
        UIIcon,
        Character,
        AnimBase,
        Camera,
        Effect,
        Panel,
        Cinematic,
        Prefabs,
        Settings,
        Scene,
        Max
    }

    public enum PackStrategy
    {
        Default,
        One,
        SubOne,
        Atlas,
        Character,
        Avatar,
        Effect
    }

    public enum DependenciesStrategy
    {
        None,
        Direct,
        DirectAndLessRedundancy,
        All
    }

    public class PackTargetConfig
    {
        public BundleType type;
        public string name;
        public string[] assetPath;
        public DependenciesStrategy dependency;
        public PackStrategy packStrategy;
        public int redundancyParam = 1;
    }

    public static class AssetBundleConfig
    {
        public const string ConstAssetTail = ".u";

        public const float ConstAsyncLoadAssetTimeLimit = 300f;
        public const float ConstAsyncLoadSceneTimeLimit = 600f;
        public static readonly string AssetBundlesPath = "assetbundle";

        private static readonly PackTargetConfig[] MyPackTargetConfigs =
        {
            new PackTargetConfig
            {
                type = BundleType.Font,
                name = "font",
                assetPath = new[]
                {
                    "UI/bitmap_font"
                },

                dependency = DependenciesStrategy.None,
                packStrategy = PackStrategy.One
            },

            new PackTargetConfig
            {
                type = BundleType.NoAtlasSprites,
                name = "noatlas_sprites",
                assetPath = new[]
                {
                    "UI/fonts",
                    "UI/noatlas_sprites"
                },

                dependency = DependenciesStrategy.None,
                packStrategy = PackStrategy.Default
            },

            new PackTargetConfig
            {
                type = BundleType.UIAtlas,
                name = "atlas",
                assetPath = new[]
                {
                    "UI/atlas"
                },

                dependency = DependenciesStrategy.None,
                packStrategy = PackStrategy.Atlas
            },

            new PackTargetConfig
            {
                type = BundleType.UIIcon,
                name = "icon",
                assetPath = new[]
                {
                    "Resources/ui/icon",
                    "Resources/ui/defaultheadicon",
                    "Resources/ui/mask",
                    "Resources/ui/rawimage",
                    "Resources/common"
                },
                dependency = DependenciesStrategy.None,
                packStrategy = PackStrategy.Default
            },

            new PackTargetConfig
            {
                type = BundleType.Panel,
                name = "Panel",
                assetPath = new[]
                {
                    "Resources/ui/mat",
                    "Resources/ui/panel",
                    "Resources/ui/template"
                },
                dependency = DependenciesStrategy.All,
                packStrategy = PackStrategy.Default
            },

            new PackTargetConfig
            {
                type = BundleType.AnimBase,
                name = "animbase",
                assetPath = new[]
                {
                    "ArtSrc/animation/system/base"
                },

                dependency = DependenciesStrategy.None,
                packStrategy = PackStrategy.One
            },

            new PackTargetConfig
            {
                type = BundleType.Character,
                name = "Character",
                assetPath = new[]
                {
                    "Resources/character",
                    "Resources/portrait",
                    "Resources/treasure",
                    "Resources/charactercustom",
                    "Resources/controller",
                    "Resources/Cinematic",
                    "Resources/animation"
                },
                dependency = DependenciesStrategy.All,
                packStrategy = PackStrategy.Character
            },

            new PackTargetConfig
            {
                type = BundleType.Effect,
                name = "Effect",
                assetPath = new[]
                {
                    "Resources/effect"
                },
                dependency = DependenciesStrategy.All,
                packStrategy = PackStrategy.Effect
            },

            new PackTargetConfig
            {
                type = BundleType.Other,
                name = "other",
                assetPath = new[]
                {
                    "Resources/camera",
                    "Resources/storyb_sequence",
                    "Resources/scene/logicalAsset"
                },
                dependency = DependenciesStrategy.All,
                packStrategy = PackStrategy.Default
            },

            new PackTargetConfig
            {
                type = BundleType.Settings,
                name = "Settings",
                assetPath = new[]
                {
                    "Resources/settings/SceneBuildSetting.asset"
                },
                dependency = DependenciesStrategy.None,
                packStrategy = PackStrategy.Default
            }
        };

        public static string GetManifestName()
        {
            return AssetBundlesPath;
        }

        public static PackTargetConfig[] GetPackTargetConfigs()
        {
            return MyPackTargetConfigs;
        }

        public static string GetAssetBundlePath(string relativePath)
        {
            return Platform.instance.GetBundlePath(AssetBundlesPath + "/" + relativePath);
        }

        public static string GetAssetBundleBundleUrl(string relativePath)
        {
            return Platform.instance.GetBundleUrl(AssetBundlesPath + "/" + relativePath);
        }
    }
}