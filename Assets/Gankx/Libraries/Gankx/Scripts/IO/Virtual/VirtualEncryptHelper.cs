namespace Gankx.IO
{
    public class VirtualEncryptHelper
    {
        public static void LeftShiftByte(byte[] szBuffer, int iOffset, int iSize, int iCount)
        {
            for (var uiIndex = 0; uiIndex < iCount; uiIndex++)
            {
                LeftShiftByte(szBuffer, iOffset, iSize);
            }
        }

        public static void LeftShiftByte(byte[] szBuffer, int iOffset, int iSize)
        {
            if (iSize == 0)
            {
                return;
            }

            if (szBuffer == null)
            {
                return;
            }

            var bFirst = (byte) ((szBuffer[iOffset] >> 7) & 0x01);

            for (uint uiIndex = 1; uiIndex < iSize; uiIndex++)
            {
                szBuffer[iOffset + uiIndex - 1] =
                    (byte) (((szBuffer[iOffset + uiIndex - 1] << 1) & 0xfe) |
                            ((szBuffer[iOffset + uiIndex] >> 7) & 0x01));
            }

            szBuffer[iOffset + iSize - 1] = (byte) (((szBuffer[iOffset + iSize - 1] << 1) & 0xfe) | bFirst);
        }

        public static void RightShiftByte(byte[] szBuffer, int iOffset, int iSize, int iCount)
        {
            for (uint uiIndex = 0; uiIndex < iCount; uiIndex++)
            {
                RightShiftByte(szBuffer, iOffset, iSize);
            }
        }

        public static void RightShiftByte(byte[] szBuffer, int iOffset, int iSize)
        {
            if (iSize == 0)
            {
                return;
            }

            if (szBuffer == null)
            {
                return;
            }

            // 取最后一位
            var bLast = (byte) ((szBuffer[iOffset + iSize - 1] << 7) & 0x80);

            for (var iIndex = iSize - 1; iIndex > 0; iIndex--)
            {
                szBuffer[iOffset + iIndex] = (byte) (((szBuffer[iOffset + iIndex - 1] << 7) & 0x80) |
                                                     ((szBuffer[iOffset + iIndex] >> 1) & 0x7f));
            }

            szBuffer[iOffset] = (byte) (bLast | ((szBuffer[iOffset] >> 1) & 0x7f));
        }

        public static void XorByte(byte[] szBuffer, int iOffset, int iSize, byte bXOR)
        {
            if (iSize == 0)
            {
                return;
            }

            if (szBuffer == null)
            {
                return;
            }

            for (var i = 0; i < iSize; ++i)
            {
                szBuffer[iOffset + i] ^= bXOR;
            }
        }
    }
}