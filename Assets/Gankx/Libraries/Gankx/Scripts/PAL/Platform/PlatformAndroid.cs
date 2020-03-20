using System.IO;
using UnityEngine;

namespace Gankx.PAL
{
    internal class PlatformAndroid : Platform
    {
        private static readonly string MyStreamingAssets = Application.streamingAssetsPath + "/data/";
        private static readonly string MyDataRoot = Application.persistentDataPath + "/data/";

        public override string dataRoot
        {
            get { return MyDataRoot; }
        }

        public override void Init()
        {
            Debug.Log("PlatformAndroid.Init...");
        }

        public override void Release()
        {
        }

        public override string GetPath(string relativePath)
        {
            var standardPath = StandardlizePath(relativePath);
            var fullPath = string.Format("{0}{1}", dataRoot, standardPath);
            if (fullPath.StartsWith("jar:file://"))
            {
                fullPath = fullPath.Substring(4);
            }

            return fullPath;
        }

        public override string GetBundlePath(string relativePath)
        {
            var standardPath = StandardlizePath(relativePath);
            var fullPath = string.Format("{0}{1}", dataRoot, standardPath);
            if (!File.Exists(fullPath))
            {
                fullPath = string.Format("{0}{1}", MyStreamingAssets, standardPath);
            }

            return fullPath;
        }

        public override string GetBundleUrl(string relativePath)
        {
            var fullPath = string.Format("{0}{1}", dataRoot, StandardlizePath(relativePath));
            if (!File.Exists(fullPath))
            {
                fullPath = string.Format("{0}{1}", MyStreamingAssets, StandardlizePath(relativePath));
            }

            return fullPath;
        }

        public override string GetWritePath(string relativePath)
        {
            return GetPath(relativePath);
        }
    }
}