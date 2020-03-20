using System.Collections.Generic;

namespace Gankx
{
    public class LruCache<K, V> where V : LruResource
    {
        private readonly int myMaxCapacity;
        private readonly Dictionary<K, LruNode<V, K>> myLruCache;
        private LruNode<V, K> myHead;
        private LruNode<V, K> myTail;

        private bool myKeepCapacity;

        public LruCache(int argMaxCapacity, bool keepCapacity = false)
        {
            myMaxCapacity = argMaxCapacity;
            myLruCache = new Dictionary<K, LruNode<V, K>>();
            myKeepCapacity = keepCapacity;
        }

        public void InsertResource(K key, V value)
        {
            if (myLruCache.ContainsKey(key))
            {
                MakeMostRecentlyUsed(myLruCache[key]);
                return;
            }

            if (myLruCache.Count >= myMaxCapacity)
            {
                if (myKeepCapacity)
                {
                    RemoveToCapacity();
                }
                else
                {
                    RemoveLeastRecentlyUsed();
                }
            }

            var insertedNode = new LruNode<V, K>(value, key);

            if (myHead == null)
            {
                myHead = insertedNode;
                myTail = myHead;
            }
            else
            {
                MakeMostRecentlyUsed(insertedNode);
            }

            myLruCache.Add(key, insertedNode);
        }

        public LruNode<V, K> GetResource(K key)
        {
            if (!myLruCache.ContainsKey(key))
            {
                return null;
            }

            MakeMostRecentlyUsed(myLruCache[key]);

            return myLruCache[key];
        }

        public LruNode<V, K> FindResource(K key)
        {
            if (!myLruCache.ContainsKey(key))
            {
                return null;
            }

            return myLruCache[key];
        }

        public void RemoveResource(K key)
        {
            if (!myLruCache.ContainsKey(key))
            {
                return;
            }

            var removedNode = myLruCache[key];

            if (removedNode.resource.beUsed)
            {
                return;
            }

            RemoveFound(removedNode);
        }

        public int Size()
        {
            return myLruCache.Count;
        }

        public string CacheFeed()
        {
            var headReference = myHead;

            var items = new List<string>();

            var index = 1;
            while (headReference != null)
            {
                items.Add(string.Format("[{0}: {1}]", index++, headReference.resource));
                headReference = headReference.next;
            }

            return string.Join(",", items.ToArray());
        }

        private void RemoveFound(LruNode<V, K> foundNode)
        {
            foundNode.resource.Free();

            myLruCache.Remove(foundNode.key);

            if (myHead == foundNode && myTail == foundNode)
            {
                myHead = myTail = null;
            }
            else if (myTail == foundNode)
            {
                myTail = foundNode.previous;
                myTail.next = null;
            }
            else if (myHead == foundNode)
            {
                myHead = foundNode.next;
                myHead.previous = null;
            }
            else
            {
                foundNode.next.previous = foundNode.previous;
                foundNode.previous.next = foundNode.next;
            }
        }

        public void RemoveLeastRecentlyUsed()
        {
            var node = myTail;
            while (node != null)
            {
                if (!node.resource.beUsed)
                {
                    RemoveFound(node);
                    break;
                }

                node = node.previous;
            }
        }

        public void RemoveToCapacity()
        {
            var node = myTail;
            while (node != null)
            {
                var prevNode = node.previous;
                if (!node.resource.beUsed)
                {
                    RemoveFound(node);
                }

                node = prevNode;
            }
        }

        private void MakeMostRecentlyUsed(LruNode<V, K> foundNode)
        {
            if (myHead == foundNode && myTail == foundNode)
            {
                return;
            }

            if (foundNode.next == null && foundNode.previous == null)
            {
                foundNode.next = myHead;
                myHead.previous = foundNode;
                if (myHead.next == null)
                {
                    myTail = myHead;
                }

                myHead = foundNode;
            }
            else if (foundNode.next == null && foundNode.previous != null)
            {
                foundNode.previous.next = null;
                myTail = foundNode.previous;
                foundNode.next = myHead;
                myHead.previous = foundNode;
                myHead = foundNode;
                myHead.previous = null;
            }
            else if (foundNode.next != null && foundNode.previous != null)
            {
                foundNode.previous.next = foundNode.next;
                foundNode.next.previous = foundNode.previous;
                foundNode.next = myHead;
                myHead.previous = foundNode;
                myHead = foundNode;
                myHead.previous = null;
            }
        }
    }
}