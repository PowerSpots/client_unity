using System.Collections.Generic;
using System.IO;
using Gankx.UI;
using UnityEditor;
using UnityEngine;

namespace Gankx.UIEditor
{
    public class DynamicImageTools
    {
        private static readonly Vector2 PivotCenter = new Vector2(0.5f, 0.5f);

        private static void DoSerialize(RectInt atlasRegion, int spriteSize, string filePath, Texture2D atlasTexture,
            int padding)
        {
            var prefab = Resources.Load<GameObject>(filePath);
            if (null == prefab)
            {
                var empty = new GameObject();
                prefab = PrefabUtility.CreatePrefab("Assets/Resources/" + filePath + ".prefab", empty);
            }

            if (null == prefab)
            {
                return;
            }

            var go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

            var spriteHolder = go.GetOrAddComponent<VirtualSpriteHolder>();

            var sprites = new List<Sprite>();
            for (var i = 0; i < atlasRegion.width; ++i)
            {
                for (var j = 0; j < atlasRegion.height; ++j)
                {
                    var rect = new Rect();
                    rect.x = atlasRegion.x + i * (spriteSize + padding);
                    rect.y = atlasRegion.y + j * (spriteSize + padding);
                    rect.width = spriteSize;
                    rect.height = spriteSize;

                    var sprite = Sprite.Create(atlasTexture, rect, PivotCenter);
                    AssetDatabase.CreateAsset(sprite,
                        string.Format("Assets/Resources/{0}_sprite{1}.asset", filePath,
                            i * atlasRegion.height + j));
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    sprite = Resources.Load<Sprite>(string.Format("{0}_sprite{1}", filePath,
                        i * atlasRegion.height + j));

                    if (null != sprite)
                    {
                        sprites.Add(sprite);
                    }
                }
            }

            spriteHolder.sprites = sprites.ToArray();

            PrefabUtility.ReplacePrefab(go, prefab, ReplacePrefabOptions.ConnectToPrefab);
            Object.DestroyImmediate(go);
        }

        private static Sprite SaveSpriteToEditorPath(Sprite sp, string path)
        {
            File.WriteAllBytes(path, sp.texture.EncodeToPNG());
            AssetDatabase.Refresh();
            AssetDatabase.AddObjectToAsset(sp, path);
            AssetDatabase.SaveAssets();

            var ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (null == ti)
            {
                return null;
            }

            ti.spritePixelsPerUnit = sp.pixelsPerUnit;
            ti.mipmapEnabled = false;
            EditorUtility.SetDirty(ti);
            ti.SaveAndReimport();

            return AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
        }

        [MenuItem("Tools/Image/Serialize Dynamic Atlas", false, 100)]
        public static void Serialize()
        {
            var atlasTexture = new Texture2D(DynamicSpriteService.TextureWidth, DynamicSpriteService.TextureHeight,
                TextureFormat.RGBA32, false);
            var colors = atlasTexture.GetPixels32();
            for (var i = 0; i < colors.Length; i++)
            {
                colors[i].a = 0;
            }

            atlasTexture.SetPixels32(colors);

            var bytes = atlasTexture.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/Resources/" + DynamicSpriteService.TexturePath + ".png", bytes);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            atlasTexture = Resources.Load<Texture2D>(DynamicSpriteService.TexturePath);
            var padding = DynamicSpriteService.TexturePadding;

            for (var i = 0; i < DynamicSpriteService.SpriteBlocks.Length; i++)
            {
                var block = DynamicSpriteService.SpriteBlocks[i];
                DoSerialize(block.rect, (int) block.matchRange.x, block.holderPath, atlasTexture, padding);
            }
        }

        private static void ConvertSingleIcon(Texture2D srcTexture)
        {
            if (srcTexture.width != srcTexture.height)
            {
                return;
            }

            if (srcTexture.width > 144 || srcTexture.width < 50)
            {
                return;
            }

            var assetPath = AssetDatabase.GetAssetPath(srcTexture);
            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (null == textureImporter || !textureImporter.isReadable)
            {
                return;
            }

            var systemFilePath = Application.dataPath + "/../" + assetPath;
            File.SetAttributes(systemFilePath, FileAttributes.Normal);

            var targetSize = 128;
            if (srcTexture.width <= 96)
            {
                targetSize = 64;
            }

            var targetTexture = Scaled(srcTexture, targetSize, targetSize);

            var bytes = targetTexture.EncodeToPNG();
            File.WriteAllBytes(systemFilePath, bytes);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(assetPath);
        }

        [MenuItem("Tools/Image/Conert 128 Icons", false, 100)]
        public static void ConvertIcons()
        {
            var basePath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(basePath))
            {
                Debug.LogError("Please select the icon folder to convert!");
                return;
            }

            var prefix = "Assets/Resources/";
            var folder = basePath.Substring(prefix.Length);
            var srcTextures = Resources.LoadAll<Texture2D>(folder);
            if (srcTextures.Length <= 0)
            {
                return;
            }

            for (var i = 0; i < srcTextures.Length; i++)
            {
                if (srcTextures[i].GetType() != typeof(Texture2D))
                {
                    continue;
                }

                ConvertSingleIcon(srcTextures[i]);
            }
        }

        private static Texture2D Scaled(Texture2D src, int width, int height, FilterMode mode = FilterMode.Trilinear)
        {
            var texR = new Rect(0, 0, width, height);
            GpuScale(src, width, height, mode);

            var result = new Texture2D(width, height, TextureFormat.ARGB32, true);
            result.Resize(width, height);
            result.ReadPixels(texR, 0, 0, true);
            return result;
        }

        private static void Scale(Texture2D tex, int width, int height, FilterMode mode = FilterMode.Trilinear)
        {
            var texR = new Rect(0, 0, width, height);
            GpuScale(tex, width, height, mode);

            tex.Resize(width, height);
            tex.ReadPixels(texR, 0, 0, true);
            tex.Apply(true);
        }

        private static void GpuScale(Texture2D src, int width, int height, FilterMode fmode)
        {
            src.filterMode = fmode;
            src.Apply(true);

            var rtt = new RenderTexture(width, height, 32);

            Graphics.SetRenderTarget(rtt);

            GL.LoadPixelMatrix(0, 1, 1, 0);

            GL.Clear(true, true, new Color(0, 0, 0, 0));
            Graphics.DrawTexture(new Rect(0, 0, 1, 1), src);
        }
    }
}