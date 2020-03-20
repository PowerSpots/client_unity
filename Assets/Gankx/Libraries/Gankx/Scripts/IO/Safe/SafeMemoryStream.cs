using System.IO;

namespace Gankx.IO
{
    public class SafeMemoryStream
    {
        private readonly FileStream myMemoryStream;

        public static SafeStream Open(MemoryStream memoryStream)
        {
            if (null == memoryStream)
            {
                return null;
            }

            return new SafeStream(memoryStream);
        }

        public static SafeStream Open(byte[] buffer)
        {
            if (null == buffer)
            {
                return null;
            }

            return Open(new MemoryStream(buffer, false));
        }
    }
}