using UnityEngine;

namespace Gankx.PAL
{
    internal class PlatformEditor : Platform
    {
        private static readonly string MyDataRootWin32 = Application.dataPath + "/../../Build/Win32/data/";

        public override string dataRoot
        {
            get { return MyDataRootWin32; }
        }

        public override void Init()
        {
        }

        public override void Release()
        {
        }

        public override string GetPath(string relativePath)
        {
            return string.Format("{0}{1}", dataRoot, StandardlizePath(relativePath));
        }

        public override string GetBundlePath(string relativePath)
        {
            return string.Format("{0}{1}", dataRoot, StandardlizePath(relativePath));
        }

        public override string GetBundleUrl(string relativePath)
        {
            return string.Format("file://{0}{1}", dataRoot, StandardlizePath(relativePath));
        }

        public override string GetWritePath(string relativePath)
        {
            return GetPath(relativePath);
        }
    }
}