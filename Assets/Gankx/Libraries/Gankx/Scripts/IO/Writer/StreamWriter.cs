using System;

namespace Gankx.IO
{
    public abstract class StreamWriter : IDisposable
    {
        /// Return false if end of file has been reached or some other error.
        /// Otherwise return true.
        public abstract bool IsOk();

        /// Write nbytes. Returns the number of bytes written.
        /// The number of bytes returned may be less than nbytes
        /// (in the case of nonblocking sockets for instance).
        public abstract int Write(byte[] buf, int nbytes);

        /// Flush any internal buffers.
        public virtual void Flush()
        {
        }

        /// Return true if seeking is supported on this stream. By default not supported.
        public virtual bool SeekTellSupported()
        {
            return false;
        }

        /// Parameter for seek method.
        public enum SeekWhence
        {
            StreamSet = 0,
            StreamCurrent = 1,
            StreamEnd = 2
        };

        /// Seek to offset from whence.
        public virtual bool Seek(long offset, SeekWhence whence)
        {
            return false;
        }

        /// Get the current file offset if supported or -1 on error.
        /// Default implementation calls seek(0, StreamCurrent);
        public virtual long Tell()
        {
            return -1;
        }

        /// When overridden in a derived class, gets the length in bytes of the stream.
        public virtual long length
        {
            get { return -1; }
        }

        public virtual void Close()
        {
        }

        /// Release all resource
        public void Dispose()
        {
            Close();
        }
    }
}