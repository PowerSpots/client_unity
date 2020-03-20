using System.Collections.Generic;
using System.Linq;

namespace Gankx
{
    public class LruRefAllocator<K, V> where V : LruRefResource
    {
        private readonly LruCache<K, V> myAllocCache;
        private readonly List<V> myFreeList;

        public LruRefAllocator(int maxCapacity, int cacheCapacity, bool keepCapacity = false)
        {
            myAllocCache = new LruCache<K, V>(cacheCapacity, keepCapacity);
            myFreeList = new List<V>(maxCapacity);
        }

        public int GetCachedCount()
        {
            return myAllocCache.Size();
        }

        public int GetFreeCount()
        {
            return myFreeList.Count;
        }

        public string CacheFeed()
        {
            return myAllocCache.CacheFeed();
        }

        public void Insert(V value)
        {
            myFreeList.Add(value);
        }

        public V Get(K key)
        {
            var cacheNode = myAllocCache.GetResource(key);
            V allocNode = null;
            if (cacheNode != null)
            {
                allocNode = cacheNode.resource;
            }

            return allocNode;
        }

        public V Allocate(K key)
        {
            var cacheNode = myAllocCache.GetResource(key);
            V allocNode = null;
            if (cacheNode != null)
            {
                allocNode = cacheNode.resource;
            }
            else
            {
                if (myFreeList.Count <= 0)
                {
                    myAllocCache.RemoveLeastRecentlyUsed();

                    if (myFreeList.Count <= 0)
                    {
                        return null;
                    }
                }

                allocNode = myFreeList.Last();
                myFreeList.RemoveAt(myFreeList.Count - 1);
                myAllocCache.InsertResource(key, allocNode);
            }

            return allocNode;
        }

        public void Deallocate(K key)
        {
            myAllocCache.RemoveResource(key);
        }
    }
}