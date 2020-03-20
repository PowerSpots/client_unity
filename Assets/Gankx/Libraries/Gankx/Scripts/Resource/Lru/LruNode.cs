namespace Gankx
{
    public class LruNode<R, K> where R : LruResource
    {
        public LruNode(R resource, K key)
        {
            this.resource = resource;
            this.key = key;
        }

        public R resource { get; private set; }
        public K key { get; private set; }
        public LruNode<R, K> previous { get; set; }
        public LruNode<R, K> next { get; set; }
    }
}