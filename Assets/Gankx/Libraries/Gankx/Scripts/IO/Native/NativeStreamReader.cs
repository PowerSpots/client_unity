using System.IO;

namespace Gankx.IO
{
    public class NativeStreamReader : SeekableStreamReader
    {
        private SafeStream myHandle;
        private bool myIsOk;

        public NativeStreamReader(SafeStream handle)
        {
            myHandle = handle;
            myIsOk = myHandle != null;
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

        public static NativeStreamReader Open(string nameIn)
        {
            var nfs = SafeFileStream.OpenRead(nameIn);
            if (null == nfs)
            {
                return null;
            }

            return new NativeStreamReader(nfs);
        }

        public override bool IsOk()
        {
            return myIsOk;
        }

        public override int Read(byte[] buf, int bufOffset, int nbytes)
        {
            if (null == myHandle)
            {
                return 0;
            }

            var nRead = myHandle.Read(buf, bufOffset, nbytes);
            // Eof
            if (nRead <= 0)
            {
                myIsOk = false;
            }

            return nRead;
        }

        public override int Peek(byte[] buf, int nbytes)
        {
            if (null == myHandle)
            {
                return 0;
            }

            var nRead = myHandle.Read(buf, 0, nbytes);

            if (nRead <= 0 || !myHandle.Seek(-nRead, SeekOrigin.Current))
            {
                myIsOk = false;
            }

            return nRead;
        }

        public override bool Seek(long offset, SeekWhence whence)
        {
            if (null == myHandle)
            {
                return false;
            }

            if (myHandle.Seek(offset, (SeekOrigin) whence))
            {
                myIsOk = true;
                return true;
            }

            myIsOk = false;
            return false;
        }

        public override long Tell()
        {
            if (null == myHandle)
            {
                return -1;
            }

            return myHandle.Tell();
        }

        protected override void Close()
        {
            if (myHandle != null)
            {
                myHandle.Close();
                myHandle = null;
            }

            myIsOk = false;
        }
    }
}