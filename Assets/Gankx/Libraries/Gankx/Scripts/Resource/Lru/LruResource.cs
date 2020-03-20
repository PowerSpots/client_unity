namespace Gankx
{
    public abstract class LruResource
    {
        public abstract bool beUsed { get; }

        public abstract void Free();
    }
}