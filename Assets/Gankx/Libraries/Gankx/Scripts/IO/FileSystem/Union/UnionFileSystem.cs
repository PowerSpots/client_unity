using System;
using System.Collections.Generic;
using System.Text;

namespace Gankx.IO
{
    public class UnionFileSystem : FileSystem
    {
        protected class MountEntry
        {
            public FileSystem fs;
            public string srcPath;
            public string dstPath;
            public bool writable;

            public bool ConvertIn2Out(string pathIn, StringBuilder pathOut)
            {
                pathOut.Length = 0;
                pathOut.Append(pathIn);

                if (pathIn.StartsWith(srcPath, StringComparison.Ordinal))
                {
                    pathOut.Remove(0, srcPath.Length);
                    pathOut.Insert(0, dstPath);
                    return true;
                }

                return false;
            }

            public bool ConvertOut2In(string pathOut, StringBuilder pathIn)
            {
                pathIn.Length = 0;
                pathIn.Append(pathOut);

                if (pathOut.StartsWith(dstPath, StringComparison.Ordinal))
                {
                    pathIn.Remove(0, dstPath.Length);
                    pathIn.Insert(0, srcPath);
                    return true;
                }

                return false;
            }
        }

        private struct MountIterator
        {
            private readonly List<MountEntry> myMounts;
            private int myIdx;
            private readonly string myPathIn;
            private readonly StringBuilder myPathOut;

            public MountIterator(List<MountEntry> mounts, string pathIn)
            {
                myMounts = mounts;
                myPathIn = PathUtil.NormalizePath(pathIn);
                myIdx = mounts.Count;
                myPathOut = new StringBuilder();

                if (!string.IsNullOrEmpty(myPathIn))
                {
                    while (pathIn.StartsWith("./"))
                    {
                        myPathIn = myPathIn.Substring(2);
                    }
                }
            }

            public bool Advance()
            {
                while (myIdx > 0)
                {
                    myIdx -= 1;

                    var m = myMounts[myIdx];

                    if (m.ConvertIn2Out(myPathIn, myPathOut))
                    {
                        return true;
                    }
                }

                return false;
            }

            public MountEntry Current()
            {
                return myMounts[myIdx];
            }

            public string Path()
            {
                return myPathOut.ToString();
            }
        }

        protected readonly List<MountEntry> myMounts = new List<MountEntry>();

        public override StreamReader OpenReader(string name, OpenFlags flags)
        {
            var it = new MountIterator(myMounts, name);
            while (it.Advance())
            {
                var m = it.Current();

                var r = m.fs.OpenReader(it.Path(), flags);
                if (r != null)
                {
                    return r;
                }
            }

            return null;
        }

        public override StreamWriter OpenWriter(string name, OpenFlags flags)
        {
            var it = new MountIterator(myMounts, name);

            while (it.Advance())
            {
                var m = it.Current();
                if (m.writable)
                {
                    var w = m.fs.OpenWriter(it.Path(), flags);
                    if (w != null)
                    {
                        return w;
                    }
                }
            }

            return null;
        }

        public override Result Remove(string path)
        {
            var it = new MountIterator(myMounts, path);

            while (it.Advance())
            {
                var m = it.Current();
                if (m.writable)
                {
                    if (m.fs.Remove(path) == Result.Ok)
                    {
                        return Result.Ok;
                    }
                }
            }

            return Result.Error;
        }

        public override Result Mkdir(string path)
        {
            var it = new MountIterator(myMounts, path);

            while (it.Advance())
            {
                var m = it.Current();
                if (m.writable)
                {
                    if (m.fs.Mkdir(path) == Result.Ok)
                    {
                        return Result.Ok;
                    }
                }
            }

            return Result.Error;
        }

        public override Result Stat(string path, out Entry entryOut)
        {
            entryOut = null;

            var it = new MountIterator(myMounts, path);

            while (it.Advance())
            {
                var m = it.Current();

                if (m.fs.Stat(it.Path(), out entryOut) == Result.Ok)
                {
                    var pathIn = new StringBuilder();
                    m.ConvertOut2In(entryOut.GetPath(), pathIn);
                    entryOut.SetPath(this, pathIn.ToString());

                    return Result.Ok;
                }
            }

            return Result.Error;
        }

        public override void GetFiles(List<string> fileNames, string path, string searchPattern, bool recursive)
        {
            var it = new MountIterator(myMounts, path);

            while (it.Advance())
            {
                var m = it.Current();
                m.fs.GetFiles(fileNames, it.Path(), searchPattern, recursive);
            }

            PathUtil.UniquePaths(fileNames);
        }

        public void Mount(FileSystem fileSystem, string srcPath, string dstPath, bool writable)
        {
            var newMountEntry = new MountEntry();
            newMountEntry.fs = fileSystem;
            newMountEntry.srcPath = PathUtil.NormalizePath(srcPath);
            newMountEntry.dstPath = PathUtil.NormalizePath(dstPath);
            newMountEntry.writable = writable;

            myMounts.Add(newMountEntry);
        }

        public override void Close()
        {
            for (var index = 0; index < myMounts.Count; ++index)
            {
                var mountEntry = myMounts[index];
                mountEntry.fs.Close();
                mountEntry.fs = null;
            }

            myMounts.Clear();
        }
    }
}