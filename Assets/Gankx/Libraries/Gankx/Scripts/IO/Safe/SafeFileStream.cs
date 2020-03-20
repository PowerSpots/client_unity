using System;
using System.IO;
using UnityEngine;

namespace Gankx.IO
{
    public class SafeFileStream
    {
        public static SafeStream Open(string path, FileMode mode)
        {
            return Open(path, mode, mode == FileMode.Append ? FileAccess.Write : FileAccess.ReadWrite, FileShare.None);
        }

        public static SafeStream Open(string path, FileMode mode, FileAccess access, FileShare share = FileShare.None)
        {
            try
            {
                var fs = File.Open(path, mode, access, share);
                return new SafeStream(fs);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        }

        public static SafeStream OpenRead(string path)
        {
            return Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public static SafeStream OpenWrite(string path)
        {
            return Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        }

        public static SafeStream OpenEdit(string path)
        {
            return Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }
    }
}