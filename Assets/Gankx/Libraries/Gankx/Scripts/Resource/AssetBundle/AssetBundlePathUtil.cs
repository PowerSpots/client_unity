using UnityEngine;

namespace Gankx
{
    public class AssetBundlePathUtil
    {
        public static string GetBundleNameInRuntime(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                Debug.LogError("AssetBundlePathUtil.GetBundleNameInRuntime: Invalid parameter!");
                return null;
            }

            resourcePath = resourcePath.ToLower();
            var bundleName = resourcePath.Replace("/", "_");
            return bundleName + AssetBundleConfig.ConstAssetTail;
        }

        public static string GetBundleName(string srcPath)
        {
            if (string.IsNullOrEmpty(srcPath))
            {
                Debug.LogError("AssetBundlePathUtil.GetBundleName: Invalid parameter!");
                return null;
            }

            srcPath = ToAssetPath(srcPath);
            srcPath = srcPath.ToLower();
            var bundleName = srcPath.Replace("/", "_");

            var dotIndex = bundleName.LastIndexOf(".");
            if (dotIndex >= 0)
            {
                bundleName = bundleName.Substring(0, dotIndex);

                if (srcPath.StartsWith("artsrc"))
                {
                    var typeName = srcPath.Substring(dotIndex + 1);
                    bundleName += "_" + typeName;
                }
            }

            if (bundleName.StartsWith("artsrc_"))
            {
                bundleName = bundleName.Remove(0, "artsrc_".Length);
            }

            if (bundleName.StartsWith("resources_"))
            {
                bundleName = bundleName.Remove(0, "resources_".Length);
            }

            return bundleName + AssetBundleConfig.ConstAssetTail;
        }

        public static string Normarlize(string s)
        {
            return s.Replace("\\", "/");
        }

        public static string ToFullPath(string srcPath)
        {
            if (string.IsNullOrEmpty(srcPath))
            {
                return string.Empty;
            }

            if (srcPath.Contains(Application.dataPath))
            {
                return srcPath;
            }

            if (srcPath.Contains("Assets"))
            {
                srcPath = srcPath.Substring("Assets".Length + 1);
            }

            srcPath = Application.dataPath + "/" + srcPath;
            return Normarlize(srcPath);
        }

        public static string ToAssetPath(string fullPath)
        {
            var path = ToProjectPath(fullPath);
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            if (path.Contains("Assets"))
            {
                path = path.Substring("Assets".Length + 1);
            }

            return Normarlize(path);
        }

        public static string ToProjectPath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return string.Empty;
            }

            if (fullPath.Contains(Application.dataPath))
            {
                fullPath = fullPath.Substring(Application.dataPath.Length);
                fullPath = "Assets" + fullPath;
            }

            return Normarlize(fullPath);
        }

        public static string ToResourcePath(string fullPath)
        {
            var path = ToAssetPath(fullPath);
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            if (path.Contains("Resources"))
            {
                path = path.Substring("Resources".Length + 1);
            }

            return Normarlize(path);
        }

        public static void ResetShaderInEditor<T>(T res)
        {
            var go = res as GameObject;

            if (go != null)
            {
                var renders = go.GetComponentsInChildren<Renderer>();
                for (var i = 0; i < renders.Length; i++)
                {
                    var materials = renders[i].sharedMaterials;
                    for (var j = 0; j < materials.Length; j++)
                    {
                        if (materials[j] != null && materials[j].shader != null)
                        {
                            materials[j].shader = Shader.Find(materials[j].shader.name);
                        }
                    }
                }
            }
        }
    }
}