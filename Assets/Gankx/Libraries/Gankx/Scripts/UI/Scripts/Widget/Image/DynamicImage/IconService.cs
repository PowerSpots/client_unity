using UnityEngine;

namespace Gankx.UI
{
    public class ManagedIcon : ManagedResource<Sprite>
    {
        protected override void Free()
        {
            if (IconService.ContainsInstance())
            {
                IconService.instance.MarkDirty();
            }
        }
    }

    public class IconResourceManager : ResourceManager<ManagedIcon, Sprite>
    {
        public IconResourceManager()
        {
            loader = LoadIcon;
            unloader = UnloadIcon;
        }

        protected override int mUnloadTotalWeight
        {
            get { return 5 * 1024 * 1024; }
        }

        public Sprite LoadIcon(string path)
        {
            return ResourceService.LoadIcon(path);
        }

        public void UnloadIcon(string path)
        {
            ResourceService.UnloadIcon(path, true);
        }

        protected override int GetSingleWeight(Sprite sprite)
        {
            if (null == sprite || null == sprite.texture)
            {
                return 0;
            }

            if (sprite.texture.width == 128 && sprite.texture.height == 128 ||
                sprite.texture.width == 80 && sprite.texture.height == 80)
            {
                return sprite.texture.width * sprite.texture.height * 8;
            }

            return sprite.texture.width * sprite.texture.height;
        }
    }

    public class IconService : Singleton<IconService>
    {
        private readonly IconResourceManager myResourceManager = new IconResourceManager();

        public void Load(string path, ResourceManager<ManagedIcon, Sprite>.ResourceLoadedDelegate onLoaded)
        {
            myResourceManager.Load(path, onLoaded);
        }

        public void LoadAsync(string path, ResourceManager<ManagedIcon, Sprite>.ResourceLoadedDelegate onLoaded)
        {
            myResourceManager.LoadAsync(path, onLoaded);
        }

        public void Unload(string path)
        {
            myResourceManager.Unload(path);
        }

        public void UnloadUnusedAssets()
        {
            myResourceManager.UnloadUnusedAssets();
        }

        public void RemoveLoadTaskByDelegate(ResourceManager<ManagedIcon, Sprite>.ResourceLoadedDelegate onLoaded)
        {
            myResourceManager.RemoveLoadTaskByDelegate(onLoaded);
        }

        private void Update()
        {
            myResourceManager.Update();
        }

        public void MarkDirty()
        {
            myResourceManager.MarkDirty();
        }
    }
}