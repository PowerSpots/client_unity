using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gankx.UI
{
    public class RawImageUrlTextureManager : Singleton<RawImageUrlTextureManager>
    {
        private class DiskEntry
        {
            private const float OutOfCacheTimeElapsed = UIConfig.RawImageUrlTextureDiskCacheTimeElapsed;
            public const float OutOfCacheTimeMin = UIConfig.RawImageUrlTextureDiskCacheTimeMin;
            public const float OutOfCacheTimeMax = UIConfig.RawImageUrlTextureDiskCacheTimeMax;
            public string path;
            public string md5;
            public DateTime lastAccessTime;
            public DateTime lastWriteTime;

            public bool IsOutOfCacheTime(string url)
            {
                if (url.StartsWith("http://album"))
                {
                    return false;
                }

                var nowTime = DateTime.Now;
                var timeSpan = nowTime - lastWriteTime;
                return timeSpan.TotalSeconds > OutOfCacheTimeElapsed;
            }
        }

        private class AsyncLoadEntry
        {
            public string md5;
            public IEnumerator coroutine;

            private readonly List<Action<RawImageUrlTextureCacheMemory.MemoryEntry>> myCallbackList =
                new List<Action<RawImageUrlTextureCacheMemory.MemoryEntry>>();

            public void AddCallback(Action<RawImageUrlTextureCacheMemory.MemoryEntry> callback)
            {
                myCallbackList.Add(callback);
            }

            public void DoCallback(RawImageUrlTextureCacheMemory.MemoryEntry tex)
            {
                for (var i = 0; i < myCallbackList.Count; i++)
                {
                    if (myCallbackList[i] != null)
                    {
                        myCallbackList[i](tex);
                    }
                }

                myCallbackList.Clear();
            }

            public void Dispose()
            {
                myCallbackList.Clear();
            }
        }

        private const int MaxDiskCount = UIConfig.RawImageUrlTextureMaxDiskCount;
        private const string DiskExtension = UIConfig.RawImageUrlTextureDiskExtension;
        private const int MaxLoadCountInFrame = UIConfig.RawImageUrlTextureMaxLoadCountInFrame;
        private readonly RawImageUrlTextureCacheMemory myTextureCacheMemory = new RawImageUrlTextureCacheMemory();
        private readonly List<DiskEntry> myDiskEntries = new List<DiskEntry>();

        private string myCachePath;
        private readonly List<string> myAsyncLoadWaitingList = new List<string>();
        private bool myInvalidate;

        private readonly Dictionary<string, AsyncLoadEntry> myAsyncLoadDict = new Dictionary<string, AsyncLoadEntry>();

        public void Init()
        {
            OnInit();
        }

        protected override void OnInit()
        {
            myCachePath = Application.persistentDataPath + UIConfig.RawImageUrlTextureDiskSimplePath;

            if (!Directory.Exists(myCachePath))
            {
                Directory.CreateDirectory(myCachePath);
                return;
            }

            var filePaths = Directory.GetFiles(myCachePath, UIConfig.RawImageUrlTextureDiskSearchPattern);
            for (var i = 0; i < filePaths.Length; i++)
            {
                var entry = new DiskEntry();
                entry.path = filePaths[i];

                var indexOfSlash = filePaths[i].LastIndexOf('/');
                var indexOfDot = filePaths[i].LastIndexOf('.');
                entry.md5 = filePaths[i].Substring(indexOfSlash + 1, indexOfDot - indexOfSlash - 1);

                entry.lastAccessTime = File.GetLastAccessTime(filePaths[i]);
                entry.lastWriteTime = File.GetLastWriteTime(filePaths[i]);

                myDiskEntries.Add(entry);
            }
        }

        public static string GetUrlMd5(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("GetUrlMd5 url is null");
                return string.Empty;
            }

            var md5 = MD5.Create();
            var urlBytes = Encoding.Default.GetBytes(url);
            var data = md5.ComputeHash(urlBytes);

            var sBuilder = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        private DiskEntry GetDiskEntry(string url)
        {
            var md5 = GetUrlMd5(url);
            if (string.IsNullOrEmpty(md5))
            {
                return null;
            }

            for (var i = 0; i < myDiskEntries.Count; i++)
            {
                if (myDiskEntries[i].md5 == md5)
                {
                    if (!myDiskEntries[i].IsOutOfCacheTime(url))
                    {
                        return myDiskEntries[i];
                    }

                    return null;
                }
            }

            return null;
        }

        private void RemoveDiskEntry(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            var diskEntry = GetDiskEntry(url);
            if (diskEntry != null)
            {
                File.Delete(diskEntry.path);
                myDiskEntries.Remove(diskEntry);
            }
        }

        private void DeleteOutOfTimeEntry(string url)
        {
            var md5 = GetUrlMd5(url);

            for (var i = 0; i < myDiskEntries.Count; i++)
            {
                if (myDiskEntries[i].md5 == md5)
                {
                    if (myDiskEntries[i].IsOutOfCacheTime(url))
                    {
                        File.Delete(myDiskEntries[i].path);
                        myDiskEntries.RemoveAt(i);
                    }

                    break;
                }
            }
        }

        private bool IsLoadingLocal(string md5)
        {
            foreach (var pair in myAsyncLoadDict)
            {
                if (md5 == pair.Value.md5)
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateDisk(string url, Texture2D tex2D)
        {
            DeleteOutOfTimeEntry(url);

            var entry = GetDiskEntry(url);

            if (null != entry && entry.path != null && entry.path != "")
            {
                entry.lastAccessTime = DateTime.Now;
                try
                {
                    File.SetLastAccessTime(entry.path, entry.lastAccessTime);
                }
                catch (Exception)
                {
                    // ignore
                }

                return;
            }

            var urlMd5 = GetUrlMd5(url);
            var urlPath = UrlToPath(url);

            Stream stream;
            try
            {
                var fileInfo = new FileInfo(urlPath);
                if (fileInfo.Exists)
                {
                    return;
                }

                stream = fileInfo.Create();
            }
            catch
            {
                return;
            }

            if (myDiskEntries.Count > MaxDiskCount)
            {
                myDiskEntries.Sort((entry1, entry2) =>
                {
                    return entry1.lastAccessTime.CompareTo(entry2.lastAccessTime);
                });

                var toDeleteNum = myDiskEntries.Count - MaxDiskCount;

                var delIndex = 0;
                for (var i = 0; i < myDiskEntries.Count && toDeleteNum > 0; i++)
                {
                    if (IsLoadingLocal(myDiskEntries[i].md5))
                    {
                        delIndex++;
                        continue;
                    }

                    toDeleteNum--;
                    File.Delete(myDiskEntries[delIndex].path);
                    myDiskEntries.RemoveAt(delIndex);
                }
            }

            try
            {
                var bytes = tex2D.EncodeToPNG();
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
                stream.Dispose();
            }
            catch (Exception)
            {
                stream.Close();
                stream.Dispose();
            }

            entry = new DiskEntry();
            entry.path = urlPath;
            entry.md5 = urlMd5;
            entry.lastAccessTime = DateTime.Now;

            entry.lastWriteTime = DateTime.Now;
            double timeOffset = Random.Range(DiskEntry.OutOfCacheTimeMin, DiskEntry.OutOfCacheTimeMax);
            entry.lastWriteTime.AddSeconds(timeOffset);

            try
            {
                File.SetLastAccessTime(urlPath, entry.lastAccessTime);
                File.SetLastWriteTime(urlPath, entry.lastWriteTime);
            }
            catch (Exception)
            {
                // ignore
            }

            myDiskEntries.Add(entry);
        }

        public bool IsDiskCached(string url)
        {
            var entry = GetDiskEntry(url);

            if (entry == null || false == IsValidDiskFile(entry.path))
            {
                return false;
            }

            return true;
        }

        private static bool IsValidDiskFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            return File.Exists(path);
        }

        public string UrlToPath(string url)
        {
            var md5 = GetUrlMd5(url);
            return myCachePath + md5 + DiskExtension;
        }

        private IEnumerator LoadDisk(string url)
        {
            var path = instance.UrlToPath(url);

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            var platformPath = "file:///" + path;
#elif UNITY_ANDROID
            var platformPath = "file://" + path;
#elif UNITY_IOS
            var platformPath = "file://" + path;
#endif

            var www = new WWW(platformPath);
            yield return www;

            AsyncLoadEntry entry;
            myAsyncLoadDict.TryGetValue(url, out entry);

            if (www.isDone)
            {
                var tex = new Texture2D(2, 2);
                tex.name = url;
                try
                {
                    if (string.IsNullOrEmpty(www.error) && tex.LoadImage(www.bytes))
                    {
                        var newMemoryEntry = myTextureCacheMemory.UpdateEntry(url, tex);

                        UpdateDisk(url, tex);

                        if (entry != null)
                        {
                            entry.DoCallback(newMemoryEntry);
                        }
                    }
                    else
                    {
                        RemoveDiskEntry(url);

                        if (entry != null)
                        {
                            entry.DoCallback(null);
                        }
                    }
                }
                catch (Exception)
                {
                    Debug.LogWarning("LoadDisk Exception error:" + platformPath);

                    if (entry != null)
                    {
                        entry.Dispose();
                    }

                    myAsyncLoadDict.Remove(url);
                    www.Dispose();
                    throw;
                }
            }
            else
            {
                Debug.LogWarning("LoadDisk www.isDone error:" + platformPath);
            }

            if (entry != null)
            {
                entry.Dispose();
            }

            myAsyncLoadDict.Remove(url);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (www != null)
            {
                www.Dispose();
            }
        }

        private IEnumerator LoadRemote(string url)
        {
#if UNITY_IPHONE
            if (!url.Contains("https"))
            {
                url = url.Replace("http", "https");
            }
#endif
            var www = new WWW(url);
            yield return www;

            AsyncLoadEntry entry;
            myAsyncLoadDict.TryGetValue(url, out entry);

            try
            {
                if (!www.isDone)
                {
                    Debug.LogWarning("LoadRemote www.isDone error:" + url);
                    if (entry != null)
                    {
                        entry.DoCallback(null);
                    }
                }
                else if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.LogWarning("LoadRemote www.error:" + www.error + " , " + url);
                    if (entry != null)
                    {
                        entry.DoCallback(null);
                    }
                }
                else if (www.isDone)
                {
                    Texture2D tex = null;
                    var contentType = www.responseHeaders["CONTENT-TYPE"];
                    if (!string.IsNullOrEmpty(contentType))
                    {
                        var contents = contentType.Split('/');
                        if (contents[0] == "image")
                        {
                            if (contents[1].ToLower() == "png" || contents[1].ToLower() == "jpeg" ||
                                contents[1].ToLower() == "jpg")
                            {
                                tex = new Texture2D(2, 2, TextureFormat.RGB24, false);
                                www.LoadImageIntoTexture(tex);
                            }
                            else if (contents[1].ToLower() == "gif")
                            {
                                var gifTexList = new List<UniGif.GifTexture>();
                                try
                                {
                                    int loop, w, h;
                                    gifTexList = UniGif.GetTextureList(www.bytes, out loop, out w, out h);
                                }
                                catch (Exception)
                                {
                                    Debug.LogWarning("LoadRemote: can not Decode Gif img : " + url);
                                }

                                tex = gifTexList[0].texture2d;

                                for (var i = 1; i < gifTexList.Count; i++)
                                {
                                    Destroy(gifTexList[i].texture2d);
                                }
                            }
                        }
                    }

                    if (null != tex)
                    {
                        tex.name = url;
                        var newMemoryEntry = myTextureCacheMemory.UpdateEntry(url, tex);
                        UpdateDisk(url, tex);
                        if (entry != null)
                        {
                            entry.DoCallback(newMemoryEntry);
                        }
                    }
                    else
                    {
                        if (entry != null)
                        {
                            entry.DoCallback(null);
                        }
                    }
                }

                if (entry != null)
                {
                    entry.Dispose();
                }

                myAsyncLoadDict.Remove(url);
            }
            catch (Exception e)
            {
                Debug.LogWarning("LoadRemote www.error:" + e);
                throw;
            }
            finally
            {
                www.Dispose();
            }
        }

        private void Update()
        {
            myTextureCacheMemory.Update();

            if (myInvalidate == false)
            {
                return;
            }

            var total = MaxLoadCountInFrame;
            if (total > myAsyncLoadWaitingList.Count)
            {
                total = myAsyncLoadWaitingList.Count;
            }

            for (var i = 0; i < total; i++)
            {
                AsyncLoadEntry entry;
                if (myAsyncLoadDict.TryGetValue(myAsyncLoadWaitingList[i], out entry))
                {
                    StartCoroutine(entry.coroutine);
                }
            }

            myAsyncLoadWaitingList.RemoveRange(0, total);
            if (myAsyncLoadWaitingList.Count == 0)
            {
                myInvalidate = false;
            }
        }

        public RawImageUrlTextureCacheMemory.MemoryEntry LoadFromCache(string url)
        {
            var memoryEntry = myTextureCacheMemory.GetEntry(url);
            return memoryEntry;
        }

        public void LoadTextureAsync(string url, Action<RawImageUrlTextureCacheMemory.MemoryEntry> callback)
        {
            if (callback == null)
            {
                Debug.LogWarning("LoadTextureAsync Callback is null");
                return;
            }

            if (string.IsNullOrEmpty(url))
            {
                Debug.LogWarning("LoadTextureAsync Url is Empty");
                callback(null);
                return;
            }

            var memoryEntry = myTextureCacheMemory.GetEntry(url);
            if (null != memoryEntry)
            {
                callback(memoryEntry);
                return;
            }

            AsyncLoadEntry entry;
            if (myAsyncLoadDict.TryGetValue(url, out entry))
            {
                entry.AddCallback(callback);
                return;
            }

            entry = new AsyncLoadEntry();
            entry.md5 = GetUrlMd5(url);
            myAsyncLoadDict.Add(url, entry);
            myAsyncLoadWaitingList.Add(url);

            entry.AddCallback(callback);

            if (IsDiskCached(url))
            {
                entry.coroutine = LoadDisk(url);
            }
            else
            {
                entry.coroutine = LoadRemote(url);
            }

            myInvalidate = true;
        }

        public void UnloadMemoryCache()
        {
        }
    }
}