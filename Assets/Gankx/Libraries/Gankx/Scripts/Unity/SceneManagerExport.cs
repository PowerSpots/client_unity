using Gankx;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneManagerExport
{
    public static void LoadSceneByName(string sceneName, int mode)
    {
        SceneMgr.LoadSceneByName(sceneName, (LoadSceneMode)mode);
    }

    public static uint LoadSceneAsyncByName(string sceneName, int mode)
    {
        CustomAsyncOperation loadOperation = SceneMgr.LoadSceneAsyncByName(sceneName, (LoadSceneMode)mode);
        return CustomAsyncOperationService.instance.Add(loadOperation);
    }

    public static void LoadSceneByIndex(int sceneBuildIndex, int mode)
    {
        SceneMgr.LoadSceneByIndex(sceneBuildIndex, (LoadSceneMode)mode);
    }

    public static uint LoadSceneAsyncByIndex(int sceneBuildIndex, int mode)
    {
        CustomAsyncOperation loadOperation = SceneMgr.LoadSceneAsyncByIndex(sceneBuildIndex, (LoadSceneMode)mode);
        return CustomAsyncOperationService.instance.Add(loadOperation);
    }

    public static bool UnloadSceneByName(string sceneName)
    {
        return SceneMgr.UnloadScene(sceneName);
    }

    public static uint UnloadSceneAsyncByName(string sceneName)
    {
        AsyncOperation loadOperation = SceneMgr.UnloadSceneAsync(sceneName);
        return CustomAsyncOperationService.instance.Add(loadOperation);
    }

    public static void SetSceneGOActiveByNameAndActivatePreScene(string sceneName, bool isActive)
    {
        SceneMgr.SetSceneGOActiveAndActivatePreScene(sceneName, isActive);
    }

    public static bool UnloadSceneByIndex(int sceneBuildIndex)
    {
        return SceneMgr.UnloadScene(sceneBuildIndex);
    }

    public static uint UnloadSceneAsyncByIndex(int sceneBuildIndex)
    {
        AsyncOperation unLoadOperation = SceneMgr.UnloadSceneAsync(sceneBuildIndex);
        return CustomAsyncOperationService.instance.Add(unLoadOperation);
    }

    public static string GetActiveSceneName()
    {
        return SceneMgr.GetActiveScene().name;
    }

    public static void SetActiveScene(string sceneName)
    {
        if (!SceneMgr.IsLoadedScene(sceneName))
            return;

        Scene scene = SceneMgr.GetSceneByName(sceneName);
        SceneMgr.SetActiveScene(scene);
    }

    public static bool IsLoadedScene(string sceneName)
    {
        return SceneMgr.IsLoadedScene(sceneName);
    }

    public static void UnloadSceneAssetbundleByName(string sceneName)
    {
        SceneMgr.UnloadSceneAssetbundleByName(sceneName);
    }

    public static void UnloadSceneAssetbundleIndex(int sceneBuildIndex)
    {
        SceneMgr.UnloadSceneAssetbundleIndex(sceneBuildIndex);
    }
}
