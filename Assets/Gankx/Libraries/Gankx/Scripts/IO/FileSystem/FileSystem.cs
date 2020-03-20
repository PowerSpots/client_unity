using System;
using System.Collections.Generic;
using System.IO;

namespace Gankx.IO
{
    public abstract class FileSystem : IDisposable
    {
        /// Represents a file or directory in a filesystem.
        public class Entry
        {
            [Flags]
            public enum FlagValues
            {
                /// Regular file
                IsFile = 1,

                /// Folder type
                IsDir = 2,

                /// Unrecognised type (e.g. link, device)
                IsUnknown = 4
            }

            /// The filesystem this entry belongs to.
            private FileSystem myFs;

            /// Path to the entry including the name.
            private string myPath;

            /// Last modified time.
            private DateTime myMtime;

            /// Number of bytes in this entry or -1 if unknown.
            private long mySize;

            /// Flags (isdir, isfile etc)
            private FlagValues myFlags;

            public Entry()
            {
                myFs = null;
                myMtime = DateTime.MinValue;
            }

            public bool IsDir()
            {
                return (myFlags & FlagValues.IsDir) == FlagValues.IsDir;
            }

            public bool IsFile()
            {
                return (myFlags & FlagValues.IsFile) == FlagValues.IsFile;
            }

            public string GetPath()
            {
                return myPath;
            }

            public string GetName()
            {
                return Path.GetFileName(myPath);
            }

            public DateTime GetMtime()
            {
                return myMtime;
            }

            public long GetSize()
            {
                return mySize;
            }

            public void SetPath(FileSystem fs, string path)
            {
                myFs = fs;
                myPath = path;
            }

            public void SetMtime(DateTime mt)
            {
                myMtime = mt;
            }

            public void SetSize(long s)
            {
                mySize = s;
            }

            public void SetFlags(FlagValues f)
            {
                myFlags = f;
            }

            public void SetAll(FileSystem fs, string fullPath, FlagValues flags, DateTime mt, long sz)
            {
                myFs = fs;
                myPath = fullPath;
                myFlags = flags;
                myMtime = mt;
                mySize = sz;
            }

            public StreamReader OpenReader(OpenFlags flags = OpenFlags.DefaultRead)
            {
                return myFs.OpenReader(myPath, flags);
            }

            public StreamWriter OpenWriter(OpenFlags flags = OpenFlags.DefaultWrite)
            {
                return myFs.OpenWriter(myPath, flags);
            }
        }

        public enum AccessMode
        {
            AccessRead = 1,
            AccessWrite = 2
        }

        public enum Result
        {
            Ok = 0,

            Error,

            NotImplemented
        }

        [Flags]
        public enum OpenFlags
        {
            /// The file is buffered. This makes peek() available and takes care of any platform-specific
            /// alignment and block size requirements.
            Buffered = 1,

            /// Truncate the file if it exists. Without this flag, the content (if any) is not
            /// truncated. In either case, the initial file offset will always be zero, so a seek is
            /// required to append data.
            Truncate = 2,

            /// Default read mode
            DefaultRead = Buffered,

            /// Default write mode
            DefaultWrite = Buffered | Truncate
        }

        /// Release all resources
        public void Dispose()
        {
            Close();
        }

        /// Returns a stream reader for file 'name' or null if unable.
        public abstract StreamReader OpenReader(string name, OpenFlags flags = OpenFlags.DefaultRead);

        /// Returns a stream reader for file 'name' or null if unable.
        public abstract StreamWriter OpenWriter(string name, OpenFlags flags = OpenFlags.DefaultWrite);

        /// Remove the given path.
        public virtual Result Remove(string path)
        {
            return Result.NotImplemented;
        }

        /// Create a folder with the given name.
        public virtual Result Mkdir(string path)
        {
            return Result.NotImplemented;
        }

        /// Look up a single named path, returns true if an entry exists.
        public abstract Result Stat(string path, out Entry entryOut);

        /// Returns the names of files (including their paths) in the specified directory that match the specified extension, using a value to determine whether to search subdirectories.
        /// only simple patterns currently supported (*.xxx)
        public abstract void GetFiles(List<string> fileNames, string path, string searchPattern, bool recursive);

        public List<string> GetFiles(string path, string searchPattern = "*", bool recursive = false)
        {
            var result = new List<string>();
            GetFiles(result, path, searchPattern, recursive);
            return result;
        }

        public virtual void Close()
        {
        }
    }
}