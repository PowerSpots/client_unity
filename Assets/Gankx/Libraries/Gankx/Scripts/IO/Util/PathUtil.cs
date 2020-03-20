using System.Collections.Generic;
using System.Text;

namespace Gankx.IO
{
    public class PathUtil 
    {
        public static string NormalizePath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return "";
            }

            StringBuilder str = new StringBuilder();
            int length = fileName.Length;
            for (int iChar = 0; iChar < length; ++iChar)
            {
                char c = fileName[iChar];
                if (c == '\\')
                {
                    c = '/';
                }
                if (c == '/')
                {
                    if (str.Length == 0 || (str[str.Length - 1] != '/'))
                    {
                        str.Append(c);
                    }
                }
                else
                {
                    str.Append(char.ToLower(c));
                }
            }

            str.Replace("/./", "/");
            return str.ToString();
        }

        public static void UniquePaths(List<string> paths)
        {
            paths.Sort();

            int last = paths.Count;
            int first = 0;
            for (int firstb; (firstb = first) != last && ++first != last; )
            {
                if (paths[firstb] == paths[first])
                {	
                    for (; ++first != last; )
                    {
                        if (paths[firstb] != paths[first])
                        {
                            paths[++firstb] = paths[first];
                        }
                    }
                    ++firstb;
                    paths.RemoveRange(firstb, last - firstb);
                    return;
                }
            }
        }
    }
}