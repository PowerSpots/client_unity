using System;
using System.IO;
using SevenZip;
using SevenZip.Compression.LZMA;
using UnityEngine;

namespace Gankx
{
    internal class CodeCallback : ICodeProgress
    {
        public long inSize { get; set; }
        public long outSize { get; set; }

        public void SetProgress(long inSizeValue, long outSizeValue)
        {
            inSize = inSizeValue;
            outSize = outSizeValue;
        }
    }

    public class ZipService
    {
        public static uint CalculateDigest(byte[] data, uint offset, uint size)
        {
            return CRC.CalculateDigest(data, offset, size);
        }

        public static bool VerifyDigest(uint digest, byte[] data, uint offset, uint size)
        {
            return CRC.VerifyDigest(digest, data, offset, size);
        }

        public static ZipResult Compress(byte[] inBuffer, int inOffset, int inSize, byte[] outBuffer, int outOffset,
            ref int outSize)
        {
            try
            {
                var encoder = new Encoder();
                var inStream = new MemoryStream(inBuffer, inOffset, inSize);
                var outStream = new MemoryStream(outBuffer, outOffset, outSize);

                var codeCallback = new CodeCallback();

                encoder.Code(inStream, outStream, inSize, outSize, codeCallback);
                if (codeCallback.inSize == inSize)
                {
                    outSize = (int) codeCallback.outSize;
                    return ZipResult.Z_OK;
                }
            }
            catch (DataErrorException)
            {
                return ZipResult.Z_DATA_ERROR;
            }
            catch (Exception)
            {
                return ZipResult.Z_ERROR;
            }

            return ZipResult.Z_ERROR;
        }

        public static ZipResult Decompress(byte[] inBuffer, int inOffset, int inSize, byte[] outBuffer, int outOffset,
            ref int outSize)
        {
            try
            {
                var decoder = new Decoder();
                var inStream = new MemoryStream(inBuffer, inOffset, inOffset);
                var outStream = new MemoryStream(outBuffer, outOffset, outSize);

                var codeCallback = new CodeCallback();
                decoder.Code(inStream, outStream, inSize, outSize, codeCallback);
                if (codeCallback.inSize == inSize)
                {
                    outSize = (int) codeCallback.outSize;
                    return ZipResult.Z_OK;
                }
            }
            catch (DataErrorException)
            {
                return ZipResult.Z_DATA_ERROR;
            }
            catch (Exception)
            {
                return ZipResult.Z_ERROR;
            }

            return ZipResult.Z_ERROR;
        }

        public static void Benchmark(int numInterations, uint dictionarySize)
        {
            Debug.LogError("Benchmark Start: " + numInterations + ", " + dictionarySize);
            LzmaBench.LzmaBenchmark(numInterations, dictionarySize);
            Debug.LogError("Benchmark Stop");
        }
    }
}