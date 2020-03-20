using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gankx;
using Gankx.IO;
using UnityEngine;
using XLua;

public class FileLoaderHelper
{
    private struct CacheFileCRC
    {
        public uint crc;
        public long lastWriteTime;
    }

    private static readonly Dictionary<string, CacheFileCRC> CachedRootFileCrcMap = new Dictionary<string, CacheFileCRC>();
    private static readonly Dictionary<string, uint> CacheLuaFileCRCMap = new Dictionary<string, uint>();

    public static string GetStreamAssetsFilePath(string path)
    {
        var filePath = Application.streamingAssetsPath + "/" + path;

#if UNITY_STANDALONE || UNITY_EDITOR
        filePath = "file:///" + filePath;
#elif UNITY_IOS
                filePath = "file://" + filePath;
#endif
        return filePath;
    }

    public static byte[] LoadFromStreamAssets(string path)
    {
        byte[] rawBytes = { };
        var filePath = Application.streamingAssetsPath + "/" + path;

#if !UNITY_EDITOR && UNITY_ANDROID
        WWW www = new WWW(filePath);
        while (!www.isDone) { }
        if(!string.IsNullOrEmpty(www.error)) {
            return rawBytes;
        }
#endif

        rawBytes =
#if !UNITY_EDITOR && UNITY_ANDROID
            www.bytes;
            www.Dispose();
#else
            File.ReadAllBytes(filePath);
#endif
        return rawBytes;
    }

    public static IEnumerator LoadFromStreamAssetsAsyc(string path, Action<byte[]> callback)
    {
        var filePath = Application.streamingAssetsPath + "/" + path;

#if UNITY_STANDALONE || UNITY_EDITOR
        filePath = "file:///" + filePath;
#elif UNITY_IOS
        filePath = "file://" + filePath;
#endif

        var www = new WWW(filePath);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            callback(www.bytes);
        }
        else
        {
            Debug.LogError(www.error);
            callback(null);
        }

