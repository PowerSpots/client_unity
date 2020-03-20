#if UNITY_EDITOR
namespace Gankx
{
    public class ResourceLoadModeInEditor : Singleton<ResourceLoadModeInEditor>
    {
        public bool UsingBundle = false;
    }
}
#endif