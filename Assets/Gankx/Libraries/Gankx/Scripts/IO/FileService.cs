using System.IO;
using UnityEngine;

namespace Gankx.IO
{
    public class FileService : Singleton<FileService>
    {
        private UnionFileSystem mySystem;

        public static FileSystem system
        {
            get
            {
                if (null == instance)
                {
                    return null;
                }

                return instance.mySystem;
            }
        }

        public static string applicationPath
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                return Application.dataPath + "/../";
#else
                return Application.persistentDataPath + "/";
#endif
            }
        }

        public static string dataPath
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                return Application.dataPath + "/../data/";
#else
                return Application.persistentDataPath + "/data/";
#endif
            }
        }

        protected void MountVirtual(string vfsFile, string srcPath, string dstPath)
        {
            // Mount the vfs
            VirtualFileSystem vfsData = new VirtualFileSystem();
            string rootFilePath = Path.Combine(dataPath, vfsFile);
            if (File.Exists(rootFilePath)) {
                if (vfsData.Open(rootFilePath, VirtualFileSystem.FileMode.Read)) {
                    mySystem.Mount(vfsData, srcPath, dstPath, false);
                }

                return;
            }

            string streamFilePath = Path.Combine("data", vfsFile);
            if (vfsData.Open(FileLoaderHelper.LoadFromStreamAssets(streamFilePath), rootFilePath)) {
                mySystem.Mount(vfsData, srcPath, dstPath, false);
            }
        }

        protected void MountNative(string srcPath, string dstPath, bool writable)
        {
            // Mount the native fs
            NativeFileSystem nativeData = new NativeFileSystem();
            mySystem.Mount(nativeData, srcPath, dstPath, writable);
        }

        protected override void OnInit()
        {
            mySystem = new UnionFileSystem();

#if UNITY_EDITOR || UNITY_STANDALONE
            // Editor mode using the native file system to read or write
            MountNative("", dataPath, true);
#else
            // Mount the script vfs
            MountVirtual("data1.bytes", "scripts/", "./");            
            // MountVirtual("data2.bytes", "audio/", "./");
            MountVirtual("data3.bytes", "scripts/resource/", "./");
            MountVirtual("data4.bytes", "scripts/libraries/", "./");
#endif
        }

        protected override void OnRelease()
        {
            if (mySystem != null)
            {
                mySystem.Close();
                mySystem = null;
            }
        }

        public void Init()
        {
            OnInit();
        }
    }
}