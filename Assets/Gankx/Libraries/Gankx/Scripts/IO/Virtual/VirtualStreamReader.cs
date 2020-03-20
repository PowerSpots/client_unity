namespace Gankx.IO
{
    public class VirtualStreamReader : SeekableStreamReader
    {
        private readonly VirtualFileSystem myFileSystem;
        private int myFileIndex;
        private int myOffset;
        private readonly int myFileSize;
        private readonly bool myCompressed;
        private readonly VirtualFileSystem.BlockBuffer myBlockBuffer;
        private bool myIsOk;

        public VirtualStreamReader(VirtualFileSystem fileSystem, int index)
        {
            myIsOk = index >= 0;
            myFileSystem = fileSystem;
            myFileIndex = index;
            myOffset = 0;

            if (myFileIndex >= 0)
            {
                var fileEntry = fileSystem.GetFileEntry(index);
                myFileSize = fileEntry.expandedSize;
                myCompressed = (fileEntry.flags & VirtualFileSystem.FileFlag.Compressed) != 0;
            }
            else
            {
                myFileSize = 0;
                myCompressed = false;
            }

            myBlockBuffer = new VirtualFileSystem.BlockBuffer();
            myBlockBuffer.index = -1;
            myBlockBuffer.compressedBuffer = null;
            myBlockBuffer.expandedBuffer = null;
        }

        public override long length
        {
            get
            {
                if (myFileIndex < 0)
                {
                    return -1;
                }

                var fileEntry = myFileSystem.GetFileEntry(myFileIndex);
                return fileEntry.expandedSize;
            }
        }

        public override bool IsOk()
        {
            return myIsOk;
        }

        public override int Read(byte[] buf, int bufOffset, int nbytes)
        {
            if (myFileIndex < 0)
            {
                return 0;
            }

            // Simply return NULL for 0 length reads
            if (nbytes == 0)
            {
                return 0;
            }

            if (buf == null)
            {
                return 0;
            }

            // Check the size of the lock to make sure it does not extend beyond the end of the file
            if (myOffset + nbytes > myFileSize)
            {
                nbytes = myFileSize - myOffset;
            }

            // Is eof
            if (nbytes <= 0)
            {
                myIsOk = false;
                return 0;
            }

            if (myCompressed && myBlockBuffer.compressedBuffer == null && myBlockBuffer.expandedBuffer == null)
            {
                myBlockBuffer.index = -1;
                myBlockBuffer.compressedBuffer = new byte[myFileSystem.BlockSize()];
                myBlockBuffer.expandedBuffer = new byte[myFileSystem.BlockSize()];
            }

            // Read the file data
            if (!myFileSystem.ReadFromFile(myFileIndex, buf, bufOffset, myOffset, nbytes, myBlockBuffer))
            {
                return 0;
            }

            myOffset += nbytes;
            return nbytes;
        }

        public override bool Seek(long offset, SeekWhence whence)
        {
            if (myFileIndex < 0)
            {
                return false;
            }

            var intOffset = (int) offset;

            switch (whence)
            {
                case SeekWhence.StreamSet:
                    myOffset = intOffset;
                    break;
                case SeekWhence.StreamCurrent:
                    myOffset += intOffset;
                    break;
                case SeekWhence.StreamEnd:
                    myOffset = myFileSize - intOffset;
                    break;
            }

            // Eof
            if (myOffset >= 0 && myOffset < myFileSize)
            {
                myIsOk = true;
                return true;
            }

            myIsOk = false;
            return false;
        }

        public override long Tell()
        {
            if (myFileIndex < 0)
            {
                return -1;
            }

            return myOffset;
        }

        protected override void Close()
        {
            if (myCompressed)
            {
                myBlockBuffer.index = -1;
                myBlockBuffer.expandedBuffer = null;
                myBlockBuffer.compressedBuffer = null;
            }

            // File has been closed need to reopen
            myFileIndex = -1;
            myIsOk = false;
        }
    }
}