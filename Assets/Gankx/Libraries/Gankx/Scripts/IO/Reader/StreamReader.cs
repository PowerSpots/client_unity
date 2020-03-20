using System;

namespace Gankx.IO
{
    public abstract class StreamReader : IDisposable
    {
        public void Dispose()
        {
            Close();
        }

        /// Return false after we have tried to read past the end of file or some other error.
        /// Otherwise return true.
        public abstract bool IsOk();

        /// Read 'nbytes' into the buffer 'buf' with 'offset' in the buffer.
        /// Return the number of characters read or zero for end of file.
        public abstract int Read(byte[] buf, int bufOffset, int nbytes);

        /// Skip nbytes bytes of input.
        /// Return the actual number of bytes skipped.
        /// The default implementation uses read().
        public virtual int Skip(int nbytes)
        {
            var buf = new byte[512];
            var bytesLeft = nbytes;
            while (bytesLeft > 0)
            {
                var n = Read(buf, 0, bytesLeft > buf.Length ? buf.Length : bytesLeft);
                if (n == 0)
                {
                    break;
                }

                bytesLeft -= n;
            }

            return nbytes - bytesLeft;
        }

        /// Read at most n characters without changing the stream position.
        /// Returns the number of characters peeked.
        public virtual int Peek(byte[] buf, int n)
        {
            return 0;
        }

        /// Return a seekable stream if seeking is supported on this stream. By default not supported.
        public virtual SeekableStreamReader IsSeekTellSupported()
        {
            return null;
        }

        protected virtual void Close()
        {
        }
    }

    public abstract class SeekableStreamReader : StreamReader
    {
        /// Parameter for seek method.
        public enum SeekWhence
        {
            StreamSet = 0,
            StreamCurrent = 1,
            StreamEnd = 2
        }

        /// When overridden in a derived class, gets the length in bytes of the stream.
        public abstract long length { get; }

        /// Return a seekable stream if seeking is supported on this stream. By default not supported.
        public override SeekableStreamReader IsSeekTellSupported()
        {
            return this;
        }

        /// Seek to offset from whence.
        public abstract bool Seek(long offset, SeekWhence whence);

        /// Get the current file offset if supported or -1 on error.
        public abstract long Tell();
    }
}