        www.Dispose();
    }

    public static bool LoadFromRootPath(string path, ref string result)
    {
        path = FileService.dataPath + path;
        if (!File.Exists(path))
        {
            return false;
        }

        result = "";
        try
        {
            result = Encoding.UTF8.GetString(File.ReadAllBytes(path));
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("加载路径 [" + path + "] 出错\n" + e);
            return false;
        }
    }

    public static byte[] LoadFromRootOrStreamAssets(string path)
    {
        var rootPath = FileService.dataPath + path;
        if (File.Exists(rootPath))
        {
            return File.ReadAllBytes(rootPath);
        }

        var streamPath = "data/" + path;
        return LoadFromStreamAssets(streamPath);
    }

    public static bool LoadFromExtraPath(string path, ref string result)
    {
        return LoadFromRootPath("../hidden8848/" + path, ref result);
    }

    public static bool LoadFromExtendPath(string path, ref string result, bool isCompressed = false)
    {
        if (LoadFromRootPath("Extend/" + path, ref result))
        {
            if (isCompressed)
            {
                result = DecompressString(result);
            }

            return true;
        }

        return false;
    }

    public static string LoadFromExtendPath(string path, bool isCompressed = false)
    {
        var result = string.Empty;

        if (LoadFromRootPath("Extend/" + path, ref result))
        {
            if (isCompressed)
            {
                result = DecompressString(result);
            }
        }

        return result;
    }

    public static bool SaveToExtendPath(string path, string info)
    {
        return SaveToRootPath("Extend/" + path, info);
    }

    public static bool SaveToRootPath(string path, string info, bool isCompressed = false)
    {
        if (isCompressed)
        {
            info = CompressString(info);
        }

        return SaveToRootPath(path, Encoding.UTF8.GetBytes(info));
    }

    public static bool SaveToExtendPathWithCRC(string path, string info, bool isCompressed = false)
    {
        return SaveToRootPathWithCRC("Extend/" + path, info, isCompressed);
    }

    public static bool SaveToRootPathWithCRC(string path, string info, bool isCompressed = false)
    {
        if (isCompressed)
        {
            info = CompressString(info);
        }

        return SaveToRootPath(path, GetBytesWithCRC(info));
    }

    public static byte[] GetBytesWithCRC(string info)
    {
        if (string.IsNullOrEmpty(info))
        {
            return null;
        }

        var infoBytes = Encoding.UTF8.GetBytes(info);
        var crc = Crc32.Compute(infoBytes);
        Debug.Log("current detect crc is" + crc.ToString("X"));
        var crcBytes = BitConverter.GetBytes(crc);
        var result = new byte[infoBytes.Length + 4];
        for (var i = 0; i < 4; i++)
        {
            if (i >= 0 && i < crcBytes.Length)
            {
                result[i] = crcBytes[i];
            }
            else
            {
                result[i] = 0;
            }
        }

        for (var i = 4; i < infoBytes.Length + 4; i++)
        {
            result[i] = infoBytes[i - 4];
        }

        return result;
    }

    public static bool SaveToRootPath(string path, byte[] info, uint size = 0)
    {
        if (!Directory.Exists(FileService.dataPath) || info == null)
        {
            return false;
        }

        path = FileService.dataPath + path;
        var parentFolder = path.Substring(0, path.LastIndexOf("/", StringComparison.Ordinal));
        if (!Directory.Exists(parentFolder))
        {
            Directory.CreateDirectory(parentFolder);
        }

        if (File.Exists(path))
        {
            File.SetAttributes(path, FileAttributes.Normal);
            File.Delete(path);
        }

        try
        {
            if (size < info.Length && size > 0)
            {
                var safeInfo = new byte[size];
                info.CopyTo(safeInfo, 0);
                File.WriteAllBytes(path, safeInfo);
            }
            else
            {
                File.WriteAllBytes(path, info);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("SaveToRootPath " + path + e);
            return false;
        }

        return true;
    }

    public static bool SaveToRootPath(string path, ulong[] info, uint size)
    {
        if (string.IsNullOrEmpty(path) || info == null || size == 0)
        {
            return false;
        }

        var infoBytes = new byte[size];
        var index = 0;
        for (var i = 0; i < info.Length; i++)
        {
            var tmp = BitConverter.GetBytes(info[i]);
            for (var j = 0; j < tmp.Length && index < size; j++)
            {
                infoBytes[index++] = tmp[j];
            }
        }

        return SaveToRootPath(path, infoBytes, size);
    }

    public static string CompressString(string lEncode)
    {
        try
        {
            var lEncodeBytes = Encoding.UTF8.GetBytes(lEncode);
            var lCompressedBytes = lz4.Compress(lEncodeBytes);
            return Convert.ToBase64String(lCompressedBytes);
        }
        catch (Exception e)
        {
            Debug.LogError("CompressString Failed!\n" + e);
            return lEncode;
        }
    }

    public static string DecompressString(string lEncodedData)
    {
        try
        {
            var lEncodedBytes = Convert.FromBase64String(lEncodedData);
            var lDecompressedBytes = lz4.Decompress(lEncodedBytes);
            var lDecodedData = Encoding.UTF8.GetString(lDecompressedBytes);
            return lDecodedData;
        }
        catch (Exception e)
        {
            Debug.LogError("DecompressString Failed!\n" + e);
            return lEncodedData;
        }
    }

    public static LuaTable GetFilesUnderRoot(string path)
    {
        if (path == null || !Directory.Exists(FileService.dataPath + path))
        {
            return null;
        }

        var directoryInfo = new DirectoryInfo(FileService.dataPath + path);

        var fileTable = LuaService.instance.NewTable();
        var files = directoryInfo.GetFiles();
        for (var i = 0; i < files.Length; i++)
        {
            fileTable.Set(i + 1, files[i].Name);
        }

        return fileTable;
    }

    public static uint GetRootFileCrc(string path)
    {
        var rootPath = FileService.dataPath + path;
        if (!File.Exists(rootPath))
        {
            return 0;
        }

        try
        {
            var fileInfo = new FileInfo(rootPath);
            CacheFileCRC crcInfo;
            if (CachedRootFileCrcMap.TryGetValue(rootPath, out crcInfo))
            {
                if (fileInfo.LastWriteTime.ToFileTime() <= crcInfo.lastWriteTime)
                {
                    return crcInfo.crc;
                }
            }

            var fileDetail = string.Empty;
            if (LoadFromRootPath(path, ref fileDetail))
            {
                var infoBytes = Encoding.UTF8.GetBytes(fileDetail);
                var crc = Crc32.Compute(infoBytes);

                crcInfo = new CacheFileCRC();
                crcInfo.crc = crc;
                crcInfo.lastWriteTime = fileInfo.LastWriteTime.ToFileTime();
                if (!CachedRootFileCrcMap.ContainsKey(rootPath))
                {
                    CachedRootFileCrcMap.Add(rootPath, crcInfo);
                }
                else
                {
                    CachedRootFileCrcMap[rootPath] = crcInfo;
                }

                return crc;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("GetRootFileCrc @path:" + path + "\n" + e);
        }

        return 0;
    }

    public static uint GetLuaFileCrc(string path, bool isCompressed)
    {
        uint crc = 0;
        if (CacheLuaFileCRCMap.TryGetValue(path, out crc))
        {
            return crc;
        }

        byte[] buff;
        var bufSize = LuaLoader.Load(path, out buff);
        if (bufSize <= 0)
        {
            Debug.LogError("GetLuaFileCrc Fail: FilePath:" + path);
            return crc;
        }

        if (isCompressed)
        {
            var info = Encoding.UTF8.GetString(buff);
            info = CompressString(info);
            buff = Encoding.UTF8.GetBytes(info);
        }

        crc = Crc32.Compute(buff);
        if (!CacheLuaFileCRCMap.ContainsKey(path))
        {
            CacheLuaFileCRCMap.Add(path, crc);
        }
        else
        {
            CacheLuaFileCRCMap[path] = crc;
        }

        return crc;
    }
}