using System;
using System.Collections.Generic;
using Gankx;
using Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : MonoBehaviour
{
    public static Scene GetActiveScene()
    {
        return SceneManager.GetActiveScene();
    }

    public static bool IsLoadedScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            return false;
        }

        for (int index = 0; index < SceneManager.sceneCount; ++index)
        {
            Scene scene = SceneManager.GetSceneAt(index);
            if (scene.name == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    //A类场景打ab
    public static void LoadStoryAScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (ResourceService.useAssetBundle)//A类场景 打ab
        {
            string scenepath = SceneBuildSetting.instance.GetScenePath(sceneName);
            AssetBundle bundle = AssetBundleManager.instance.LoadSceneAssetbundle(scenepath);

            if (bundle != null && bundle.isStreamedSceneAssetBundle)
            {
                SceneManager.LoadScene(sceneName, mode);
            }

            //同步加载完后 下一帧卸载掉该场景引用的所有ab 
            UnloadSceneAssetbundleByPath(scenepath);
        }
        else
        {
            SceneManager.LoadScene(sceneName, mode);
        }
    }

    private static readonly List<string> ForceNonBundleScene = new List<string> { "Update"};

    public static void LoadSceneByName(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (ResourceService.useAssetBundle && !ForceNonBundleScene.Contains(sceneName))
        {
            string scenepath = SceneBuildSetting.instance.GetScenePath(sceneName);
            AssetBundle bundle = AssetBundleManager.instance.LoadSceneAssetbundle(scenepath);

            if (bundle != null && bundle.isStreamedSceneAssetBundle)
            {
                SceneManager.LoadScene(sceneName, mode);
            }

            //同步加载完后 下一帧卸载掉该场景引用的所有ab 
            //UnloadSceneAssetbundleByPath(scenepath);
        }
        else
        {
            SceneManager.LoadScene(sceneName, mode);
        }
    }

    public static CustomAsyncOperation LoadSceneAsyncByName(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (ResourceService.useAssetBundle)
        {
            string scenepath = SceneBuildSetting.instance.GetScenePath(sceneName);
            AssetBundle bundle = AssetBundleManager.instance.LoadSceneAssetbundle(scenepath);

            if (bundle != null && bundle.isStreamedSceneAssetBundle)
            {
                AsyncOperation option = SceneManager.LoadSceneAsync(sceneName, mode);
                return new UnityAsyncOperation(option);
            }
            else
            {
                UnloadSceneAssetbundleByPath(scenepath);
                return null;
            }
        }
        else
        {
            AsyncOperation option = SceneManager.LoadSceneAsync(sceneName, mode);
            return new UnityAsyncOperation(option);
        }
    }

    public static void LoadSceneByIndex(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (ResourceService.useAssetBundle)
        {
            string scenepath = SceneBuildSetting.instance.GetScenePathByIndex(sceneBuildIndex);
            AssetBundle bundle = AssetBundleManager.instance.LoadSceneAssetbundle(scenepath);

            if (bundle != null && bundle.isStreamedSceneAssetBundle)
            {
                SceneManager.LoadScene(sceneBuildIndex, mode);
            }

            //同步加载完后 下一帧卸载掉该场景引用的所有ab 
            UnloadSceneAssetbundleByPath(scenepath);
        }
        else
        {
            SceneManager.LoadScene(sceneBuildIndex, mode);
        }
    }

    public static CustomAsyncOperation LoadSceneAsyncByIndex(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (ResourceService.useAssetBundle)
        {
            string scenepath = SceneBuildSetting.instance.GetScenePathByIndex(sceneBuildIndex);
            AssetBundle bundle = AssetBundleManager.instance.LoadSceneAssetbundle(scenepath);

            if (bundle != null && bundle.isStreamedSceneAssetBundle)
            {
                AsyncOperation option = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
                return new UnityAsyncOperation(option);
            }
            else
            {
                UnloadSceneAssetbundleByPath(scenepath);
                return null;
            }
        }
        else
        {
            AsyncOperation option = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
            return new UnityAsyncOperation(option);
        }
    }

    [Obsolete("Unity提示不安全接口，建议使用异步卸载接口")]
    public static bool UnloadScene(Scene scene)
    {
        return SceneManager.UnloadScene(scene);
    }

    [Obsolete("Unity提示不安全接口，建议使用异步卸载接口")]
    public static bool UnloadScene(string sceneName)
    {
        return SceneManager.UnloadScene(sceneName);
    }

    [Obsolete("Unity提示不安全接口，建议使用异步卸载接口")]
    public static bool UnloadScene(int sceneBuildIndex)
    {
        return SceneManager.UnloadScene(sceneBuildIndex);
    }

    public static AsyncOperation UnloadSceneAsync(Scene scene)
    {
        return SceneManager.UnloadSceneAsync(scene);
    }

    public static AsyncOperation UnloadSceneAsync(string sceneName)
    {
        return SceneManager.UnloadSceneAsync(sceneName);
    }

    public static Scene GetSceneAt(int index)
    {
        return GamePortal.instance.GetSceneAt(index);
    }

    public static void SetSceneGOActiveAndActivatePreScene(string sceneName,bool isActive)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.IsValid())
            return;

        List<GameObject> list = ListPool<GameObject>.Get();
        scene.GetRootGameObjects(list);

        for (int i = 0; i < list.Count; ++i)
        {
            GameObject go = list[i];
            go.SetActive(isActive);

        }

        ListPool<GameObject>.Release(list);

        int sceneCount = SceneManager.sceneCount;
        if (sceneCount <= 1)
            return;

        Scene toActiveScene = GetSceneAt(sceneCount - 2);
        if (toActiveScene.IsValid() && toActiveScene.isLoaded && toActiveScene != scene)
        {
            SetActiveScene(toActiveScene);
        }
    }

    public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex)
    {
        return SceneManager.UnloadSceneAsync(sceneBuildIndex);
    }

    public static bool SetActiveScene(Scene scene)
    {
        bool value = SceneManager.SetActiveScene(scene);
        return value;
    }

    public static Scene GetSceneByName(string name)
    {
        return SceneManager.GetSceneByName(name);
    }

    public static void UnloadSceneAssetbundleByPath(string scenepath)
    {
        AssetBundleManager.instance.UnloadSceneAssetbundle(scenepath);
    }

    public static void UnloadSceneAssetbundleByName(string sceneName)
    {
        string scenepath = SceneBuildSetting.instance.GetScenePath(sceneName);
        AssetBundleManager.instance.UnloadSceneAssetbundle(scenepath);
    }

    public static void UnloadSceneAssetbundleIndex(int sceneBuildIndex)
    {
        string scenepath = SceneBuildSetting.instance.GetScenePathByIndex(sceneBuildIndex);
        AssetBundleManager.instance.UnloadSceneAssetbundle(scenepath);
    }
}
