using Gankx;
using UnityEngine;

public static class ResourcesExport
{
    public static uint UnloadUnusedAssets()
    {
        AsyncOperation unloadOperation = Resources.UnloadUnusedAssets();
        return CustomAsyncOperationService.instance.Add(unloadOperation);
    }
}
