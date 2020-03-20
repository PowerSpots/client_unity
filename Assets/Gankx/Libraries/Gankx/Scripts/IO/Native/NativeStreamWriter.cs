using System.IO;

namespace Gankx.IO
{
    public class NativeStreamWriter : StreamWriter
    {
        private SafeStream myHandle;

        public NativeStreamWriter(SafeStream handle)
        {
            myHandle = handle;
        }

        public override long length
        {
            get
            {
                if (null == myHandle)
                {
                    return -1;
                }

                return myHandle.length;
            }
        }

        public static NativeStreamWriter Open(string nameIn, FileSystem.OpenFlags openFlags)
        {
            var nfs = SafeFileStream.Open(
                nameIn,
                (openFlags & FileSystem.OpenFlags.Truncate) != 0
                    ? FileMode.Create
                    : FileMode.OpenOrCreate,
                FileAccess.Write, FileShare.None);

            if (null == nfs)
            {
                return null;
            }

            return new NativeStreamWriter(nfs);
        }

        public override bool IsOk()
        {
            return myHandle != null;
        }

        public override int Write(byte[] buf, int nbytes)
        {
            if (null == myHandle || nbytes <= 0)
            {
                return 0;
            }

            if (!myHandle.Write(buf, 0, nbytes))
            {
                return 0;
            }

            return nbytes;
        }

        public override void Flush()
        {
            if (null == myHandle)
            {
                return;
            }

            myHandle.Flush();
        }

        public override bool SeekTellSupported()
        {
            return true;
        }

        public override bool Seek(long offset, SeekWhence whence)
        {
            if (null == myHandle)
            {
                return false;
            }

            return myHandle.Seek(offset, (SeekOrigin) whence);
        }

        public override long Tell()
        {
            if (null == myHandle)
            {
                return -1;
            }

            return myHandle.Tell();
        }

        public override void Close()
        {
            if (myHandle != null)
            {
                myHandle.Close();
                myHandle = null;
            }
        }
    }
}