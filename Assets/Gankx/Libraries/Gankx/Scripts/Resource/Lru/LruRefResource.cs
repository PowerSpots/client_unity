namespace Gankx
{
    public abstract class LruRefResource : LruResource
    {
        public int refCount { get; private set; }

        public override bool beUsed
        {
            get { return refCount > 0; }
        }

        public virtual void IncRef()
        {
            refCount++;
        }

        public virtual void DecRef()
        {
            refCount--;
        }
    }
}