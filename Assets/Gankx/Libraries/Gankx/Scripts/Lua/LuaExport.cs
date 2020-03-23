using System.IO;
using System.Text;
using Gankx.IO;
using UnityEngine;
using XLua;

namespace Gankx
{
    public class LuaExport
    {
        public static LuaFunction LoadFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            byte[] buff;
            var bufSize = LuaLoader.Load(fileName, out buff);
            if (bufSize <= 0)
            {
                return null;
            }

            return LuaService.instance.env.LoadString<LuaFunction>(buff, fileName);
        }

        public static bool DoFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Debug.LogError("DoFile: FileName is empty");
                return false;
            }

            byte[] buff;
            var bufSize = LuaLoader.Load(fileName, out buff);
            if (bufSize <= 0)
            {
                Debug.LogError("DoFile: file size is empty");
                return false;
            }

            LuaService.instance.env.DoString(buff, fileName);

            return true;
        }

        public static bool DoExtendFile(string fileName, bool isCompressed)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Debug.LogError("DoExtendFile: FileName is empty");
                return false;
            }

            var buff = string.Empty;
            if (!FileLoaderHelper.LoadFromExtendPath(fileName, ref buff, isCompressed))
            {
                Debug.LogError("DoExtendFile: load file failed!");
                return false;
            }

            LuaService.instance.env.DoString(buff, fileName);

            return true;
        }

        public static bool FileExists(string path)
        {
            FileSystem.Entry entryOut;

            return FileService.system.Stat(path, out entryOut) == FileSystem.Result.Ok;
        }

        public static LuaTable GetSysFiles(string path, string searchPattern, bool recursive)
        {
            var filesTable = LuaService.instance.NewTable();
            var files = FileService.system.GetFiles(path, searchPattern, recursive);
            for (var i = 0; i < files.Count; i++)
            {
                filesTable.Set(i + 1, files[i]);
            }

            return filesTable;
        }

        public static bool CreateSysDirectory(string path)
        {
            return FileService.system.Mkdir(path) == FileSystem.Result.Ok;
        }

        public static string ReadSysFile(string path)
        {
            var sr = FileService.system.OpenReader(path);
            if (null == sr)
            {
                return null;
            }

            var ssr = sr.IsSeekTellSupported();
            if (null == ssr)
            {
                return null;
            }

            var readBytes = new byte[ssr.length];
            var bytesRead = sr.Read(readBytes, 0, readBytes.Length);
            return Encoding.UTF8.GetString(readBytes, 0, bytesRead);
        }

        public static int WriteSysFile(string path, string bytes)
        {
            var sw = FileService.system.OpenWriter(path);
            if (null == sw)
            {
                return 0;
            }

            var writeBytes = Encoding.UTF8.GetBytes(bytes);
            var result = sw.Write(writeBytes, writeBytes.Length);

            sw.Close();

            return result;
        }

        public static void BootError(string errMessage)
        {
            Debug.LogError(errMessage);
        }

        public static string GetLuaDirectory()
        {
            return FileService.dataPath;
        }

        public static void WriteAppFile(string path, string content)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }

            var filePath = Application.persistentDataPath + path;
            File.WriteAllText(filePath, content);
        }
        
        public static bool IsDebug()
        {
            if (Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.OSXEditor ||
                Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                return true;
            }

            return false;
        }

        public static void SetBackgroundLoadingPriority(int level)
        {
            Application.backgroundLoadingPriority = (ThreadPriority)level;
        }
    }
}
