using System.Collections.Generic;
using UnityEngine;

namespace Gankx.UI
{
    public class RawImageUrlTextureCacheMemory
    {
        public class MemoryEntry
        {
            private const int UnloadThreshold = 100;

            private static int UnloadCount;
            public string url;
            public Texture2D texture;
            private int myRefCount;

            private RawImageUrlTextureCacheMemory myTextureCacheManager;

            public MemoryEntry(string url, Texture2D texture)
            {
                this.url = url;
                this.texture = texture;
                myRefCount = 0;
            }

            public void SetCacheManager(RawImageUrlTextureCacheMemory textureCacheManager)
            {
                myTextureCacheManager = textureCacheManager;
            }

            public void UnloadUnused()
            {
                if (myRefCount > 0)
                {
                    return;
                }

                if (null != texture)
                {
                    Object.Destroy(texture);

                    texture = null;
                    url = null;

                    UnloadCount++;
                    if (UnloadCount > UnloadThreshold)
                    {
                        UnloadCount = 0;
                        Resources.UnloadUnusedAssets();
                    }
                }

                myTextureCacheManager.RemoveEntry(this);
            }

            public void IncRef()
            {
                myRefCount++;
            }

            public void DecRef()
            {
                if (myRefCount <= 0)
                {
                    Debug.LogError("MemoryEntry.DecRef occured Error: Invalid usage, the refCount already le 0");
                    return;
                }

                myRefCount--;
            }
        }

        private const float UnloadUnusedCacheInterval = 5f;

        private readonly List<MemoryEntry> myMemoryEntries = new List<MemoryEntry>();

        private float myUnloadUnusedCacheLeftTime = UnloadUnusedCacheInterval;

        public void Update()
        {
            myUnloadUnusedCacheLeftTime -= Time.deltaTime;
            if (myUnloadUnusedCacheLeftTime <= 0)
            {
                for (var i = 0; i < myMemoryEntries.Count; i++)
                {
                    myMemoryEntries[i].UnloadUnused();
                }

                myUnloadUnusedCacheLeftTime = UnloadUnusedCacheInterval;
            }
        }

        private int FindEntryIndex(string url)
        {
            for (var i = 0; i < myMemoryEntries.Count; i++)
            {
                if (myMemoryEntries[i].url == url)
                {
                    return i;
                }
            }

            return -1;
        }

        public MemoryEntry UpdateEntry(string url, Texture2D tex2D)
        {
            var entryIndex = FindEntryIndex(url);
            if (-1 != entryIndex)
            {
                return myMemoryEntries[entryIndex];
            }

            var newEntry = new MemoryEntry(url, tex2D);
            newEntry.SetCacheManager(this);

            myMemoryEntries.Add(newEntry);

            return newEntry;
        }

        public MemoryEntry GetEntry(string url)
        {
            var index = FindEntryIndex(url);
            if (-1 == index)
            {
                return null;
            }

            return myMemoryEntries[index];
        }

        private void RemoveEntry(MemoryEntry entry)
        {
            for (var i = 0; i < myMemoryEntries.Count; i++)
            {
                if (myMemoryEntries[i] == entry)
                {
                    myMemoryEntries.RemoveAt(i);

                    break;
                }
            }
        }
    }
}