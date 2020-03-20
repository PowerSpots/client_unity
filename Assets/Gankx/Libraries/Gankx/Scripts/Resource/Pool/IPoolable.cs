
namespace Gankx
{
    public interface IPoolable
    {
        void OnInstantiate();
        void OnSpawn();
        void OnUnspawn();
    }
}