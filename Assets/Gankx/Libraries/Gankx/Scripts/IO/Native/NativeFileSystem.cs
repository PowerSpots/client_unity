using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gankx.IO
{
    public class NativeFileSystem : FileSystem
    {
        public override StreamReader OpenReader(string name, OpenFlags flags = OpenFlags.Buffered)
        {
            return NativeStreamReader.Open(name);
        }

        public override StreamWriter OpenWriter(string name, OpenFlags flags = OpenFlags.DefaultWrite)
        {
            return NativeStreamWriter.Open(name, flags);
        }

        public override Result Remove(string path)
        {
            try
            {
                File.Delete(path);
                return Result.Ok;
            }
            catch (Exception)
            {
                return Result.Error;
            }
        }

        public override Result Mkdir(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
                return Result.Ok;
            }
            catch (Exception)
            {
                return Result.Error;
            }
        }

        public override Result Stat(string path, out Entry entryOut)
        {
            entryOut = null;

            try
            {
                var fa = File.GetAttributes(path);
                var dt = File.GetLastWriteTimeUtc(path);
                using (var fs = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    entryOut = new Entry();
                    entryOut.SetAll(
                        this,
                        path,
                        (fa & FileAttributes.Directory) == FileAttributes.Directory
                            ? Entry.FlagValues.IsDir
                            : Entry.FlagValues.IsFile,
                        dt,
                        fs.Length
                    );
                }

                return Result.Ok;
            }
            catch (Exception)
            {
                return Result.Error;
            }
        }

        public override void GetFiles(List<string> fileNames, string path, string searchPattern, bool recursive)
        {
            if (null == fileNames || null == path || null == searchPattern)
            {
                return;
            }

            if (path.Last() != '/')
            {
                path += '/';
            }

            try
            {
                var allFiles = Directory.GetFiles(path, searchPattern,
                    recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                for (var index = 0; index < allFiles.Length; ++index)
                {
                    var filePath = allFiles[index];
                    if (filePath.Length > path.Length)
                    {
                        fileNames.Add(PathUtil.NormalizePath(filePath.Substring(path.Length)));
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}