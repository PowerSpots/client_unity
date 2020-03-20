using System;
using Gankx.IO;

namespace Gankx
{
    public class LuaLoader
    {
        public static int Load(string fileName, out byte[] buf)
        {
            buf = null;

            try
            {
                using (var sr = FileService.system.OpenReader(fileName))
                {
                    if (null == sr)
                    {
                        return 0;
                    }

                    var ssr = sr.IsSeekTellSupported();
                    if (null == ssr)
                    {
                        return 0;
                    }

                    buf = new byte[ssr.length];
                    return sr.Read(buf, 0, buf.Length);
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}