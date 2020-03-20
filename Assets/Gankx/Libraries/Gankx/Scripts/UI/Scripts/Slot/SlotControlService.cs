using UnityEngine;

namespace Gankx.UI
{
    public class ManagedSlotControl : ManagedResource<GameObject>
    {
        protected override void Free()
        {
            // ignore
        }
    }

    public class SlotControlResourceManager : ResourceManager<ManagedSlotControl, GameObject>
    {
        public SlotControlResourceManager()
        {
            loader = LoadSlotControl;
            unloader = UnloadSlotControl;
        }

        public static GameObject LoadSlotControl(string path)
        {
            return ResourceService.LoadControl(path);
        }

        public static void UnloadSlotControl(string path)
        {
            ResourceService.UnloadControl(path, true);
        }

        protected override int GetSingleWeight(GameObject control)
        {
            return 0;
        }
    }

    public class SlotControlService : Singleton<SlotControlService>
    {
        private readonly SlotControlResourceManager myResourceManager = new SlotControlResourceManager();

        public void Load(string path, ResourceManager<ManagedSlotControl, GameObject>.ResourceLoadedDelegate onLoaded)
        {
            myResourceManager.Load(path, onLoaded);
        }

        public void LoadAsync(string path,
            ResourceManager<ManagedSlotControl, GameObject>.ResourceLoadedDelegate onLoaded)
        {
            myResourceManager.LoadAsync(path, onLoaded);
        }

        public void Unload(string path)
        {
            myResourceManager.Unload(path);
        }

        public void RemoveLoadTaskByDelegate(
            ResourceManager<ManagedSlotControl, GameObject>.ResourceLoadedDelegate onLoaded)
        {
            myResourceManager.RemoveLoadTaskByDelegate(onLoaded);
        }

        private void Update()
        {
            myResourceManager.Update();
        }

        public void UnloadUnusedAssets()
        {
            myResourceManager.UnloadUnusedAssets();
        }
    }
}