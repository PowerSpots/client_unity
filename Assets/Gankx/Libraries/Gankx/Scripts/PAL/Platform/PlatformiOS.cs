using System.IO;
using UnityEngine;

namespace Gankx.PAL
{
    internal class PlatformiOS : Platform
    {
        private static readonly string MyDataRoot = Application.persistentDataPath + "/data/";
        private static readonly string MyStreamingAssets = Application.streamingAssetsPath + "/data/";

        public override string dataRoot
        {
            get { return MyDataRoot; }
        }

        public override void Init()
        {
            Debug.Log("PlatformiOS.Init...");
        }

        public override void Release()
        {
        }

        public override string GetPath(string relativePath)
        {
            var standardPath = StandardlizePath(relativePath);
            var fullPath = string.Format("{0}{1}", dataRoot, standardPath);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }

            fullPath = string.Format("{0}/../data/{1}", Application.dataPath, standardPath);
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
            var standardPath = StandardlizePath(relativePath);
            var fullPath = string.Format("{0}{1}", dataRoot, standardPath);
            if (!File.Exists(fullPath))
            {
                fullPath = string.Format("{0}{1}", MyStreamingAssets, standardPath);
            }

            return string.Format("file://{0}", fullPath);
        }

        public override string GetWritePath(string relativePath)
        {
            return string.Format("{0}{1}", dataRoot, StandardlizePath(relativePath));
        }
    }
}