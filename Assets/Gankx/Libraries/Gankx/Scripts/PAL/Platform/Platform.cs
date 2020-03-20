using Gankx.IO;
using Core;
using UnityEngine;

namespace Gankx.PAL
{
    internal abstract class Platform
    {
        private static Platform MyInstance;

        public static Platform instance
        {
            get
            {
                if (MyInstance == null)
                {
                    CreateInstance();
                }

                return MyInstance;
            }
        }

        public abstract string dataRoot { get; }

        public bool isAndroid
        {
            get { return Application.platform == RuntimePlatform.Android; }
        }

        public bool isIos
        {
            get { return Application.platform == RuntimePlatform.IPhonePlayer; }
        }

        public bool isEditor
        {
            get
            {
                return Application.platform == RuntimePlatform.WindowsEditor ||
                       Application.platform == RuntimePlatform.OSXEditor;
            }
        }

        public static void CreateInstance()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                {
                    MyInstance = new PlatformEditor();
                }
                    break;
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.OSXPlayer:
                {
                    MyInstance = new PlatformWin();
                }
                    break;
                case RuntimePlatform.Android:
                {
                    MyInstance = new PlatformAndroid();
                }
                    break;
                case RuntimePlatform.IPhonePlayer:
                {
                    MyInstance = new PlatformiOS();
                }
                    break;
            }
        }

        public abstract void Init();
        public abstract void Release();
        public abstract string GetPath(string relativePath);
        public abstract string GetBundleUrl(string relativePath);
        public abstract string GetBundlePath(string relativePath);
        public abstract string GetWritePath(string relativePath);

        protected string StandardlizePath(string path)
        {
            return PathUtil.NormalizePath(path);
        }
    }
}